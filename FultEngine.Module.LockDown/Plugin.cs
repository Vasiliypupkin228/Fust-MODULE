using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.LoaderModule;
using HintServiceMeow.Core.Enum;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.Module.LockDown;

public sealed class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class ContainmentLoop_d_31 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

		private IEnumerator<Player> __s__1;

		private Player player;

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
		public ContainmentLoop_d_31(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__1 = null;
			player = null;
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
			if (__4__this.IsLocked)
			{
				__s__1 = Player.List.GetEnumerator();
				try
				{
					while (__s__1.MoveNext())
					{
						player = __s__1.Current;
						if (player == (Player)null || !player.IsAlive)
						{
							continue;
						}
						if (!__4__this.ShouldContain(player))
						{
							ClearContainmentEffects(player);
							continue;
						}
						if (__4__this._config.ApplyBlindness)
						{
							player.EnableEffect((EffectType)5, (byte)1, __4__this._config.EffectRefreshInterval + 0.15f, false);
						}
						if (__4__this._config.DisableMovement)
						{
							player.EnableEffect((EffectType)11, (byte)1, __4__this._config.EffectRefreshInterval + 0.15f, false);
						}
						player = null;
					}
				}
				finally
				{
					if (__s__1 != null)
					{
						__s__1.Dispose();
					}
				}
				__s__1 = null;
				__2__current = Timing.WaitForSeconds(__4__this._config.EffectRefreshInterval);
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
	private sealed class FlickerLights_d_33 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

		private float elapsed;

		private IEnumerator<Room> __s__2;

		private Room room;

		private IEnumerator<Room> __s__4;

		private Room room;

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
		public FlickerLights_d_33(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__2 = null;
			room = null;
			__s__4 = null;
			room = null;
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
				elapsed = 0f;
				break;
			case 1:
				__1__state = -1;
				__s__4 = Room.List.GetEnumerator();
				try
				{
					while (__s__4.MoveNext())
					{
						room = __s__4.Current;
						room.AreLightsOff = false;
						room = null;
					}
				}
				finally
				{
					if (__s__4 != null)
					{
						__s__4.Dispose();
					}
				}
				__s__4 = null;
				__2__current = Timing.WaitForSeconds(0.35f);
				__1__state = 2;
				return true;
			case 2:
				__1__state = -1;
				elapsed += 0.7f;
				break;
			}
			if (elapsed < __4__this._config.FlickerDuration)
			{
				__s__2 = Room.List.GetEnumerator();
				try
				{
					while (__s__2.MoveNext())
					{
						room = __s__2.Current;
						room.AreLightsOff = true;
						room = null;
					}
				}
				finally
				{
					if (__s__2 != null)
					{
						__s__2.Dispose();
					}
				}
				__s__2 = null;
				__2__current = Timing.WaitForSeconds(0.35f);
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
	private sealed class LockdownTimer_d_30 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

		private float remaining;

		private IEnumerator<Player> __s__2;

		private Player player;

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
		public LockdownTimer_d_30(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__2 = null;
			player = null;
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
			if (__4__this.IsLocked && Time.time < __4__this._unlockAt)
			{
				remaining = Math.Max(0f, __4__this._unlockAt - Time.time);
				__s__2 = Player.List.GetEnumerator();
				try
				{
					while (__s__2.MoveNext())
					{
						player = __s__2.Current;
						if (!(player == (Player)null) && player.IsAlive && __4__this.ShouldContain(player))
						{
							__4__this.ShowLockdownHint(player, remaining);
							player = null;
						}
					}
				}
				finally
				{
					if (__s__2 != null)
					{
						__s__2.Dispose();
					}
				}
				__s__2 = null;
				__2__current = Timing.WaitForSeconds(__4__this._config.HintRefreshInterval);
				__1__state = 1;
				return true;
			}
			__4__this.UnlockAll();
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

	private LockDownConfig _config;

	private CoroutineHandle _timerHandle;

	private CoroutineHandle _effectHandle;

	private float _unlockAt;

	private static readonly HashSet<RoleTypeId> LockedScps = new HashSet<RoleTypeId>
	{
		(RoleTypeId)0,
		(RoleTypeId)9,
		(RoleTypeId)16,
		(RoleTypeId)5
	};

	private static readonly DoorType[] LockedDoorTypes;

	public static Plugin Instance { get; private set; }

	public string Name => "LockDown";

	public string Author => "FUST";

	public Version Version => new Version(2, 2, 0);

	public bool IsLocked { get; private set; }

	public void OnEnabled()
	{
		Instance = this;
		Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
		Player.InteractingDoor += (CustomEventHandler<InteractingDoorEventArgs>)OnInteractingDoor;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
	}

	public void OnDisabled()
	{
		Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
		Player.InteractingDoor -= (CustomEventHandler<InteractingDoorEventArgs>)OnInteractingDoor;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _timerHandle });
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _effectHandle });
		foreach (Player item in Player.List)
		{
			ClearContainmentEffects(item);
		}
		Instance = null;
	}

	public Type GetConfigType()
	{
		return typeof(LockDownConfig);
	}

	public object GetDefaultConfig()
	{
		return new LockDownConfig();
	}

	public void SetConfig(object config)
	{
		_config = (LockDownConfig)config;
	}

	private void OnRoundStarted()
	{
		StartLockdown();
	}

	private void OnLeft(LeftEventArgs ev)
	{
		ClearContainmentEffects(((JoinedEventArgs)ev).Player);
	}

	private void OnDying(DyingEventArgs ev)
	{
		ClearContainmentEffects(ev.Player);
	}

	public void StartLockdown()
	{
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _timerHandle });
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _effectHandle });
		IsLocked = true;
		_unlockAt = Time.time + _config.LockDuration;
		_timerHandle = Timing.RunCoroutine(LockdownTimer());
		_effectHandle = Timing.RunCoroutine(ContainmentLoop());
		if (_config.PlayStartCassie && !string.IsNullOrWhiteSpace(_config.StartCassieWords))
		{
			Cassie.MessageTranslated(_config.StartCassieWords, _config.StartCassieTranslationRu, false, false, true);
		}
	}

	public void ForceUnlock()
	{
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _timerHandle });
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _effectHandle });
		UnlockAll();
	}

	[IteratorStateMachine(typeof(LockdownTimer_d_30))]
	private IEnumerator<float> LockdownTimer()
	{
		return new LockdownTimer_d_30(0)
		{
			__4__this = this
		};
	}

	[IteratorStateMachine(typeof(ContainmentLoop_d_31))]
	private IEnumerator<float> ContainmentLoop()
	{
		return new ContainmentLoop_d_31(0)
		{
			__4__this = this
		};
	}

	private void UnlockAll()
	{
		IsLocked = false;
		foreach (Door item in Door.List)
		{
			if (item != null && (IsBlockedDoor(item) || IsContainmentDoorByRoomName(item)))
			{
				item.IsOpen = true;
			}
		}
		foreach (Player item2 in Player.List)
		{
			ClearContainmentEffects(item2);
		}
		if (_config.FlickerDuration > 0f)
		{
			Timing.RunCoroutine(FlickerLights());
		}
		if (_config.PlayUnlockCassie && !string.IsNullOrWhiteSpace(_config.UnlockCassieWords))
		{
			Cassie.MessageTranslated(_config.UnlockCassieWords, _config.UnlockCassieTranslationRu, false, false, true);
		}
	}

	[IteratorStateMachine(typeof(FlickerLights_d_33))]
	private IEnumerator<float> FlickerLights()
	{
		return new FlickerLights_d_33(0)
		{
			__4__this = this
		};
	}

	private void OnInteractingDoor(InteractingDoorEventArgs ev)
	{
		if (IsLocked && !(ev.Player == (Player)null) && ev.Door != null && ShouldContain(ev.Player) && (IsBlockedDoor(ev.Door) || IsContainmentDoorByRoomName(ev.Door)))
		{
			ev.IsAllowed = false;
			ev.Player.ShowMeowHint(1.5f, "<size=17><b><color=#ff6a6a>КАМЕРА ЗАКРЫТА</color></b></size>\n<size=13><b><color=#ffffffb8>Ожидайте окончания локдауна.</color></b></size>", (HintVerticalAlign)0, 840, -20, (HintAlignment)2);
		}
	}

	private bool ShouldContain(Player player)
	{
		if (player == (Player)null || !player.IsAlive || !player.IsScp)
		{
			return false;
		}
		if (!LockedScps.Contains(player.Role.Type))
		{
			return false;
		}
		Room currentRoom = player.CurrentRoom;
		if ((Object)(object)currentRoom == (Object)null)
		{
			return false;
		}
		if (currentRoom.Doors.Any(IsBlockedDoor))
		{
			return true;
		}
		RoomType type = currentRoom.Type;
		return IsContainmentRoomName(((object)(RoomType)(ref type)).ToString());
	}

	private static bool IsBlockedDoor(Door door)
	{
		if (door == null)
		{
			return false;
		}
		return CollectionExtensions.Contains<DoorType>(LockedDoorTypes, door.Type);
	}

	private static bool IsContainmentDoorByRoomName(Door door)
	{
		if ((Object)(object)((door != null) ? door.Room : null) == (Object)null)
		{
			return false;
		}
		RoomType type = door.Room.Type;
		return IsContainmentRoomName(((object)(RoomType)(ref type)).ToString());
	}

	private static bool IsContainmentRoomName(string roomTypeName)
	{
		if (string.IsNullOrWhiteSpace(roomTypeName))
		{
			return false;
		}
		string text = roomTypeName.ToLowerInvariant();
		return text.Contains("173") || text.Contains("049") || text.Contains("096") || text.Contains("939");
	}

	private void ShowLockdownHint(Player player, float remaining)
	{
		int num = (int)Math.Ceiling(remaining);
		string text = TimeSpan.FromSeconds(num).ToString((num >= 3600) ? "hh\\:mm\\:ss" : "mm\\:ss");
		string message = "<size=17><b><color=white>Локдаун:</color></b></size>\n<size=17><b><color=#FF7F50><size=23>" + text + "</size></color></b></size>\n<size=17><b><color=#FF7F50>→ Не покидайте камеру</color></b></size>\n<size=17><b><color=white><size=11>[ До открытия осталось: `" + num + " сек` ]</size></color></b></size>";
		player.ShowMeowHint(_config.HintRefreshInterval + 0.15f, message, (HintVerticalAlign)0, 139, -390, (HintAlignment)0);
		player.ShowMeowHint(_config.HintRefreshInterval + 0.15f, "<size=83><color=#61616193><b>|</b></color></size>", (HintVerticalAlign)0, 127, -409, (HintAlignment)0);
	}

	private static void ClearContainmentEffects(Player player)
	{
		if (!(player == (Player)null))
		{
			player.DisableEffect((EffectType)5);
			player.DisableEffect((EffectType)11);
		}
	}

	static Plugin()
	{
		DoorType[] array = new DoorType[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		LockedDoorTypes = (DoorType[])(object)array;
	}
}
