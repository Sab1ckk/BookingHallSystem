using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookingSystem.ViewModel
{
    internal class EventViewModel : INotifyPropertyChanged
    {
        private SystemModel _db = new SystemModel();

        // Списки данных
        public List<EventTypes> AllEventTypes { get; set; }

        private ObservableCollection<Events> _allEvents;
        public ObservableCollection<Events> AllEvents
        {
            get => _allEvents;
            set { _allEvents = value; OnPropertyChanged(); }
        }

        // Поля для создания нового мероприятия
        private string _newTitle;
        public string NewTitle
        {
            get => _newTitle;
            set { _newTitle = value; OnPropertyChanged(); }
        }

        private int _newClientCount;
        public int NewClientCount
        {
            get => _newClientCount;
            set { _newClientCount = value; OnPropertyChanged(); }
        }

        private EventTypes _selectedEventType;
        public EventTypes SelectedEventType
        {
            get => _selectedEventType;
            set { _selectedEventType = value; OnPropertyChanged(); }
        }

        // Выбранное мероприятие в списке (для удаления)
        public Events SelectedEvent { get; set; }

        public ICommand AddEventCommand { get; }
        public ICommand DeleteEventCommand { get; }

        public EventViewModel()
        {
            // Загрузка справочника типов и списка мероприятий[cite: 2, 3]
            AllEventTypes = _db.EventTypes.ToList();
            LoadEvents();

            AddEventCommand = new RelayCommand(obj => AddEvent(), can => !string.IsNullOrEmpty(NewTitle) && SelectedEventType != null);
            DeleteEventCommand = new RelayCommand(obj => DeleteEvent(), can => SelectedEvent != null);
        }

        private void LoadEvents()
        {
            var events = _db.Events.Include("EventTypes").ToList();
            AllEvents = new ObservableCollection<Events>(events);
        }

        private void AddEvent()
        {
            try
            {
                var newEvent = new Events
                {
                    Title = NewTitle,
                    ClientCount = NewClientCount,
                    EventTypeId = SelectedEventType.Id
                };

                _db.Events.Add(newEvent);
                _db.SaveChanges();

                LoadEvents();
                ClearFields();
                MessageBox.Show("Мероприятие добавлено!");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void DeleteEvent()
        {
            try
            {
                var eventToDelete = _db.Events.Find(SelectedEvent.Id);
                if (eventToDelete != null)
                {
                    _db.Events.Remove(eventToDelete);
                    _db.SaveChanges();
                    LoadEvents();
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при удалении: " + ex.Message); }
        }

        private void ClearFields()
        {
            NewTitle = string.Empty;
            NewClientCount = 0;
            SelectedEventType = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

