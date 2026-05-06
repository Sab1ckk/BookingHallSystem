using BookingSystem.Help;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookingSystem.ViewModel
{
    internal class ReportViewModel : INotifyPropertyChanged
    {
        private SystemModel _db = new SystemModel();

        private IEnumerable<object> _reportData;
        public IEnumerable<object> ReportData
        {
            get => _reportData;
            set { _reportData = value; OnPropertyChanged(); }
        }

        private string _reportTitle = "Выберите отчет для генерации";
        public string ReportTitle
        {
            get => _reportTitle;
            set { _reportTitle = value; OnPropertyChanged(); }
        }

        public ICommand FreeRoomsCommand { get; }
        public ICommand MonthReservationsCommand { get; }
        public ICommand BestEmployeeCommand { get; }

        public ReportViewModel()
        {
            FreeRoomsCommand = new RelayCommand(obj => GetFreeRooms());
            MonthReservationsCommand = new RelayCommand(obj => GetMonthReservationsCount());
            BestEmployeeCommand = new RelayCommand(obj => GetEmployeeOfTheMonth());
        }

        private void GetFreeRooms()
        {
            DateTime today = DateTime.Today;
            var occupiedRoomIds = _db.Reservations
                .Where(r => r.ReservationDate == today)
                .Select(r => r.RoomId)
                .ToList();

            ReportData = _db.Rooms
                .Where(r => !occupiedRoomIds.Contains(r.Id))
                .Select(r => new { r.Id, r.Name, r.Capacity, Status = "Свободен" })
                .ToList();

            ReportTitle = $"Свободные залы на сегодня ({today.ToShortDateString()})";
        }

        private void GetMonthReservationsCount()
        {
            DateTime now = DateTime.Now;
            var count = _db.Reservations
                .Count(r => r.ReservationDate.Month == now.Month && r.ReservationDate.Year == now.Year);

            ReportData = new List<object> { new { Период = now.ToString("MMMM yyyy"), Общее_Количество = count } };
            ReportTitle = "Статистика бронирований за текущий месяц";
        }

        private void GetEmployeeOfTheMonth()
        {
            DateTime now = DateTime.Now;
            var bestEmployee = _db.Reservations
                .Where(r => r.ReservationDate.Month == now.Month && r.ReservationDate.Year == now.Year)
                .GroupBy(r => r.Employees)
                .Select(g => new
                {
                    ФИО = g.Key.FullName,
                    Должность = g.Key.Positions.Name,
                    Количество_Броней = g.Count()
                })
                .OrderByDescending(x => x.Количество_Броней)
                .FirstOrDefault();

            if (bestEmployee != null)
            {
                ReportData = new List<object> { bestEmployee };
                ReportTitle = "Лучший сотрудник текущего месяца";
            }
            else
            {
                MessageBox.Show("В этом месяце еще не было бронирований.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

