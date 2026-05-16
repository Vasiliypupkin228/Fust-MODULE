using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using AdminToys;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using Mirror;
using UnityEngine;

namespace FultEngine.CustomItems.SubFire;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class SubFire : CustomWeapon
{
	[CompilerGenerated]
	private sealed class AnimateEffect_d_49 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Vector3 start;

		public Vector3 end;

		public SubFire __4__this;

		private List<PrimitiveObjectToy> layers;

		private PrimitiveObjectToy glow;

		private float elapsed;

		private float t;

		private Vector3 currentEnd;

		private Vector3 direction;

		private float distance;

		private int i;

		private PrimitiveObjectToy layer;

		private float width;

		private float pulse;

		private List<PrimitiveObjectToy>.Enumerator __s__12;

		private PrimitiveObjectToy layer;

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
		public AnimateEffect_d_49(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			layers = null;
			glow = null;
			layer = null;
			__s__12 = default(List<PrimitiveObjectToy>.Enumerator);
			layer = null;
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
				layers = new List<PrimitiveObjectToy>
				{
					__4__this.CreatePrimitive((PrimitiveType)2, __4__this._цветЛуча),
					__4__this.CreatePrimitive((PrimitiveType)2, Color.white),
					__4__this.CreatePrimitive((PrimitiveType)2, __4__this._цветСвечения)
				};
				glow = __4__this.CreatePrimitive((PrimitiveType)0, __4__this._цветПопадания);
				if ((Object)(object)glow != (Object)null)
				{
					((Component)glow).transform.position = end;
					((Component)glow).transform.localScale = Vector3.zero;
				}
				elapsed = 0f;
				goto IL_0364;
			case 1:
				__1__state = -1;
				goto IL_0364;
			case 2:
				__1__state = -1;
				__2__current = Timing.WaitUntilDone(Timing.RunCoroutine(__4__this.CreateImpactExplosion(end)));
				__1__state = 3;
				return true;
			case 3:
				{
					__1__state = -1;
					PrimitiveObjectToy obj = glow;
					DestroyNetworked((obj != null) ? ((Component)obj).gameObject : null);
					return false;
				}
				IL_0364:
				if (elapsed < 0.15f)
				{
					elapsed += 0.02f;
					t = Mathf.Clamp01(elapsed / 0.15f);
					currentEnd = Vector3.Lerp(start, end, t);
					Vector3 val = currentEnd - start;
					direction = ((Vector3)(ref val)).normalized;
					distance = Vector3.Distance(start, currentEnd);
					i = 0;
					while (i < layers.Count)
					{
						layer = layers[i];
						if (!((Object)(object)layer == (Object)null))
						{
							width = Mathf.Max(0.02f, 0.08f - (float)i * 0.025f);
							((Component)layer).transform.position = start + direction * (distance / 2f);
							((Component)layer).transform.localScale = new Vector3(width, width, distance);
							((Component)layer).transform.rotation = Quaternion.LookRotation(direction);
							if (i == 1)
							{
								pulse = Mathf.Sin(t * (float)Math.PI * 10f) * 0.5f + 0.5f;
								layer.NetworkMaterialColor = Color.Lerp(Color.white, Color.red, pulse);
							}
							layer = null;
						}
						i++;
					}
					if ((Object)(object)glow != (Object)null)
					{
						((Component)glow).transform.position = currentEnd;
						((Component)glow).transform.localScale = Vector3.one * (Mathf.Clamp01(t * 3f) * 0.5f);
					}
					__2__current = Timing.WaitForSeconds(0.02f);
					__1__state = 1;
					return true;
				}
				__s__12 = layers.GetEnumerator();
				try
				{
					while (__s__12.MoveNext())
					{
						layer = __s__12.Current;
						PrimitiveObjectToy obj2 = layer;
						DestroyNetworked((obj2 != null) ? ((Component)obj2).gameObject : null);
						layer = null;
					}
				}
				finally
				{
					((IDisposable)__s__12).Dispose();
				}
				__s__12 = default(List<PrimitiveObjectToy>.Enumerator);
				__2__current = Timing.WaitForSeconds(0.05f);
				__1__state = 2;
				return true;
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
	private sealed class CreateImpactExplosion_d_50 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Vector3 position;

		public SubFire __4__this;

		private PrimitiveObjectToy mainSphere;

		private List<PrimitiveObjectToy> particles;

		private List<Vector3> directions;

		private float time;

		private int i;

		private PrimitiveObjectToy particle;

		private float progress;

		private float ease;

		private Color fade;

		private int i;

		private float speed;

		private Color c;

		private List<PrimitiveObjectToy>.Enumerator __s__13;

		private PrimitiveObjectToy particle;

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
		public CreateImpactExplosion_d_50(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			mainSphere = null;
			particles = null;
			directions = null;
			particle = null;
			__s__13 = default(List<PrimitiveObjectToy>.Enumerator);
			particle = null;
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
				mainSphere = __4__this.CreatePrimitive((PrimitiveType)0, __4__this._цветПопадания);
				if ((Object)(object)mainSphere != (Object)null)
				{
					((Component)mainSphere).transform.position = position;
					((Component)mainSphere).transform.localScale = Vector3.zero;
				}
				particles = new List<PrimitiveObjectToy>();
				directions = new List<Vector3>();
				i = 0;
				while (i < 12)
				{
					particle = __4__this.CreatePrimitive((PrimitiveType)0, __4__this._цветСвечения);
					if (!((Object)(object)particle == (Object)null))
					{
						((Component)particle).transform.position = position;
						((Component)particle).transform.localScale = Vector3.one * 0.1f;
						particles.Add(particle);
						directions.Add(Random.onUnitSphere);
						particle = null;
					}
					i++;
				}
				time = 0f;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (time < 0.8f)
			{
				time += 0.03f;
				progress = Mathf.Clamp01(time / 0.8f);
				ease = 1f - Mathf.Pow(1f - progress, 3f);
				if ((Object)(object)mainSphere != (Object)null)
				{
					((Component)mainSphere).transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.2f, ease);
					fade = __4__this._цветПопадания;
					fade.a = Mathf.Lerp(1f, 0f, progress * 1.5f);
					mainSphere.NetworkMaterialColor = fade;
				}
				i = 0;
				while (i < particles.Count)
				{
					if (!((Object)(object)particles[i] == (Object)null))
					{
						speed = 1.5f * (1f - progress * 0.7f);
						Transform transform = ((Component)particles[i]).transform;
						transform.position += directions[i] * speed * 0.03f;
						((Component)particles[i]).transform.localScale = Vector3.one * Mathf.Lerp(0.1f, 0f, progress);
						c = __4__this._цветСвечения;
						c.a = Mathf.Lerp(0.9f, 0f, progress);
						particles[i].NetworkMaterialColor = c;
					}
					i++;
				}
				__2__current = Timing.WaitForSeconds(0.03f);
				__1__state = 1;
				return true;
			}
			PrimitiveObjectToy obj = mainSphere;
			DestroyNetworked((obj != null) ? ((Component)obj).gameObject : null);
			__s__13 = particles.GetEnumerator();
			try
			{
				while (__s__13.MoveNext())
				{
					particle = __s__13.Current;
					PrimitiveObjectToy obj2 = particle;
					DestroyNetworked((obj2 != null) ? ((Component)obj2).gameObject : null);
					particle = null;
				}
			}
			finally
			{
				((IDisposable)__s__13).Dispose();
			}
			__s__13 = default(List<PrimitiveObjectToy>.Enumerator);
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

	private readonly SubClassSelector _selector = new SubClassSelector();

	private readonly Color _цветЛуча = new Color(1f, 0.15f, 0.15f, 0.55f);

	private readonly Color _цветПопадания = new Color(1f, 0.15f, 0.15f, 0.35f);

	private readonly Color _цветСвечения = new Color(1f, 0.6f, 0.6f, 0.25f);

	public override uint Id { get; set; } = 301u;


	public override string Name { get; set; } = "<b><color=#28a82ddf>Пистолет выдачи подклассов</color></b>";


	public override string Description { get; set; } = "F-категория: фонарик — смена категории, noclip — смена роли, выстрел — выдать подкласс";


	public override ItemType Type { get; set; } = (ItemType)13;


	public override float Weight { get; set; } = 1f;


	public override float Damage { get; set; } = 0f;


	public override byte ClipSize { get; set; } = 228;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; } = (AttachmentName[])(object)new AttachmentName[1] { (AttachmentName)15 };


