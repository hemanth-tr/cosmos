using CosomosConsoleApp;

namespace CosmosConsoleApp;

public class Program
{
	public static async Task Main(string[] args)
	{
		await Startup.InitializeAsync(args);
		Console.Read();
	}
}
