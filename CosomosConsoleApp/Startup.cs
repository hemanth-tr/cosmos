using System.Configuration;
using Azure.Identity;
using CosmosConsoleApp;
using CosomosConsoleApp.Options;
using CosomosConsoleApp.Apps;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CosomosConsoleApp.Services;

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
		var app = serviceProvider.GetService<IApp>();
		if (app == null)
		{
			Console.WriteLine("App is not available to execute");
			return;
		}

		await app.ExecuteAsync(args);
	}

	private static void ConfigureServices(this ServiceCollection collection, IConfiguration configuration)
	{
		collection.AddOptions<CosmosConfiguration>().Bind(configuration.GetSection("cosmos"));

		collection.AddTransient((serviceProvider) =>
		{
			var configuration = serviceProvider.GetService<IOptions<CosmosConfiguration>>()?.Value;
			if (configuration == null)
			{
				throw new ConfigurationErrorsException("configuration doesn't exist");
			}

			return new CosmosClient(configuration.Endpoint, configuration.AuthorizationKey);
		});

		collection.AddTransient<ICosmosService, CosmosDataService>();
		collection.AddTransient<IApp, DefaultCosmosApp>();
	}
}
