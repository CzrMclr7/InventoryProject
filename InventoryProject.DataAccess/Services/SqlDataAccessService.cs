using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace InventoryProject.DataAccess.Services;

public class SqlDataAccessService : ISqlDataAccessService
{
    private readonly IConfiguration _config;

    public SqlDataAccessService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> LoadData<T, U>(
        string storedProcedure,
        U parameters,
        CommandType commandType = CommandType.StoredProcedure,
        string connectionId = "Default")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: commandType);
    }

    public async Task<T> LoadSingle<T, U>(
        string storedProcedure,
        U parameters,
        CommandType commandType = CommandType.StoredProcedure,
        string connectionId = "Default")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        var result = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: commandType);

        return result.FirstOrDefault();
    }

    public async Task<List<object>> LoadMultiTypedAsync(
        string storedProcedure,
        Type[] resultTypes,
        object parameters = null,
        CommandType commandType = CommandType.StoredProcedure,
    string connectionId = "Default")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        using var multi = await connection.QueryMultipleAsync(storedProcedure, parameters, commandType: commandType);

        var results = new List<object>();

        foreach (var type in resultTypes)
        {
            var method = typeof(SqlMapper).GetMethods()
                .First(m => m.Name == "Read" && m.IsGenericMethod && m.GetParameters().Length == 1)
                .MakeGenericMethod(type);

            var result = method.Invoke(null, new object[] { multi });
            results.Add(((IEnumerable<object>)result).ToList());
        }

        return results;
    }

    public async Task ExecuteAsync<T>(string storedProcedure, T parameters, string connectionId = "Default")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            await connection.ExecuteAsync(storedProcedure, parameters, transaction, commandType: CommandType.StoredProcedure);
            transaction.Commit();
        }
        catch (Exception) { transaction.Rollback(); throw; }
    }

    public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            await connection.ExecuteAsync(storedProcedure, parameters, transaction, commandType: CommandType.StoredProcedure);
            transaction.Commit();
        }
        catch (Exception) { transaction.Rollback(); throw; }
    }
}