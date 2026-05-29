using System;
using System.Threading.Tasks;

namespace CHATBOTPART2
{
    // Minimal voice service. Uses System.Speech.Synthesis when available on Windows.
    public class VoiceService
    {
        private readonly object? synth;

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
        }

        public void Speak(string text)
        {
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

            // fallback simple beep sequence (not actual speech)
            try
            {
                // simple beep sequence as fallback
                Console.Beep(700, 80);
                Console.Beep(880, 80);
            }
            catch { }
        }


        public void SpeakAsync(string text)
        {
            Task.Run(() => Speak(text));
        }
    }
}