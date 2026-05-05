using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.ViewModel
{
    internal class HomeViewModel : INotifyPropertyChanged
    {
        private List<Rooms> _freeRooms;
        private string _mostPopularRoom;

        public List<Rooms> FreeRooms
        {
            get => _freeRooms;
            set { _freeRooms = value; OnPropertyChanged(); }
        }

        public string MostPopularRoom
        {
            get => _mostPopularRoom;
            set { _mostPopularRoom = value; OnPropertyChanged(); }
        }

        public HomeViewModel(SystemModel db) // Передаем контекст БД
        {
            DateTime today = DateTime.Today;

            // 1. Получаем свободные залы на сегодня
            // Свободным считаем зал, у которого нет броней на сегодня ВООБЩЕ
            FreeRooms = db.Rooms
                .Where(r => !r.Reservations.Any(res => res.ReservationDate == today))
                .ToList();

            // 2. Ищем самый популярный зал (за все время)
            var popular = db.Rooms
                .OrderByDescending(r => r.Reservations.Count)
                .FirstOrDefault();

            MostPopularRoom = popular?.Name ?? "Нет данных";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
