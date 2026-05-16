using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.API.Libraries.SSBinds;
using FultEngine.LoaderModule;
using HintServiceMeow.Core.Enum;
using PlayerRoles;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.Module.Escape;

public class Plugin : IFultEngineModule
{
	public string Name => "Escape";

	public string Author => "FUST";

	public Version Version => new Version(0, 0, 1);

	public void OnEnabled()
	{
		KeybindManager.AddCustomKeybind(7, "╔ <color=#DC143C>\ud83c\udfc3</color> Сбежать из комплекса на улицу", (KeyCode)101, preventInteractionOnGUI: false, "");
		Player.Escaping += (CustomEventHandler<EscapingEventArgs>)OnEscaping;
		Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
		ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
	}

	public void OnDisabled()
	{
		Player.Escaping -= (CustomEventHandler<EscapingEventArgs>)OnEscaping;
		Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
		ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
	}

	public Type GetConfigType()
	{
		return null;
	}

	public object GetDefaultConfig()
	{
		return null;
	}

	public void SetConfig(object config)
	{
	}

	public void OnRoundStarted()
	{
		foreach (Door item in Door.List)
		{
			if ((int)item.Type == 59)
			{
				item.Unlock();
				item.IsOpen = true;
			}
		}
	}

	private void OnEscaping(EscapingEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			ev.IsAllowed = false;
		}
	}

	private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		if (hub.IsHost)
		{
			return;
		}
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val == null || ((ServerSpecificSettingBase)val).SettingId != 7 || !val.SyncIsPressed)
		{
			return;
		}
		Player val2 = Player.Get(hub);
		if (!(val2 == (Player)null) && !val2.IsDead)
		{
			Vector3 center = default(Vector3);
			((Vector3)(ref center))._002Ector(-40.91f, 291.881f, -36.031f);
			Vector3 center2 = default(Vector3);
			((Vector3)(ref center2))._002Ector(128.171f, 288.791f, 28.495f);
			if (IsPlayerInRadius(center, 3f, val2) || IsPlayerInRadius(center2, 3f, val2))
			{
				val2.Role.Set((RoleTypeId)2, (SpawnReason)7);
			}
			else
			{
				val2.ShowMeowHint(5f, "<size=29><b><color=#61616193>|</color></size> <size=19>Чтобы сбежать, нужно подойти к двери на выходе</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
			}
		}
	}

	private bool IsPlayerInRadius(Vector3 center, float radius, Player player)
	{
		return Vector3.Distance(player.Position, center) <= radius;
	}
}
