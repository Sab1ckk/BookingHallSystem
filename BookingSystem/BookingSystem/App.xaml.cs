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
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainWindow _mainView;
        public void Logout()
        {
            LoginWindow loginWindow = new LoginWindow();
            Window oldWindow = Application.Current.MainWindow; 
            
            // Если пользователь успешно вошел (DialogResult == true)
            if (loginWindow.ShowDialog() == true)
            {   
                _mainView = new MainWindow();
                // Передаем данные пользователя во ViewModel главного окна
                var viewModel = new MainViewModel(loginWindow.AuthorizedUser);
                _mainView.DataContext = viewModel;
                
                Application.Current.MainWindow = _mainView;
                _mainView.Show();

                if (oldWindow != null)
                {
                    oldWindow.Closed -= OnMainWindowClosed;
                    oldWindow?.Close();
                }

                _mainView.Closed += OnMainWindowClosed;
            }
            else
            {
                // Если закрыли окно логина — выходим из приложения
                Shutdown();
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {   
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Logout();
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
