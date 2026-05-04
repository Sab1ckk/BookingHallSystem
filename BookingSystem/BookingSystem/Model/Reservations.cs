namespace BookingSystem
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Reservations
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public int EmployeeId { get; set; }

        public int EventId { get; set; }

        [Column(TypeName = "date")]
        public DateTime ReservationDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public virtual Employees Employees { get; set; }

        public virtual Events Events { get; set; }

        public virtual Rooms Rooms { get; set; }
    }
}
