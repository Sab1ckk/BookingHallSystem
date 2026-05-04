namespace BookingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                GO
                CREATE TRIGGER trg_CheckReservationOverlap
                ON Reservations
                INSTEAD OF INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
    
                    -- Проверка: пересечение по времени в одной комнате
                    IF EXISTS (
                        SELECT 1
                        FROM inserted i
                        INNER JOIN Reservations r 
                            ON r.RoomId = i.RoomId 
                            AND r.ReservationDate = i.ReservationDate
                            AND (
                                (i.StartTime < r.EndTime AND i.EndTime > r.StartTime)
                            )
                    )
                    BEGIN
                        RAISERROR('Ошибка: время бронирования пересекается с существующим в этой комнате', 16, 1);
                        ROLLBACK TRANSACTION;
                        RETURN;
                    END
    
                    -- Если нет пересечений – вставляем
                    INSERT INTO Reservations (RoomId, EmployeeId, EventId, ReservationDate, StartTime, EndTime)
                    SELECT RoomId, EmployeeId, EventId, ReservationDate, StartTime, EndTime
                    FROM inserted;
                END;");
        }
        
        public override void Down()
        {
            Sql("DEOP TRIGGER trg_CheckReservationOverlap");
        }
    }
}
