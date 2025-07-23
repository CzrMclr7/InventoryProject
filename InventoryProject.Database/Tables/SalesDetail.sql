CREATE TABLE [dbo].[SalesDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SalesId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [DateCreated] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [CreatedById] INT NOT NULL, 
    [DateModified] DATETIME2 NULL, 
    [ModifiedById] INT NULL, 
    [Quantity] INT NOT NULL, 
    [Price] DECIMAL(18, 2) NOT NULL, 
    CONSTRAINT FK_SalesDetail_Sales FOREIGN KEY (SalesId) REFERENCES Sales(Id),
    CONSTRAINT FK_SalesDetail_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
)
