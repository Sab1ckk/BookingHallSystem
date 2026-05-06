use BookingSystem;

CREATE TABLE Positions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
);

-- Типы событий (отдельная таблица)
CREATE TABLE EventTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
);

-- Сотрудники (ФИО разбито на 3 части)
CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LastName NVARCHAR(100) NOT NULL,      -- Фамилия
    FirstName NVARCHAR(100) NOT NULL,     -- Имя
    Patronymic NVARCHAR(100),             -- Отчество (может быть NULL)
    PositionId INT NOT NULL FOREIGN KEY REFERENCES Positions(Id),
    ContactInfo NVARCHAR(256),
    
    -- Вычисляемое поле для удобства
    FullName AS (LastName + ' ' + FirstName + ISNULL(' ' + Patronymic, '')) PERSISTED
);

-- Залы
CREATE TABLE Rooms (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Capacity INT NOT NULL CHECK (Capacity > 0)
);

-- События (тип вынесен в отдельную таблицу)
CREATE TABLE Events (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL, -- обязательно
    ClientCount INT NOT NULL CHECK (ClientCount > 0),
    EventTypeId INT NOT NULL FOREIGN KEY REFERENCES EventTypes(Id)
);

-- Пользователи
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT UNIQUE FOREIGN KEY REFERENCES Employees(Id) ON DELETE CASCADE,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Admin', 'Manager', 'Employee'))
);

-- Бронирования
CREATE TABLE Reservations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoomId INT NOT NULL FOREIGN KEY REFERENCES Rooms(Id) ON DELETE CASCADE,
    EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(Id),
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(Id),
    ReservationDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    
    -- Уникальность: не более одного мероприятия на комнату и дату
    CONSTRAINT UQ_Room_Date UNIQUE (RoomId, ReservationDate),
    -- Сотрудник может бронировать только 1 раз в сутки
    CONSTRAINT UQ_Employee_PerDay UNIQUE (EmployeeId, ReservationDate),
    -- Доп. проверка времени
    CONSTRAINT CHK_TimeRange CHECK (StartTime < EndTime)
);