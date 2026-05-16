using System;
using Exiled.API.Features;
using FultEngine.API.Libraries.SSBinds;
using FultEngine.LoaderModule;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.Module.Reanimate;

public class Plugin : IFultEngineModule
{
	private Config _config;

	private readonly ReanimationManager _reanimationManager = new ReanimationManager();

	public string Name => "Reanimate";

	public string Author => "FUST";

	public Version Version => new Version(0, 0, 1);

	public void OnEnabled()
	{
		if (_config.IsEnabled)
		{
			KeybindManager.AddCustomKeybind(1, "[2] | Бинд для реанимации игрока", (KeyCode)0, preventInteractionOnGUI: false, "");
			KeybindManager.Initialize();
			KeybindManager.OnObjectInteraction += OnKeybindPressed;
		}
	}

	public void OnDisabled()
	{
		KeybindManager.OnObjectInteraction -= OnKeybindPressed;
		KeybindManager.Uninitialize();
		_reanimationManager.Clear();
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

	private void OnKeybindPressed(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		if (_config.IsEnabled && !hub.IsHost)
		{
			SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
			if (val != null && setting.SettingId == 1 && val.SyncIsPressed)
			{
				Player player = Player.Get(hub);
				_reanimationManager.TryStartReanimation(player, _config);
			}
		}
	}
}
