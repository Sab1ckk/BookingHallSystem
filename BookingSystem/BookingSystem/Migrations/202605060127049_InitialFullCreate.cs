namespace BookingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialFullCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        Patronymic = c.String(maxLength: 100),
                        PositionId = c.Int(nullable: false),
                        ContactInfo = c.String(maxLength: 256),
                        FullName = c.String(nullable: false, maxLength: 302),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Positions", t => t.PositionId)
                .Index(t => t.PositionId);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        ReservationDate = c.DateTime(nullable: false, storeType: "date"),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.RoomId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        ClientCount = c.Int(nullable: false),
                        EventTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventTypes", t => t.EventTypeId)
                .Index(t => t.EventTypeId);
            
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        Login = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                        Role = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            Sql(@"
                INSERT INTO Positions (Name) VALUES 
                ('Администратор'),
                ('Менеджер по работе с клиентами'),
                ('Технический специалист'),
                ('Руководитель отдела'),
                ('Координатор мероприятий');
                INSERT INTO EventTypes (Name) VALUES 
                ('Корпоратив'),
                ('День рождения'),
                ('Конференция'),
                ('Семинар'),
                ('Свадьба'),
                ('Выставка');
                INSERT INTO Employees (LastName, FirstName, Patronymic, PositionId, ContactInfo) VALUES 
                ('Иванов', 'Иван', 'Иванович', 1, 'ivanov@example.com; +7-999-123-4567'),
                ('Петрова', 'Елена', 'Сергеевна', 2, 'petrova@example.com; +7-999-234-5678'),
                ('Сидоров', 'Алексей', 'Владимирович', 3, 'sidorov@example.com; +7-999-345-6789'),
                ('Козлова', 'Мария', 'Андреевна', 4, 'kozlova@example.com; +7-999-456-7890'),
                ('Смирнов', 'Дмитрий', 'Николаевич', 5, 'smirnov@example.com; +7-999-567-8901'),
                ('Волкова', 'Анна', 'Павловна', 2, 'volkova@example.com; +7-999-678-9012');
                INSERT INTO Rooms (Name, Capacity) VALUES 
                ('Зал ""Торжественный""', 150),
                ('Зал ""Бизнес-класс""', 50),
                ('Зал ""Презентационный""', 80),
                ('Конференц-зал ""Лидер""', 120),
                ('Банкетный зал ""Уют""', 200);
                INSERT INTO Events (Title, ClientCount, EventTypeId) VALUES 
                ('Годовой корпоратив компании', 120, 1),
                ('Юбилей Ивана Петрова', 80, 2),
                ('IT-конференция 2026', 150, 3),
                ('Тренинг ""Эффективные продажи""', 40, 4),
                ('Свадьба Алексея и Марии', 100, 5),
                ('Художественная выставка ""Вдохновение""', 60, 6),
                ('Новогодний корпоратив', 180, 1),
                ('День рождения компании', 90, 2);
                INSERT INTO Users (EmployeeId, Login, PasswordHash, Role) VALUES 
                (1, 'admin', '25f43b1486ad95a1398e3eeb3d83bc4010015fcc9bedb35b432e00298d5021f7', 'Admin'),
                (2, 'elena', '0ce93c9606f0685bf60e051265891d256381f639d05c0aec67c84eec49d33cc1', 'Manager'),
                (3, 'alexey', '2f38250872acde70afec463d4fffb05def835771ca29b7abe1cb62286944637b', 'Employee');
                INSERT INTO Reservations (RoomId, EmployeeId, EventId, ReservationDate, StartTime, EndTime) VALUES 
                (1, 1, 1, '2026-05-20', '10:00:00', '18:00:00'),  
                (2, 2, 3, '2026-06-05', '09:00:00', '17:00:00'),  
                (3, 3, 4, '2026-06-10', '11:00:00', '15:00:00'),  
                (4, 4, 2, '2026-05-25', '14:00:00', '22:00:00'),  
                (5, 5, 5, '2026-07-15', '12:00:00', '23:00:00'),  
                (1, 6, 6, '2026-08-01', '10:00:00', '19:00:00'),  
                (2, 2, 7, '2026-12-25', '18:00:00', '23:59:00'),  
                (3, 3, 8, '2026-09-18', '13:00:00', '20:00:00');  
                ");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Reservations", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Reservations", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Reservations", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "EventTypeId", "dbo.EventTypes");
            DropForeignKey("dbo.Employees", "PositionId", "dbo.Positions");
            DropIndex("dbo.Users", new[] { "EmployeeId" });
            DropIndex("dbo.Events", new[] { "EventTypeId" });
            DropIndex("dbo.Reservations", new[] { "EventId" });
            DropIndex("dbo.Reservations", new[] { "EmployeeId" });
            DropIndex("dbo.Reservations", new[] { "RoomId" });
            DropIndex("dbo.Employees", new[] { "PositionId" });
            DropTable("dbo.Users");
            DropTable("dbo.Rooms");
            DropTable("dbo.EventTypes");
            DropTable("dbo.Events");
            DropTable("dbo.Reservations");
            DropTable("dbo.Positions");
            DropTable("dbo.Employees");
        }
    }
}
