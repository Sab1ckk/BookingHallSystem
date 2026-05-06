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
        private readonly SystemModel _db;
        private string _login;
        private string _password;
        private string _selectedRole;
        private Employees _selectedEmployee;
        private Users _selectedUser;

        private List<Users> _users;
        private List<Employees> _employees;
        private List<string> _roles;

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
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

        public List<Users> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        public List<Employees> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(); }
        }

        public List<string> Roles
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
            _db = new SystemModel();
            LoadData();

            Roles = new List<string> { "Admin", "Manager", "User" };

            AddCommand = new RelayCommand(obj => AddUser());
            UpdateCommand = new RelayCommand(obj => UpdateUser(), obj => SelectedUser != null);
            DeleteCommand = new RelayCommand(obj => DeleteUser(), obj => SelectedUser != null);
            ClearCommand = new RelayCommand(obj => ClearForm());
        }

        private void LoadData()
        {
            Users = _db.Users.Include("Employees").ToList();
            Employees = _db.Employees.ToList();
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

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(SelectedRole))
                return;

            var user = new Users
            {
                Login = Login,
                PasswordHash = HashPassword(Password),
                Role = SelectedRole,
                EmployeeId = SelectedEmployee?.Id
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void UpdateUser()
        {
            if (SelectedUser == null) return;

            SelectedUser.Login = Login;
            SelectedUser.Role = SelectedRole;
            SelectedUser.EmployeeId = SelectedEmployee?.Id;

            _db.SaveChanges();
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
                _db.Users.Remove(SelectedUser);
                _db.SaveChanges();
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
