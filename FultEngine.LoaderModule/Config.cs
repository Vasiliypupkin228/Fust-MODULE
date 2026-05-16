using Exiled.API.Interfaces;
using YamlDotNet.Serialization;

namespace FultEngine.LoaderModule;

public class Config : IConfig
{
	[YamlIgnore]
	public bool IsEnabled { get; set; } = true;

	[YamlIgnore]
	public bool Debug { get; set; } = false;
}
