using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookingSystem.ViewModel
{
    internal class ReservationViewModel
    {
        private SystemModel _db = new SystemModel();

        // Списки для ComboBox
        public List<Rooms> AllRooms { get; set; }
        public List<Events> AllEvents { get; set; }
        public List<Reservations> AllReservations { get; set; }

        // Поля для новой брони
        public Employees ResponsibleEmployee { get; set; }
        public Rooms SelectedRoom { get; set; }
        public Events SelectedEvent { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public string StartTimeStr { get; set; } = "12:00";
        public string EndTimeStr { get; set; } = "14:00";

        public ICommand AddReservationCommand { get; }

        public ReservationViewModel(Employees currentEmployee)
        {
            ResponsibleEmployee = currentEmployee;

            // Загрузка данных из БД
            AllRooms = _db.Rooms.ToList();
            AllEvents = _db.Events.ToList();
            LoadReservations();

            AddReservationCommand = new RelayCommand(obj => SaveReservation());
        }

        private void LoadReservations()
        {
            AllReservations = _db.Reservations.Include("Rooms").Include("Events").Include("Employees").ToList();
            OnPropertyChanged(nameof(AllReservations));
        }

        private void SaveReservation()
        {
            try
            {
                var newRes = new Reservations
                {
                    RoomId = SelectedRoom.Id,
                    EventId = SelectedEvent.Id,
                    EmployeeId = ResponsibleEmployee.Id,
                    ReservationDate = SelectedDate,
                    StartTime = TimeSpan.Parse(StartTimeStr),
                    EndTime = TimeSpan.Parse(EndTimeStr)
                };

                _db.Reservations.Add(newRes);
                _db.SaveChanges();

                LoadReservations(); // Обновляем таблицу
                MessageBox.Show("Бронирование успешно добавлено!");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
