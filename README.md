# CHATBOTPART2

Chatbot Part2 is a Cybersecurity Awareness Chatbot developed in C# using .NET 8 and Visual Studio 2026.

The chatbot helps educate users about cybersecurity topics such as:

Password safety
Phishing scams
Safe browsing
Malware
Social engineering
VPN security
Ransomware
Banking scams
Privacy protection
Social media safety

The chatbot includes South African cybersecurity examples and awareness topics such as load shedding scams, banking fraud, and phishing attacks.

Features
Voice Greeting
Plays a welcome WAV audio when the application starts.
ASCII Art Logo
Displays a cybersecurity-themed ASCII logo.
Personalized User Interaction
Asks the user for their name.
Uses the user’s name throughout the conversation.
Keyword Recognition

The chatbot recognizes cybersecurity keywords such as:

password
phishing
malware
scam
privacy
vpn
ransomware
banking
social media
updates
Random Responses

The chatbot provides random cybersecurity tips for topics such as:

Password safety
Phishing
Privacy
Scams

This creates more natural conversations.

Conversation Flow

The chatbot remembers the previous topic and supports follow-up questions like:

“tell me more”
“another tip”
“explain more”
Memory and Recall

The chatbot remembers:

User name
Favourite topic
Previous discussion topic
Sentiment Detection

The chatbot detects user emotions such as:

worried
frustrated
curious

It responds with supportive and helpful cybersecurity guidance.

GUI Interface

The project includes:

WPF graphical user interface
Chat display area
Input textbox
Send button
Styled interface with colors and spacing
Technologies Used
C#
.NET 8
WPF
Visual Studio 2026
GitHub Actions
Object-Oriented Programming (OOP)
Project Structure
ChatbotPart2/
│
├── Models/
│   ├── UserMemory.cs
│   ├── SentimentResult.cs
│   └── ChatHistory.cs
│
├── Services/
│   ├── MemoryService.cs
│   ├── RandomResponseService.cs
│   ├── ConversationService.cs
│   ├── SentimentService.cs
│   └── KeywordRecognitionService.cs
│
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
│
├── Cybersecurity.cs
├── Program.cs
├── UIHelper.cs
├── App.xaml
├── App.xaml.cs
├── welcome.wav
└── README.md
