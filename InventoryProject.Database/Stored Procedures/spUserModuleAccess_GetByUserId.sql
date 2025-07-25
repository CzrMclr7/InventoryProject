CREATE PROCEDURE [dbo].[spUserModuleAccess_GetByUserId]
	@userId int = 0
AS
    SELECT 
        uma.Id,
        m.ModuleName,
        m.ModuleCode,
        uma.ModuleId,
        uma.CanCreate,
        uma.CanEdit,
        uma.CanDelete,
        uma.CanView
    FROM 
        UserModuleAccess uma
    INNER JOIN 
        Module m ON uma.ModuleId = m.Id
    WHERE 
        uma.UserId = @userId
RETURN 0
