using CosomosConsoleApp.Services;
using Microsoft.Azure.Cosmos;

namespace CosomosConsoleApp.Apps;

internal class DefaultCosmosApp : IApp
{
    private ICosmosService _cosmosDataService;

    public DefaultCosmosApp(ICosmosService cosmosService)
    {
        _cosmosDataService = cosmosService ?? throw new ArgumentNullException(nameof(cosmosService));
        _cosmosDataService.InitializeClientAsync("default", "users", "/id").GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(string[] args)
    {
        var response = await this._cosmosDataService.GetItemsAsync<User>("SELECT * FROM c");
        Console.WriteLine(response.Count());
    }
}