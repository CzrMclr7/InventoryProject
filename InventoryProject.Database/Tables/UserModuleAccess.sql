CREATE TABLE [dbo].[UserModuleAccess]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [ModuleId] INT NOT NULL, 
    [CanCreate] BIT NULL, 
    [CanEdit] BIT NULL, 
    [CanDelete] BIT NULL, 
    [CanView] BIT NULL, 
    [CreatedById] INT NOT NULL, 
    [DateCreated] DATETIME2 NOT NULL, 
    [ModifiedById] INT NULL, 
    [DateModified] DATETIME2 NULL,
    CONSTRAINT FK_UserModuleAccess_User FOREIGN KEY (UserId) REFERENCES [User](Id),
    CONSTRAINT FK_UserModuleAccess_Module FOREIGN KEY (ModuleId) REFERENCES [Module](Id)
)
