
using System.Windows;

namespace Timer.Views
{
    public partial class NotificationWindow : Window
    {
        public bool Confirmed { get; private set; }

        public NotificationWindow(string title, string message)
        {
            InitializeComponent();

            MessageTitle.Text = title;
            MessageText.Text = message;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            DialogResult = false;
        }
    }
}
