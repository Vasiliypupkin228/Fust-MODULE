using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CustomEffectsAndItems;
using CustomEffectsAndItems.Utilities;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.CustomItems;
using MEC;
using UnityEngine;

namespace AnacondaFullRP.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Railgun : CustomWeapon
{
	private class RailgunState
	{
		public bool[] AmmoStates { get; }

		public float[] AmmoCooldowns { get; }

		public RailgunState()
		{
			int num = COOLDOWN_DURATIONS.Length;
			AmmoStates = new bool[num];
			for (int i = 0; i < num; i++)
			{
				AmmoStates[i] = true;
			}
			AmmoCooldowns = new float[num];
		}
	}

	[CompilerGenerated]
	private sealed class DisplayHintRoutine_d_37 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Railgun __4__this;

		private IEnumerator<Player> __s__1;

		private Player holder;

		private string weaponId;

		private RailgunState state;

		private string ammoDisplay;

		private Exception ex;

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
		public DisplayHintRoutine_d_37(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__1 = null;
			holder = null;
			weaponId = null;
			state = null;
			ammoDisplay = null;
			ex = null;
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
			try
			{
				__s__1 = Player.List.GetEnumerator();
				try
				{
					while (__s__1.MoveNext())
					{
						holder = __s__1.Current;
						if (!(holder == (Player)null) && holder.IsAlive && holder.CurrentItem != null && ((CustomItem)__4__this).Check(holder.CurrentItem))
						{
							weaponId = __4__this.GetWeaponId(holder.CurrentItem);
							if (!__4__this._railgunStates.TryGetValue(weaponId, out state))
							{
								state = new RailgunState();
								__4__this._railgunStates[weaponId] = state;
							}
							ammoDisplay = __4__this.BuildAmmoDisplay(state.AmmoStates, state.AmmoCooldowns);
							holder.ShowHint(ammoDisplay, 0.3f);
							weaponId = null;
							state = null;
							ammoDisplay = null;
							holder = null;
						}
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
			}
			catch (Exception ex)
			{
				ex = ex;
				Log.Error($"Ошибка в DisplayHintRoutine: {ex}");
			}
			__2__current = Timing.WaitForSeconds(0.2f);
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

	[CompilerGenerated]
	private sealed class RestoreAmmo_d_43 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public string weaponId;

		public int ammoIndex;

		public Player player;

		public float duration;

		public Railgun __4__this;

		private RailgunState state;

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
		public RestoreAmmo_d_43(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			state = null;
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
				__2__current = Timing.WaitForSeconds(duration);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				if (__4__this._railgunStates.TryGetValue(weaponId, out state))
				{
					state.AmmoStates[ammoIndex] = true;
				}
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

	private const float PERPETUAL_DAMAGE_MULTIPLIER = 0.5f;

	private const float HintUpdateInterval = 0.2f;

	private static readonly float[] COOLDOWN_DURATIONS = new float[4] { 4f, 6f, 8f, 10f };

	private static readonly Color EffectColorSuc = new Color(0f, 1f, 1f, 1f);

	private static readonly Color EffectColorFailed = new Color(1f, 0f, 0f, 1f);

	private readonly Dictionary<string, RailgunState> _railgunStates = new Dictionary<string, RailgunState>();

	private CoroutineHandle _globalHintCoroutine = default(CoroutineHandle);

	public override uint Id { get; set; } = 20u;


	public override string Name { get; set; } = "<color=#139bbaf2><b>QPHC</b></color>";


	public override string Description { get; set; } = "Устройство Лазерного Поражения на Основе Квантово-Фотонных Гипер Кристаллов (5 зарядов: 1 перпетуальный, 4 с кулдауном)";


	public override float Weight { get; set; } = 5f;


	public override SpawnProperties SpawnProperties { get; set; }

	public override float Damage { get; set; } = 0f;


	public override byte ClipSize { get; set; } = 155;


	protected override void SubscribeEvents()
	{
		Player.Shooting += (CustomEventHandler<ShootingEventArgs>)OnShooting;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.PickingUpItem += (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		_globalHintCoroutine = Timing.RunCoroutine(DisplayHintRoutine());
		((CustomWeapon)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.Shooting -= (CustomEventHandler<ShootingEventArgs>)OnShooting;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.PickingUpItem -= (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		if (((CoroutineHandle)(ref _globalHintCoroutine)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _globalHintCoroutine });
		}
		((CustomWeapon)this).UnsubscribeEvents();
	}

	[IteratorStateMachine(typeof(DisplayHintRoutine_d_37))]
	private IEnumerator<float> DisplayHintRoutine()
	{
		return new DisplayHintRoutine_d_37(0)
		{
			__4__this = this
		};
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
		}
	}

	private void OnShooting(ShootingEventArgs ev)
	{
		if (!((CustomItem)this).Check(ev.Player.CurrentItem))
		{
			return;
		}
		Item currentItem = ev.Player.CurrentItem;
		Firearm val = (Firearm)(object)((currentItem is Firearm) ? currentItem : null);
		if (val == null)
		{
			return;
		}
		string weaponId = GetWeaponId(ev.Player.CurrentItem);
		if (!_railgunStates.TryGetValue(weaponId, out var value))
		{
			value = new RailgunState();
			_railgunStates[weaponId] = value;
		}
		if (val.MagazineAmmo < 3)
		{
			val.MagazineAmmo = 155;
			ev.IsAllowed = false;
			CustomEffectsAndItemsManager.FireShot((Item)(object)val, PlayerExtensions.GetMuzzlePosition(ev.Player, 0f, true), ev.Player.CameraTransform.forward, EffectColorFailed);
			return;
		}
		int num = -1;
		for (int i = 0; i < value.AmmoStates.Length; i++)
		{
			if (value.AmmoStates[i])
			{
				num = i;
				break;
			}
		}
		bool flag = false;
		float num2 = 50f;
		if (num == -1)
		{
			flag = true;
			num2 *= 0.5f;
		}
		try
		{
			Vector3 forward = ev.Player.CameraTransform.forward;
			Vector3 muzzlePosition = PlayerExtensions.GetMuzzlePosition(ev.Player, 0f, true);
			int magazineAmmo = val.MagazineAmmo;
			val.MagazineAmmo = magazineAmmo - 1;
			if (!flag)
			{
				float num3 = COOLDOWN_DURATIONS[num];
				value.AmmoStates[num] = false;
				value.AmmoCooldowns[num] = Time.time + num3;
				Timing.RunCoroutine(RestoreAmmo(weaponId, num, ev.Player, num3));
			}
			CustomEffectsAndItemsManager.FireShot((Item)(object)val, muzzlePosition, forward, EffectColorSuc);
			RaycastHit[] source = Physics.RaycastAll(muzzlePosition, forward, 50f, LayerMask.GetMask(new string[2] { "Player", "Ignore Raycast" }));
			foreach (RaycastHit item in source.OrderBy((RaycastHit h) => Vector3.Distance(muzzlePosition, ((RaycastHit)(ref h)).point)))
			{
				RaycastHit current = item;
				Player val2 = Player.Get(((RaycastHit)(ref current)).collider);
				if (val2 != (Player)null && val2.IsAlive && val2 != ev.Player)
				{
					val2.Hurt(num2, (DamageType)0, "");
					break;
				}
			}
			ev.Player.ShowHint(BuildAmmoDisplay(value.AmmoStates, value.AmmoCooldowns, fullCooldown: false, flag), 3f);
		}
		catch (Exception arg)
		{
			Log.Error($"Ошибка при стрельбе из QPHC: {arg}");
		}
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		if (!((CustomItem)this).Check(ev.Item))
		{
		}
	}

	private void OnPickingUpItem(PickingUpItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Pickup))
		{
			CIMessage.SendMessage(ev.Player, "QPHC\n<size=19>Устройство Лазерного Поражения на \nОснове Квантово-Фотонных \nГипер Кристаллов</size>");
			string weaponId = GetWeaponId(ev.Pickup);
			if (!_railgunStates.ContainsKey(weaponId))
			{
				_railgunStates[weaponId] = new RailgunState();
			}
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (ev.Player != (Player)null && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "QPHC\n<size=19>Устройство Лазерного Поражения на \nОснове Квантово-Фотонных \nГипер Кристаллов</size>");
			string weaponId = GetWeaponId(ev.Item);
			if (!_railgunStates.TryGetValue(weaponId, out var value))
			{
				value = new RailgunState();
				_railgunStates[weaponId] = value;
			}
		}
	}

	[IteratorStateMachine(typeof(RestoreAmmo_d_43))]
	private IEnumerator<float> RestoreAmmo(string weaponId, int ammoIndex, Player player, float duration)
	{
		return new RestoreAmmo_d_43(0)
		{
			__4__this = this,
			weaponId = weaponId,
			ammoIndex = ammoIndex,
			player = player,
			duration = duration
		};
	}

	private string BuildAmmoDisplay(bool[] ammoStates, float[] ammoCooldowns, bool fullCooldown = false, bool usedPerpetual = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = ammoStates.Count((bool s) => s);
		bool flag = num == COOLDOWN_DURATIONS.Length;
		float overheatPercentage = Mathf.RoundToInt((float)(COOLDOWN_DURATIONS.Length - num) * 25f);
		stringBuilder.AppendLine("\n\n\n\n\n\n\n\n<size=19><b><color=#404040c5>╔</color> <color=#008bc7c5>QHPC</color> <color=#404040c5>╗</color></b></size>");
		string coreTemperatureStatus = GetCoreTemperatureStatus(overheatPercentage);
		string coreTempColor = GetCoreTempColor(overheatPercentage);
		stringBuilder.AppendLine("<size=19><b><color=#404040c5>╠</color> <color=#008bc7c5>ТЕМП. ЯДРА: </color><color=" + coreTempColor + ">" + coreTemperatureStatus + "</color> <color=#404040c5>╣</color></b></size>");
		stringBuilder.Append("<size=23><b><color=#404040c5>╠</color> ");
		stringBuilder.Append(usedPerpetual ? "<color=#ff8800>★</color>" : "<color=#00fffffd>★</color>");
		for (int i = 0; i < COOLDOWN_DURATIONS.Length; i++)
		{
			float num2 = COOLDOWN_DURATIONS[i];
			stringBuilder.Append(" ");
			if (ammoStates[i])
			{
				stringBuilder.Append("<color=#00fffffd>■</color>");
				continue;
			}
			float num3 = ammoCooldowns[i] - Time.time;
			num3 = Mathf.Max(0f, num3);
			float num4 = Mathf.Clamp01(1f - num3 / num2);
			int num5 = Mathf.RoundToInt(num4 * 100f);
			stringBuilder.Append("<color=#0d91917a>▨</color>");
			if (num5 < 100)
			{
				stringBuilder.Append($"<pos=22%><size=18><color=#00ffd0>{num5}%</color></size>");
			}
		}
		stringBuilder.AppendLine(" <color=#404040c5>╣</color></b></size>");
		string text = (flag ? "<color=#00ff80>МАКСИМУМ</color>" : "<color=#ff8800>ПОДЗАРЯДКА</color>");
		stringBuilder.AppendLine("<size=19><b><color=#404040c5>╠</color> <color=#008bc7c5>СТАТУС ЯДРА: " + text + "</color> <color=#404040c5>╣</color></b></size>");
		return stringBuilder.ToString();
	}

	private string GetOverheatColor(float percentage)
	{
		if (percentage < 25f)
		{
			return "#00ff80";
		}
		if (percentage < 50f)
		{
			return "#ffff00";
		}
		if (percentage < 75f)
		{
			return "#ff8800";
		}
		return "#ff5555";
	}

	private string GetCoreTempColor(float overheatPercentage)
	{
		if (overheatPercentage < 25f)
		{
			return "#00ff80";
		}
		if (overheatPercentage < 50f)
		{
			return "#ffff00";
		}
		if (overheatPercentage < 75f)
		{
			return "#ff8800";
		}
		return "#ff5555";
	}

	private string GetCoreTemperatureStatus(float overheatPercentage)
	{
		if (overheatPercentage < 25f)
		{
			return "НОРМАЛЬНАЯ";
		}
		if (overheatPercentage < 50f)
		{
			return "ПОВЫШЕННАЯ";
		}
		if (overheatPercentage < 75f)
		{
			return "ВЫСОКАЯ";
		}
		return "КРИТИЧЕСКАЯ";
	}

	private string GetWeaponId(Item item)
	{
		return ((item != null) ? item.Serial.ToString() : null) ?? "unknown";
	}

	private string GetWeaponId(Pickup pickup)
	{
		return ((pickup != null) ? pickup.Serial.ToString() : null) ?? "unknown";
	}
}
