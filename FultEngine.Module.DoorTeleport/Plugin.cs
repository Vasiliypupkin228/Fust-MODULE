using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.LoaderModule;
using FultEngine.Module.DoorTeleport.Features;
using HintServiceMeow.Core.Enum;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.Module.DoorTeleport;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class TeleportCoroutine_d_25 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Vector3 targetPosition;

		public Plugin __4__this;

		private Vector3 initialPosition;

		private float startTime;

		private int segment;

		float IEnumerator<float>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public TeleportCoroutine_d_25(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__1__state = -2;
		}

		private bool MoveNext()
		{
			switch (__1__state)
			{
			default:
				return false;
			case 0:
				__1__state = -1;
				initialPosition = player.Position;
				startTime = Time.time;
				segment = 0;
				break;
			case 1:
				__1__state = -1;
				segment++;
				break;
			}
			if (segment < 10)
			{
				if (player == (Player)null || !player.IsAlive)
				{
					__4__this.StopTeleport(player);
					return false;
				}
				if (Time.time - startTime > 2f)
				{
					__4__this.StopTeleport(player);
					return false;
				}
				if (__4__this._cancelRequests.ContainsKey(player) && __4__this._cancelRequests[player])
				{
					__4__this.StopTeleport(player);
					return false;
				}
				if (Vector3.Distance(player.Position, initialPosition) > 1.5f)
				{
					__4__this.StopTeleport(player);
					return false;
				}
				__4__this.UpdateTeleportUI(player, "Телепортация", segment, 10);
				__2__current = Timing.WaitForSeconds(0.17f);
				__1__state = 1;
				return true;
			}
			if (player != (Player)null && player.IsAlive)
			{
				player.Position = targetPosition + Vector3.up * 0.25f;
				__4__this._cooldowns[player] = Time.time + 1.25f;
			}
			__4__this.StopTeleport(player);
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private readonly Dictionary<Player, CoroutineHandle> _activeTeleports = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, bool> _cancelRequests = new Dictionary<Player, bool>();

	private readonly Dictionary<Player, float> _cooldowns = new Dictionary<Player, float>();

	public static Plugin Instance { get; private set; }

	public string Name => "DoorSystem";

	public string Author => "FUST";

	public Version Version => new Version(2, 0, 0);

	public void OnEnabled()
	{
		Instance = this;
		if (!Directory.Exists(Frequency.ConfigPath))
		{
			Directory.CreateDirectory(Frequency.ConfigPath);
		}
		ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnPlayerDied;
	}

	public void OnDisabled()
	{
		ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnPlayerDied;
		foreach (CoroutineHandle value in _activeTeleports.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		_activeTeleports.Clear();
		_cancelRequests.Clear();
		_cooldowns.Clear();
		Instance = null;
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

	private void OnPlayerLeft(LeftEventArgs ev)
	{
		StopTeleport(((JoinedEventArgs)ev).Player);
	}

	private void OnPlayerDied(DiedEventArgs ev)
	{
		StopTeleport(ev.Player);
	}

	private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		if (hub == (ReferenceHub)null || hub.IsHost)
		{
			return;
		}
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val != null && ((ServerSpecificSettingBase)val).SettingId == Frequency.KeybindSettingId && val.SyncIsPressed)
		{
			Player val2 = Player.Get(hub);
			if (!(val2 == (Player)null) && val2.IsAlive && (!_cooldowns.TryGetValue(val2, out var value) || !(Time.time < value)) && TryGetNearbyDoorName(val2, out var doorName))
			{
				HandleDoor(val2, doorName);
			}
		}
	}

	private void HandleDoor(Player player, string sourceDoorName)
	{
		List<DoorPairConfig> source = Frequency.LoadAllDoorPairs();
		DoorPairConfig doorPairConfig = source.FirstOrDefault((DoorPairConfig x) => string.Equals(x.SchematicName1, sourceDoorName, StringComparison.OrdinalIgnoreCase) || string.Equals(x.SchematicName2, sourceDoorName, StringComparison.OrdinalIgnoreCase));
		if (doorPairConfig != null)
		{
			string text = (string.Equals(doorPairConfig.SchematicName1, sourceDoorName, StringComparison.OrdinalIgnoreCase) ? doorPairConfig.SchematicName2 : doorPairConfig.SchematicName1);
			Transform val = FindTeleportTransform(text);
			if ((Object)(object)val == (Object)null)
			{
				Log.Warn("[FULT-ENGINE.DoorTeleport] Не найден целевой объект '" + text + "'.");
				return;
			}
			if (_activeTeleports.ContainsKey(player))
			{
				_cancelRequests[player] = true;
				return;
			}
			_cancelRequests[player] = false;
			CoroutineHandle value = Timing.RunCoroutine(TeleportCoroutine(player, val.position));
			_activeTeleports[player] = value;
		}
	}

	private bool TryGetNearbyDoorName(Player player, out string doorName)
	{
		doorName = null;
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(player.CameraTransform.position, player.CameraTransform.forward);
		RaycastHit val2 = default(RaycastHit);
		if (!Physics.Raycast(val, ref val2, 2.5f, -1, (QueryTriggerInteraction)1))
		{
			return false;
		}
		List<DoorPairConfig> list = Frequency.LoadAllDoorPairs();
		if (list.Count == 0)
		{
			return false;
		}
		HashSet<string> hashSet = new HashSet<string>(from x in list.SelectMany((DoorPairConfig x) => new string[2] { x.SchematicName1, x.SchematicName2 })
			where !string.IsNullOrWhiteSpace(x)
			select x);
		Transform val3 = ((Component)((RaycastHit)(ref val2)).collider).transform;
		while ((Object)(object)val3 != (Object)null)
		{
			if (hashSet.Contains(((Object)val3).name))
			{
				doorName = ((Object)val3).name;
				return true;
			}
			val3 = val3.parent;
		}
		Transform val4 = FindClosestKnownTransform(((RaycastHit)(ref val2)).point, hashSet, 3f);
		if ((Object)(object)val4 == (Object)null)
		{
			return false;
		}
		doorName = ((Object)val4).name;
		return true;
	}

	private Transform FindClosestKnownTransform(Vector3 point, HashSet<string> knownNames, float maxDistance)
	{
		float num = maxDistance * maxDistance;
		Transform result = null;
		Transform[] array = Object.FindObjectsOfType<Transform>();
		foreach (Transform val in array)
		{
			if (!((Object)(object)val == (Object)null) && knownNames.Contains(((Object)val).name))
			{
				Vector3 val2 = val.position - point;
				float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
				if (!(sqrMagnitude >= num))
				{
					num = sqrMagnitude;
					result = val;
				}
			}
		}
		return result;
	}

	private Transform FindTeleportTransform(string targetName)
	{
		Transform[] array = Object.FindObjectsOfType<Transform>();
		foreach (Transform val in array)
		{
			if (!((Object)(object)val == (Object)null) && string.Equals(((Object)val).name, targetName, StringComparison.OrdinalIgnoreCase))
			{
				return val;
			}
		}
		return null;
	}

	[IteratorStateMachine(typeof(TeleportCoroutine_d_25))]
	private IEnumerator<float> TeleportCoroutine(Player player, Vector3 targetPosition)
	{
		return new TeleportCoroutine_d_25(0)
		{
			__4__this = this,
			player = player,
			targetPosition = targetPosition
		};
	}

	private void UpdateTeleportUI(Player player, string processName, int currentSegment, int totalSegments)
	{
		float num = (float)(currentSegment + 1) / (float)totalSegments;
		int num2 = Mathf.FloorToInt(num * 10f);
		string text = string.Empty;
		for (int i = 0; i < 10; i++)
		{
			text += ((i < num2) ? "<color=#7B68EE>━</color>" : "<color=black>━</color>");
		}
		float num3 = (float)(totalSegments - currentSegment - 1) * 0.17f;
		int num4 = Mathf.CeilToInt(Mathf.Max(0f, num3));
		string text2 = "<size=27><color=#ffffff1d>┏ " + processName + " ┓</color></size>\n<size=29>" + text + "</size>\n" + $"<size=15><color=#ffffff1d> Осталось: {num4} секунд </color></size>\n" + "<size=23><color=#ffffff1d>┗ Нажмите ещё раз для отмены ┛</color></size>";
		try
		{
			player.ShowMeowHint(1.5f, text2, (HintVerticalAlign)0, 935, 0, (HintAlignment)2);
		}
		catch (Exception ex)
		{
			Log.Error("DoorTeleport UI error for " + player.Nickname + ": " + ex.Message);
			player.ShowHint(text2, 1.1f);
		}
	}

	private void StopTeleport(Player player)
	{
		if (!(player == (Player)null))
		{
			if (_activeTeleports.TryGetValue(player, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_activeTeleports.Remove(player);
			}
			_cancelRequests.Remove(player);
		}
	}
}
