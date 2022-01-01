using Abp.Data;
using Abp.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Odco.PointOfSales.EntityFrameworkCore.Repositories
{
    public class StoredProcedureAppService : PointOfSalesRepositoryBase<StockBalance, Guid>, IStoredProcedureAppService
    {
        private readonly IActiveTransactionProvider _transactionProvider;

        public StoredProcedureAppService(
            IDbContextProvider<PointOfSalesDbContext> dbContextProvider,
            IActiveTransactionProvider transactionProvider)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
        }

        public async Task<List<StockBalance>> GetStockBalancesByStockBalanceId(Guid[] stockBalanceIds)
        {
            try
            {
                await EnsureConnectionOpenAsync();

                var stockBalances = new List<StockBalance>();

                if(stockBalanceIds.Length > 0)
                {
                    // Comma seperated with inverted commas for each values
                    var arrayToString = string.Join(",", Array.ConvertAll(stockBalanceIds, x => string.Format("'{0}'", x.ToString())));

                    // Setting up the WHERE clause => Raw SQL condition
                    var wClause = string.Format("WHERE Id IN ({0})", arrayToString);

                    using (var command = CreateCommand("spGetStockBalancesByStockBalanceId", CommandType.StoredProcedure, new SqlParameter("wClause", wClause)))
                    {
                        using (var dataReader = await command.ExecuteReaderAsync())
                        {
                            while (dataReader.Read())
                            {
                                var sb = new StockBalance
                                {
                                    Id = SafeGetGuid(dataReader, "Id").Value,
                                    SequenceNumber = SafeGetInteger(dataReader, "SequenceNumber").Value,
                                    ProductId = SafeGetGuid(dataReader, "ProductId").Value,
                                    ProductCode = SafeGetString(dataReader, "ProductCode"),
                                    ProductName = SafeGetString(dataReader, "ProductName"),
                                    WarehouseId = SafeGetGuid(dataReader, "WarehouseId"),
                                    WarehouseCode = SafeGetString(dataReader, "WarehouseCode"),
                                    WarehouseName = SafeGetString(dataReader, "WarehouseName"),
                                    ExpiryDate = SafeGetDateTime(dataReader, "ExpiryDate"),
                                    BatchNumber = SafeGetString(dataReader, "BatchNumber"),
                                    ReceivedQuantity = SafeGetDecimal(dataReader, "ReceivedQuantity").Value,
                                    ReceivedQuantityUnitOfMeasureUnit = SafeGetString(dataReader, "ReceivedQuantityUnitOfMeasureUnit"),
                                    BookBalanceQuantity = SafeGetDecimal(dataReader, "BookBalanceQuantity").Value,
                                    BookBalanceUnitOfMeasureUnit = SafeGetString(dataReader, "BookBalanceUnitOfMeasureUnit"),
                                    OnOrderQuantity = SafeGetDecimal(dataReader, "OnOrderQuantity").Value,
                                    OnOrderQuantityUnitOfMeasureUnit = SafeGetString(dataReader, "OnOrderQuantityUnitOfMeasureUnit"),
                                    AllocatedQuantity = SafeGetDecimal(dataReader, "AllocatedQuantity").Value,
                                    AllocatedQuantityUnitOfMeasureUnit = SafeGetString(dataReader, "AllocatedQuantityUnitOfMeasureUnit"),
                                    CostPrice = SafeGetDecimal(dataReader, "CostPrice").Value,
                                    SellingPrice = SafeGetDecimal(dataReader, "SellingPrice").Value,
                                    MaximumRetailPrice = SafeGetDecimal(dataReader, "MaximumRetailPrice").Value,
                                    GoodsRecievedNumber = SafeGetString(dataReader, "GoodsRecievedNumber"),
                                    PurchaseOrderNumber = SafeGetString(dataReader, "PurchaseOrderNumber"),

                                    CreationTime = SafeGetDateTime(dataReader, "CreationTime").Value,
                                    CreatorUserId = SafeGetLong(dataReader, "CreatorUserId"),
                                    LastModificationTime = SafeGetDateTime(dataReader, "LastModificationTime"),
                                    LastModifierUserId = SafeGetLong(dataReader, "LastModifierUserId"),
                                    IsDeleted = SafeGetBoolean(dataReader, "IsDeleted"),
                                    DeleterUserId = SafeGetLong(dataReader, "DeleterUserId"),
                                    DeletionTime = SafeGetDateTime(dataReader, "DeletionTime")
                                };
                                stockBalances.Add(sb);
                            }

                            dataReader.Close();
                        }
                    }
                }

                return stockBalances;
            }
            catch (Exception ex)
            {
                throw new Exception("Sql Error => spGetStockBalancesByStockBalanceId");
            }

        }

        #region Methods which are used to convert data types
        private Guid? SafeGetGuid(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return Guid.Parse(reader[columnName].ToString());
            return null;
        }

        private string SafeGetString(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return reader[columnName].ToString();
            return null;
        }

        private decimal? SafeGetDecimal(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return Convert.ToDecimal(reader[columnName].ToString());
            return null;
        }

        private int? SafeGetInteger(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return int.Parse(reader[columnName].ToString());
            return null;
        }

        private DateTime? SafeGetDateTime(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return Convert.ToDateTime(reader[columnName]);
            return null;
        }

        private long? SafeGetLong(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return long.Parse(reader[columnName].ToString());
            return null;
        }

        private Boolean SafeGetBoolean(DbDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(columnName))
                return Boolean.Parse(reader[columnName].ToString());
            return false;
        }
        #endregion

        #region Private Methods
        private DbCommand CreateCommand(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var command = this.GetContext().Database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = GetActiveTransaction();

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        private async Task EnsureConnectionOpenAsync()
        {
            var connection = this.GetContext().Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
        }

        private DbTransaction GetActiveTransaction()
        {
            return (DbTransaction)_transactionProvider.GetActiveTransaction(new ActiveTransactionProviderArgs
            {
                {"ContextType", typeof(PointOfSalesDbContext) },
                {"MultiTenancySide", MultiTenancySide }
            });
        }
        #endregion

    }
}
