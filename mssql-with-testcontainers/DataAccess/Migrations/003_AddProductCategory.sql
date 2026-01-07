-- Migration: 003_AddProductCategory
-- Adds a Category column to the Products table

ALTER TABLE Products
    ADD Category NVARCHAR(50) NULL;
