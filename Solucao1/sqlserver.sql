CREATE DATABASE faturas_db COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

USE faturas_db;
GO

CREATE LOGIN faturas WITH PASSWORD = 'S3nh@Fatur@2024';
GO

CREATE USER faturas FOR LOGIN faturas;
GO

EXEC sp_addrolemember 'db_owner', 'faturas';
GO
