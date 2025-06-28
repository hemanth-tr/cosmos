using System;
using Newtonsoft.Json;

namespace CosomosConsoleApp.Models;

public class User
{
	[JsonProperty("id")]
	public string Id { get; set; }
	[JsonProperty("name")]
	public string Name { get; set; }
	[JsonProperty("email")]
	public string Email { get; set; }
	[JsonProperty("createdAt")]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
