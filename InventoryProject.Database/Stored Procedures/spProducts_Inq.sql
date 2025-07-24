CREATE PROCEDURE [dbo].[spProducts_Inq] 
	@id int = null -- Optional parameter to filter by Id
 AS 
	SELECT 
		*  
	FROM 
		Products 
	WHERE 
		Id = COALESCE(@id, Id) 
RETURN 0