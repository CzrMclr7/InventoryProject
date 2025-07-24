CREATE PROCEDURE [dbo].[spSales_Inq] 
	@id int = null -- Optional parameter to filter by Id
 AS 
	SELECT 
		*  
	FROM 
		Sales 
	WHERE 
		Id = COALESCE(@id, Id) 
RETURN 0