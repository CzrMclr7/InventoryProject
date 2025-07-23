CREATE TABLE [dbo].[UserAccess]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [ModuleId] NCHAR(10) NULL, 
    [CanCreate] BIT NULL, 
    [CanEdit] BIT NULL, 
    [CanDelete] BIT NULL, 
    [CanView] BIT NULL, 
    [CreatedById] INT NOT NULL, 
    [DateCreated] DATETIME2 NOT NULL, 
    [ModifiedById] INT NULL, 
    [DateModified] DATETIME2 NULL,
    CONSTRAINT FK_UserAccess_User FOREIGN KEY (UserId) REFERENCES [User](Id)
)