	protected override void SubscribeEvents()
	{
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.PickingUpItem += (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.TogglingWeaponFlashlight += (CustomEventHandler<TogglingWeaponFlashlightEventArgs>)OnTogglingFlashlight;
		Player.TogglingNoClip += (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		((CustomWeapon)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.PickingUpItem -= (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.TogglingWeaponFlashlight -= (CustomEventHandler<TogglingWeaponFlashlightEventArgs>)OnTogglingFlashlight;
		Player.TogglingNoClip -= (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		((CustomWeapon)this).UnsubscribeEvents();
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Пистолет выдачи подклассов\n<size=19>Фонарик — категория\nNoClip — роль\nВыстрел по игроку — выдать подкласс</size>");
			_selector.ShowInfo(ev.Player);
		}
	}

	private void OnPickingUpItem(PickingUpItemEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ev.Pickup != null && ((CustomItem)this).Check(ev.Pickup))
		{
			CIMessage.SendMessage(ev.Player, "Пистолет выдачи подклассов\n<size=19>Фонарик — категория\nNoClip — роль\nВыстрел по игроку — выдать подкласс</size>");
			_selector.ShowInfo(ev.Player);
		}
	}

	private void OnPlayerLeft(LeftEventArgs ev)
	{
		if (!(((ev != null) ? ((JoinedEventArgs)ev).Player : null) == (Player)null))
		{
			_selector.ClearPlayer(((JoinedEventArgs)ev).Player);
		}
	}

	private void OnTogglingFlashlight(TogglingWeaponFlashlightEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ev.Player.CurrentItem != null && ((CustomItem)this).Check(ev.Player.CurrentItem))
		{
			ev.IsAllowed = false;
			_selector.CycleCategory(ev.Player);
		}
	}

	private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ev.Player.CurrentItem != null && ((CustomItem)this).Check(ev.Player.CurrentItem))
		{
			ev.IsAllowed = false;
			_selector.CycleRole(ev.Player);
		}
	}

