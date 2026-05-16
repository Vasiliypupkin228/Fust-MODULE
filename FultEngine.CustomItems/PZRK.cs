using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class PZRK : CustomWeapon
{
	[CompilerGenerated]
	private sealed class __c__DisplayClass43_0
	{
		public Player shooter;

		public Projectile projectile;

		internal bool TargetingCoroutine_b_0(Player p)
		{
			return FPV.flyingPlayers.Contains(p) && p.IsAlive && (int)p.Role.Type != 2 && p != shooter && Vector3.Distance(((Pickup)projectile).Position, p.Position) <= 55f;
		}

		internal float TargetingCoroutine_b_1(Player p)
		{
			return Vector3.Distance(((Pickup)projectile).Position, p.Position);
		}
	}

	[CompilerGenerated]
	private sealed class TargetingCoroutine_d_43 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Projectile projectile;

		public Player shooter;

		public PZRK __4__this;

		private __c__DisplayClass43_0 __8__1;

		private float currentSpeed;

		private Player target;

		private Rigidbody rb;

		private bool isTargetingEnabled;

		private Vector3 desiredDirection;

		private Vector3 newDirection;

		private Quaternion targetRotation;

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
		public TargetingCoroutine_d_43(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__8__1 = null;
			target = null;
			rb = null;
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
				__8__1 = new __c__DisplayClass43_0();
				__8__1.shooter = shooter;
				__8__1.projectile = projectile;
				currentSpeed = 5f;
				target = null;
				rb = ((Pickup)__8__1.projectile).GameObject.GetComponent<Rigidbody>();
				isTargetingEnabled = Random.value >= 0.25f;
				__2__current = Timing.WaitForSeconds(0.5f);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				break;
			case 2:
				__1__state = -1;
				break;
			}
			if (__8__1.projectile != null && (Object)(object)((Pickup)__8__1.projectile).GameObject != (Object)null && (Object)(object)rb != (Object)null)
			{
				if (currentSpeed < 35f)
				{
					currentSpeed += 1.5f * Time.deltaTime;
					currentSpeed = Mathf.Min(currentSpeed, 35f);
				}
				Vector3 val;
				if (isTargetingEnabled)
				{
					if (target == (Player)null)
					{
						target = (from p in Player.List
							where FPV.flyingPlayers.Contains(p) && p.IsAlive && (int)p.Role.Type != 2 && p != __8__1.shooter && Vector3.Distance(((Pickup)__8__1.projectile).Position, p.Position) <= 55f
							orderby Vector3.Distance(((Pickup)__8__1.projectile).Position, p.Position)
							select p).FirstOrDefault();
					}
					if (target != (Player)null)
					{
						if (!target.IsAlive)
						{
							target = null;
						}
						else
						{
							val = target.Position - ((Pickup)__8__1.projectile).Position;
							desiredDirection = ((Vector3)(ref val)).normalized;
							val = rb.velocity;
							newDirection = Vector3.Lerp(((Vector3)(ref val)).normalized, desiredDirection, 0.02f);
							rb.velocity = ((Vector3)(ref newDirection)).normalized * currentSpeed;
							targetRotation = Quaternion.LookRotation(rb.velocity);
							((Pickup)__8__1.projectile).GameObject.transform.rotation = Quaternion.Slerp(((Pickup)__8__1.projectile).GameObject.transform.rotation, targetRotation, 0.02f);
						}
					}
					else
					{
						Rigidbody obj = rb;
						val = rb.velocity;
						obj.velocity = ((Vector3)(ref val)).normalized * currentSpeed;
						((Pickup)__8__1.projectile).GameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
					}
				}
				else
				{
					Rigidbody obj2 = rb;
					val = rb.velocity;
					obj2.velocity = ((Vector3)(ref val)).normalized * currentSpeed;
					((Pickup)__8__1.projectile).GameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
				}
				__2__current = float.NegativeInfinity;
				__1__state = 2;
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

	private const float TargetAcquisitionDelay = 0.5f;

	private const float AcquisitionRange = 55f;

	private const float InitialMissileSpeed = 5f;

	private const float MaxMissileSpeed = 35f;

	private const float MissileAcceleration = 1.5f;

	private const float TargetingCorrectionRate = 0.02f;

	private const float TargetingFailureChance = 0.25f;

	public override uint Id { get; set; } = 18u;


	public override string Name { get; set; } = "<color=#ff006b><b>ПЗРК</b></color>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 70f;


	public override float Damage { get; set; } = 0f;


	public override byte ClipSize { get; set; } = 1;


	public override SpawnProperties SpawnProperties { get; set; }

	protected override void SubscribeEvents()
	{
		Player.Shooting += (CustomEventHandler<ShootingEventArgs>)OnShooting;
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		((CustomWeapon)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.Shooting -= (CustomEventHandler<ShootingEventArgs>)OnShooting;
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		((CustomWeapon)this).UnsubscribeEvents();
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Pickup))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnShooting(ShootingEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item) && ev.Player.CurrentItem is Firearm)
		{
			HandleRPGEffects(ev);
			ev.Item.Destroy();
			ev.IsAllowed = false;
		}
	}

	private void HandleRPGEffects(ShootingEventArgs ev)
	{
		Vector3 val = ev.Player.CameraTransform.TransformPoint(Vector3.zero);
		Projectile projectile = ev.Player.ThrowGrenade((ProjectileType)1, true).Projectile;
		Rigidbody val2 = default(Rigidbody);
		if (((Pickup)projectile).GameObject.TryGetComponent<Rigidbody>(ref val2))
		{
			val2.useGravity = false;
			val2.velocity = ev.Player.CameraTransform.forward * 5f;
		}
		AudioManager.DestroyForPlayer(ev.Player);
		AudioPlayerFactory.PlayAudioAtPosition(val, "ShotRPG", 2.5f, 555f, 2.1f);
		AudioManager.CreateForGameObject(((Pickup)projectile).GameObject, "Svist", 3f, 60f, 1.15f);
		((Pickup)projectile).GameObject.transform.SetPositionAndRotation(val, ev.Player.CameraTransform.rotation);
		((Pickup)projectile).GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, projectile.Base);
		CreateRPGVisualEffects(projectile);
		Timing.RunCoroutine(TargetingCoroutine(projectile, ev.Player));
	}

	private void CreateRPGVisualEffects(Projectile projectile)
	{
		SchematicObject val = ObjectSpawner.SpawnSchematic("Rocket", ((Pickup)projectile).Position, (Quaternion?)Quaternion.Euler(0f, 0f, 0f), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		((Component)val).transform.parent = ((Pickup)projectile).Transform;
	}

	private void ShowItemHint(Player player)
	{
		CIMessage.SendMessage(player, "ПЗРК \n<size=19>Переносной комплекс для\r\nточного поражения воздушных целей</size>");
	}

	[IteratorStateMachine(typeof(TargetingCoroutine_d_43))]
	private IEnumerator<float> TargetingCoroutine(Projectile projectile, Player shooter)
	{
		return new TargetingCoroutine_d_43(0)
		{
			__4__this = this,
			projectile = projectile,
			shooter = shooter
		};
	}
}
