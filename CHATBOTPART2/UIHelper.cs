using System;
using System.IO;
using System.Media;
using System.Threading;

namespace CHATBOTPART2
{
    public static class UIHelper
    {
        public enum AppColor
        {
            Welcome,
            User,
            Bot,
            Info,
            Error
        }

        // Method to play a welcome sound when the application starts
        public static void PlayWelcomeSound()
        {
            try
            {
                using var player = new SoundPlayer("welcome.wav");
                player.PlaySync();
            }
            catch (FileNotFoundException)
            {
                ColorWriteLine("Voice greeting ready! (Place 'welcome.wav' in bin/Debug/net8.0-windows for audio)", AppColor.Info);
                Thread.Sleep(1500);
            }
            catch (Exception ex)
            {
                UIHelper.ColorWriteLine($"Audio init error: {ex.Message}", UIHelper.AppColor.Error);
                Thread.Sleep(1500);
            }
        }


        public static void DisplayLogo()
        {
            ColorWriteLine("=== Cybersecurity Chatbot ===", AppColor.Welcome);
        }

        public static void ColorWriteLine(string text, AppColor color)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color switch
            {
                AppColor.Welcome => ConsoleColor.Cyan,
                AppColor.User => ConsoleColor.Yellow,
                AppColor.Bot => ConsoleColor.Green,
                AppColor.Info => ConsoleColor.White,
                AppColor.Error => ConsoleColor.Red,
                _ => original
            };
            Console.WriteLine(text);
            Console.ForegroundColor = original;
        }

        public static void TypingEffect(string text, AppColor color = AppColor.Bot, int delayMs = 6)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color switch
            {
                AppColor.Welcome => ConsoleColor.Cyan,
                AppColor.User => ConsoleColor.Yellow,
                AppColor.Bot => ConsoleColor.Green,
                AppColor.Info => ConsoleColor.White,
                AppColor.Error => ConsoleColor.Red,
                _ => original
            };

            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();

            Console.ForegroundColor = original;
        }

        public static void ShowMenu()
        {
            ColorWriteLine("Menu: passwords | phishing | scam | privacy | exit", AppColor.Info);
        }

        public static void DrawBorder(int width, char ch)
        {
            Console.WriteLine(new string(ch, Math.Max(0, width)));
        }
    }
}
