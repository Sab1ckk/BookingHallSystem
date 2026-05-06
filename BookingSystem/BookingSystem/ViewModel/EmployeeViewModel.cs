using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookingSystem.ViewModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly SystemModel _context;
        private string _lastName;
        private string _firstName;
        private string _patronymic;
        private Positions _selectedPosition;
        private string _contactInfo;
        private Employees _selectedEmployee;

        private ObservableCollection<Employees> _employees;
        private ObservableCollection<Positions> _positions;

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        public string Patronymic
        {
            get => _patronymic;
            set { _patronymic = value; OnPropertyChanged(); }
        }

        public Positions SelectedPosition
        {
            get => _selectedPosition;
            set { _selectedPosition = value; OnPropertyChanged(); }
        }

        public string ContactInfo
        {
            get => _contactInfo;
            set { _contactInfo = value; OnPropertyChanged(); }
        }

        public Employees SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (value != null)
                {
                    LastName = value.LastName;
                    FirstName = value.FirstName;
                    Patronymic = value.Patronymic;
                    SelectedPosition = value.Positions;
                    ContactInfo = value.ContactInfo;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Employees> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Positions> Positions
        {
            get => _positions;
            set { _positions = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public EmployeeViewModel()
        {
            _context = new SystemModel();
            LoadData();

            AddCommand = new RelayCommand(_ => AddEmployee());
            UpdateCommand = new RelayCommand(_ => UpdateEmployee(), _ => SelectedEmployee != null);
            DeleteCommand = new RelayCommand(_ => DeleteEmployee(), _ => SelectedEmployee != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
        }

        private void LoadData()
        {
            Employees = new ObservableCollection<Employees>(
                _context.Employees.Include("Positions").ToList());
            Positions = new ObservableCollection<Positions>(_context.Positions.ToList());
        }

        private void AddEmployee()
        {
            if (string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                SelectedPosition == null)
                return;

            var employee = new Employees
            {
                LastName = LastName,
                FirstName = FirstName,
                Patronymic = Patronymic,
                PositionId = SelectedPosition.Id,
                ContactInfo = ContactInfo
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void UpdateEmployee()
        {
            if (SelectedEmployee == null) return;

            SelectedEmployee.LastName = LastName;
            SelectedEmployee.FirstName = FirstName;
            SelectedEmployee.Patronymic = Patronymic;
            SelectedEmployee.PositionId = SelectedPosition?.Id ?? SelectedEmployee.PositionId;
            SelectedEmployee.ContactInfo = ContactInfo;

            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            var result = System.Windows.MessageBox.Show("Удалить выбранного сотрудника?", "Подтверждение",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _context.Employees.Remove(SelectedEmployee);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            Patronymic = string.Empty;
            SelectedPosition = null;
            ContactInfo = string.Empty;
            SelectedEmployee = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
