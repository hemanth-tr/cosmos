using CosomosConsoleApp.Models;
using CosomosConsoleApp.Services;

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
        await this.CreateSimpleObjectAsync();
        await this.ReadItemAsync();
        await this.ReadItemsAsync();
    }

    private async Task CreateUserAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Hemanth T R",
            Email = ""
        };
        var response = await this._cosmosDataService.CreateItemAsync(user, user.Id);
        Console.WriteLine(response);
    }

    private async Task CreateSimpleObjectAsync()
    {
        var user = new SimpleObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Hemanth T R"
        };
        var response = await this._cosmosDataService.CreateItemAsync(user, user.Id);
        Console.WriteLine(response);
    }

    private async Task ReadItemsAsync()
    {
        var id = "0da047fd-4582-40d9-933a-bcaa249ddd10";
        var response = await this._cosmosDataService.GetItemsAsync<User>(id);

    }

    private async Task ReadItemAsync()
    {
        var id = "0da047fd-4582-40d9-933a-bcaa249ddd10";
        var response = await this._cosmosDataService.GetItemAsync<User>(id, id);

    }
}