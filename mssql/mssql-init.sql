
DROP DATABASE IF EXISTS [WorldWideImporters];
GO

DROP DATABASE IF EXISTS [ScratchDb];
GO

CREATE DATABASE [ScratchDb];
GO

USE [ScratchDb];
GO

CREATE TABLE ScratchTable (
    Id UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    PRIMARY KEY (Id)
);
GO

INSERT INTO [ScratchTable] ([Id], [Name])
VALUES
(NEWID(), 'John'),
(NEWID(), 'Jane');
GO
