-- Создание базы данных для системы отслеживания заявок
-- SQL Server

-- Создание базы данных
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TicketTrackerDB')
BEGIN
    CREATE DATABASE TicketTrackerDB;
END
GO

USE TicketTrackerDB;
GO

-- Создание таблицы пользователей
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Login NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        Role INT NOT NULL, -- 1=Operator, 2=Specialist, 3=Executor, 4=Administrator
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        LastLoginAt DATETIME2 NULL
    );
END
GO

-- Создание таблицы заявок
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tickets' AND xtype='U')
BEGIN
    CREATE TABLE Tickets (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(2000) NOT NULL,
        Status INT NOT NULL DEFAULT 1, -- 1=New, 2=InProgress, 3=Completed, 4=Postponed, 5=Closed
        Priority INT NOT NULL DEFAULT 2, -- 1=Low, 2=Medium, 3=High, 4=Critical
        CreatedById INT NOT NULL,
        AssignedToId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        ClosedAt DATETIME2 NULL,
        Resolution NVARCHAR(500) NULL,
        
        CONSTRAINT FK_Tickets_CreatedBy FOREIGN KEY (CreatedById) REFERENCES Users(Id),
        CONSTRAINT FK_Tickets_AssignedTo FOREIGN KEY (AssignedToId) REFERENCES Users(Id)
    );
END
GO

-- Создание таблицы комментариев
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comments' AND xtype='U')
BEGIN
    CREATE TABLE Comments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TicketId INT NOT NULL,
        UserId INT NOT NULL,
        Text NVARCHAR(1000) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsInternal BIT NOT NULL DEFAULT 0,
        
        CONSTRAINT FK_Comments_Ticket FOREIGN KEY (TicketId) REFERENCES Tickets(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Comments_User FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO

-- Создание таблицы истории заявок
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TicketHistory' AND xtype='U')
BEGIN
    CREATE TABLE TicketHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TicketId INT NOT NULL,
        UserId INT NOT NULL,
        Action NVARCHAR(100) NOT NULL,
        OldValue NVARCHAR(500) NULL,
        NewValue NVARCHAR(500) NULL,
        Description NVARCHAR(1000) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT FK_TicketHistory_Ticket FOREIGN KEY (TicketId) REFERENCES Tickets(Id) ON DELETE CASCADE,
        CONSTRAINT FK_TicketHistory_User FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO

-- Создание индексов для оптимизации производительности
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Login')
BEGIN
    CREATE INDEX IX_Users_Login ON Users(Login);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tickets_Status')
BEGIN
    CREATE INDEX IX_Tickets_Status ON Tickets(Status);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tickets_CreatedById')
BEGIN
    CREATE INDEX IX_Tickets_CreatedById ON Tickets(CreatedById);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tickets_AssignedToId')
BEGIN
    CREATE INDEX IX_Tickets_AssignedToId ON Tickets(AssignedToId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Comments_TicketId')
BEGIN
    CREATE INDEX IX_Comments_TicketId ON Comments(TicketId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TicketHistory_TicketId')
BEGIN
    CREATE INDEX IX_TicketHistory_TicketId ON TicketHistory(TicketId);
END
GO

PRINT 'База данных TicketTrackerDB успешно создана!';
GO
