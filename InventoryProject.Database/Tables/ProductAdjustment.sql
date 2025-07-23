CREATE TABLE [dbo].[ProductAdjustment]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductId] INT NOT NULL, 
    [Action] NVARCHAR(50) NOT NULL, 
    [Quantity] INT NULL, 
    [SalesDetailId] INT NULL,
    [DateCreated] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NOT NULL, 
    [DateModified] DATETIME2 NULL, 
    [ModifiedById] INT NULL, 
    CONSTRAINT FK_ProductAdjusment_SalesDetail FOREIGN KEY (SalesDetailId) REFERENCES SalesDetail(Id),
    CONSTRAINT FK_ProductAdjusment_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
)
