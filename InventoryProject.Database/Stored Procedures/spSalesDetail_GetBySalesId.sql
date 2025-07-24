CREATE PROCEDURE [dbo].[spSalesDetail_GetBySalesId]
	@id int
AS
	SET NOCOUNT ON;
	SELECT 
		ISNULL(sd.SalesId, '') SalesId, 
		ISNULL(sd.ProductId, '') ProductId, 
		ISNULL(p.Name, '') Name, 
		ISNULL(sd.Price,0) Price, 
		ISNULL(sd.Quantity,0) Quantity
	FROM 
		SalesDetail as sd
	INNER JOIN 
		Products as p ON sd.ProductId = p.Id
	WHERE 
		SalesId = @id;
RETURN 0
