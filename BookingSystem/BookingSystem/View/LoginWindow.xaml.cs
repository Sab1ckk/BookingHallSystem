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
    /// Окно авторизации пользователя.
    /// Предоставляет форму входа с проверкой учетных данных.
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// Авторизованный пользователь (устанавливается после успешного входа)
        /// </summary>
        public Users AuthorizedUser { get; private set; }

        /// <summary>
        /// Контекст базы данных для проверки учетных данных
        /// </summary>
        private SystemModel _db = new SystemModel();

        /// <summary>
        /// Конструктор окна входа
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Хеширует пароль с использованием алгоритма SHA256.
        /// </summary>
        /// <param name="password">Пароль в открытом виде</param>
        /// <returns>Хеш пароля в шестнадцатеричном формате</returns>
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

        /// <summary>
        /// Обработчик нажатия кнопки входа.
        /// Проверяет учетные данные пользователя в базе данных.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginBox.Text;
                string password = HashPassword(PasswordBox.Password);

                // Поиск пользователя в БД по логину и хешу пароля
                Users user = _db.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

                if (user != null)
                {
                    // Успешная авторизация
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
