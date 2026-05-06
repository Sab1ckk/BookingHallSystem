use BookingSystem;

go

CREATE TRIGGER trg_CheckReservationOverlap
ON Reservations
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- ѕроверка: пересечение по времени в одной комнате
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
        RAISERROR('ќшибка: врем€ бронировани€ пересекаетс€ с существующим в этой комнате', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- ≈сли нет пересечений Ц вставл€ем
    INSERT INTO Reservations (RoomId, EmployeeId, EventId, ReservationDate, StartTime, EndTime)
    SELECT RoomId, EmployeeId, EventId, ReservationDate, StartTime, EndTime
    FROM inserted;
END;
GO