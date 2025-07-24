CREATE PROCEDURE [dbo].[spRpt_ProductAdjustment]
	@actionType VARCHAR(10) = ''
AS
    SELECT 
        pa.SalesDetailId,
        p.Name,
        pa.Action,
        pa.Quantity,
        pa.DateCreated
    FROM 
        ProductAdjustment pa
    INNER JOIN 
        Products p ON p.Id = pa.ProductId
    WHERE 
        1 = CASE 
                WHEN @actionType = 'All' THEN 1
                WHEN @actionType = 'IN' AND pa.Action = 'IN' THEN 1
                WHEN @actionType = 'OUT' AND pa.Action = 'OUT' THEN 1
                ELSE 0
            END
    ORDER BY 
        pa.DateCreated DESC;
RETURN 0
