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
    public class EventTypeViewModel : INotifyPropertyChanged
    {
        private readonly SystemModel _context;
        private string _name;
        private EventTypes _selectedEventType;

        private ObservableCollection<EventTypes> _eventTypes;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public EventTypes SelectedEventType
        {
            get => _selectedEventType;
            set
            {
                _selectedEventType = value;
                if (value != null)
                {
                    Name = value.Name;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EventTypes> EventTypes
        {
            get => _eventTypes;
            set { _eventTypes = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public EventTypeViewModel()
        {
            _context = new SystemModel();
            LoadData();

            AddCommand = new RelayCommand(_ => AddEventType());
            UpdateCommand = new RelayCommand(_ => UpdateEventType(), _ => SelectedEventType != null);
            DeleteCommand = new RelayCommand(_ => DeleteEventType(), _ => SelectedEventType != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
        }

        private void LoadData()
        {
            EventTypes = new ObservableCollection<EventTypes>(_context.EventTypes.ToList());
        }

        private void AddEventType()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            var eventType = new EventTypes
            {
                Name = Name
            };

            _context.EventTypes.Add(eventType);
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void UpdateEventType()
        {
            if (SelectedEventType == null) return;

            SelectedEventType.Name = Name;

            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void DeleteEventType()
        {
            if (SelectedEventType == null) return;

            if (SelectedEventType.Events.Any())
            {
                System.Windows.MessageBox.Show("Нельзя удалить тип события, к которому привязаны события!",
                    "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            var result = System.Windows.MessageBox.Show("Удалить выбранный тип события?", "Подтверждение",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _context.EventTypes.Remove(SelectedEventType);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            SelectedEventType = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
