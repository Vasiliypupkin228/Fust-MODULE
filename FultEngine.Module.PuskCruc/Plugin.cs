using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using UnityEngine;

namespace FultEngine.Module.PuskCruc;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class ExplosionTimer_d_16 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player ply;

		public Item item;

		public Plugin __4__this;

		private float maxTime;

		private ItemType itemType;

		private float elapsed;

		private float left;

		private Projectile itemProjectile;

		private ExplosiveGrenade grenade;

		private ExplosiveGrenade flash;

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
		public ExplosionTimer_d_16(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			itemProjectile = null;
			grenade = null;
			flash = null;
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
				maxTime = 3f;
				itemType = item.Type;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			elapsed = Time.realtimeSinceStartup - __4__this._startTime[ply];
			left = maxTime - elapsed;
			if (left <= 0f)
			{
				ply.RemoveItem(item, true);
				item.Destroy();
				itemProjectile = null;
				if ((int)itemType == 25)
				{
					grenade = (ExplosiveGrenade)Item.Create((ItemType)25, (Player)null);
					grenade.FuseTime = 0.1f;
					itemProjectile = (Projectile)(object)grenade.SpawnActive(ply.Position + Vector3.up, ply);
					grenade = null;
				}
				else if ((int)itemType == 26)
				{
					flash = (ExplosiveGrenade)Item.Create((ItemType)26, (Player)null);
					flash.FuseTime = 0.1f;
					itemProjectile = (Projectile)(object)flash.SpawnActive(ply.Position + Vector3.up, ply);
					flash = null;
				}
				__4__this.CancelTimer(ply);
				return false;
			}
			ply.ClearBroadcasts();
			ply.Broadcast((ushort)1, $"<b><color=#3b4345cd><size=21>Скобы: нет</color> <color=#5e1116df>{left:F1}</color></size><b>", (BroadcastFlags)0, false);
			__2__current = Timing.WaitForSeconds(0.1f);
			__1__state = 1;
			return true;
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

	private readonly Dictionary<Player, CoroutineHandle> _timers = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, float> _startTime = new Dictionary<Player, float>();

	private readonly List<ItemType> _timedGrenadeTypes = new List<ItemType>
	{
		(ItemType)25,
		(ItemType)26
	};

	public static Plugin Instance { get; private set; }

	public string Name => "PuskCruc";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 1);

	public void OnEnabled()
	{
		Instance = this;
		Player.TogglingNoClip += (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
		Player.ThrownProjectile += (CustomEventHandler<ThrownProjectileEventArgs>)OnThrownProjectile;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
	}

	public void OnDisabled()
	{
		Player.TogglingNoClip -= (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
		Player.ThrownProjectile -= (CustomEventHandler<ThrownProjectileEventArgs>)OnThrownProjectile;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		foreach (CoroutineHandle value in _timers.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		_timers.Clear();
		_startTime.Clear();
		Instance = null;
	}

	private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
	{
		if (ev.IsAllowed && ev.Player.CurrentItem != null && _timedGrenadeTypes.Contains(ev.Player.CurrentItem.Type) && !_timers.ContainsKey(ev.Player))
		{
			_startTime[ev.Player] = Time.realtimeSinceStartup;
			_timers[ev.Player] = Timing.RunCoroutine(ExplosionTimer(ev.Player, ev.Player.CurrentItem));
		}
	}

	[IteratorStateMachine(typeof(ExplosionTimer_d_16))]
	private IEnumerator<float> ExplosionTimer(Player ply, Item item)
	{
		return new ExplosionTimer_d_16(0)
		{
			__4__this = this,
			ply = ply,
			item = item
		};
	}

	private void OnThrownProjectile(ThrownProjectileEventArgs ev)
	{
		if (!_timedGrenadeTypes.Contains(ev.Item.Type) || !_startTime.TryGetValue(ev.Player, out var value))
		{
			return;
		}
		float num = Time.realtimeSinceStartup - value;
		float remaining = Mathf.Max(0.3f, 3f - num);
		CancelTimer(ev.Player);
		Timing.CallDelayed(0.05f, (Action)delegate
		{
			Projectile projectile = ev.Projectile;
			ExplosionGrenadeProjectile val = (ExplosionGrenadeProjectile)(object)((projectile is ExplosionGrenadeProjectile) ? projectile : null);
			if (val != null)
			{
				((TimeGrenadeProjectile)val).FuseTime = remaining;
			}
			else
			{
				Projectile projectile2 = ev.Projectile;
				ExplosionGrenadeProjectile val2 = (ExplosionGrenadeProjectile)(object)((projectile2 is ExplosionGrenadeProjectile) ? projectile2 : null);
				if (val2 != null)
				{
					((TimeGrenadeProjectile)val2).FuseTime = remaining;
				}
			}
		});
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		if (_timedGrenadeTypes.Contains(ev.Item.Type))
		{
			ev.IsAllowed = false;
		}
	}

	private void CancelTimer(Player ply)
	{
		if (_timers.TryGetValue(ply, out var value))
		{
			ply.ClearBroadcasts();
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_timers.Remove(ply);
		}
		_startTime.Remove(ply);
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
}
