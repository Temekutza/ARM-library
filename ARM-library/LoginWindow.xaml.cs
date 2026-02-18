using System;
using System.Windows;
using ARM_library.Data;
using ARM_library.Security;

namespace ARM_library
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _auth;

        public LoginWindow()
        {
            InitializeComponent();
            _auth = new AuthService(new DataAccess());
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty;

            var username = (UsernameBox.Text ?? string.Empty).Trim();
            var password = PasswordBox.Password ?? string.Empty;

            try
            {
                var identity = _auth.Authenticate(username, password);
                UserContext.Current = identity;

                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception ex)
            {
                // Чуть более понятное сообщение для частой проблемы с провайдером БД
                if (ex.Message != null && ex.Message.IndexOf("data provider", StringComparison.OrdinalIgnoreCase) >= 0)
                    ErrorText.Text = ex.Message + "\n\nПроверь, что установлен MySQL-провайдер. В проекте подключен пакет MySqlConnector.";
                else
                    ErrorText.Text = ex.Message;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = string.Empty;
            try
            {
                var w = new RegistrationWindow();
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    UsernameBox.Text = w.RegisteredUsername;
                    PasswordBox.Password = string.Empty;
                    ErrorText.Text = "Регистрация выполнена. Теперь войдите.";
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }
    }
}


