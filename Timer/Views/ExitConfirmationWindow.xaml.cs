using System.Windows;

namespace Timer.Views
{
    public partial class ExitConfirmationWindow : Window
    {
        public bool ExitConfirmed { get; private set; }

        public ExitConfirmationWindow()
        {
            InitializeComponent();
        }

        private void Stay_Click(object sender, RoutedEventArgs e)
        {
            ExitConfirmed = false;
            Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitConfirmed = true;
            Close();
        }
    }
}