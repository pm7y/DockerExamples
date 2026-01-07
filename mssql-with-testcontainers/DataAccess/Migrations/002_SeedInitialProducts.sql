-- Migration: 002_SeedInitialProducts
-- Seeds the database with some initial product data

INSERT INTO Products (Name, Price)
VALUES ('Widget', 9.99),
       ('Gadget', 19.99),
       ('Gizmo', 29.99);
