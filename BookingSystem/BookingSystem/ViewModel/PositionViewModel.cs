using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookingSystem.Help;

namespace BookingSystem.ViewModel
{
    public class PositionViewModel : INotifyPropertyChanged
    {
        private readonly SystemModel _context;
        private string _name;
        private string _description;
        private Positions _selectedPosition;

        private ObservableCollection<Positions> _positions;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public Positions SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                _selectedPosition = value;
                if (value != null)
                {
                    Name = value.Name;
                    Description = value.Description;
                }
                OnPropertyChanged();
            }
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

        public PositionViewModel()
        {
            _context = new SystemModel();
            LoadData();

            AddCommand = new RelayCommand(_ => AddPosition());
            UpdateCommand = new RelayCommand(_ => UpdatePosition(), _ => SelectedPosition != null);
            DeleteCommand = new RelayCommand(_ => DeletePosition(), _ => SelectedPosition != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
        }

        private void LoadData()
        {
            Positions = new ObservableCollection<Positions>(_context.Positions.ToList());
        }

        private void AddPosition()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            var position = new Positions
            {
                Name = Name,
                Description = Description
            };

            _context.Positions.Add(position);
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void UpdatePosition()
        {
            if (SelectedPosition == null) return;

            SelectedPosition.Name = Name;
            SelectedPosition.Description = Description;

            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void DeletePosition()
        {
            if (SelectedPosition == null) return;

            if (SelectedPosition.Employees.Any())
            {
                System.Windows.MessageBox.Show("Нельзя удалить должность, к которой привязаны сотрудники!",
                    "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            var result = System.Windows.MessageBox.Show("Удалить выбранную должность?", "Подтверждение",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _context.Positions.Remove(SelectedPosition);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Description = string.Empty;
            SelectedPosition = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
