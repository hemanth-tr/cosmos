using Azure.Identity;
using CosmosConsoleApp;
using CosomosConsoleApp.Models;
using CosomosConsoleApp.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CosomosConsoleApp;

public static class Startup
{
	public static async Task InitializeAsync(string[] args)
	{
		var builder = new ConfigurationBuilder();
		builder.SetBasePath(Directory.GetCurrentDirectory());
		builder.AddJsonFile("appsettings.json");
		builder.AddUserSecrets<Program>();

		var configuration = builder.Build();
		var serviceCollection = new ServiceCollection();
		serviceCollection.ConfigureServices(configuration);
		var serviceProvider = serviceCollection.BuildServiceProvider();

		await serviceProvider.GetService<IApp>().ExecuteAsync(args);
	}

	private static void ConfigureServices(this ServiceCollection collection, IConfiguration configuration)
	{
		collection.AddOptions<CosmosConfiguration>().Bind(configuration.GetSection("cosmos"));

		collection.AddTransient((serviceProvider) =>
		{
			var configuration = serviceProvider.GetService<IOptions<CosmosConfiguration>>().Value;
			return new CosmosClient(configuration.Endpoint, new DefaultAzureCredential());
        });
		collection.AddTransient<IApp, DefaultCosmosApp>();
	}
}
