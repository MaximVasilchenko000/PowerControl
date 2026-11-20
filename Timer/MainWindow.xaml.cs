using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.ComponentModel;
using System.Windows;
using Timer.ViewModels;
using Timer.Views;

namespace Timer
{
    public partial class MainWindow : Window
    {
        private bool _isExiting;
        private TimerViewModel _timerViewModel;

        public MainWindow()
        {
            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();
            DataContext = viewModel;

            _timerViewModel = viewModel.TimerViewModel;

            viewModel.TimerViewModel.NotificationRequested +=
                (title, message) =>
                {
                    TrayIcon.ShowBalloonTip(title,message,BalloonIcon.Info);
                };

            Closing += MainWindow_Closing;
        }

        // Открытие окна через меню трея
        private void OpenWindow_Click(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        // Полный выход из программы
        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, запущен ли таймер или находится на паузе
            if (_timerViewModel.IsTimerActive)
            {
                ExitConfirmationWindow confirmationWindow =
                    new ExitConfirmationWindow();

                confirmationWindow.ShowDialog();

                // Если пользователь отказался от выхода
                if (!confirmationWindow.ExitConfirmed)
                    return;
            }

            // Завершаем приложение
            _isExiting = true;

            TrayIcon.Dispose();

            Application.Current.Shutdown();
        }

        // Сворачивание окна в трей
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        // Обработка нажатия на крестик
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                Hide();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            TrayIcon.Dispose();
            base.OnClosed(e);
        }


    }
}
