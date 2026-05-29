using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CHATBOTPART2
{
    // Minimal voice service. Uses System.Speech.Synthesis when available on Windows.
    public class VoiceService
    {
        private readonly object? synth;
        private readonly object? sapi;
        private readonly string? googleApiKey;
        // Keep active MediaPlayer instances alive while audio is playing
        private static readonly List<MediaPlayer> activePlayers = new();
        private static readonly object playersLock = new();

        public VoiceService()
        {
            try
            {
                // Try to create System.Speech.Synthesis.SpeechSynthesizer dynamically to avoid hard dependency
                var t = Type.GetType("System.Speech.Synthesis.SpeechSynthesizer, System.Speech");
                if (t != null)
                {
                    synth = Activator.CreateInstance(t);
                }
            }
            catch
            {
                synth = null;
            }

            // Try COM SAPI.SpVoice as a fallback (works on Windows)
            try
            {
                var sapiType = Type.GetTypeFromProgID("SAPI.SpVoice");
                if (sapiType != null)
                {
                    sapi = Activator.CreateInstance(sapiType);
                }
            }
            catch
            {
                // ignore
            }

            // Optionally use Google Cloud Text-to-Speech if API key is provided via environment variable
            googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_TTS_API_KEY");
        }

        public void Speak(string text)
        {
            // Prefer Google TTS if API key available
            if (!string.IsNullOrWhiteSpace(googleApiKey))
            {
                try
                {
                    // fire-and-forget synchronous wrapper
                    GoogleSpeakAsync(text, googleApiKey).GetAwaiter().GetResult();
                    return;
                }
                catch
                {
                    // fallthrough to other options
                }
            }

            if (synth != null)
            {
                try
                {
                    // Invoke Speak method
                    var mi = synth.GetType().GetMethod("Speak", new Type[] { typeof(string) });
                    mi?.Invoke(synth, new object[] { text });
                    return;
                }
                catch
                {
                    // fallthrough to fallback
                }
            }

            // Try COM SAPI.SpVoice if available
            if (sapi != null)
            {
                try
                {
                    var mi2 = sapi.GetType().GetMethod("Speak");
                    mi2?.Invoke(sapi, new object[] { text });
                    return;
                }
                catch
                {
                    // ignore and fallback
                }
            }

            // fallback: try to play embedded greeting if it matches the requested text
            try
            {
                if (!string.IsNullOrEmpty(EmbeddedGreeting.GreetingBase64) && text.Trim().StartsWith("Welcome to chatbot", StringComparison.OrdinalIgnoreCase))
                {
                    var audioBytes = Convert.FromBase64String(EmbeddedGreeting.GreetingBase64);
                    var temp = Path.Combine(Path.GetTempPath(), $"chatbot_embedded_{Guid.NewGuid()}.mp3");
                    File.WriteAllBytes(temp, audioBytes);
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        var player = new MediaPlayer();
                        lock (playersLock) { activePlayers.Add(player); }
                        player.Open(new Uri(temp));
                        player.MediaEnded += (s, e) =>
                        {
                            player.Close();
                            lock (playersLock) { activePlayers.Remove(player); }
                            try { File.Delete(temp); } catch { }
                        };
                        player.MediaFailed += (s, e) =>
                        {
                            player.Close();
                            lock (playersLock) { activePlayers.Remove(player); }
                            try { File.Delete(temp); } catch { }
                        };
                        player.Play();
                    });
                    return;
                }
            }
            catch
            {
                // ignore
            }

            return;
        }


        public void SpeakAsync(string text)
        {
            Task.Run(() => Speak(text));
        }

        // Stop any ongoing playback or speech
        public void Stop()
        {
            // Stop MediaPlayers
            try
            {
                lock (playersLock)
                {
                    foreach (var p in activePlayers.ToArray())
                    {
                        try
                        {
                            p.Stop();
                            p.Close();
                        }
                        catch { }
                    }
                    activePlayers.Clear();
                }
            }
            catch { }

            // Try to cancel SpeechSynthesizer async operations if available
            try
            {
                if (synth != null)
                {
                    var cancel = synth.GetType().GetMethod("SpeakAsyncCancelAll");
                    if (cancel != null)
                    {
                        cancel.Invoke(synth, Array.Empty<object>());
                    }
                    else
                    {
                        var disp = synth.GetType().GetMethod("Dispose");
                        disp?.Invoke(synth, Array.Empty<object>());
                    }
                }
            }
            catch { }

            // Try to pause/stop SAPI
            try
            {
                if (sapi != null)
                {
                    var pause = sapi.GetType().GetMethod("Pause");
                    if (pause != null)
                    {
                        pause.Invoke(sapi, Array.Empty<object>());
                    }
                }
            }
            catch { }
        }

        private async Task GoogleSpeakAsync(string text, string apiKey)
        {
            try
            {
                using var client = new HttpClient();
                var url = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={apiKey}";
                var payload = new
                {
                    input = new { text = text },
                    voice = new { languageCode = "en-US", name = "en-US-Wavenet-D" },
                    audioConfig = new { audioEncoding = "MP3" }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(url, content).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return;
                var respJson = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                using var doc = JsonDocument.Parse(respJson);
                if (!doc.RootElement.TryGetProperty("audioContent", out var audioElem)) return;
                var audioBase64 = audioElem.GetString();
                if (string.IsNullOrEmpty(audioBase64)) return;

                var audioBytes = Convert.FromBase64String(audioBase64);
                var temp = Path.Combine(Path.GetTempPath(), $"chatbot_tts_{Guid.NewGuid()}.mp3");
                await File.WriteAllBytesAsync(temp, audioBytes).ConfigureAwait(false);

                // Play on UI dispatcher using MediaPlayer
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            var player = new MediaPlayer();
                            lock (playersLock) { activePlayers.Add(player); }
                            player.Open(new Uri(temp));
                            player.MediaEnded += (s, e) =>
                            {
                                player.Close();
                                lock (playersLock) { activePlayers.Remove(player); }
                                try { File.Delete(temp); } catch { }
                            };
                            player.MediaFailed += (s, e) =>
                            {
                                player.Close();
                                lock (playersLock) { activePlayers.Remove(player); }
                                try { File.Delete(temp); } catch { }
                            };
                            player.Play();
                        }
                        catch
                        {
                            try { File.Delete(temp); } catch { }
                        }
                    });
            }
            catch
            {
                // ignore and fallback
            }
        }
    }
}