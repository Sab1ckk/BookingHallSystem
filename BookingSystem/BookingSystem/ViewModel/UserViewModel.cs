using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookingSystem.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private readonly SystemModel _context;
        private string _login;
        private string _selectedRole;
        private Employees _selectedEmployee;
        private Users _selectedUser;

        private ObservableCollection<Users> _users;
        private ObservableCollection<Employees> _employees;
        private ObservableCollection<string> _roles;

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        public Employees SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        public Users SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                if (value != null)
                {
                    Login = value.Login;
                    SelectedRole = value.Role;
                    SelectedEmployee = value.Employees;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Users> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Employees> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Roles
        {
            get => _roles;
            set { _roles = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public UserViewModel()
        {
            _context = new SystemModel();
            LoadData();

            Roles = new ObservableCollection<string> { "Admin", "Manager", "User" };

            AddCommand = new RelayCommand(_ => AddUser());
            UpdateCommand = new RelayCommand(_ => UpdateUser(), _ => SelectedUser != null);
            DeleteCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
        }

        private void LoadData()
        {
            Users = new ObservableCollection<Users>(
                _context.Users.Include("Employees").ToList());
            Employees = new ObservableCollection<Employees>(_context.Employees.ToList());
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(SelectedRole))
                return;

            // В реальном приложении нужно получить пароль из PasswordBox
            string password = "default123"; // Временное решение
            string passwordHash = HashPassword(password);

            var user = new Users
            {
                Login = Login,
                PasswordHash = passwordHash,
                Role = SelectedRole,
                EmployeeId = SelectedEmployee?.Id
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void UpdateUser()
        {
            if (SelectedUser == null) return;

            SelectedUser.Login = Login;
            SelectedUser.Role = SelectedRole;
            SelectedUser.EmployeeId = SelectedEmployee?.Id;

            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = System.Windows.MessageBox.Show("Удалить выбранного пользователя?", "Подтверждение",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _context.Users.Remove(SelectedUser);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            Login = string.Empty;
            SelectedRole = null;
            SelectedEmployee = null;
            SelectedUser = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
