using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHATBOTPART2
{

    internal class Program
    {


        // Converted console entry point to a named method to avoid multiple Main entry points
        // The WPF App.xaml provides the application's real entry point.
        static void RunConsole()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            UIHelper.PlayWelcomeSound();
            UIHelper.DisplayLogo();

            UIHelper.ColorWriteLine(" Hello! What is your name?", UIHelper.AppColor.Welcome);
            string? nameInput;
            do
            {
                UIHelper.ColorWriteLine(" Enter your name: ", UIHelper.AppColor.User);
                nameInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nameInput))
                {
                    break;
                }
                UIHelper.ColorWriteLine(" Please enter your name to start chatting!", UIHelper.AppColor.Error);
            } while (true);

            var bot = new CybersecurityBot { UserName = nameInput!.Trim() };

            UIHelper.TypingEffect($" Welcome, {bot.UserName}! Lets Engage more to help you to be aware about Cyber Security Attacks - now with SA topics like load shedding & banking scams!", UIHelper.AppColor.Welcome);

            // Ask permission to show topics and display them only if user agrees
            UIHelper.TypingEffect("Welcome to chatbot. Can I show you the topics? (yes/no)", UIHelper.AppColor.Info);
            string? showTopicsResponse = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(showTopicsResponse) &&
                (showTopicsResponse.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                 showTopicsResponse.Trim().Equals("y", StringComparison.OrdinalIgnoreCase)))
            {
                UIHelper.ShowMenu();
            }

            // Main chat loop
            UIHelper.DrawBorder(60, '─');
            string? input;
            while (true)
            {
                UIHelper.ColorWriteLine("\n You say: ", UIHelper.AppColor.User);
                input = Console.ReadLine();
                if (bot.ShouldExit(input ?? string.Empty))
                {
                    UIHelper.TypingEffect($" Goodbye, {bot.UserName}! Stay safe online! 🔒", UIHelper.AppColor.Bot);
                    break;
                }

                string response = bot.GetResponse(input ?? string.Empty);
                UIHelper.TypingEffect($" {response}", UIHelper.AppColor.Bot);
            }
            UIHelper.DrawBorder(60, '─');

            UIHelper.ColorWriteLine("\n Thanks for chatting! Press Enter to exit...", UIHelper.AppColor.Info);
            Console.ReadLine();
        }
    }
}
