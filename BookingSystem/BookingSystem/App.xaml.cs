using BookingSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BookingSystem.View;

namespace BookingSystem
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {   
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            LoginWindow loginWindow = new LoginWindow();

            // Если пользователь успешно вошел (DialogResult == true)
            if (loginWindow.ShowDialog() == true)
            {
                var mainView = new MainWindow();
                // Передаем данные пользователя во ViewModel главного окна
                var viewModel = new MainViewModel(loginWindow.AuthorizedUser);
                mainView.DataContext = viewModel;
                mainView.Show();
            }
            else
            {
                // Если закрыли окно логина — выходим из приложения
                Shutdown();
            }
        }
    }
}
