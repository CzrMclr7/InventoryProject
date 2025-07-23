using System.Data;

namespace InventoryProject.DataAccess.Services;

public interface ISqlDataAccessService
{
    Task ExecuteAsync<T>(string storedProcedure, T parameters, string connectionId = "Default");

    Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, CommandType commandType = CommandType.StoredProcedure, string connectionId = "Default");

    Task<List<object>> LoadMultiTypedAsync(string storedProcedure, Type[] resultTypes, object parameters = null, CommandType commandType = CommandType.StoredProcedure, string connectionId = "Default");

    Task<T> LoadSingle<T, U>(string storedProcedure, U parameters, CommandType commandType = CommandType.StoredProcedure, string connectionId = "Default");

    Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default");
}