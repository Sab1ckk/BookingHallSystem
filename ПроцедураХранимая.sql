use BookingSystem;

go

CREATE OR ALTER PROCEDURE CreateEvent
    @Title NVARCHAR(255),
    @ClientCount INT,
    @EventTypeName NVARCHAR(100),
    @EventId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Проверяем корректность входных данных
        IF @Title IS NULL OR LTRIM(RTRIM(@Title)) = ''
        BEGIN
            RAISERROR('Название мероприятия не может быть пустым', 16, 1);
            RETURN;
        END
        
        IF @ClientCount <= 0
        BEGIN
            RAISERROR('Количество клиентов должно быть больше 0', 16, 1);
            RETURN;
        END
        
        -- Проверяем или создаём тип мероприятия
        DECLARE @EventTypeId INT;
        
        SELECT @EventTypeId = Id 
        FROM EventTypes 
        WHERE Name = @EventTypeName;
        
        IF @EventTypeId IS NULL
        BEGIN
            INSERT INTO EventTypes (Name) 
            VALUES (@EventTypeName);
            
            SET @EventTypeId = SCOPE_IDENTITY();
        END
        
        -- Вставляем мероприятие
        INSERT INTO Events (Title, ClientCount, EventTypeId)
        VALUES (@Title, @ClientCount, @EventTypeId);
        
        -- Возвращаем ID созданного мероприятия
        SET @EventId = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        -- Выводим информацию об успешном создании
        PRINT N'Создано мероприятие с ID = ' + CAST(@EventId AS NVARCHAR);
END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Выводим информацию об ошибке
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;