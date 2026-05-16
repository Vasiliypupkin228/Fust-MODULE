using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.API.Libraries.SSBinds;
using FultEngine.LoaderModule;
using HintServiceMeow.Core.Enum;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.Module.FunctionMenu;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class ArrestCoroutine_d_44 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Player target;

		public float duration;

		public Plugin __4__this;

		private float startTime;

		private float endTime;

		private CoroutineHandle playerHandle;

		private CoroutineHandle targetHandle;

		private float progress;

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
		public ArrestCoroutine_d_44(int __1__state)
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
				startTime = Time.time;
				endTime = startTime + duration;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime)
			{
				if (player.IsAlive && target.IsAlive && !(Vector3.Distance(player.Position, target.Position) > 2f) && __4__this.IsLookingAtPlayer(player, target))
				{
					progress = (Time.time - startTime) / duration;
					__4__this.ShowArrestProgress(player, target, progress, isTarget: false);
					__4__this.ShowArrestProgress(target, player, progress, isTarget: true);
					__2__current = Timing.WaitForSeconds(0.1f);
					__1__state = 1;
					return true;
				}
				__4__this.ShowMeowHint(player, 3f, "Арест прерван!");
				if (target.IsAlive)
				{
					__4__this.ShowMeowHint(target, 3f, "Арест прерван!");
				}
			}
			if (Time.time >= endTime && player.IsAlive && target.IsAlive)
			{
				__4__this.ArrestPlayer(player, target);
			}
			__4__this._activeArrests.Remove(player);
			if (__4__this._arrestCoroutines.TryGetValue(player, out playerHandle))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { playerHandle });
				__4__this._arrestCoroutines.Remove(player);
			}
			if (__4__this._arrestCoroutines.TryGetValue(target, out targetHandle))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { targetHandle });
				__4__this._arrestCoroutines.Remove(target);
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
	private sealed class Cooldown_d_58 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public int optionIndex;

		public float duration;

		public Plugin __4__this;

		private float endTime;

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
		public Cooldown_d_58(int __1__state)
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
				endTime = Time.time + duration;
				__2__current = Timing.WaitForSeconds(duration);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				__4__this._cooldowns.Remove((player, optionIndex));
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
	private sealed class GrabCoroutine_d_47 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Player target;

		public float duration;

		public Plugin __4__this;

		private float startTime;

		private float endTime;

		private CoroutineHandle playerHandle;

		private CoroutineHandle targetHandle;

		private float progress;

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
		public GrabCoroutine_d_47(int __1__state)
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
				startTime = Time.time;
				endTime = startTime + duration;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime)
			{
				if (player.IsAlive && target.IsAlive && !(Vector3.Distance(player.Position, target.Position) > 2f) && __4__this.IsLookingAtPlayer(player, target))
				{
					progress = (Time.time - startTime) / duration;
					__4__this.ShowGrabProgress(player, target, progress, isTarget: false);
					__4__this.ShowGrabProgress(target, player, progress, isTarget: true);
					__2__current = Timing.WaitForSeconds(0.1f);
					__1__state = 1;
					return true;
				}
				__4__this.ShowMeowHint(player, 3f, "Хватание прервано!");
				if (target.IsAlive)
				{
					__4__this.ShowMeowHint(target, 3f, "Хватание прервано!");
				}
			}
			if (Time.time >= endTime && player.IsAlive && target.IsAlive)
			{
				__4__this.GrabPlayer(player, target);
			}
			__4__this._activeGrabs.Remove(player);
			if (__4__this._grabCoroutines.TryGetValue(player, out playerHandle))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { playerHandle });
				__4__this._grabCoroutines.Remove(player);
			}
			if (__4__this._grabCoroutines.TryGetValue(target, out targetHandle))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { targetHandle });
				__4__this._grabCoroutines.Remove(target);
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
	private sealed class GrabPhysicsCoroutine_d_57 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Player target;

		public Plugin __4__this;

		private float duration;

		private float elapsed;

		private Rigidbody targetRb;

		private bool hasRigidbody;

		private Vector3 targetPosition;

		private Vector3 forceDirection;

		private float distance;

		private float forceMultiplier;

		private Vector3 pushDirection;

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
		public GrabPhysicsCoroutine_d_57(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			targetRb = null;
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
				duration = 10f;
				elapsed = 0f;
				targetRb = target.GameObject.GetComponent<Rigidbody>();
				hasRigidbody = (Object)(object)targetRb != (Object)null;
				if (hasRigidbody)
				{
					targetRb.drag = 10f;
					targetRb.angularDrag = 10f;
					targetRb.useGravity = false;
				}
				player.EnableEffect((EffectType)43, (byte)39, 0f, false);
				break;
			case 1:
				__1__state = -1;
				elapsed += 0.02f;
				break;
			}
			Vector3 val;
			if (elapsed < duration && player.IsAlive && target.IsAlive && Vector3.Distance(player.Position, target.Position) < 7f && __4__this._grabbingPlayers.ContainsKey(player))
			{
				player.Stamina = 0f;
				target.Stamina = 0f;
				targetPosition = player.Position + player.CameraTransform.forward * 1f;
				if (hasRigidbody)
				{
					val = targetPosition - target.Position;
					forceDirection = ((Vector3)(ref val)).normalized;
					distance = Vector3.Distance(target.Position, targetPosition);
					forceMultiplier = Mathf.Clamp(distance * 15f, 5f, 30f);
					targetRb.AddForce(forceDirection * forceMultiplier, (ForceMode)5);
					val = targetRb.velocity;
					if (((Vector3)(ref val)).magnitude > 5f)
					{
						Rigidbody obj = targetRb;
						val = targetRb.velocity;
						obj.velocity = ((Vector3)(ref val)).normalized * 5f;
					}
				}
				else
				{
					target.Position = Vector3.Lerp(target.Position, targetPosition, 0.2f);
				}
				__2__current = Timing.WaitForSeconds(0.02f);
				__1__state = 1;
				return true;
			}
			player.DisableEffect((EffectType)43);
			if (hasRigidbody && (Object)(object)targetRb != (Object)null)
			{
				targetRb.drag = 0f;
				targetRb.angularDrag = 0.05f;
				targetRb.useGravity = true;
				if (player.IsAlive)
				{
					val = player.CameraTransform.forward;
					pushDirection = ((Vector3)(ref val)).normalized * 2f;
					targetRb.AddForce(pushDirection, (ForceMode)1);
				}
			}
			if (__4__this._menuCoroutines.ContainsKey(target))
			{
				__4__this._menuCoroutines.Remove(target);
			}
			if (__4__this._grabbingPlayers.ContainsKey(player))
			{
				__4__this._grabbingPlayers.Remove(player);
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
	private sealed class InspectionCoroutine_d_41 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Player target;

		public float duration;

		public Plugin __4__this;

		private float startTime;

		private float endTime;

		private CoroutineHandle playerHandle;

		private CoroutineHandle targetHandle;

		private float progress;

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
		public InspectionCoroutine_d_41(int __1__state)
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
				startTime = Time.time;
				endTime = startTime + duration;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime)
			{
				if (player.IsAlive && target.IsAlive && !(Vector3.Distance(player.Position, target.Position) > 2f) && __4__this.IsLookingAtPlayer(player, target))
				{
					progress = (Time.time - startTime) / duration;
					__4__this.ShowInspectionProgress(player, target, progress, isTarget: false);
					__4__this.ShowInspectionProgress(target, player, progress, isTarget: true);
					__2__current = Timing.WaitForSeconds(0.1f);
					__1__state = 1;
					return true;
				}
				__4__this.ShowMeowHint(player, 3f, "Осмотр прерван!");
				if (target.IsAlive)
				{
					__4__this.ShowMeowHint(target, 3f, "Осмотр прерван!");
				}
			}
			if (Time.time >= endTime && player.IsAlive && target.IsAlive)
			{
				__4__this.InspectInventory(player, target);
			}
			__4__this._activeInspections.Remove(player);
			if (__4__this._inspectCoroutines.TryGetValue(player, out playerHandle))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { playerHandle });
				__4__this._inspectCoroutines.Remove(player);
			}
			if (__4__this._inspectCoroutines.TryGetValue(target, out targetHandle))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { targetHandle });
				__4__this._inspectCoroutines.Remove(target);
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
	private sealed class ShowMenuCoroutine_d_28 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Plugin __4__this;

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
		public ShowMenuCoroutine_d_28(int __1__state)
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
			if (player.IsAlive && !player.IsScp && !__4__this._requestCoroutines.ContainsKey(player) && !__4__this._inspectCoroutines.ContainsKey(player) && !__4__this._arrestCoroutines.ContainsKey(player) && !__4__this._grabCoroutines.ContainsKey(player))
			{
				__4__this.UpdateMenu(player);
				__2__current = Timing.WaitForSeconds(0.2f);
				__1__state = 1;
				return true;
			}
			if (!__4__this._requestCoroutines.ContainsKey(player) && !__4__this._inspectCoroutines.ContainsKey(player) && !__4__this._arrestCoroutines.ContainsKey(player) && !__4__this._grabCoroutines.ContainsKey(player))
			{
				__4__this.StopMenu(player);
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
	private sealed class TargetArrestCoroutine_d_45 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player target;

		public Player player;

		public float duration;

		public Plugin __4__this;

		private float startTime;

		private float endTime;

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
		public TargetArrestCoroutine_d_45(int __1__state)
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
				startTime = Time.time;
				endTime = startTime + duration;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime && player.IsAlive && target.IsAlive)
			{
				__2__current = Timing.WaitForSeconds(0.1f);
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
	private sealed class TargetGrabCoroutine_d_48 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player target;

		public Player player;

		public float duration;

		public Plugin __4__this;

		private float startTime;

		private float endTime;

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
		public TargetGrabCoroutine_d_48(int __1__state)
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
				startTime = Time.time;
				endTime = startTime + duration;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime && player.IsAlive && target.IsAlive)
			{
				__2__current = Timing.WaitForSeconds(0.1f);
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
	private sealed class TargetInspectionCoroutine_d_42 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player target;

		public Player player;

		public float duration;

		public Plugin __4__this;

		private float startTime;

		private float endTime;

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
		public TargetInspectionCoroutine_d_42(int __1__state)
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
				startTime = Time.time;
				endTime = startTime + duration;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime && player.IsAlive && target.IsAlive)
			{
				__2__current = Timing.WaitForSeconds(0.1f);
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

	private readonly Dictionary<Player, CoroutineHandle> _menuCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> _requestCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, int> _selectedOption = new Dictionary<Player, int>();

	private readonly Dictionary<Player, Player> _targetedPlayers = new Dictionary<Player, Player>();

	private readonly Dictionary<(Player player, int optionIndex), (CoroutineHandle handle, float endTime)> _cooldowns = new Dictionary<(Player, int), (CoroutineHandle, float)>();

	private readonly Dictionary<Player, Player> _grabbingPlayers = new Dictionary<Player, Player>();

	private readonly Dictionary<Player, CoroutineHandle> _inspectCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, (Player target, float startTime, float duration)> _activeInspections = new Dictionary<Player, (Player, float, float)>();

	private readonly Dictionary<Player, CoroutineHandle> _arrestCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, (Player target, float startTime, float duration)> _activeArrests = new Dictionary<Player, (Player, float, float)>();

	private readonly Dictionary<Player, CoroutineHandle> _grabCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, (Player target, float startTime, float duration)> _activeGrabs = new Dictionary<Player, (Player, float, float)>();

	private readonly HashSet<Player> _tankMenuPlayers = new HashSet<Player>();

	public string Name { get; } = "FunctionMenu";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 1);


	private string[] GetMenuOptions(Player player)
	{
		if (_grabbingPlayers.ContainsKey(player))
		{
			return new string[3] { "Осмотреть инвентарь", "Арестовать игрока", "Отпустить игрока" };
		}
		if (_grabbingPlayers.ContainsValue(player))
		{
			return new string[1] { "Выбраться" };
		}
		if (_tankMenuPlayers.Contains(player))
		{
			return new string[5] { "Сесть: водитель", "Сесть: наводчик", "Сесть: командир", "Покинуть танк", "Статус экипажа" };
		}
		return new string[3] { "Осмотреть инвентарь", "Арестовать игрока", "Тащить за собой" };
	}

	public void OnEnabled()
	{
		KeybindManager.AddCustomKeybind(81, "╔ <color=#c2151539>\ud83c\udf32</color> Открыть/Закрыть взаимодействия меню", (KeyCode)118, preventInteractionOnGUI: false, "");
		KeybindManager.AddCustomKeybind(82, "╠ <color=#2415c239>\ud83c\udf40</color> Навигация вверх", (KeyCode)273, preventInteractionOnGUI: false, "");
		KeybindManager.AddCustomKeybind(83, "╠ <color=#1a699639>\ud83d\udc32</color> Навигация вниз", (KeyCode)274, preventInteractionOnGUI: false, "");
		KeybindManager.AddCustomKeybind(84, "╚ <color=#1a964c39>\ud83d\udc33</color> Выбрать действие", (KeyCode)13, preventInteractionOnGUI: false, "");
		KeybindManager.OnObjectInteraction += OnKeybindPressed;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
	}

	public void OnDisabled()
	{
		KeybindManager.OnObjectInteraction -= OnKeybindPressed;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		foreach (CoroutineHandle value in _menuCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		foreach (CoroutineHandle value2 in _requestCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
		}
		foreach (CoroutineHandle value3 in _inspectCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value3 });
		}
		foreach (CoroutineHandle value4 in _arrestCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value4 });
		}
		foreach (CoroutineHandle value5 in _grabCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value5 });
		}
		foreach (var value6 in _cooldowns.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value6.handle });
		}
		_menuCoroutines.Clear();
		_requestCoroutines.Clear();
		_selectedOption.Clear();
		_targetedPlayers.Clear();
		_cooldowns.Clear();
		_grabbingPlayers.Clear();
		_inspectCoroutines.Clear();
		_activeInspections.Clear();
		_arrestCoroutines.Clear();
		_activeArrests.Clear();
		_grabCoroutines.Clear();
		_activeGrabs.Clear();
		_tankMenuPlayers.Clear();
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (ev.Player != (Player)null)
		{
			StopMenu(ev.Player);
		}
	}

	private void OnDying(DyingEventArgs ev)
	{
		if (ev.Player == (Player)null)
		{
			return;
		}
		if (_grabbingPlayers.ContainsValue(ev.Player))
		{
			Player key = _grabbingPlayers.FirstOrDefault((KeyValuePair<Player, Player> x) => x.Value == ev.Player).Key;
			if (key != (Player)null)
			{
				ShowMeowHint(key, 5f, ev.Player.DisplayNickname + " умер и был освобождён");
			}
		}
		if (_grabbingPlayers.ContainsKey(ev.Player))
		{
			Player player = _grabbingPlayers[ev.Player];
			ShowMeowHint(player, 5f, ev.Player.DisplayNickname + " умер и отпустил вас");
		}
		StopMenu(ev.Player);
	}

	private void OnKeybindPressed(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val == null || !val.SyncIsPressed)
		{
			return;
		}
		Player val2 = Player.Get(hub);
		if (val2 == (Player)null || !val2.IsAlive || val2.IsScp)
		{
			return;
		}
		switch (((ServerSpecificSettingBase)val).SettingId)
		{
		case 81:
			if (!_requestCoroutines.ContainsKey(val2) && !_inspectCoroutines.ContainsKey(val2) && !_arrestCoroutines.ContainsKey(val2) && !_grabCoroutines.ContainsKey(val2))
			{
				if (!_menuCoroutines.ContainsKey(val2))
				{
					_selectedOption[val2] = 0;
					_menuCoroutines[val2] = Timing.RunCoroutine(ShowMenuCoroutine(val2));
					ShowMeowHint(val2, 5f, "Меню взаимодействия открыто");
				}
				else
				{
					StopMenu(val2);
					ShowMeowHint(val2, 5f, "Меню взаимодействия закрыто");
				}
			}
			break;
		case 82:
			if (_menuCoroutines.ContainsKey(val2))
			{
				_selectedOption[val2] = (_selectedOption[val2] - 1 + GetMenuOptions(val2).Length) % GetMenuOptions(val2).Length;
				UpdateMenu(val2);
			}
			break;
		case 83:
			if (_menuCoroutines.ContainsKey(val2))
			{
				_selectedOption[val2] = (_selectedOption[val2] + 1) % GetMenuOptions(val2).Length;
				UpdateMenu(val2);
			}
			break;
		case 84:
			if (_menuCoroutines.ContainsKey(val2))
			{
				ExecuteSelectedMenuAction(val2);
			}
			break;
		}
	}

	[IteratorStateMachine(typeof(ShowMenuCoroutine_d_28))]
	private IEnumerator<float> ShowMenuCoroutine(Player player)
	{
		return new ShowMenuCoroutine_d_28(0)
		{
			__4__this = this,
			player = player
		};
	}

	private void UpdateMenu(Player player)
	{
		if (player == (Player)null || !player.IsAlive || player.IsScp)
		{
			StopMenu(player);
			return;
		}
		if (_grabbingPlayers.ContainsValue(player))
		{
			_tankMenuPlayers.Remove(player);
			_targetedPlayers.Remove(player);
			ClampSelectedOption(player);
			ShowEscapeMenu(player);
			return;
		}
		Player val = FindTargetPlayer(player);
		if (val != (Player)null)
		{
			_tankMenuPlayers.Remove(player);
			_targetedPlayers[player] = val;
			ClampSelectedOption(player);
			ShowMenu(player, val);
			return;
		}
		_targetedPlayers.Remove(player);
		if (IsNearTank(player))
		{
			_tankMenuPlayers.Add(player);
			ClampSelectedOption(player);
			ShowMenu(player, null);
		}
		else
		{
			_tankMenuPlayers.Remove(player);
			ClampSelectedOption(player);
			ShowEmptyMenu(player);
		}
	}

	private Player FindTargetPlayer(Player player)
	{
		if ((Object)(object)((player != null) ? player.CameraTransform : null) == (Object)null)
		{
			return null;
		}
		Player result = null;
		float num = float.MinValue;
		Vector3 position = player.CameraTransform.position;
		Vector3 forward = player.CameraTransform.forward;
		RaycastHit val4 = default(RaycastHit);
		foreach (Player item in Player.List)
		{
			if (item == (Player)null || item == player || !item.IsAlive || item.IsScp)
			{
				continue;
			}
			Vector3 val = item.Position + Vector3.up * 0.8f;
			Vector3 val2 = val - position;
			float magnitude = ((Vector3)(ref val2)).magnitude;
			if (magnitude > 3.2f || magnitude <= 0.05f)
			{
				continue;
			}
			Vector3 val3 = val2 / magnitude;
			float num2 = Vector3.Dot(forward, val3);
			if (num2 < 0.72f)
			{
				continue;
			}
			if (Physics.Raycast(position, val3, ref val4, magnitude, -1, (QueryTriggerInteraction)1))
			{
				Player val5 = Player.Get(((Component)((RaycastHit)(ref val4)).collider).gameObject);
				if (val5 != (Player)null && val5 != item)
				{
					continue;
				}
			}
			float num3 = num2 * 10f - magnitude;
			if (num3 > num)
			{
				num = num3;
				result = item;
			}
		}
		return result;
	}

	private void ClampSelectedOption(Player player)
	{
		string[] menuOptions = GetMenuOptions(player);
		if (menuOptions == null || menuOptions.Length == 0)
		{
			_selectedOption[player] = 0;
			return;
		}
		if (!_selectedOption.ContainsKey(player))
		{
			_selectedOption[player] = 0;
		}
		if (_selectedOption[player] < 0)
		{
			_selectedOption[player] = 0;
		}
		else if (_selectedOption[player] >= menuOptions.Length)
		{
			_selectedOption[player] = menuOptions.Length - 1;
		}
	}

	private void ShowEmptyMenu(Player player)
	{
		string message = "<size=29><b><color=#61616193>『</color></size> <size=21>Меню взаимодействий</size> <size=29><b><color=#61616193>』</color></size>\n<size=17><b><color=#ffffffb8>Наведитесь на игрока рядом</color></b></size>\n<size=15><b><color=#61616193>Дистанция: до 3 метров</color></b></size>";
		player.ShowMeowHint(0.2f, message, (HintVerticalAlign)0, 870, 0, (HintAlignment)2);
	}

	private void ShowMenu(Player player, Player target)
	{
		string text = "<size=29><b><color=#61616193>『</color></size> <size=21>Меню взаимодействий</size> <size=29><b><color=#61616193>』</color></size>\n";
		string[] menuOptions = GetMenuOptions(player);
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		for (int i = 0; i < menuOptions.Length; i++)
		{
			if (_cooldowns.TryGetValue((player, i), out (CoroutineHandle, float) value) && ((CoroutineHandle)(ref value.Item1)).IsRunning)
			{
				float num = value.Item2 - Time.time;
				if (num > 0f)
				{
					dictionary[i] = num;
				}
			}
		}
		for (int j = 0; j < menuOptions.Length; j++)
		{
			string text2 = "";
			if (dictionary.TryGetValue(j, out var value2))
			{
				text2 = $" <color=#61616193>[<color=#7a060678>{Mathf.CeilToInt(value2)}с</color><color=#61616193>]</color>";
			}
			text += string.Format("<size={0}><b>{1} {2} <color=#61616193>{3}</color>{4}</b></color></size>\n", 19, (_selectedOption[player] == j) ? "<color=#61616193>→</color>" : "", menuOptions[j], (_selectedOption[player] == j) ? "←" : "", text2);
		}
		player.ShowMeowHint(0.2f, text, (HintVerticalAlign)0, 870, 0, (HintAlignment)2);
	}

	private void ShowEscapeMenu(Player player)
	{
		string text = "<size=29><b><color=#61616193>『</color></size> <size=21>Меню взаимодействий</size> <size=29><b><color=#61616193>』</color></size>\n";
		string[] menuOptions = GetMenuOptions(player);
		int[] array = new int[1] { 19 };
		text += string.Format("<size={0}><b>{1} {2} <color=#61616193>{3}</color></b></size>\n", array[0], (_selectedOption[player] == 0) ? "<color=#61616193>→</color>" : "", menuOptions[0], (_selectedOption[player] == 0) ? "←" : "");
		player.ShowMeowHint(0.2f, text, (HintVerticalAlign)0, 870, 0, (HintAlignment)2);
	}

	private bool IsNearTank(Player player)
	{
		if (player == (Player)null || !player.IsAlive)
		{
			return false;
		}
		float num = 100f;
		Transform[] array = Object.FindObjectsOfType<Transform>();
		foreach (Transform val in array)
		{
			if ((Object)(object)val == (Object)null)
			{
				continue;
			}
			string text = ((Object)val).name ?? string.Empty;
			if (text.IndexOf("Nu7PanzerAnimation", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				Vector3 val2 = val.position - player.Position;
				float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ExecuteSelectedMenuAction(Player player)
	{
		if (player == (Player)null || !player.IsAlive)
		{
			return;
		}
		ClampSelectedOption(player);
		if (_grabbingPlayers.ContainsValue(player))
		{
			TryEscapeGrab(player);
			return;
		}
		if (_tankMenuPlayers.Contains(player))
		{
			ExecuteTankMenuAction(player);
			return;
		}
		if (!_targetedPlayers.TryGetValue(player, out var value) || value == (Player)null || !value.IsAlive || value.IsScp || Vector3.Distance(player.Position, value.Position) > 3.5f)
		{
			value = FindTargetPlayer(player);
			if (value == (Player)null)
			{
				ShowMeowHint(player, 2f, "Наведитесь на живого игрока рядом");
				return;
			}
			_targetedPlayers[player] = value;
		}
		ExecuteAction(player, value);
	}

	private void TryEscapeGrab(Player target)
	{
		Player key = _grabbingPlayers.FirstOrDefault((KeyValuePair<Player, Player> x) => x.Value == target).Key;
		if (key == (Player)null)
		{
			StopMenu(target);
			return;
		}
		ReleasePlayer(key);
		ShowMeowHint(target, 4f, "Вы вырвались");
		ShowMeowHint(key, 4f, target.DisplayNickname + " вырвался");
		StopMenu(target);
	}

	private void ExecuteTankMenuAction(Player player)
	{
		string[] menuOptions = GetMenuOptions(player);
		int value;
		int num = (_selectedOption.TryGetValue(player, out value) ? value : 0);
		string text = ((num >= 0 && num < menuOptions.Length) ? menuOptions[num] : "действие");
		ShowMeowHint(player, 3f, "Действие танка '" + text + "' пока не подключено к BTR-модулю");
	}

	private void ExecuteAction(Player player, Player target)
	{
		string[] menuOptions = GetMenuOptions(player);
		int num = _selectedOption[player];
		if (_cooldowns.TryGetValue((player, num), out (CoroutineHandle, float) value) && ((CoroutineHandle)(ref value.Item1)).IsRunning)
		{
			return;
		}
		switch (num)
		{
		case 0:
			StartInventoryInspection(player, target);
			_cooldowns[(player, num)] = (Timing.RunCoroutine(Cooldown(player, num, 10f)), Time.time + 10f);
			break;
		case 1:
			if (!player.CurrentItem.IsWeapon)
			{
				ShowMeowHint(player, 5f, "Возьмите оружие в руки!");
				break;
			}
			StartArrest(player, target);
			_cooldowns[(player, num)] = (Timing.RunCoroutine(Cooldown(player, num, 15f)), Time.time + 15f);
			break;
		case 2:
			if (_grabbingPlayers.ContainsKey(player))
			{
				ReleasePlayer(player);
				break;
			}
			StartGrab(player, target);
			_cooldowns[(player, num)] = (Timing.RunCoroutine(Cooldown(player, num, 5f)), Time.time + 5f);
			break;
		}
	}

	private void StartInventoryInspection(Player player, Player target)
	{
		if (!_inspectCoroutines.ContainsKey(player))
		{
			StopMenu(player);
			float num = 7f;
			_activeInspections[player] = (target, Time.time, num);
			_inspectCoroutines[player] = Timing.RunCoroutine(InspectionCoroutine(player, target, num));
			_inspectCoroutines[target] = Timing.RunCoroutine(TargetInspectionCoroutine(target, player, num));
		}
	}

	[IteratorStateMachine(typeof(InspectionCoroutine_d_41))]
	private IEnumerator<float> InspectionCoroutine(Player player, Player target, float duration)
	{
		return new InspectionCoroutine_d_41(0)
		{
			__4__this = this,
			player = player,
			target = target,
			duration = duration
		};
	}

	[IteratorStateMachine(typeof(TargetInspectionCoroutine_d_42))]
	private IEnumerator<float> TargetInspectionCoroutine(Player target, Player player, float duration)
	{
		return new TargetInspectionCoroutine_d_42(0)
		{
			__4__this = this,
			target = target,
			player = player,
			duration = duration
		};
	}

	private void StartArrest(Player player, Player target)
	{
		if (!_arrestCoroutines.ContainsKey(player))
		{
			if (!player.Items.Any((Item i) => i.IsArmor))
			{
				ShowMeowHint(player, 5f, "Наденьте бронежилет");
				return;
			}
			StopMenu(player);
			float num = 10f;
			_activeArrests[player] = (target, Time.time, num);
			_arrestCoroutines[player] = Timing.RunCoroutine(ArrestCoroutine(player, target, num));
			_arrestCoroutines[target] = Timing.RunCoroutine(TargetArrestCoroutine(target, player, num));
		}
	}

	[IteratorStateMachine(typeof(ArrestCoroutine_d_44))]
	private IEnumerator<float> ArrestCoroutine(Player player, Player target, float duration)
	{
		return new ArrestCoroutine_d_44(0)
		{
			__4__this = this,
			player = player,
			target = target,
			duration = duration
		};
	}

	[IteratorStateMachine(typeof(TargetArrestCoroutine_d_45))]
	private IEnumerator<float> TargetArrestCoroutine(Player target, Player player, float duration)
	{
		return new TargetArrestCoroutine_d_45(0)
		{
			__4__this = this,
			target = target,
			player = player,
			duration = duration
		};
	}

	private void StartGrab(Player player, Player target)
	{
		if (!_grabCoroutines.ContainsKey(player))
		{
			StopMenu(player);
			float num = 5f;
			_activeGrabs[player] = (target, Time.time, num);
			_grabCoroutines[player] = Timing.RunCoroutine(GrabCoroutine(player, target, num));
			_grabCoroutines[target] = Timing.RunCoroutine(TargetGrabCoroutine(target, player, num));
		}
	}

	[IteratorStateMachine(typeof(GrabCoroutine_d_47))]
	private IEnumerator<float> GrabCoroutine(Player player, Player target, float duration)
	{
		return new GrabCoroutine_d_47(0)
		{
			__4__this = this,
			player = player,
			target = target,
			duration = duration
		};
	}

	[IteratorStateMachine(typeof(TargetGrabCoroutine_d_48))]
	private IEnumerator<float> TargetGrabCoroutine(Player target, Player player, float duration)
	{
		return new TargetGrabCoroutine_d_48(0)
		{
			__4__this = this,
			target = target,
			player = player,
			duration = duration
		};
	}

	private void ShowGrabProgress(Player player, Player target, float progress, bool isTarget)
	{
		string arg = (isTarget ? "ВАС ХВАТАЮТ" : "ХВАТАНИЕ ИГРОКА");
		string text = (isTarget ? player.DisplayNickname : target.DisplayNickname);
		int num = 18;
		int num2 = Mathf.RoundToInt(progress * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt(progress * 100f);
		string arg2 = "<color=#00ff00>" + new string('▒', num2) + "</color><color=#006600>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>{arg}</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg2}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{num3}%</size> <size=29><b><color=#61616193>|</color></size></b>";
		player.ShowMeowHint(0.2f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}

	private void ShowArrestProgress(Player player, Player target, float progress, bool isTarget)
	{
		string arg = (isTarget ? "ВАС АРЕСТОВЫВАЮТ" : "АРЕСТ ИГРОКА");
		string text = (isTarget ? player.DisplayNickname : target.DisplayNickname);
		int num = 18;
		int num2 = Mathf.RoundToInt(progress * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt(progress * 100f);
		string arg2 = "<color=#ff0000>" + new string('▒', num2) + "</color><color=#730b0b>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>{arg}</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg2}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{num3}%</size> <size=29><b><color=#61616193>|</color></size></b>";
		player.ShowMeowHint(0.2f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}

	private void ArrestPlayer(Player player, Player target)
	{
		try
		{
			target.Handcuff();
			string message = "Вы арестовали " + target.DisplayNickname;
			ShowMeowHint(player, 7f, message);
			string message2 = player.DisplayNickname + " арестовал вас";
			ShowMeowHint(target, 7f, message2);
		}
		catch (Exception arg)
		{
			Log.Error($"Ошибка при аресте игрока: {arg}");
			ShowMeowHint(player, 5f, "Ошибка при аресте игрока");
		}
	}

	private bool IsLookingAtPlayer(Player player, Player target)
	{
		Vector3 val = target.Position - player.Position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float num = Vector3.Dot(player.CameraTransform.forward, normalized);
		return num > 0.7f;
	}

	private void ShowInspectionProgress(Player player, Player target, float progress, bool isTarget)
	{
		string arg = (isTarget ? "ВАС ОСМАТРИВАЮТ" : "ОСМОТР ИНВЕНТАРЯ");
		string text = (isTarget ? player.DisplayNickname : target.DisplayNickname);
		int num = 18;
		int num2 = Mathf.RoundToInt(progress * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt(progress * 100f);
		string arg2 = "<color=#ffffff>" + new string('▒', num2) + "</color><color=#616161>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>{arg}</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg2}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{num3}%</size> <size=29><b><color=#61616193>|</color></size></b>";
		player.ShowMeowHint(0.2f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}

	private void InspectInventory(Player player, Player target)
	{
		try
		{
			if (((target != null) ? target.Items : null) == null)
			{
				ShowMeowHint(player, 7f, "Инвентарь " + target.DisplayNickname + " пуст");
				return;
			}
			string[] array = target.Items.Select((Item item) => $"<size=19><color=#61616193><b>┏</color> {item.Type} <color=#61616193>┓</color></b></size>").ToArray();
			if (array.Length == 0)
			{
				ShowMeowHint(player, 7f, "Инвентарь " + target.DisplayNickname + " пуст");
			}
			else
			{
				string text = string.Join("\n", array);
				string message = "<size=29><b><color=#61616193>『</color></size> <size=21>Инвентарь " + target.DisplayNickname + "</size> <size=29><b><color=#61616193>』</color></size>\n" + text;
				player.ShowMeowHint(9f, message, (HintVerticalAlign)0, 359, 779, (HintAlignment)2);
			}
			ShowMeowHint(target, 5f, player.DisplayNickname + " осмотрел ваш инвентарь");
		}
		catch (Exception arg)
		{
			Log.Error($"Ошибка при осмотре инвентаря: {arg}");
			ShowMeowHint(player, 5f, "Ошибка при осмотре инвентаря");
		}
	}

	private void GrabPlayer(Player player, Player target)
	{
		if (_menuCoroutines.TryGetValue(target, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_menuCoroutines.Remove(target);
		}
		_grabbingPlayers[player] = target;
		_menuCoroutines[target] = Timing.RunCoroutine(ShowMenuCoroutine(target));
		_menuCoroutines[player] = Timing.RunCoroutine(GrabPhysicsCoroutine(player, target));
		string message = player.DisplayNickname + " тащит вас, откройте меню и выберите 'Выбраться'</color></b></size>";
		ShowMeowHint(target, 7f, message);
	}

	private void ReleasePlayer(Player player)
	{
		if (_grabbingPlayers.TryGetValue(player, out var value))
		{
			if (_menuCoroutines.TryGetValue(value, out var value2))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
				_menuCoroutines.Remove(value);
			}
			if (_menuCoroutines.TryGetValue(player, out var value3))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value3 });
				_menuCoroutines.Remove(player);
			}
			ResetTargetPhysics(value);
			player.DisableEffect((EffectType)43);
			_grabbingPlayers.Remove(player);
			string message = player.DisplayNickname + " отпустил вас";
			ShowMeowHint(value, 7f, message);
		}
	}

	[IteratorStateMachine(typeof(GrabPhysicsCoroutine_d_57))]
	private IEnumerator<float> GrabPhysicsCoroutine(Player player, Player target)
	{
		return new GrabPhysicsCoroutine_d_57(0)
		{
			__4__this = this,
			player = player,
			target = target
		};
	}

	[IteratorStateMachine(typeof(Cooldown_d_58))]
	private IEnumerator<float> Cooldown(Player player, int optionIndex, float duration)
	{
		return new Cooldown_d_58(0)
		{
			__4__this = this,
			player = player,
			optionIndex = optionIndex,
			duration = duration
		};
	}

	private void StopMenu(Player player)
	{
		if (_menuCoroutines.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_menuCoroutines.Remove(player);
		}
		if (_requestCoroutines.TryGetValue(player, out var value2))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
			_requestCoroutines.Remove(player);
		}
		if (_inspectCoroutines.TryGetValue(player, out var value3))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value3 });
			_inspectCoroutines.Remove(player);
			_activeInspections.Remove(player);
		}
		if (_arrestCoroutines.TryGetValue(player, out var value4))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value4 });
			_arrestCoroutines.Remove(player);
			_activeArrests.Remove(player);
		}
		if (_grabCoroutines.TryGetValue(player, out var value5))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value5 });
			_grabCoroutines.Remove(player);
			_activeGrabs.Remove(player);
		}
		if (_grabbingPlayers.TryGetValue(player, out var value6))
		{
			if (_menuCoroutines.TryGetValue(value6, out var value7))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value7 });
				_menuCoroutines.Remove(value6);
			}
			ResetTargetPhysics(value6);
			player.DisableEffect((EffectType)43);
			_grabbingPlayers.Remove(player);
		}
		if (_grabbingPlayers.ContainsValue(player))
		{
			Player key = _grabbingPlayers.FirstOrDefault((KeyValuePair<Player, Player> x) => x.Value == player).Key;
			if (key != (Player)null && _grabbingPlayers.Remove(key))
			{
				ResetTargetPhysics(player);
				key.DisableEffect((EffectType)43);
			}
		}
		_selectedOption.Remove(player);
		_targetedPlayers.Remove(player);
		_tankMenuPlayers.Remove(player);
		List<(Player, int)> list = _cooldowns.Keys.Where(((Player player, int optionIndex) x) => x.player == player).ToList();
		foreach (var item in list)
		{
			if (_cooldowns.TryGetValue(item, out (CoroutineHandle, float) value8))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value8.Item1 });
			}
			_cooldowns.Remove(item);
		}
	}

	private void ResetTargetPhysics(Player target)
	{
		if (!((Object)(object)((target != null) ? target.GameObject : null) == (Object)null))
		{
			Rigidbody component = target.GameObject.GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				component.drag = 0f;
				component.angularDrag = 0.05f;
				component.useGravity = true;
			}
		}
	}

	private void ShowMeowHint(Player player, float time, string message)
	{
		player.ShowMeowHint(time, "<size=29><b><color=#61616193>|</color></size> <size=19>" + message + "</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
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
		config = null;
	}
}
