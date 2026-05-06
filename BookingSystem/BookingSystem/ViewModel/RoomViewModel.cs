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
    public class RoomViewModel : INotifyPropertyChanged
    {
        private readonly SystemModel _context;
        private string _name;
        private int _capacity;
        private Rooms _selectedRoom;
        private ObservableCollection<Rooms> _rooms;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public int Capacity
        {
            get => _capacity;
            set { _capacity = value; OnPropertyChanged(); }
        }

        public Rooms SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                if (value != null)
                {
                    Name = value.Name;
                    Capacity = value.Capacity;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Rooms> Rooms
        {
            get => _rooms;
            set { _rooms = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public RoomViewModel()
        {
            _context = new SystemModel();
            LoadRooms();
            AddCommand = new RelayCommand(_ => AddRoom());
            UpdateCommand = new RelayCommand(_ => UpdateRoom(), _ => SelectedRoom != null);
            DeleteCommand = new RelayCommand(_ => DeleteRoom(), _ => SelectedRoom != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
        }

        private void LoadRooms()
        {
            Rooms = new ObservableCollection<Rooms>(_context.Rooms.ToList());
        }

        private void AddRoom()
        {
            if (string.IsNullOrWhiteSpace(Name) || Capacity <= 0)
                return;

            var room = new Rooms
            {
                Name = Name,
                Capacity = Capacity
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();
            LoadRooms();
            ClearForm();
        }

        private void UpdateRoom()
        {
            if (SelectedRoom == null) return;

            SelectedRoom.Name = Name;
            SelectedRoom.Capacity = Capacity;
            _context.SaveChanges();
            LoadRooms();
            ClearForm();
        }

        private void DeleteRoom()
        {
            if (SelectedRoom == null) return;

            _context.Rooms.Remove(SelectedRoom);
            _context.SaveChanges();
            LoadRooms();
            ClearForm();
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Capacity = 0;
            SelectedRoom = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
