using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FultEngine.Module.DoorTeleport.Features;

public static class Frequency
{
	public static readonly string ConfigPath = Path.Combine(Paths.Plugins, "PluginCoreDoorSystem");

	public static readonly int KeybindSettingId = 1;

	public const float MaxRayDistance = 2.5f;

	private static readonly IDeserializer Deserializer = ((BuilderSkeleton<DeserializerBuilder>)new DeserializerBuilder()).WithNamingConvention(CamelCaseNamingConvention.Instance).IgnoreUnmatchedProperties().Build();

	public static List<DoorPairConfig> LoadAllDoorPairs()
	{
		List<DoorPairConfig> list = new List<DoorPairConfig>();
		if (!Directory.Exists(ConfigPath))
		{
			Directory.CreateDirectory(ConfigPath);
		}
		string[] files = Directory.GetFiles(ConfigPath, "door_*.yml");
		string[] array = files;
		foreach (string path in array)
		{
			try
			{
				string text = File.ReadAllText(path);
				DoorPairConfig doorPairConfig = Deserializer.Deserialize<DoorPairConfig>(text);
				if (doorPairConfig != null && !string.IsNullOrWhiteSpace(doorPairConfig.SchematicName1) && !string.IsNullOrWhiteSpace(doorPairConfig.SchematicName2))
				{
					list.Add(doorPairConfig);
				}
			}
			catch
			{
			}
		}
		return list;
	}
}
