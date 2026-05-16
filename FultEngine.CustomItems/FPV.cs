using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.API.Libraries.SSBinds;
using HintServiceMeow.Core.Enum;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class FPV : CustomItem
{
	[CompilerGenerated]
	private sealed class __c__DisplayClass91_0
	{
		public Player player;

		public string nickname;

		internal void RespawnSequence_b_0()
		{
			try
			{
				if (player != (Player)null && player.IsConnected && player.ReferenceHub != (ReferenceHub)null && (Object)(object)player.ReferenceHub.nicknameSync != (Object)null)
				{
					player.DisplayNickname = nickname;
				}
			}
			catch (Exception ex)
			{
				Log.Warn("[FPV] Не удалось вернуть DisplayNickname: " + ex.Message);
			}
		}
	}

	[CompilerGenerated]
	private sealed class FlightCoroutine_d_95 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public FPV __4__this;

		private float currentSpeed;

		private float flightTime;

		private float time;

		private float speed;

		private Vector3 flightDirection;

		private Vector3 movement;

		private Vector3 forward;

		private Vector3 raycastStart;

		private RaycastHit hit;

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
		public FlightCoroutine_d_95(int __1__state)
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
				currentSpeed = __4__this.DefaultFlightSpeed;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (flyingPlayers.Contains(player) && player.IsAlive)
			{
				if (__4__this.playerFlightTimes.ContainsKey(player))
				{
					__4__this.playerFlightTimes[player] += Time.deltaTime;
				}
				flightTime = (__4__this.playerFlightTimes.TryGetValue(player, out time) ? time : 0f);
				if (__4__this.playerFlightSpeeds.TryGetValue(player, out speed))
				{
					currentSpeed = speed;
				}
				if (flightTime >= __4__this.MaxFlightTime)
				{
					__4__this.StopFlight(player, forced: false, exploded: true, teleportBack: true);
					return false;
				}
				if (flightTime > __4__this.ExplosionDelay)
				{
					forward = player.CameraTransform.forward;
					raycastStart = player.Position + forward * 0.5f;
					if (Physics.Raycast(raycastStart, forward, ref hit, __4__this.CollisionDistance, ~player.GameObject.layer) && ((Component)((RaycastHit)(ref hit)).collider).GetComponentInParent<ReferenceHub>() == (ReferenceHub)null)
					{
						__4__this.StopFlight(player, forced: true, exploded: true, teleportBack: true);
						return false;
					}
				}
				flightDirection = player.CameraTransform.forward;
				if (flightDirection.y < 0.1f && flightDirection.y > -0.1f)
				{
					flightDirection.y = 0.05f;
				}
				movement = ((Vector3)(ref flightDirection)).normalized * currentSpeed * Time.deltaTime;
				Player obj = player;
				obj.Position += movement;
				__2__current = float.NegativeInfinity;
				__1__state = 1;
				return true;
			}
			if (flyingPlayers.Contains(player))
			{
				__4__this.StopFlight(player, forced: true, exploded: false, teleportBack: true);
			}
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

	[CompilerGenerated]
	private sealed class FlightHintCoroutine_d_96 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public FPV __4__this;

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
		public FlightHintCoroutine_d_96(int __1__state)
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
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (flyingPlayers.Contains(player))
			{
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			}
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

	[CompilerGenerated]
	private sealed class RespawnSequence_d_91 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public RoleTypeId role;

		public Vector3 position;

		public string nickname;

		public FPV __4__this;

		private __c__DisplayClass91_0 __8__1;

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
		public RespawnSequence_d_91(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__8__1 = null;
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
				__8__1 = new __c__DisplayClass91_0();
				__8__1.player = player;
				__8__1.nickname = nickname;
				if (__8__1.player == (Player)null)
				{
					return false;
				}
				__8__1.player.Role.Set(role, (SpawnReason)7);
				__2__current = Timing.WaitForSeconds(1.5f);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				__8__1.player.Position = position;
				Timing.CallDelayed(0.6f, (Action)delegate
				{
					try
					{
						if (__8__1.player != (Player)null && __8__1.player.IsConnected && __8__1.player.ReferenceHub != (ReferenceHub)null && (Object)(object)__8__1.player.ReferenceHub.nicknameSync != (Object)null)
						{
							__8__1.player.DisplayNickname = __8__1.nickname;
						}
					}
					catch (Exception ex)
					{
						Log.Warn("[FPV] Не удалось вернуть DisplayNickname: " + ex.Message);
					}
				});
				__8__1.player.IsGodModeEnabled = false;
				return false;
			}
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

	[CompilerGenerated]
	private sealed class SchematicFollowCoroutine_d_92 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public SchematicObject schematic;

		public FPV __4__this;

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
		public SchematicFollowCoroutine_d_92(int __1__state)
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
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (flyingPlayers.Contains(player) && (Object)(object)schematic != (Object)null)
			{
				((MapEditorObject)schematic).Position = player.Position;
				((MapEditorObject)schematic).Rotation = player.Rotation * Quaternion.Euler(0f, 180f, 0f);
				__2__current = Timing.WaitForSeconds(0.1f);
				__1__state = 1;
				return true;
			}
			if ((Object)(object)schematic != (Object)null)
			{
				((MapEditorObject)schematic).Destroy();
			}
			__4__this.schematicCoroutines.Remove(player);
			__4__this.activeSchematics.Remove(player);
			__4__this.droneComponents.Remove(player);
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

	[CompilerGenerated]
	private sealed class SoundLoopCoroutine_d_94 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public FPV __4__this;

		private float currentSpeed;

		private float flightTime;

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
		public SoundLoopCoroutine_d_94(int __1__state)
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
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (flyingPlayers.Contains(player))
			{
				if (player != (Player)null && __4__this.playerFlightSpeeds.TryGetValue(player, out currentSpeed) && __4__this.playerFlightTimes.TryGetValue(player, out flightTime))
				{
					AudioManager.CreateForPlayer(player, "drone", 1f, 13f);
					__4__this.DisplayDroneTelemetry(player, flightTime, currentSpeed);
				}
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			}
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

	private const float SpeedStep = 1.5f;

	private const float MinSpeed = 5f;

	private const float MaxSpeed = 9f;

	public static readonly HashSet<Player> flyingPlayers = new HashSet<Player>();

	private readonly Dictionary<Player, CoroutineHandle> flightCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> soundCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> schematicCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, SchematicObject> activeSchematics = new Dictionary<Player, SchematicObject>();

	public static readonly Dictionary<Player, ReferenceHub> playerDummies = new Dictionary<Player, ReferenceHub>();

	private readonly Dictionary<Player, DroneComponent> droneComponents = new Dictionary<Player, DroneComponent>();

	private readonly Dictionary<Player, Vector3> originalPositions = new Dictionary<Player, Vector3>();

	private readonly Dictionary<Player, float> playerFlightSpeeds = new Dictionary<Player, float>();

	private readonly Dictionary<Player, float> playerFlightTimes = new Dictionary<Player, float>();

	private readonly Dictionary<Player, RoleTypeId> savedRoles = new Dictionary<Player, RoleTypeId>();

	private readonly Dictionary<Player, string> savedDisplayNames = new Dictionary<Player, string>();

	private readonly HashSet<Player> activationInProgress = new HashSet<Player>();

	public override uint Id { get; set; } = 19u;


	public override string Name { get; set; } = "<color=#7f2e2ed6><b>FPV-Дрон</b></color>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 1f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public float DefaultFlightSpeed { get; set; } = 5.9f;


	public float MaxFlightTime { get; set; } = 75f;


	public float CollisionDistance { get; set; } = 2f;


	public float ExplosionDelay { get; set; } = 0.1f;


	public float ExplosionDamage { get; set; } = 666f;


	public float DroneMaxHealth { get; set; } = 75f;


	public Vector3 NormalScale { get; set; } = Vector3.one;


	public Vector3 FlightScale { get; set; } = new Vector3(3f, 0.3f, 3f);


	private string StripTags(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		return Regex.Replace(input, "<[^>]*>", string.Empty);
	}

	protected override void SubscribeEvents()
	{
		KeybindManager.AddCustomKeybind(111, "╔ <color=#c2151539>⬆</color> Увеличить скорость", (KeyCode)98, preventInteractionOnGUI: false, "Увеличить скорость");
		KeybindManager.AddCustomKeybind(112, "╚ <color=#1a964c39>⬇</color> Уменьшить скорость", (KeyCode)13, preventInteractionOnGUI: false, "Уменьшить скорость");
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
		Player.InteractingDoor += (CustomEventHandler<InteractingDoorEventArgs>)OnInteractionDenied;
		Player.InteractingLocker += (CustomEventHandler<InteractingLockerEventArgs>)OnInteractionDenied;
		Player.UsingRadioBattery += (CustomEventHandler<UsingRadioBatteryEventArgs>)OnInteractionDenied;
		Player.Jumping += (CustomEventHandler<JumpingEventArgs>)OnJumping;
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
		KeybindManager.OnObjectInteraction += OnKeybindPressed;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.ChangingRadioPreset += (CustomEventHandler<ChangingRadioPresetEventArgs>)OnChangingRadioPreset;
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Jumping -= (CustomEventHandler<JumpingEventArgs>)OnJumping;
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		Player.InteractingDoor -= (CustomEventHandler<InteractingDoorEventArgs>)OnInteractionDenied;
		Player.InteractingLocker -= (CustomEventHandler<InteractingLockerEventArgs>)OnInteractionDenied;
		Player.UsingRadioBattery -= (CustomEventHandler<UsingRadioBatteryEventArgs>)OnInteractionDenied;
		KeybindManager.OnObjectInteraction -= OnKeybindPressed;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.ChangingRadioPreset -= (CustomEventHandler<ChangingRadioPresetEventArgs>)OnChangingRadioPreset;
		foreach (Player item in flyingPlayers.ToList())
		{
			StopFlight(item, forced: true);
		}
		flyingPlayers.Clear();
		flightCoroutines.Clear();
		soundCoroutines.Clear();
		schematicCoroutines.Clear();
		activeSchematics.Clear();
		originalPositions.Clear();
		playerFlightSpeeds.Clear();
		playerFlightTimes.Clear();
		savedRoles.Clear();
		savedDisplayNames.Clear();
		playerDummies.Clear();
		droneComponents.Clear();
		activationInProgress.Clear();
		((CustomItem)this).UnsubscribeEvents();
	}

	private void OnShot(ShotEventArgs ev)
	{
		if (ev.Player == (Player)null || ev.Target == (Player)null)
		{
			return;
		}
		Player target = ev.Target;
		if (flyingPlayers.Contains(target))
		{
			if (droneComponents.TryGetValue(target, out var value))
			{
				value.TakeDamage(ev.Firearm.Damage);
				ev.Player.ShowHitMarker(0.8f);
				AudioManager.CreateForPlayer(ev.Player, "shield", 1f, 5f);
				if (value.Health <= 0f)
				{
					StopFlight(target, forced: true, exploded: true, teleportBack: true);
				}
			}
			return;
		}
		Player val = null;
		List<Player> list = playerDummies.Keys.ToList();
		foreach (Player item in list)
		{
			if (playerDummies.TryGetValue(item, out var value2))
			{
				Player val2 = Player.Get(value2);
				if (val2 != (Player)null && val2 == target)
				{
					val = item;
					break;
				}
			}
		}
		if (val != (Player)null && flyingPlayers.Contains(val) && droneComponents.TryGetValue(val, out var value3))
		{
			value3.TakeDamage(ev.Firearm.Damage);
			ev.Player.ShowHitMarker(0.8f);
			AudioManager.CreateForPlayer(ev.Player, "shield", 1f, 5f);
			if (value3.Health <= 0f)
			{
				StopFlight(val, forced: true, exploded: true, teleportBack: true);
			}
		}
	}

	private void OnKeybindPressed(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val == null || !val.SyncIsPressed)
		{
			return;
		}
		Player val2 = Player.Get(hub);
		if (!(val2 == (Player)null) && flyingPlayers.Contains(val2))
		{
			if (!playerFlightSpeeds.TryGetValue(val2, out var value))
			{
				value = DefaultFlightSpeed;
			}
			switch (((ServerSpecificSettingBase)val).SettingId)
			{
			default:
				return;
			case 111:
				value = Mathf.Min(value + 1.5f, 9f);
				break;
			case 112:
				value = Mathf.Max(value - 1.5f, 5f);
				break;
			}
			playerFlightSpeeds[val2] = value;
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (!((CustomItem)this).Check(ev.Item))
		{
			return;
		}
		ev.IsAllowed = false;
		if (!activationInProgress.Contains(ev.Player))
		{
			if (flyingPlayers.Contains(ev.Player))
			{
				StopFlight(ev.Player, forced: false, exploded: false, teleportBack: true);
			}
			else if (!TryStartFlight(ev.Player, ev.Item))
			{
				ev.Player.ShowMeowHint(3f, "<b><color=#ff4d4d>Дрон не удалось активировать. Попробуй ещё раз.</color></b>", (HintVerticalAlign)0, 725, 0, (HintAlignment)2);
			}
		}
	}

	private void OnJumping(JumpingEventArgs ev)
	{
		if (flyingPlayers.Contains(ev.Player))
		{
			StopFlight(ev.Player, forced: false, exploded: false, teleportBack: true);
		}
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		if (!(ev.Player != (Player)null) || !flyingPlayers.Contains(ev.Player) || !ev.Player.IsAlive || !(ev.Amount > 0f))
		{
			return;
		}
		DroneComponent value;
		if (ev.Player.IsGodModeEnabled)
		{
			ev.IsAllowed = false;
		}
		else if (droneComponents.TryGetValue(ev.Player, out value))
		{
			value.TakeDamage(ev.Amount);
			if (value.Health <= 0f)
			{
				StopFlight(ev.Player, forced: true, exploded: true, teleportBack: true);
			}
			ev.IsAllowed = false;
		}
		else
		{
			StopFlight(ev.Player, forced: true, exploded: true, teleportBack: true);
			ev.IsAllowed = true;
		}
	}

	private void OnDying(DyingEventArgs ev)
	{
		if (ev.Player == (Player)null)
		{
			return;
		}
		Player owner;
		if (flyingPlayers.Contains(ev.Player))
		{
			ev.IsAllowed = false;
			StopFlight(ev.Player, forced: true);
			RespawnPlayer(ev.Player);
		}
		else if (TryGetOwnerByDummy(ev.Player.ReferenceHub, out owner) && flyingPlayers.Contains(owner))
		{
			ev.IsAllowed = false;
			if (droneComponents.TryGetValue(owner, out var value) && value.Health <= 0f)
			{
				StopFlight(owner, forced: true, exploded: true, teleportBack: true);
				owner.Kill((DamageType)11, "");
			}
		}
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (ev.Player != (Player)null && flyingPlayers.Contains(ev.Player))
		{
			StopFlight(ev.Player, forced: true);
		}
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		foreach (Player item in flyingPlayers.ToList())
		{
			StopFlight(item, forced: true);
		}
	}

	private void OnRestartingRound()
	{
		foreach (Player item in flyingPlayers.ToList())
		{
			StopFlight(item, forced: true);
		}
	}

	private void OnInteractionDenied(InteractingDoorEventArgs ev)
	{
		if (flyingPlayers.Contains(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	private void OnInteractionDenied(InteractingLockerEventArgs ev)
	{
		if (flyingPlayers.Contains(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	private void OnInteractionDenied(UsingRadioBatteryEventArgs ev)
	{
		if (flyingPlayers.Contains(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (flyingPlayers.Contains(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	private void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev)
	{
		if (flyingPlayers.Contains(ev.Player) && ev.Radio != null)
		{
			ev.IsAllowed = false;
		}
	}

	private bool HasAnyFlightState(Player player)
	{
		return player != (Player)null && (flyingPlayers.Contains(player) || activationInProgress.Contains(player) || flightCoroutines.ContainsKey(player) || soundCoroutines.ContainsKey(player) || schematicCoroutines.ContainsKey(player) || activeSchematics.ContainsKey(player) || playerDummies.ContainsKey(player) || droneComponents.ContainsKey(player) || originalPositions.ContainsKey(player) || playerFlightSpeeds.ContainsKey(player) || playerFlightTimes.ContainsKey(player));
	}

	private bool TryGetOwnerByDummy(ReferenceHub hub, out Player owner)
	{
		owner = null;
		if (hub == (ReferenceHub)null)
		{
			return false;
		}
		foreach (KeyValuePair<Player, ReferenceHub> playerDummy in playerDummies)
		{
			if (playerDummy.Value == hub)
			{
				owner = playerDummy.Key;
				return owner != (Player)null;
			}
		}
		return false;
	}

	private void CleanupFlightArtifacts(Player player, bool clearSavedState = false)
	{
		if (!(player == (Player)null))
		{
			if (soundCoroutines.TryGetValue(player, out var value) && ((CoroutineHandle)(ref value)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			}
			soundCoroutines.Remove(player);
			AudioManager.DestroyForPlayer(player);
			if (flightCoroutines.TryGetValue(player, out var value2) && ((CoroutineHandle)(ref value2)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
			}
			flightCoroutines.Remove(player);
			if (schematicCoroutines.TryGetValue(player, out var value3) && ((CoroutineHandle)(ref value3)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value3 });
			}
			schematicCoroutines.Remove(player);
			if (activeSchematics.TryGetValue(player, out var value4) && (Object)(object)value4 != (Object)null)
			{
				((MapEditorObject)value4).Destroy();
			}
			activeSchematics.Remove(player);
			if (droneComponents.TryGetValue(player, out var value5) && (Object)(object)value5 != (Object)null && (Object)(object)((Component)value5).gameObject != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)value5).gameObject);
			}
			droneComponents.Remove(player);
			if (playerDummies.TryGetValue(player, out var value6) && value6 != (ReferenceHub)null && (Object)(object)((Component)value6).gameObject != (Object)null)
			{
				NetworkServer.Destroy(((Component)value6).gameObject);
			}
			playerDummies.Remove(player);
			flyingPlayers.Remove(player);
			activationInProgress.Remove(player);
			playerFlightSpeeds.Remove(player);
			playerFlightTimes.Remove(player);
			if (clearSavedState)
			{
				originalPositions.Remove(player);
				savedRoles.Remove(player);
				savedDisplayNames.Remove(player);
			}
		}
	}

	private bool TryStartFlight(Player player, Item usedItem)
	{
		if (player == (Player)null || (int)player.Role.Team == 5 || !player.IsAlive)
		{
			return false;
		}
		if (activationInProgress.Contains(player))
		{
			return false;
		}
		activationInProgress.Add(player);
		try
		{
			if (HasAnyFlightState(player))
			{
				CleanupFlightArtifacts(player);
			}
			originalPositions[player] = player.Position;
			savedRoles[player] = player.Role.Type;
			savedDisplayNames[player] = StripTags(player.DisplayNickname ?? player.Nickname);
			ReferenceHub val = DummyUtils.SpawnDummy(player.Nickname + "'s FPV");
			if (val == (ReferenceHub)null)
			{
				Log.Warn("[FPV] Не удалось создать dummy для " + player.Nickname + ".");
				return false;
			}
			Player val2 = Player.Get(val);
			if (val2 == (Player)null)
			{
				if ((Object)(object)((Component)val).gameObject != (Object)null)
				{
					NetworkServer.Destroy(((Component)val).gameObject);
				}
				Log.Warn("[FPV] Dummy создан, но Player.Get(dummyHub) вернул null для " + player.Nickname + ".");
				return false;
			}
			val2.Role.Set(Role.op_Implicit(player.Role), (SpawnReason)7);
			val2.Position = player.Position;
			playerDummies[player] = val;
			if (usedItem != null && player.Items.Any((Item i) => i.Serial == usedItem.Serial))
			{
				player.RemoveItem(usedItem, true);
			}
			else
			{
				Item val3 = ((IEnumerable<Item>)player.Items).FirstOrDefault((Func<Item, bool>)((Item item) => ((CustomItem)this).Check(item)));
				if (val3 != null)
				{
					player.RemoveItem(val3, true);
				}
			}
			if (player.Items.Count > 0)
			{
				player.DropItems();
			}
			ReferenceHub referenceHub = player.ReferenceHub;
			object obj;
			if (referenceHub == null)
			{
				obj = null;
			}
			else
			{
				PlayerStats playerStats = referenceHub.playerStats;
				obj = ((playerStats != null) ? playerStats.GetModule<AdminFlagsStat>() : null);
			}
			AdminFlagsStat val4 = (AdminFlagsStat)obj;
			if (val4 != null)
			{
				val4.SetFlag((AdminFlags)1, true);
			}
			player.EnableEffect((EffectType)56, byte.MaxValue, 0f, false);
			player.EnableEffect((EffectType)57, byte.MaxValue, 0f, false);
			player.Scale = FlightScale;
			player.ClearInventory(true);
			SchematicObject val5 = ObjectSpawner.SpawnSchematic("FPV", player.Position, (Quaternion?)Quaternion.Euler(0f, 0f, 0f), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			activeSchematics[player] = val5;
			if ((Object)(object)val5 != (Object)null && (Object)(object)((Component)val5).gameObject != (Object)null)
			{
				DroneComponent droneComponent = ((Component)val5).gameObject.AddComponent<DroneComponent>();
				droneComponent.Owner = player;
				droneComponent.Health = DroneMaxHealth;
				droneComponent.MaxHealth = DroneMaxHealth;
				droneComponent.FPVInstance = this;
				droneComponents[player] = droneComponent;
			}
			else
			{
				Log.Warn("[FPV] Не удалось заспавнить schematic FPV для " + player.Nickname + ".");
			}
			playerFlightSpeeds[player] = DefaultFlightSpeed;
			playerFlightTimes[player] = 0f;
			flyingPlayers.Add(player);
			flightCoroutines[player] = Timing.RunCoroutine(FlightCoroutine(player));
			soundCoroutines[player] = Timing.RunCoroutine(SoundLoopCoroutine(player));
			if ((Object)(object)val5 != (Object)null)
			{
				schematicCoroutines[player] = Timing.RunCoroutine(SchematicFollowCoroutine(player, val5));
			}
			Timing.RunCoroutine(FlightHintCoroutine(player));
			return true;
		}
		catch (Exception arg)
		{
			Log.Error($"[FPV] Ошибка старта дрона для {((player != null) ? player.Nickname : null)}: {arg}");
			CleanupFlightArtifacts(player);
			try
			{
				if (player != null)
				{
					player.DisableEffect((EffectType)56);
				}
				if (player != null)
				{
					player.DisableEffect((EffectType)57);
				}
				if (player != (Player)null)
				{
					player.Scale = NormalScale;
					player.IsGodModeEnabled = false;
					ReferenceHub referenceHub2 = player.ReferenceHub;
					object obj2;
					if (referenceHub2 == null)
					{
						obj2 = null;
					}
					else
					{
						PlayerStats playerStats2 = referenceHub2.playerStats;
						obj2 = ((playerStats2 != null) ? playerStats2.GetModule<AdminFlagsStat>() : null);
					}
					AdminFlagsStat val6 = (AdminFlagsStat)obj2;
					if (val6 != null)
					{
						val6.SetFlag((AdminFlags)1, false);
					}
				}
			}
			catch
			{
			}
			return false;
		}
		finally
		{
			activationInProgress.Remove(player);
		}
	}

	private void StartFlight(Player player)
	{
		TryStartFlight(player, null);
	}

	private void RespawnPlayer(Player player)
	{
		if (!(player == (Player)null))
		{
			Vector3 value = Vector3.zero;
			RoleTypeId value2 = (RoleTypeId)(-1);
			string value3 = player.Nickname;
			originalPositions.TryGetValue(player, out value);
			savedRoles.TryGetValue(player, out value2);
			savedDisplayNames.TryGetValue(player, out value3);
			originalPositions.Remove(player);
			savedRoles.Remove(player);
			savedDisplayNames.Remove(player);
			Timing.RunCoroutine(RespawnSequence(player, value2, value, value3));
		}
	}

	[IteratorStateMachine(typeof(RespawnSequence_d_91))]
	private IEnumerator<float> RespawnSequence(Player player, RoleTypeId role, Vector3 position, string nickname)
	{
		return new RespawnSequence_d_91(0)
		{
			__4__this = this,
			player = player,
			role = role,
			position = position,
			nickname = nickname
		};
	}

	[IteratorStateMachine(typeof(SchematicFollowCoroutine_d_92))]
	private IEnumerator<float> SchematicFollowCoroutine(Player player, SchematicObject schematic)
	{
		return new SchematicFollowCoroutine_d_92(0)
		{
			__4__this = this,
			player = player,
			schematic = schematic
		};
	}

	private void DisplayDroneTelemetry(Player player, float flightTime, float currentSpeed)
	{
		if (!(player == (Player)null) && flyingPlayers.Contains(player))
		{
			float num = MaxFlightTime - flightTime;
			string text = ((num > 0f) ? $"{(int)num:D2}" : "00");
			float num2 = currentSpeed * 3.6f;
			string text2 = $"{num2:F1}";
			player.ShowMeowHint(1f, "<size=1999><color=#141414b6>█</size></color>", (HintVerticalAlign)0, -275, 1, (HintAlignment)2);
			player.ShowMeowHint(1f, "<size=55><color=#ffffff95>⌜</size></color>", (HintVerticalAlign)0, 150, -1000, (HintAlignment)2);
			player.ShowMeowHint(1f, "<size=55><color=#ffffff95>⌝</size></color>", (HintVerticalAlign)0, 150, 1000, (HintAlignment)2);
			player.ShowMeowHint(1f, "<size=55><color=#ffffff95>⌞</size></color>", (HintVerticalAlign)0, 868, -1000, (HintAlignment)2);
			player.ShowMeowHint(1f, "<size=55><color=#ffffff95>⌟</size></color>", (HintVerticalAlign)0, 868, 1000, (HintAlignment)2);
			player.ShowMeowHint(1f, "<size=55><color=#ffffff95>⚙</size></color>", (HintVerticalAlign)0, 505, 0, (HintAlignment)2);
			player.ShowMeowHint(1f, "<size=19><color=#ffffff95><b>OBJ</b></color></size>\n<size=15><color=#ffffff95><b>M/S: " + text2 + "</b></color></size>\n<size=15><color=#ffffff95><b>TIME: " + text + "</b></color></size>\n" + $"<size=15><color=#ffffff95><b>HP: {player.Health}/{player.MaxHealth}</b></color></size>", (HintVerticalAlign)0, 355, -319, (HintAlignment)0);
		}
	}

	[IteratorStateMachine(typeof(SoundLoopCoroutine_d_94))]
	private IEnumerator<float> SoundLoopCoroutine(Player player)
	{
		return new SoundLoopCoroutine_d_94(0)
		{
			__4__this = this,
			player = player
		};
	}

	[IteratorStateMachine(typeof(FlightCoroutine_d_95))]
	private IEnumerator<float> FlightCoroutine(Player player)
	{
		return new FlightCoroutine_d_95(0)
		{
			__4__this = this,
			player = player
		};
	}

	[IteratorStateMachine(typeof(FlightHintCoroutine_d_96))]
	private IEnumerator<float> FlightHintCoroutine(Player player)
	{
		return new FlightHintCoroutine_d_96(0)
		{
			__4__this = this,
			player = player
		};
	}

	public void StopFlight(Player player, bool forced = false, bool exploded = false, bool teleportBack = false)
	{
		if (!(player == (Player)null) && HasAnyFlightState(player))
		{
			CleanupFlightArtifacts(player);
			player.DisableEffect((EffectType)56);
			player.DisableEffect((EffectType)57);
			ReferenceHub referenceHub = player.ReferenceHub;
			object obj;
			if (referenceHub == null)
			{
				obj = null;
			}
			else
			{
				PlayerStats playerStats = referenceHub.playerStats;
				obj = ((playerStats != null) ? playerStats.GetModule<AdminFlagsStat>() : null);
			}
			AdminFlagsStat val = (AdminFlagsStat)obj;
			if (val != null)
			{
				val.SetFlag((AdminFlags)1, false);
			}
			player.Scale = NormalScale;
			player.IsGodModeEnabled = true;
			if (exploded)
			{
				Map.Explode(player.Position, (ProjectileType)1, (Player)null);
				Map.Explode(player.Position, (ProjectileType)1, (Player)null);
				Map.Explode(player.Position, (ProjectileType)1, (Player)null);
			}
			if (teleportBack && originalPositions.TryGetValue(player, out var value))
			{
				player.Position = value;
				originalPositions.Remove(player);
				savedRoles.Remove(player);
				savedDisplayNames.Remove(player);
			}
			player.IsGodModeEnabled = false;
		}
	}
}
