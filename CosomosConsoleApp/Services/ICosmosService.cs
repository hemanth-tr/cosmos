using System;

namespace CosomosConsoleApp.Services;

public interface ICosmosService
{
	Task InitializeClientAsync(string database, string container, string partitionKey = "/id");
	Task<T> CreateItemAsync<T>(T item, string partitionKey);
	Task<T> GetItemAsync<T>(string id, string partitionKey);
	Task<T> UpdateItemAsync<T>(string id, string partitionKey, T item);
	Task DeleteItemAsync(string id, string partitionKey);
	Task<IEnumerable<T>> GetItemsAsync<T>(string query);
}
