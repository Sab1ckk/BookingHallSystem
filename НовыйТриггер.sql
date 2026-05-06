CREATE TRIGGER trg_CheckReservationOverlap
ON Reservations
AFTER INSERT, UPDATE -- Проверяем и при создании, и при редактировании
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Reservations r
        JOIN inserted i ON r.RoomId = i.RoomId 
            AND r.ReservationDate = i.ReservationDate
            AND r.Id <> i.Id -- Не сравниваем запись саму с собой
        WHERE i.StartTime < r.EndTime AND i.EndTime > r.StartTime
    )
    BEGIN
        RAISERROR ('Ошибка пересечения времени!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;