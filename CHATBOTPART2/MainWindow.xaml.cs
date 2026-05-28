using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CHATBOTPART2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CybersecurityBot bot;
        private readonly VoiceService voiceService;
        private string lastBotMessage = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            bot = new CybersecurityBot();

            UIHelper.PlayWelcomeSound();

            AppendMessage(" Welcome to Chatbot Part2!", true);

            // Initialize voice service
            voiceService = new VoiceService();

            // Ask permission to show topics in the WPF UI
            AppendMessage("Welcome to chatbot. What is your name? Please enter your name to start.", true);

            // Disable topic and control buttons until we have a user name
            SetTopicButtonsEnabled(false);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text;

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            AppendMessage($" You: {input}", false);

            // If the bot doesn't have a username, decide whether this input is a name or a command/sentiment/topic
            if (string.IsNullOrWhiteSpace(bot.UserName))
            {
                // Allow explicit syntax: name: John
                if (input.StartsWith("name:", StringComparison.OrdinalIgnoreCase))
                {
                    var name = input.Substring(5).Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        bot.UserName = name;
                        AppendMessage($"🤖 Hello, {bot.UserName}! You can now ask about the topics or use the buttons.", true);
                        SetTopicButtonsEnabled(true);
                        UserInput.Clear();
                        UserInput.Focus();
                        return;
                    }
                }

                // If the input looks like a sentiment, topic, or command, forward to the bot instead of capturing as a name
                var lower = input.Trim().ToLowerInvariant();
                var sentimentOrCommandPattern = "^(worried|frustrated|curious|help|hi|hello|hey|menu|topics|show topics|remember|exit|quit|bye)$";
                if (System.Text.RegularExpressions.Regex.IsMatch(lower, sentimentOrCommandPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                    || bot.GetResponse(lower).StartsWith("Topics:")
                    || bot.GetResponse(lower).Length > 0 && !char.IsLetter(lower.FirstOrDefault()))
                {
                    // treat as regular input
                    var tempResponse = bot.GetResponse(input);
                    AppendMessage($"🤖 Bot: {tempResponse}", true);
                    lastBotMessage = tempResponse;
                    var check2 = this.FindName("VoiceEnabled") as CheckBox;
                    if (check2?.IsChecked == true)
                    {
                        voiceService.SpeakAsync(tempResponse);
                    }
                    UserInput.Clear();
                    UserInput.Focus();
                    return;
                }

                // Otherwise use the provided input as user name
                bot.UserName = input.Trim();
                AppendMessage($"🤖 Welcome, {bot.UserName}! You can now ask about the topics or use the buttons.", true);
                SetTopicButtonsEnabled(true);
                UserInput.Clear();
                UserInput.Focus();
                return;
            }

            string response = bot.GetResponse(input);

            AppendMessage($"🤖 Bot: {response}", true);
            lastBotMessage = response;
            var check = this.FindName("VoiceEnabled") as CheckBox;
            if (check?.IsChecked == true)
            {
                voiceService.SpeakAsync(response);
            }

            UserInput.Clear();
            UserInput.Focus();
        }

        private void UserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserInput.Focus();
        }

        private void AppendMessage(string message, bool isBot)
        {
            Paragraph paragraph = new Paragraph(new Run(message));

            ChatArea.Document.Blocks.Add(paragraph);

            ChatArea.ScrollToEnd();
        }

        private void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                if (string.IsNullOrWhiteSpace(bot.UserName))
                {
                    AppendMessage("🤖 Please enter your name first to access topics.", true);
                    return;
                }

                AppendMessage($"👤 You selected: {btn.Content}", false);
                string response = bot.GetResponse(tag);
                AppendMessage($"🤖 Bot: {response}", true);
                lastBotMessage = response;
                var check = this.FindName("VoiceEnabled") as CheckBox;
                if (check?.IsChecked == true)
                {
                    voiceService.SpeakAsync(response);
                }
            }
        }

        private void ShowTopicsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bot.UserName))
            {
                AppendMessage("🤖 Please enter your name first to access topics.", true);
                return;
            }
            string response = bot.GetResponse("menu");
            AppendMessage($"🤖 Bot: {response}", true);
            lastBotMessage = response;
            var check = this.FindName("VoiceEnabled") as CheckBox;
            if (check?.IsChecked == true)
            {
                voiceService.SpeakAsync(response);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bot.UserName))
            {
                AppendMessage("🤖 Please enter your name first to exit.", true);
                return;
            }
            var farewell = $"👋 Goodbye! Stay safe online!";
            AppendMessage($"🤖 Bot: {farewell}", true);
            var check = this.FindName("VoiceEnabled") as CheckBox;
            if (check?.IsChecked == true)
            {
                voiceService.SpeakAsync(farewell);
            }
            // Give time for speech to start
            Task.Delay(400).ContinueWith(_ => Dispatcher.Invoke(Close));
        }

        private void SetTopicButtonsEnabled(bool enabled)
        {
            this.PasswordsButton.IsEnabled = enabled;
            this.PhishingButton.IsEnabled = enabled;
            this.ScamsButton.IsEnabled = enabled;
            this.PrivacyButton.IsEnabled = enabled;
            this.BankingButton.IsEnabled = enabled;
            this.VpnButton.IsEnabled = enabled;
            this.RansomwareButton.IsEnabled = enabled;
            this.ShowTopicsButton.IsEnabled = enabled;
            this.ExitButton.IsEnabled = enabled;
        }
    }
}