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
    public class MainViewModel : INotifyPropertyChanged
    {
        private Users _currentUser;
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public ICommand OpenHomeCommand { get; }
        public ICommand OpenReservationCommand { get; }
        public ICommand OpenUsersCommand { get; }
        public ICommand OpenEventsCommand { get; }
        public ICommand OpenReportCommand { get; }
        public ICommand LogoutCommand { get; }

        public ICommand OpenEventTypesCommand { get; }
        public ICommand OpenRoomsCommand { get; }
        public ICommand OpenEmployyesCommand { get; }
        public ICommand OpenPositionsCommand { get; }
        public MainViewModel(Users user)
        {
            try
            {
                // Инициализируем команды с помощью RelayCommand
                OpenHomeCommand = new RelayCommand(obj => CurrentPage = new HomePage());

                OpenReservationCommand = new RelayCommand(obj => CurrentPage = new CelendarPage(user.Employees),
                    obj => _currentUser.Role == "Admin" || _currentUser.Role == "Manager" || _currentUser.Role == "Employee");

                OpenUsersCommand = new RelayCommand(obj => CurrentPage = new UserPage(),
                    obj => _currentUser.Role == "Admin");

                OpenEventsCommand = new RelayCommand(obj => CurrentPage = new EventsPage(),
                    obj => _currentUser.Role == "Admin" || _currentUser.Role == "Manager");

                OpenReportCommand = new RelayCommand(obj => CurrentPage = new ReportPage(),
                    obj => _currentUser.Role == "Admin" || _currentUser.Role == "Manager");

                OpenEventTypesCommand = new RelayCommand(obj => CurrentPage = new EventTypePage(),
                    obj => _currentUser.Role == "Admin");

                OpenRoomsCommand = new RelayCommand(obj => CurrentPage = new RoomPage(),
                    obj => _currentUser.Role == "Admin");

                OpenEmployyesCommand = new RelayCommand(obj => CurrentPage = new EmployeePage(),
                    obj => _currentUser.Role == "Admin");

                OpenPositionsCommand = new RelayCommand(obj => CurrentPage = new PositionPage(),
                    obj => _currentUser.Role == "Admin");


                LogoutCommand = new RelayCommand(obj => { 
                    if (Application.Current is App app)
                    { app.Logout(); } 
                });


                CurrentPage = new HomePage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
