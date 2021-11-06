CREATE PROCEDURE spGetStockBalancesByStockBalanceId @wClause nvarchar(MAX)
AS
BEGIN
	CREATE TABLE TempTable(Id uniqueidentifier)  

	DECLARE 
	@id uniqueidentifier,
    @productId uniqueidentifier,
    @warehouseId uniqueidentifier,
	@command NVARCHAR(MAX) = '
	DECLARE cursor_sb CURSOR FOR 
		SELECT [Id], [ProductId], [WarehouseId] FROM [dbo].[Inventory.StockBalance] ' + @wClause;

	Execute SP_ExecuteSQL @command

	OPEN cursor_sb;

	FETCH NEXT FROM cursor_sb INTO @id, @productId, @warehouseId;

	WHILE @@FETCH_STATUS = 0
		BEGIN
			PRINT @id;

			INSERT INTO TempTable (Id)
				SELECT Id FROM [dbo].[Inventory.StockBalance] WHERE (Id = @id ) OR SequenceNumber = 0 AND ProductId = @productId AND (WarehouseId = @warehouseId OR WarehouseId IS NULL)

			FETCH NEXT FROM cursor_sb INTO @id, @productId, @warehouseId;
		END;

	CLOSE cursor_sb;

	DEALLOCATE cursor_sb;
	
	SELECT * FROM [dbo].[Inventory.StockBalance] Where Id IN (SELECT DISTINCT Id FROM TempTable)

	DROP TABLE TempTable

End 