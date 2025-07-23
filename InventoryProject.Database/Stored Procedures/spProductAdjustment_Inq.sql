CREATE PROCEDURE [dbo].[spProductAdjustment_Inq] 
	@id int = null -- Optional parameter to filter by Id
 AS 
	SELECT 
		pa.Id,
		p.Name,
		pa.ProductId,
		pa.Action,
		pa.Quantity,
		pa.DateCreated
	FROM ProductAdjustment pa
	INNER JOIN Products p ON p.Id = pa.ProductId
	WHERE pa.Id = COALESCE(@id, pa.Id) 
	ORDER BY pa.DateCreated DESC
RETURN 0