-- Migration: 001_CreateProductsTable
-- Creates the Products table for storing product catalog data

CREATE TABLE Products
(
    Id        INT IDENTITY (1,1) PRIMARY KEY,
    Name      NVARCHAR(100)  NOT NULL,
    Price     DECIMAL(18, 2) NOT NULL,
    CreatedAt DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

-- Create index for common queries
CREATE INDEX IX_Products_Name ON Products (Name);