	private void OnShot(ShotEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ev.Firearm != null && ev.Player.CurrentItem != null && ((CustomItem)this).Check(ev.Player.CurrentItem))
		{
			ev.CanHurt = false;
			ev.Firearm.MagazineAmmo = ev.Firearm.MaxMagazineAmmo;
			Vector3 start = ev.Player.CameraTransform.position + ev.Player.CameraTransform.forward * 0.5f;
			Vector3 end = ev.Player.CameraTransform.position + ev.Player.CameraTransform.forward * 20f;
			if (ev.Target != (Player)null)
			{
				end = ev.Target.Position + Vector3.up;
			}
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, ref val, 50f))
			{
				end = ((RaycastHit)(ref val)).point;
			}
			Timing.RunCoroutine(AnimateEffect(start, end));
			if (ev.Target != (Player)null && ev.Target.IsAlive)
			{
				_selector.TryAssign(ev.Player, ev.Target);
			}
		}
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		try
		{
			if (ev != null && !(ev.Attacker == (Player)null) && !(ev.Player == (Player)null) && !(ev.Attacker == ev.Player) && ev.Attacker.CurrentItem != null && ((CustomItem)this).Check(ev.Attacker.CurrentItem))
			{
				ev.IsAllowed = false;
				ev.Amount = 0f;
			}
		}
		catch (Exception arg)
		{
			Log.Warn($"[SubFire] Ошибка в OnHurting: {arg}");
		}
	}

	[IteratorStateMachine(typeof(AnimateEffect_d_49))]
	private IEnumerator<float> AnimateEffect(Vector3 start, Vector3 end)
	{
		return new AnimateEffect_d_49(0)
		{
			__4__this = this,
			start = start,
			end = end
		};
	}

	[IteratorStateMachine(typeof(CreateImpactExplosion_d_50))]
	private IEnumerator<float> CreateImpactExplosion(Vector3 position)
	{
		return new CreateImpactExplosion_d_50(0)
		{
			__4__this = this,
			position = position
		};
	}

	private PrimitiveObjectToy CreatePrimitive(PrimitiveType type, Color color)
	{
		PrimitiveObjectToy val = NetworkClient.prefabs.Values.Select((GameObject p) => p.GetComponent<PrimitiveObjectToy>()).FirstOrDefault((Func<PrimitiveObjectToy, bool>)((PrimitiveObjectToy p) => (Object)(object)p != (Object)null));
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		PrimitiveObjectToy val2 = Object.Instantiate<PrimitiveObjectToy>(val);
		val2.NetworkPrimitiveType = type;
		val2.NetworkMaterialColor = color;
		val2.PrimitiveFlags = (PrimitiveFlags)2;
		NetworkServer.Spawn(((Component)val2).gameObject, (NetworkConnection)null);
		return val2;
	}

	private static void DestroyNetworked(GameObject obj)
	{
		if ((Object)(object)obj != (Object)null && NetworkServer.active)
		{
			NetworkServer.Destroy(obj);
		}
	}
}
