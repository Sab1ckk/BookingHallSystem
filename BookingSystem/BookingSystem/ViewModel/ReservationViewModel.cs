using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookingSystem.ViewModel
{
    internal class ReservationViewModel : INotifyPropertyChanged
    {
        private SystemModel _db = new SystemModel();

        public List<Rooms> AllRooms { get; set; }
        public List<Events> AllEvents { get; set; }

        private List<Reservations> _allReservations;
        public List<Reservations> AllReservations
        {
            get => _allReservations;
            set { _allReservations = value; OnPropertyChanged(); }
        }

        public Employees ResponsibleEmployee { get; set; }
        public Rooms SelectedRoom { get; set; }
        public Events SelectedEvent { get; set; }

        public Reservations SelectedReservation { get; set; }

        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public string StartTimeStr { get; set; } = "12:00";
        public string EndTimeStr { get; set; } = "14:00";

        public ICommand AddReservationCommand { get; }
        public ICommand DeleteReservationCommand { get; }

        public ReservationViewModel(Employees currentEmployee)
        {
            ResponsibleEmployee = currentEmployee;
            AllRooms = _db.Rooms.ToList();
            AllEvents = _db.Events.ToList();
            LoadReservations();

            AddReservationCommand = new RelayCommand(obj => SaveReservation());
            DeleteReservationCommand = new RelayCommand(obj => DeleteReservation(), can => SelectedReservation != null);
        }

        private void LoadReservations()
        {
            AllReservations = _db.Reservations
                .Include(r => r.Rooms)
                .Include(r => r.Events)
                .Include(r => r.Employees)
                .ToList();
        }

        private void SaveReservation()
        {
            if (SelectedRoom == null || SelectedEvent == null)
            {
                MessageBox.Show("Выберите зал и мероприятие!");
                return;
            }

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

                LoadReservations();
                MessageBox.Show("Бронирование успешно добавлено!");
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.InnerException as System.Data.SqlClient.SqlException;
                var sqlEx = ex.InnerException?.InnerException as System.Data.SqlClient.SqlException
                ?? ex.InnerException as System.Data.SqlClient.SqlException;

                if (sqlEx != null)
                {
                    MessageBox.Show(sqlEx.Message);
                }
                if (inner != null && inner.Number == 2627) 
                {
                    MessageBox.Show("Вы уже создали бронирование на эту дату. " +
                                    "Ограничение 'одна бронь в день' нарушено.");
                }
                else
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void DeleteReservation()
        {
            try
            {
                var resToDelete = _db.Reservations.Find(SelectedReservation.Id);
                if (resToDelete != null)
                {
                    _db.Reservations.Remove(resToDelete);
                    _db.SaveChanges();
                    LoadReservations();
                    MessageBox.Show("Запись удалена");
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при удалении: " + ex.Message); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
