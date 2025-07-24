-- Insert initial seed data into Products table 
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products]) 
BEGIN 
    PRINT 'Seeding initial Products...'; 

    INSERT INTO [dbo].[Products] (Name, Qty, Price, DateCreated, CreatedById) 
    VALUES  
    ('Laptop', 10, 49999.99, GETDATE(), 1), 
    ('Monitor', 25, 8999.99, GETDATE(), 1), 
    ('Mouse', 100, 599.99, GETDATE(), 1), 
    ('Keyboard', 75, 999.99, GETDATE(), 1), 
    ('Webcam', 30, 2499.99, GETDATE(), 1); 
END
ELSE
BEGIN 
    PRINT 'Products table already seeded.'; 
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Module]) 
BEGIN 
    PRINT 'Seeding initial Modules...'; 

    INSERT INTO [dbo].[Module] (ModuleName, ModuleCode) 
    VALUES  
    ('Sales', 'SALES'), 
    ('Product', 'PRODUCT'), 
    ('Product Adjustment', 'ADJUSTMENT'); 
END
ELSE
BEGIN 
    PRINT 'Modules table already seeded.'; 
END
GO
