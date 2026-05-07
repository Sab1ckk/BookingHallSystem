using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookingSystem.View;
using System.Windows;

namespace BookingSystem.ViewModel
{
    /// <summary>
    /// Основная ViewModel приложения.
    /// Управляет навигацией между страницами и правами доступа пользователя.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        private Users _currentUser;

        /// <summary>
        /// Текущая отображаемая страница
        /// </summary>
        private object _currentPage;

        /// <summary>
        /// Свойство для привязки к текущей странице
        /// </summary>
        public object CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        // Команды навигации по разделам приложения
        public ICommand OpenHomeCommand { get; }
        public ICommand OpenReservationCommand { get; }
        public ICommand OpenUsersCommand { get; }
        public ICommand OpenEventsCommand { get; }
        public ICommand OpenReportCommand { get; }
        public ICommand LogoutCommand { get; }

        // Команды навигации по справочникам (доступны только Admin)
        public ICommand OpenEventTypesCommand { get; }
        public ICommand OpenRoomsCommand { get; }
        public ICommand OpenEmployyesCommand { get; }
        public ICommand OpenPositionsCommand { get; }

        /// <summary>
        /// Конструктор MainViewModel.
        /// Инициализирует команды с проверкой прав доступа.
        /// </summary>
        /// <param name="user">Авторизованный пользователь</param>
        public MainViewModel(Users user)
        {
            try
            {
                _currentUser = user;
                // Инициализируем команды с помощью RelayCommand
                
                // Команда открытия главной страницы - доступна всем
                OpenHomeCommand = new RelayCommand(obj => CurrentPage = new HomePage());

                // Команда открытия страницы бронирования - доступна Admin, Manager, Employee
                OpenReservationCommand = new RelayCommand(obj => CurrentPage = new CelendarPage(_currentUser.Employees),
                    obj => _currentUser.Role == "Admin" || _currentUser.Role == "Manager" || _currentUser.Role == "Employee");

                // Команда открытия страницы пользователей - только Admin
                OpenUsersCommand = new RelayCommand(obj => CurrentPage = new UserPage(),
                    obj => _currentUser.Role == "Admin");

                // Команда открытия страницы мероприятий - доступна Admin и Manager
                OpenEventsCommand = new RelayCommand(obj => CurrentPage = new EventsPage(),
                    obj => _currentUser.Role == "Admin" || _currentUser.Role == "Manager");

                // Команда открытия страницы отчетов - доступна Admin и Manager
                OpenReportCommand = new RelayCommand(obj => CurrentPage = new ReportPage(),
                    obj => _currentUser.Role == "Admin" || _currentUser.Role == "Manager");

                // Команда открытия страницы типов мероприятий - только Admin
                OpenEventTypesCommand = new RelayCommand(obj => CurrentPage = new EventTypePage(),
                    obj => _currentUser.Role == "Admin");

                // Команда открытия страницы залов - только Admin
                OpenRoomsCommand = new RelayCommand(obj => CurrentPage = new RoomPage(),
                    obj => _currentUser.Role == "Admin");

                // Команда открытия страницы сотрудников - только Admin
                OpenEmployyesCommand = new RelayCommand(obj => CurrentPage = new EmployeePage(),
                    obj => _currentUser.Role == "Admin");

                // Команда открытия страницы должностей - только Admin
                OpenPositionsCommand = new RelayCommand(obj => CurrentPage = new PositionPage(),
                    obj => _currentUser.Role == "Admin");


                // Команда выхода из системы
                LogoutCommand = new RelayCommand(obj => { 
                    if (Application.Current is App app)
                    {
                        app.Logout();
                    } 
                });


                // По умолчанию открываем главную страницу
                CurrentPage = new HomePage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Событие изменения свойств для реализации INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод уведомления об изменении свойства
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
