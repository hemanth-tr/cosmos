using System;
using Newtonsoft.Json;

namespace CosomosConsoleApp.Models;

public class SimpleObject
{
	[JsonProperty("id")]
	public string Id { get; set; }
	[JsonProperty("name")]
	public string Name { get; set; }
}
