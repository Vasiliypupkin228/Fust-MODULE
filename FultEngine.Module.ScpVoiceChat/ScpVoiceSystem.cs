using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace FultEngine.Module.ScpVoiceChat;

public class ScpVoiceSystem : IDisposable
{
	private readonly Dictionary<Player, bool> _scpVoiceEnabled = new Dictionary<Player, bool>();

	private int _keybindId = 400;

	public void SubscribeEvents()
	{
		Player.TogglingNoClip += (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
		PlayerEvents.SendingVoiceMessage += OnPlayerSendingVoiceMessage;
		Player.Spawning += (CustomEventHandler<SpawningEventArgs>)OnSpawning;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Destroying += (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
	}

	public void UnsubscribeEvents()
	{
		Player.TogglingNoClip += (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
		PlayerEvents.SendingVoiceMessage -= OnPlayerSendingVoiceMessage;
		Player.Spawning -= (CustomEventHandler<SpawningEventArgs>)OnSpawning;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Destroying -= (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
	}

	public void Dispose()
	{
		UnsubscribeEvents();
	}

	private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
	{
		if (IsValidScp(ev.Player))
		{
			ToggleScpVoice(ev.Player);
		}
	}

	private void OnPlayerSendingVoiceMessage(PlayerSendingVoiceMessageEventArgs ev)
	{
		Player val = Player.op_Implicit(ev.Player);
		if (IsValidScp(val) && _scpVoiceEnabled.TryGetValue(val, out var value) && value && (int)ev.Message.Channel == 3)
		{
			ev.IsAllowed = false;
			ProcessScpVoice(val, ev.Message);
		}
	}

	private void ProcessScpVoice(Player sender, VoiceMessage message)
	{
		VoiceMessage val = default(VoiceMessage);
		val.Channel = (VoiceChatChannel)1;
		val.Data = message.Data;
		val.DataLength = message.DataLength;
		val.Speaker = message.Speaker;
		VoiceMessage val2 = val;
		foreach (Player item in Player.List)
		{
			if (!(item == sender) && item.IsAlive && !item.IsScp)
			{
				ReferenceHub referenceHub = item.ReferenceHub;
				if (((referenceHub != null) ? ((NetworkBehaviour)referenceHub).connectionToClient : null) != null && IsPlayerInProximity(sender, item))
				{
					((NetworkConnection)((NetworkBehaviour)item.ReferenceHub).connectionToClient).Send<VoiceMessage>(val2, 1);
				}
			}
		}
	}

	private void ToggleScpVoice(Player player)
	{
		bool value;
		bool flag = !_scpVoiceEnabled.TryGetValue(player, out value) || !value;
		_scpVoiceEnabled[player] = flag;
		player.ShowMeowHint(3f, "<size=29><b><color=#61616193>|</color></size> <size=19>Голос SCP: <color=" + (flag ? "green" : "red") + ">" + (flag ? "ВКЛ" : "ВЫКЛ") + "</color></size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
		Log.Debug("[SCP Voice] " + player.Nickname + ": " + (flag ? "enabled" : "disabled"));
	}

	private void OnSpawning(SpawningEventArgs ev)
	{
		Player player = ev.Player;
		if (player != null && player.IsScp)
		{
			_scpVoiceEnabled[ev.Player] = false;
		}
	}

	private void OnSpawned(SpawnedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Player.IsScp)
		{
			ev.Player.ShowMeowHint(7f, "<size=29><b><color=#61616193>|</color></size> <size=19>Вы можете использовать голосовой чат с людьми <size=21>[ALT]</size></size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
		}
	}

	private void OnDestroying(DestroyingEventArgs ev)
	{
		_scpVoiceEnabled.Remove(ev.Player);
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		_scpVoiceEnabled.Clear();
	}

	private bool IsValidScp(Player player)
	{
		return player != null && player.IsScp && (int)player.Role.Type != 7;
	}

	private bool IsPlayerInProximity(Player p1, Player p2)
	{
		return p2.ReferenceHub != (ReferenceHub)null && Vector3.SqrMagnitude(p1.Position - p2.Position) <= 35f;
	}
}
