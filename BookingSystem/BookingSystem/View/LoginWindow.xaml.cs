using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace BookingSystem.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public Users AuthorizedUser { get; private set; }
        private SystemModel _db = new SystemModel();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginBox.Text;
                string password = HashPassword(PasswordBox.Password);

                // Здесь ваша логика проверки через БД
                Users user = _db.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

                if (user != null)
                {
                    AuthorizedUser = user;
                    this.DialogResult = true; // Закрывает окно и возвращает true
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
