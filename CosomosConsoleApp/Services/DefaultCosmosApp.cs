using Microsoft.Azure.Cosmos;

namespace CosomosConsoleApp.Services
{
    internal class DefaultCosmosApp : IApp
    {
        private CosmosClient _client;

        public DefaultCosmosApp(CosmosClient cosmosClient)
        {
            _client = cosmosClient;
        }

        public async Task ExecuteAsync(string[] args)
        {

            var database = await _client.CreateDatabaseIfNotExistsAsync("default", throughput: 400);
            var container = await database.Database.CreateContainerIfNotExistsAsync("users", "/id");
        }
    }
}
