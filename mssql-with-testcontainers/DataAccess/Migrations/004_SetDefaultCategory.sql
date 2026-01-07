-- Migration: 004_SetDefaultCategory
-- Sets a default category for existing products

UPDATE Products
SET Category = 'General'
WHERE Category IS NULL;
