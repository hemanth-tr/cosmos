using Microsoft.Azure.Cosmos;

namespace CosomosConsoleApp.Services;

public class CosmosDataService : ICosmosService
{
	public CosmosDataService(CosmosClient cosmosClient)
	{
		this.CosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
	}

	private CosmosClient CosmosClient { get; }

	private Container Container { get; set; }

	public async Task InitializeClientAsync(string database, string container, string partitionKey = "/id")
	{
		if (string.IsNullOrEmpty(database))
		{
			throw new ArgumentNullException(nameof(database));
		}

		if (string.IsNullOrEmpty(container))
		{
			throw new ArgumentNullException(nameof(container));
		}

		await this.CosmosClient.CreateDatabaseIfNotExistsAsync(database);
		var response = await this.CosmosClient.GetDatabase(database).CreateContainerIfNotExistsAsync(container, "/id");
		this.Container = response.Container;
	}

	public async Task<T> CreateItemAsync<T>(T item, string partitionKey)
	{
		this.ValidateContainerInitialization();
		this.ValidateItem(item);
		this.ValidatePartitionKey(partitionKey);

		var createdItem = await this.Container.CreateItemAsync(item, new PartitionKey(partitionKey));
		this.LogRequestCost(createdItem);
		return createdItem.Resource;
	}

	public async Task DeleteItemAsync(string id, string partitionKey)
	{
		this.ValidateContainerInitialization();
		this.ValidateId(id);
		this.ValidatePartitionKey(partitionKey);

		var deletedItem = await this.Container.DeleteItemAsync<object>(id, new PartitionKey(partitionKey));
		this.LogRequestCost(deletedItem);
	}

	public async Task<T> GetItemAsync<T>(string id, string partitionKey)
	{
		this.ValidateContainerInitialization();
		this.ValidateId(id);
		this.ValidatePartitionKey(partitionKey);

		var response = await this.Container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
		this.LogRequestCost(response);
		return response.Resource;
	}

	public async Task<IEnumerable<T>> GetItemsAsync<T>(string id)
	{
		string whereClause = string.IsNullOrWhiteSpace(id) ? string.Empty : " WHERE c.id = @id";
		string query = "SELECT * FROM c" + whereClause;
		this.ValidateContainerInitialization();

		var queryDefinition = new QueryDefinition(query);
		if (!string.IsNullOrWhiteSpace(id))
		{
			queryDefinition.WithParameter("@id", id);
		}

		var iterator = this.Container.GetItemQueryIterator<T>(queryDefinition);
		var results = new List<T>();

		while (iterator.HasMoreResults)
		{
			var response = await iterator.ReadNextAsync();
			this.LogRequestCost(response);
			if (response == null || response.Count == 0)
			{
				continue;
			}

			results.AddRange(response);
		}

		return results;
	}

	public async Task<T> UpdateItemAsync<T>(string id, string partitionKey, T item)
	{
		this.ValidateContainerInitialization();
		this.ValidateId(id);
		this.ValidatePartitionKey(partitionKey);
		this.ValidateItem(item);

		var response = await this.Container.ReplaceItemAsync(item, id, new PartitionKey(partitionKey));
		this.LogRequestCost(response);
		return response.Resource;
	}

	private void ValidateContainerInitialization()
	{
		if (this.Container == null)
		{
			throw new InvalidOperationException("CosmosDB container is not initialized. Call InitializeClientAsync first.");
		}
	}

	private void ValidateId(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			throw new ArgumentNullException(nameof(id), "Id cannot be null or empty.");
		}
	}

	private void ValidatePartitionKey(string partitionKey)
	{
		if (string.IsNullOrWhiteSpace(partitionKey))
		{
			throw new ArgumentNullException(nameof(partitionKey), "Partition key cannot be null or empty.");
		}
	}

	private void ValidateItem<T>(T item)
	{
		if (item == null)
		{
			throw new ArgumentNullException(nameof(item), "Item cannot be null.");
		}
	}

	private void LogRequestCost<T>(ItemResponse<T> itemResponse)
	{
		if (itemResponse == null)
		{
			Console.WriteLine("Request Charge is not available.");
			return;
		}

		Console.WriteLine($"Operation: {itemResponse.Diagnostics.GetType().Name}, Request Charge: {itemResponse.RequestCharge} RU/s");
	}

	private void LogRequestCost<T>(FeedResponse<T> itemResponse)
	{
		if (itemResponse == null)
		{
			Console.WriteLine("Request Charge is not available.");
			return;
		}

		Console.WriteLine($"Operation: {itemResponse.Diagnostics.GetType().Name}, Request Charge: {itemResponse.RequestCharge} RU/s");
	}
}
