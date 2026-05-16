using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.API.Libraries.SSBinds;
using FultEngine.CustomItems;
using HintServiceMeow.Core.Enum;
using InventorySystem.Items.Pickups;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace CustomItems.Item;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Airstrike : CustomGrenade
{
	private class CollisionHandler : MonoBehaviour
	{
		private Player _owner;

		private Projectile _projectile;

		private Airstrike _airstrike;

		public void Init(Player owner, Projectile projectile, Airstrike airstrike)
		{
			_owner = owner;
			_projectile = projectile;
			_airstrike = airstrike;
		}

		private void OnCollisionEnter(Collision collision)
		{
			Rigidbody val = default(Rigidbody);
			if ((collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ceiling")) && ((Pickup)_projectile).GameObject.TryGetComponent<Rigidbody>(ref val))
			{
				val.isKinematic = true;
				val.useGravity = false;
				val.velocity = Vector3.zero;
				val.angularVelocity = Vector3.zero;
			}
		}
	}

	[CompilerGenerated]
	private sealed class InitiateAirstrike_d_61 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Vector3 grenadePos;

		public Pickup pickup;

		public Player thrower;

		public float delayTime;

		public Airstrike __4__this;

		private float elapsedTime;

		private float updateInterval;

		private float progress;

		private int i;

		private Vector3 randomPos;

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
		public InitiateAirstrike_d_61(int __1__state)
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
				elapsedTime = 0f;
				updateInterval = 0.2f;
				goto IL_00aa;
			case 1:
				__1__state = -1;
				goto IL_00aa;
			case 2:
				{
					__1__state = -1;
					i++;
					break;
				}
				IL_00aa:
				if (elapsedTime < delayTime)
				{
					progress = elapsedTime / delayTime;
					__4__this.ShowAirstrikeProgress(thrower, progress);
					elapsedTime += updateInterval;
					__2__current = Timing.WaitForSeconds(updateInterval);
					__1__state = 1;
					return true;
				}
				__4__this.ShowAirstrikeProgress(thrower, 1f);
				Map.Explode(grenadePos, (ProjectileType)1, (Player)null);
				i = 0;
				break;
			}
			if (i < __4__this.ExplosionCount)
			{
				randomPos = grenadePos + Random.insideUnitSphere * __4__this.Radius;
				randomPos.y = grenadePos.y;
				Map.Explode(randomPos, (ProjectileType)1, (Player)null);
				__2__current = Timing.WaitForSeconds(Random.Range(__4__this.ExplosionDelayMin, __4__this.ExplosionDelayMax));
				__1__state = 2;
				return true;
			}
			pickup.Destroy();
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

	private readonly HashSet<uint> _processedProjectiles = new HashSet<uint>();

	private readonly Dictionary<Player, float> _playerTimerSettings = new Dictionary<Player, float>();

	private readonly float _defaultTimer = 7f;

	private readonly float _minTimer = 5f;

	private readonly float _maxTimer = 31f;

	public override uint Id { get; set; } = 3u;


	public override string Name { get; set; } = "<b><color=#FF6347>Авиаудар</b></color>";


	public override float Weight { get; set; } = 0.75f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override float FuseTime { get; set; } = 31f;


	public override bool ExplodeOnCollision { get; set; } = false;


	public float Radius { get; set; } = 19f;


	public int ExplosionCount { get; set; } = 35;


	public float ExplosionDelayMin { get; set; } = 0.1f;


	public float ExplosionDelayMax { get; set; } = 0.3f;


	public bool IsSticky { get; set; } = true;


	protected override void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.ThrownProjectile += (CustomEventHandler<ThrownProjectileEventArgs>)((CustomGrenade)this).OnThrownProjectile;
		Player.Destroying += (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		KeybindManager.OnObjectInteraction += OnSettingValueReceived;
		((CustomGrenade)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.ThrownProjectile -= (CustomEventHandler<ThrownProjectileEventArgs>)((CustomGrenade)this).OnThrownProjectile;
		Player.Destroying -= (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		KeybindManager.OnObjectInteraction -= OnSettingValueReceived;
		_playerTimerSettings.Clear();
		((CustomGrenade)this).UnsubscribeEvents();
	}

	protected override void OnWaitingForPlayers()
	{
		_processedProjectiles.Clear();
		_playerTimerSettings.Clear();
		((CustomItem)this).OnWaitingForPlayers();
	}

	private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val == null || !val.SyncIsPressed)
		{
			return;
		}
		Player val2 = Player.Get(hub);
		if (val2 == (Player)null || val2.IsScp)
		{
			return;
		}
		Item val3 = ((IEnumerable<Item>)val2.Items).FirstOrDefault((Func<Item, bool>)((Item item) => ((CustomItem)this).Check(item)));
		if (val3 != null)
		{
			float num = (_playerTimerSettings.ContainsKey(val2) ? _playerTimerSettings[val2] : _defaultTimer);
			float num2 = num;
			switch (((ServerSpecificSettingBase)val).SettingId)
			{
			case 353:
				num2 = Mathf.Min(num + 1f, _maxTimer);
				break;
			case 354:
				num2 = Mathf.Max(num - 1f, _minTimer);
				break;
			}
			if (num2 != num)
			{
				_playerTimerSettings[val2] = num2;
				val2.ShowMeowHint(5f, $"<size=29><b><color=#61616193>|</color></size> <size=19>Таймер авиаудара установлен на {num2} секунд</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
			}
		}
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Pickup))
		{
			if (!_playerTimerSettings.ContainsKey(ev.Player))
			{
				_playerTimerSettings[ev.Player] = _defaultTimer;
			}
			ShowItemHint(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
			if (!_playerTimerSettings.ContainsKey(ev.Player))
			{
				_playerTimerSettings[ev.Player] = _defaultTimer;
			}
			ShowItemHint(ev.Player);
		}
	}

	private void ShowItemHint(Player player)
	{
		float num = (_playerTimerSettings.ContainsKey(player) ? _playerTimerSettings[player] : _defaultTimer);
		CIMessage.SendMessage(player, $"</b>Авиаудар\n<size=19>Дымовая граната, вызывающая серию\nвзрывов в зоне броска,\nприлипает к поверхностям. Таймер: {num} сек.</size>");
	}

	protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
	{
		if (!((CustomGrenade)this).Check(ev.Projectile) || _processedProjectiles.Contains(((Pickup)ev.Projectile).Serial))
		{
			return;
		}
		_processedProjectiles.Add(((Pickup)ev.Projectile).Serial);
		Pickup val = Pickup.Get((ItemPickupBase)(object)ev.Projectile.Base);
		RaycastHit val2 = default(RaycastHit);
		Vector3 val3;
		if (Physics.Raycast(ev.Player.Position, ev.Player.CameraTransform.forward, ref val2, 100f))
		{
			val3 = ((RaycastHit)(ref val2)).point;
			((Pickup)ev.Projectile).GameObject.transform.position = val3;
			Rigidbody val4 = default(Rigidbody);
			if (((Pickup)ev.Projectile).GameObject.TryGetComponent<Rigidbody>(ref val4))
			{
				val4.isKinematic = true;
				val4.useGravity = false;
				val4.velocity = Vector3.zero;
				val4.angularVelocity = Vector3.zero;
			}
			if (IsSticky)
			{
				((Pickup)ev.Projectile).GameObject.AddComponent<CollisionHandler>().Init(ev.Player, ev.Projectile, this);
			}
		}
		else
		{
			val3 = ((Component)ev.Projectile.Base).transform.position;
		}
		Room currentRoom = ev.Player.CurrentRoom;
		if ((Object)(object)currentRoom == (Object)null || (int)currentRoom.Zone != 8)
		{
			ev.Player.ShowMeowHint(7f, "<size=29><b><color=#61616193>|</color></size> <size=19>Авиаудар можно вызвать только снаружи комплекса</size> <size=29><b><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2, 27, priority: true);
			val.Destroy();
		}
		else
		{
			float delayTime = (_playerTimerSettings.ContainsKey(ev.Player) ? _playerTimerSettings[ev.Player] : _defaultTimer);
			Timing.RunCoroutine(InitiateAirstrike(val3, val, ev.Player, delayTime));
		}
	}

	[IteratorStateMachine(typeof(InitiateAirstrike_d_61))]
	private IEnumerator<float> InitiateAirstrike(Vector3 grenadePos, Pickup pickup, Player thrower, float delayTime)
	{
		return new InitiateAirstrike_d_61(0)
		{
			__4__this = this,
			grenadePos = grenadePos,
			pickup = pickup,
			thrower = thrower,
			delayTime = delayTime
		};
	}

	private void ShowAirstrikeProgress(Player player, float progress)
	{
		int num = 18;
		int num2 = Mathf.RoundToInt(progress * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt(progress * 100f);
		string arg = "<color=#ff0000>" + new string('▒', num2) + "</color><color=#730b0b>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>ПОДГОТОВКА АВИАУДАРА</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg}</size> <size=29><b><color=#61616193>|</color></b></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{num3}%</size> <size=29><b><color=#61616193>|</color></b></size>";
		player.ShowMeowHint(0.2f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}

	private void OnDestroying(DestroyingEventArgs ev)
	{
		foreach (Pickup item in Pickup.List.Where((Pickup p) => ((CustomItem)this).Check(p) && p.PreviousOwner == ev.Player))
		{
			item.Destroy();
		}
		_playerTimerSettings.Remove(ev.Player);
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		foreach (Pickup item in Pickup.List.Where((Func<Pickup, bool>)((CustomItem)this).Check))
		{
			item.Destroy();
		}
		_processedProjectiles.Clear();
		_playerTimerSettings.Clear();
	}
}
