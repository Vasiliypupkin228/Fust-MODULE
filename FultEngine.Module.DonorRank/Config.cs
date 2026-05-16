using System.Collections.Generic;

namespace FultEngine.Module.DonorRank;

public class Config
{
	public string ApiUrl { get; set; } = "http://localhost:8090";


	public string ApiKey { get; set; } = "nezerhill-plugin-secret-2026";


	public Dictionary<string, string> Groups { get; set; } = new Dictionary<string, string>();


	public bool SendWelcomeHint { get; set; } = true;


	public string WelcomeMessage { get; set; } = string.Empty;

}
