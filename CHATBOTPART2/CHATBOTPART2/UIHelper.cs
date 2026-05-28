using System;
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

        public static void PlayWelcomeSound()
        {
            // Minimal cross-platform friendly sound: beep
            try
            {
                Console.Beep(800, 120);
            }
            catch
            {
                // Ignore on platforms that don't support Console.Beep
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
