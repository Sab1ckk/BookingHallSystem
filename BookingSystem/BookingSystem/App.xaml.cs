using BookingSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BookingSystem.View;
using System.ComponentModel;

namespace BookingSystem
{
    /// <summary>
    /// Основной класс приложения WPF.
    /// Управляет жизненным циклом приложения, включая запуск и выход из системы.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Ссылка на главное окно приложения
        /// </summary>
        public MainWindow _mainView;

        /// <summary>
        /// Метод выхода из системы/логина.
        /// Отображает окно входа и при успешной авторизации открывает главное окно.
        /// </summary>
        public void Logout()
        {
            // Создаем новое окно входа
            LoginWindow loginWindow = new LoginWindow();
            // Сохраняем ссылку на текущее главное окно
            Window oldWindow = Application.Current.MainWindow; 
            
            // Если пользователь успешно вошел (DialogResult == true)
            if (loginWindow.ShowDialog() == true)
            {   
                // Создаем главное окно приложения
                _mainView = new MainWindow();
                // Передаем данные пользователя во ViewModel главного окна
                var viewModel = new MainViewModel(loginWindow.AuthorizedUser);
                _mainView.DataContext = viewModel;
                
                // Устанавливаем главное окно приложения
                Application.Current.MainWindow = _mainView;
                _mainView.Show();

                // Отписываемся от события закрытия старого окна
                if (oldWindow != null)
                {
                    oldWindow.Closed -= OnMainWindowClosed;
                    oldWindow?.Close();
                }

                // Подписываемся на событие закрытия нового главного окна
                _mainView.Closed += OnMainWindowClosed;
            }
            else
            {
                // Если закрыли окно логина — выходим из приложения
                Shutdown();
            }
        }

        /// <summary>
        /// Вызывается при запуске приложения.
        /// Инициализирует приложение и отображает окно входа.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {   
            base.OnStartup(e);

            // Устанавливаем режим завершения - явный вызов Shutdown()
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Запускаем процесс авторизации
            Logout();
        }

        /// <summary>
        /// Обработчик события закрытия главного окна.
        /// Завершает работу приложения.
        /// </summary>
        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
