CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Username] NVARCHAR(50) NOT NULL, 
    [Password] NVARCHAR(100) NOT NULL, 
    [PasswordSalt] NVARCHAR(50) NOT NULL, 
    [CreatedById] INT NOT NULL, 
    [DateCreated] DATETIME2 NOT NULL, 
    [ModifiedById] INT NULL, 
    [DateModified] DATETIME2 NULL, 
    [FirstName] NVARCHAR(50) NULL, 
    [LastName] NVARCHAR(50) NULL, 
    [Age] INT NULL, 
    [Email] NVARCHAR(100) NULL, 
    [IsAdmin] BIT NULL, 
    [ProfilePicture] NVARCHAR(255) NULL, 
    [PhoneNumber] INT NULL
)
