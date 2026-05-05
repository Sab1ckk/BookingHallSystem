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

        public HomeViewModel(SystemModel db)
        {
            DateTime today = DateTime.Today;

            FreeRooms = db.Rooms
                .Where(r => !r.Reservations.Any(res => res.ReservationDate == today))
                .ToList();

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
