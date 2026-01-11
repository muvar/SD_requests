-- Начальные данные для системы отслеживания заявок
-- SQL Server

USE TicketTrackerDB;
GO

-- Вставка администратора по умолчанию
IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'admin')
BEGIN
    INSERT INTO Users (Login, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('admin', '$2a$11$8K1p/a0dL2LkqvQOuuHDMOaZf4JmqV8n3/8RjYwjZQGzFqJQzQzQy', 'Системный администратор', 4, 1, GETDATE());
    -- Пароль: admin123 (хешированный с помощью BCrypt)
END
GO

-- Создание тестовых пользователей для демонстрации
IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'operator1')
BEGIN
    INSERT INTO Users (Login, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('operator1', '$2a$11$8K1p/a0dL2LkqvQOuuHDMOaZf4JmqV8n3/8RjYwjZQGzFqJQzQzQy', 'Иван Петров', 1, 1, GETDATE());
    -- Пароль: password123
END
GO

IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'specialist1')
BEGIN
    INSERT INTO Users (Login, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('specialist1', '$2a$11$8K1p/a0dL2LkqvQOuuHDMOaZf4JmqV8n3/8RjYwjZQGzFqJQzQzQy', 'Мария Сидорова', 2, 1, GETDATE());
    -- Пароль: password123
END
GO

IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'executor1')
BEGIN
    INSERT INTO Users (Login, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('executor1', '$2a$11$8K1p/a0dL2LkqvQOuuHDMOaZf4JmqV8n3/8RjYwjZQGzFqJQzQzQy', 'Алексей Козлов', 3, 1, GETDATE());
    -- Пароль: password123
END
GO

IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'executor2')
BEGIN
    INSERT INTO Users (Login, PasswordHash, FullName, Role, IsActive, CreatedAt)
    VALUES ('executor2', '$2a$11$8K1p/a0dL2LkqvQOuuHDMOaZf4JmqV8n3/8RjYwjZQGzFqJQzQzQy', 'Елена Волкова', 3, 1, GETDATE());
    -- Пароль: password123
END
GO

-- Создание тестовых заявок для демонстрации
DECLARE @OperatorId INT = (SELECT Id FROM Users WHERE Login = 'operator1');
DECLARE @SpecialistId INT = (SELECT Id FROM Users WHERE Login = 'specialist1');
DECLARE @ExecutorId INT = (SELECT Id FROM Users WHERE Login = 'executor1');

IF NOT EXISTS (SELECT * FROM Tickets WHERE Title = 'Проблема с принтером')
BEGIN
    INSERT INTO Tickets (Title, Description, Status, Priority, CreatedById, CreatedAt)
    VALUES ('Проблема с принтером', 'Принтер в офисе не печатает документы. Требуется диагностика и ремонт.', 1, 2, @OperatorId, GETDATE());
    
    DECLARE @TicketId INT = SCOPE_IDENTITY();
    
    -- Добавление записи в историю
    INSERT INTO TicketHistory (TicketId, UserId, Action, Description, CreatedAt)
    VALUES (@TicketId, @OperatorId, 'Создана заявка', 'Заявка создана: Проблема с принтером', GETDATE());
END
GO

IF NOT EXISTS (SELECT * FROM Tickets WHERE Title = 'Установка нового ПО')
BEGIN
    INSERT INTO Tickets (Title, Description, Status, Priority, CreatedById, AssignedToId, CreatedAt)
    VALUES ('Установка нового ПО', 'Необходимо установить новую версию антивируса на все рабочие станции.', 2, 3, @OperatorId, @ExecutorId, GETDATE());
    
    DECLARE @TicketId2 INT = SCOPE_IDENTITY();
    
    -- Добавление записей в историю
    INSERT INTO TicketHistory (TicketId, UserId, Action, Description, CreatedAt)
    VALUES (@TicketId2, @OperatorId, 'Создана заявка', 'Заявка создана: Установка нового ПО', GETDATE());
    
    INSERT INTO TicketHistory (TicketId, UserId, Action, OldValue, NewValue, Description, CreatedAt)
    VALUES (@TicketId2, @SpecialistId, 'Назначение исполнителя', 'Не назначен', 'Алексей Козлов', 'Заявка назначена исполнителю', GETDATE());
END
GO

IF NOT EXISTS (SELECT * FROM Tickets WHERE Title = 'Настройка сетевого доступа')
BEGIN
    INSERT INTO Tickets (Title, Description, Status, Priority, CreatedById, AssignedToId, CreatedAt, ClosedAt, Resolution)
    VALUES ('Настройка сетевого доступа', 'Настроить доступ к сетевым папкам для нового сотрудника.', 5, 1, @OperatorId, @ExecutorId, DATEADD(day, -2, GETDATE()), DATEADD(day, -1, GETDATE()), 'Доступ настроен, пользователь добавлен в соответствующие группы безопасности.');
    
    DECLARE @TicketId3 INT = SCOPE_IDENTITY();
    
    -- Добавление записей в историю
    INSERT INTO TicketHistory (TicketId, UserId, Action, Description, CreatedAt)
    VALUES (@TicketId3, @OperatorId, 'Создана заявка', 'Заявка создана: Настройка сетевого доступа', DATEADD(day, -2, GETDATE()));
    
    INSERT INTO TicketHistory (TicketId, UserId, Action, OldValue, NewValue, Description, CreatedAt)
    VALUES (@TicketId3, @SpecialistId, 'Назначение исполнителя', 'Не назначен', 'Алексей Козлов', 'Заявка назначена исполнителю', DATEADD(day, -2, GETDATE()));
    
    INSERT INTO TicketHistory (TicketId, UserId, Action, OldValue, NewValue, Description, CreatedAt)
    VALUES (@TicketId3, @ExecutorId, 'Закрытие заявки', 'InProgress', 'Закрыта', 'Заявка закрыта. Решение: Доступ настроен, пользователь добавлен в соответствующие группы безопасности.', DATEADD(day, -1, GETDATE()));
END
GO

-- Добавление тестовых комментариев
DECLARE @TestTicketId INT = (SELECT TOP 1 Id FROM Tickets WHERE Title = 'Установка нового ПО');
DECLARE @TestExecutorId INT = (SELECT Id FROM Users WHERE Login = 'executor1');

IF @TestTicketId IS NOT NULL AND NOT EXISTS (SELECT * FROM Comments WHERE TicketId = @TestTicketId)
BEGIN
    INSERT INTO Comments (TicketId, UserId, Text, CreatedAt)
    VALUES (@TestTicketId, @TestExecutorId, 'Начал работу над заявкой. Планирую завершить до конца дня.', GETDATE());
    
    INSERT INTO Comments (TicketId, UserId, Text, CreatedAt)
    VALUES (@TestTicketId, @SpecialistId, 'Хорошо, держите меня в курсе прогресса.', GETDATE());
END
GO

PRINT 'Начальные данные успешно добавлены!';
PRINT 'Тестовые учетные записи:';
PRINT 'Администратор: admin / admin123';
PRINT 'Оператор: operator1 / password123';
PRINT 'Специалист: specialist1 / password123';
PRINT 'Исполнитель: executor1 / password123';
PRINT 'Исполнитель: executor2 / password123';
GO
