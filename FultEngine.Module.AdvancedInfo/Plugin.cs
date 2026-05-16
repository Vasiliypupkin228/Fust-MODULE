using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;

namespace FultEngine.Module.AdvancedInfo;

public class Plugin : IFultEngineModule
{
	public Config _config;

	private static IFultEngineModule _instance;

	public string Name { get; } = "AdvancedCustomInfo";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 0, 1);


	public void OnEnabled()
	{
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
	}

	public void OnDisabled()
	{
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config();
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}

	private void OnPlayerSpawned(SpawnedEventArgs ev)
	{
		if (!_config.IsEnabled || ev.Player == (Player)null)
		{
			return;
		}
		Timing.CallDelayed(3f, (Action)delegate
		{
			if (!ev.Player.IsScp)
			{
				ev.Player.CustomInfo = FormatDefaultInfo(ev.Player);
			}
			else
			{
				ev.Player.CustomInfo = "";
			}
		});
	}

	private string FormatDefaultInfo(Player player)
	{
		SubClassData value;
		string newValue = (FultEngine.Module.Plugin.PlayerSubclasses.TryGetValue(player, out value) ? value.Id : "-");
		return _config.DefaultInfoFormat.Replace("{PlayerId}", player.Id.ToString()).Replace("{Nickname}", player.Nickname).Replace("{Role}", newValue);
	}

	public static IFultEngineModule GetInstance()
	{
		return _instance;
	}

	public Plugin()
	{
		_instance = this;
	}
}
