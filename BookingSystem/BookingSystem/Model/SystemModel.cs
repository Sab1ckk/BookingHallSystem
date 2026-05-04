using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BookingSystem
{
    public partial class SystemModel : DbContext
    {
        public SystemModel()
            : base("name=SystemModel")
        {
        }

        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<EventTypes> EventTypes { get; set; }
        public virtual DbSet<Positions> Positions { get; set; }
        public virtual DbSet<Reservations> Reservations { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employees>()
                .HasMany(e => e.Reservations)
                .WithRequired(e => e.Employees)
                .HasForeignKey(e => e.EmployeeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employees>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Employees)
                .HasForeignKey(e => e.EmployeeId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Events>()
                .HasMany(e => e.Reservations)
                .WithRequired(e => e.Events)
                .HasForeignKey(e => e.EventId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EventTypes>()
                .HasMany(e => e.Events)
                .WithRequired(e => e.EventTypes)
                .HasForeignKey(e => e.EventTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Positions>()
                .HasMany(e => e.Employees)
                .WithRequired(e => e.Positions)
                .HasForeignKey(e => e.PositionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Rooms>()
                .HasMany(e => e.Reservations)
                .WithRequired(e => e.Rooms)
                .HasForeignKey(e => e.RoomId);
        }

        public virtual int CreateEvent(string title, Nullable<int> clientCount, string eventTypeName, ObjectParameter eventId)
        {
            var titleParameter = title != null ?
                new ObjectParameter("Title", title) :
                new ObjectParameter("Title", typeof(string));

            var clientCountParameter = clientCount.HasValue ?
                new ObjectParameter("ClientCount", clientCount) :
                new ObjectParameter("ClientCount", typeof(int));

            var eventTypeNameParameter = eventTypeName != null ?
                new ObjectParameter("EventTypeName", eventTypeName) :
                new ObjectParameter("EventTypeName", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CreateEvent", titleParameter, clientCountParameter, eventTypeNameParameter, eventId);
        }
    }
}

