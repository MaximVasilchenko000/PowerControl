⚡ PowerControl

A modern Windows power management application with a countdown timer, system notifications, and system tray integration.

PowerControl is a desktop application designed to make managing your computer's power state easier and more convenient. Schedule shutdowns, restarts, or sleep mode using a simple timer with a clean and intuitive interface.

🚀 Features

⏱️ Flexible Countdown Timer

Set a custom countdown using hours and minutes.

Choose from predefined time presets.

Start, pause, resume, and reset the timer.

Add or remove 10 minutes while the timer is running.

Track the remaining time with a visual circular progress indicator.

⚡ Power Management

PowerControl supports three system actions:

Shutdown — automatically turn off your computer.

Restart — automatically restart Windows.

Sleep — put your computer into sleep mode.

🔔 Notifications

Notifications before the scheduled action.

Configurable notification lead time.

System tray notifications.

Status messages showing the current timer state.

🖥️ System Tray Integration

Minimize the application to the system tray.

Restore the application with a double-click on its tray icon.

Access application commands through the tray context menu.

Keep the timer running while the main window is hidden.

🎨 User Interface

Modern dark interface.

Dark blue and electric blue color scheme.

Neon-inspired visual elements.

Circular countdown progress indicator.

Compact and easy-to-use layout.

🛠️ Technologies

PowerControl is built using the following technologies:

Technology

Purpose

C#

Main programming language

WPF

Desktop user interface

.NET 8

Application platform

XAML

UI layout and styling

CommunityToolkit.Mvvm

MVVM architecture and commands

Hardcodet.NotifyIcon.Wpf

System tray integration

💻 System Requirements

Windows operating system.

.NET 8 SDK.

Visual Studio 2022 or another compatible .NET development environment.

📦 Installation

Option 1 — Run from Source

Clone this repository:

git clone https://github.com/MaximVasilchenko000/PowerControl.git

Open Timer.sln in Visual Studio 2022.

Restore NuGet packages.

Build the solution.

Run the application.

Option 2 — Download a Release

Prebuilt releases can be added here when they become available.

📖 How to Use

Launch PowerControl.

Select the desired power action.

Choose a time preset or enter a custom duration.

Configure the notification time if needed.

Press Start to begin the countdown.

Use the pause, resume, or reset controls whenever necessary.

You can also adjust the remaining time using the +10 min and -10 min buttons.

When the countdown reaches zero, PowerControl executes the selected system action.

⚠️ Important Notice

PowerControl can shut down or restart your computer.

Save all important documents and close applications before starting a countdown.

The developer is not responsible for unsaved data lost as a result of scheduled system actions.

📁 Project Structure

PowerControl/
│
├── Timer/
│   ├── Helpers/
│   ├── Models/
│   ├── Resources/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   ├── App.xaml
│   ├── MainWindow.xaml
│   └── Timer.csproj
│
├── .gitignore
├── LICENSE
├── README.md
└── Timer.sln

📄 License

This project is distributed under the MIT License.

You are free to use, modify, and distribute this software in accordance with the license terms.

See the LICENSE file for details.

👨‍💻 About the Project

PowerControl is a personal development project created to explore and practice:

C# desktop application development.

WPF and XAML.

MVVM architecture.

Windows system integration.

Timer and background task management.

User interface design.

Thanks for checking out PowerControl!

If you find this project useful, feel free to ⭐ star the repository.