CREATE PROCEDURE [dbo].[spRpt_DetailedSales]
@id int = null -- Optional parameter to filter by Id
AS
	SELECT 
		ISNULL(s.Name, '') SalesName,
		ISNULL(p.Name, '') ProductName,
		ISNULL(sd.Quantity, 0) Quantity,
		ISNULL(sd.Price, 0) Price,
		ISNULL(s.TotalPrice, 0) TotalPrice
	FROM Sales s
	INNER JOIN SalesDetail sd ON s.Id = sd.SalesId
	INNER JOIN Products p ON sd.ProductId = p.Id
	WHERE s.Id = COALESCE(@id, s.Id) 

	GROUP BY
	s.Name, p.Name, sd.Quantity, sd.Price, s.TotalPrice
RETURN 0
