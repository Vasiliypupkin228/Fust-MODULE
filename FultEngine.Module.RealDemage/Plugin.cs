using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.CustomItems.API.Features;
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

namespace FultEngine.Module.RealDemage;

public class Plugin : IFultEngineModule
{
	private class MedicalKitInventory
	{
		public int Tourniquets { get; set; } = 2;


		public int HemostaticBandages { get; set; } = 1;


		public int CompressionBandages { get; set; } = 1;


		public int OcclusivePatches { get; set; } = 1;


		public bool Forceps { get; set; } = true;

	}

	private enum BoneType
	{
		Head,
		Body,
		LeftArm,
		RightArm,
		LeftLeg,
		RightLeg,
		Unknown
	}

	[CompilerGenerated]
	private sealed class __c__DisplayClass85_0
	{
		public Player player;

		public Func<KeyValuePair<Player, Player>, bool> __9__0;

		internal bool HintCoroutine_b_0(KeyValuePair<Player, Player> p)
		{
			return p.Value == player;
		}
	}

	[CompilerGenerated]
	private sealed class __c__DisplayClass86_0
	{
		public Player player;

		internal bool HealingCoroutine_b_1(Injury i)
		{
			return i.Type == "Перелом" && i.BodyPart != "Грудь" && !player.IsInjuredFromFall(i);
		}

		internal bool HealingCoroutine_b_5(Injury i)
		{
			return i.Type == "Перелом" && player.IsInjuredFromFall(i);
		}
	}

	[CompilerGenerated]
	private sealed class AdrenalineResetCoroutine_d_77 : IEnumerator<float>, IDisposable, IEnumerator
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
		public AdrenalineResetCoroutine_d_77(int __1__state)
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
				__2__current = Timing.WaitForSeconds(540f);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				__4__this.ResetAdrenalineUses(player);
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
	private sealed class DamageCoroutine_d_84 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Plugin __4__this;

		private List<Injury> injuries;

		private List<Injury> bleedingInjuries;

		private float totalBleedDps;

		private float otherDps;

		private float totalDamage;

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
		public DamageCoroutine_d_84(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			injuries = null;
			bleedingInjuries = null;
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
				if (!__4__this._playerInjuries.ContainsKey(player))
				{
					return false;
				}
				break;
			case 1:
				__1__state = -1;
				injuries = null;
				bleedingInjuries = null;
				break;
			}
			if (__4__this._playerInjuries.ContainsKey(player) && player.IsAlive)
			{
				injuries = __4__this._playerInjuries[player];
				__4__this.RemoveExpiredBleedingInjuries(player, injuries);
				if (!__4__this._playerInjuries.ContainsKey(player))
				{
					return false;
				}
				injuries = __4__this._playerInjuries[player];
				bleedingInjuries = injuries.Where((Injury i) => i.IsBleeding).ToList();
				totalBleedDps = bleedingInjuries.Sum((Injury i) => i.DamagePerSecond * i.BleedIntensity);
				totalBleedDps = Mathf.Min(totalBleedDps, 2f);
				otherDps = injuries.Where((Injury i) => !i.IsBleeding).Sum((Injury i) => i.DamagePerSecond);
				totalDamage = (totalBleedDps + otherDps) * 0.1f;
				if (totalDamage > 0f)
				{
					player.Health = Mathf.Max(1f, player.Health - totalDamage);
				}
				if (bleedingInjuries.Any())
				{
					player.EnableEffect((EffectType)4, (byte)3, 0.5f, false);
					player.EnableEffect((EffectType)13, (byte)1, 0.5f, false);
				}
				if (player.Health <= 1f)
				{
					player.Kill((DamageType)6, "");
					return false;
				}
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
	private sealed class HealingCoroutine_d_86 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Plugin __4__this;

		private __c__DisplayClass86_0 __8__1;

		private List<Injury> injuries;

		private byte slownessIntensity;

		private float healPerSec;

		private int i;

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
		public HealingCoroutine_d_86(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__8__1 = null;
			injuries = null;
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
				__8__1 = new __c__DisplayClass86_0();
				__8__1.player = player;
				if (!__4__this._playerInjuries.ContainsKey(__8__1.player))
				{
					return false;
				}
				injuries = __4__this._playerInjuries[__8__1.player];
				injuries.RemoveAll((Injury i) => i.Type == "Кровотечение");
				__8__1.player.DisableEffect((EffectType)13);
				slownessIntensity = (byte)(injuries.Any((Injury i) => i.Type == "Перелом" && i.BodyPart != "Грудь" && !__8__1.player.IsInjuredFromFall(i)) ? 63 : (injuries.Any((Injury i) => i.Type == "Перелом грудной клетки") ? 33 : (injuries.Any((Injury i) => i.Type == "Пневмоторакс") ? 40 : (injuries.Any((Injury i) => i.Type == "Растяжение") ? 20 : (injuries.Any((Injury i) => i.Type == "Перелом" && __8__1.player.IsInjuredFromFall(i)) ? 45 : 0)))));
				goto IL_022c;
			case 1:
				__1__state = -1;
				goto IL_022c;
			case 2:
				{
					__1__state = -1;
					i++;
					goto IL_04d2;
				}
				IL_04d2:
				if (i < 10)
				{
					if (!__8__1.player.IsAlive || __8__1.player.Health >= 75f)
					{
						return false;
					}
					__8__1.player.Health = Mathf.Min(__8__1.player.Health + healPerSec, 75f);
					__2__current = Timing.WaitForSeconds(1f);
					__1__state = 2;
					return true;
				}
				return false;
				IL_022c:
				if (slownessIntensity > 0)
				{
					slownessIntensity = (byte)Mathf.Max(0, slownessIntensity - 10);
					if (slownessIntensity > 0)
					{
						__8__1.player.EnableEffect((EffectType)43, slownessIntensity, 1f, false);
					}
					else
					{
						__8__1.player.DisableEffect((EffectType)43);
					}
					__2__current = Timing.WaitForSeconds(1f);
					__1__state = 1;
					return true;
				}
				injuries.RemoveAll((Injury i) => i.Type == "Кровотечение" || i.Type == "Осколочное ранение");
				injuries.RemoveAll((Injury i) => i.Type == "Сильное кровотечение");
				__8__1.player.DisableEffect((EffectType)4);
				__8__1.player.DisableEffect((EffectType)13);
				__8__1.player.DisableEffect((EffectType)26);
				if (injuries.Any((Injury i) => i.Type == "Контузия"))
				{
					__8__1.player.DisableEffect((EffectType)9);
					__8__1.player.DisableEffect((EffectType)5);
					__8__1.player.DisableEffect((EffectType)11);
				}
				if (injuries.Any((Injury i) => i.Type.Contains("Перелом") || i.Type.Contains("Глубокая рана")))
				{
					__8__1.player.DisableEffect((EffectType)26);
				}
				if (injuries.Any((Injury i) => i.Type.Contains("Ушиб") || i.Type.Contains("Пневмоторакс") || i.Type.Contains("Гематома")))
				{
					__8__1.player.DisableEffect((EffectType)13);
				}
				if (injuries.Any((Injury i) => i.Type.Contains("Сотрясение мозга")))
				{
					__8__1.player.DisableEffect((EffectType)5);
				}
				__4__this._playerInjuries.Remove(__8__1.player);
				if (!__8__1.player.IsAlive || !(__8__1.player.Health < 75f))
				{
					break;
				}
				healPerSec = 3.5f;
				i = 0;
				goto IL_04d2;
			}
			if (__4__this._hintCoroutines.ContainsKey(__8__1.player))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { __4__this._hintCoroutines[__8__1.player] });
				__4__this._hintCoroutines.Remove(__8__1.player);
				__8__1.player.ShowHint("", 0.1f);
			}
			__4__this._healingCoroutines.Remove(__8__1.player);
			injuries.RemoveAll((Injury i) => i.IsBleeding);
			__8__1.player.DisableEffect((EffectType)4);
			__8__1.player.DisableEffect((EffectType)13);
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
	private sealed class HintCoroutine_d_85 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Plugin __4__this;

		private __c__DisplayClass85_0 __8__1;

		private string hintText;

		private IEnumerator<Player> __s__3;

		private Player spectator;

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
		public HintCoroutine_d_85(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__8__1 = null;
			hintText = null;
			__s__3 = null;
			spectator = null;
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
				__8__1 = new __c__DisplayClass85_0();
				__8__1.player = player;
				if (!__4__this._playerInjuries.ContainsKey(__8__1.player))
				{
					return false;
				}
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (__4__this._playerInjuries.ContainsKey(__8__1.player) && __8__1.player.IsAlive)
			{
				if (__4__this._playerInjuries[__8__1.player].Count > 0)
				{
					hintText = __4__this.FormatInjuryHint(__4__this._playerInjuries[__8__1.player]);
					__8__1.player.ShowMeowHint(0.1f, hintText, (HintVerticalAlign)0, 301, -319, (HintAlignment)0);
					__s__3 = (from p in __4__this._spectatedPlayers
						where p.Value == __8__1.player
						select p.Key).GetEnumerator();
					try
					{
						while (__s__3.MoveNext())
						{
							spectator = __s__3.Current;
							spectator.ShowMeowHint(0.1f, hintText, (HintVerticalAlign)0, 301, -319, (HintAlignment)0);
							spectator = null;
						}
					}
					finally
					{
						if (__s__3 != null)
						{
							__s__3.Dispose();
						}
					}
					__s__3 = null;
					hintText = null;
				}
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
	private sealed class PostTreatmentRegenCoroutine_d_80 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Plugin __4__this;

		private float endTime;

		private float healPerTick;

		private List<Injury> injuries;

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
		public PostTreatmentRegenCoroutine_d_80(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			injuries = null;
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
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				endTime = Time.time + 18f;
				healPerTick = 1f;
				break;
			case 2:
				__1__state = -1;
				injuries = null;
				break;
			}
			if (player != (Player)null && player.IsAlive && Time.time < endTime && (!__4__this._playerInjuries.TryGetValue(player, out injuries) || !injuries.Any((Injury i) => i.IsBleeding)) && !(player.Health >= player.MaxHealth))
			{
				player.Health = Mathf.Min(player.MaxHealth, player.Health + healPerTick);
				__2__current = Timing.WaitForSeconds(0.5f);
				__1__state = 2;
				return true;
			}
			__4__this._postTreatmentRegenCoroutines.Remove(player);
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
	private sealed class ShowMedkitMenuCoroutine_d_48 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player p;

		public ushort s;

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
		public ShowMedkitMenuCoroutine_d_48(int __1__state)
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
			if (p.IsAlive && !p.IsScp)
			{
				Item currentItem = p.CurrentItem;
				if (currentItem != null && (int)currentItem.Type == 14 && p.CurrentItem.Serial == s && !__4__this._realHealCoroutines.ContainsKey(p))
				{
					__4__this.UpdateMedkitMenu(p, s);
					__2__current = Timing.WaitForSeconds(0.2f);
					__1__state = 1;
					return true;
				}
			}
			if (!__4__this._realHealCoroutines.ContainsKey(p))
			{
				__4__this.StopMedkitMenu(p);
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
	private sealed class TreatmentCoroutine_d_55 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player p;

		public Player t;

		public int type;

		public ushort s;

		public Plugin __4__this;

		private float start;

		private MedicalKitInventory medicalKit;

		private int __s__3;

		private float prog;

		private string name;

		private string role;

		private int bars;

		private int filled;

		private int empty;

		private string bar;

		private int pct;

		private string msg;

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
		public TreatmentCoroutine_d_55(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			medicalKit = null;
			name = null;
			role = null;
			bar = null;
			msg = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			switch (__1__state)
			{
			default:
				return false;
			case 0:
			{
				__1__state = -1;
				p.EnableEffect((EffectType)43, (byte)35, 999f, false);
				start = Time.time;
				if (!__4__this._medkitInventories.TryGetValue(s, out medicalKit))
				{
					__4__this.ShowMeowHint(p, 3f, "Ошибка: аптечка не найдена!");
					return false;
				}
				int num = type;
				__s__3 = num;
				switch (__s__3)
				{
				case 0:
					medicalKit.Tourniquets--;
					break;
				case 1:
					medicalKit.HemostaticBandages--;
					break;
				case 2:
					medicalKit.CompressionBandages--;
					break;
				case 3:
					medicalKit.OcclusivePatches--;
					break;
				}
				break;
			}
			case 1:
				__1__state = -1;
				name = null;
				role = null;
				bar = null;
				msg = null;
				break;
			}
			if (Time.time < start + 7f)
			{
				if (p.IsAlive && t.IsAlive)
				{
					Item currentItem = p.CurrentItem;
					if (currentItem != null && (int)currentItem.Type == 14 && !(Vector3.Distance(p.Position, t.Position) > 2f))
					{
						prog = (Time.time - start) / 7f;
						if (1 == 0)
						{
						}
						string text = type switch
						{
							0 => "Жгут-турникет", 
							1 => "Гемостатическая повязка", 
							2 => "Компрессионная повязка", 
							3 => "Окклюзионная наклейка", 
							4 => "Щипцы для осколков", 
							_ => "Лечение", 
						};
						if (1 == 0)
						{
						}
						name = text;
						role = ((t == p) ? "ЛЕЧЕНИЕ СЕБЯ" : ("ЛЕЧЕНИЕ " + t.DisplayNickname));
						bars = 18;
						filled = Mathf.RoundToInt(prog * (float)bars);
						empty = bars - filled;
						bar = "<color=#00ff00>" + new string('▒', filled) + "</color><color=#006600>" + new string('▒', empty) + "</color>";
						pct = Mathf.RoundToInt(prog * 100f);
						msg = $"<size=25><b><color=#61616193>『</color></size> <size=21>{role}</size> <size=25><b><color=#61616193>』</color></size>\n<size=19>{name}</size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{bar}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{pct}%</size> <size=29><b><color=#61616193>|</color></size></b>";
						p.ShowMeowHint(0.2f, msg, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
						if (t != p)
						{
							t.ShowMeowHint(0.2f, msg, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
						}
						__2__current = Timing.WaitForSeconds(0.1f);
						__1__state = 1;
						return true;
					}
				}
				__4__this.ShowMeowHint(p, 3f, "Лечение прервано!");
				if (t != p && t.IsAlive)
				{
					__4__this.ShowMeowHint(t, 3f, "Лечение прервано!");
				}
			}
			p.DisableEffect((EffectType)43);
			if (Time.time >= start + 7f && p.IsAlive && t.IsAlive)
			{
				__4__this.CompleteTreatment(p, t, type, medicalKit, s);
			}
			__4__this._realHealCoroutines.Remove(p);
			if (p.IsAlive && !p.IsScp)
			{
				Item currentItem2 = p.CurrentItem;
				if (currentItem2 != null && (int)currentItem2.Type == 14)
				{
					__4__this._menuCoroutines[p] = Timing.RunCoroutine(__4__this.ShowMedkitMenuCoroutine(p, s));
				}
			}
			if (p.IsAlive && !p.IsScp)
			{
				Item currentItem3 = p.CurrentItem;
				if (currentItem3 != null && (int)currentItem3.Type == 14 && p.CurrentItem.Serial == s)
				{
					if (!__4__this._medkitInventories.ContainsKey(s))
					{
						__4__this._medkitInventories[s] = new MedicalKitInventory();
					}
					__4__this._selectedOption[p] = 0;
					__4__this._menuCoroutines[p] = Timing.RunCoroutine(__4__this.ShowMedkitMenuCoroutine(p, s));
				}
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

	public static bool IsWork = true;

	private Config _config;

	public readonly Dictionary<Player, List<Injury>> _playerInjuries = new Dictionary<Player, List<Injury>>();

	private readonly Dictionary<Player, CoroutineHandle> _damageCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> _hintCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> _healingCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> _postTreatmentRegenCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, int> _adrenalineUses = new Dictionary<Player, int>();

	private readonly Dictionary<Player, int> _medkitUses = new Dictionary<Player, int>();

	private readonly Dictionary<Player, CoroutineHandle> _adrenalineResetCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, Player> _spectatedPlayers = new Dictionary<Player, Player>();

	private const float LimbHeightThreshold = 0.3f;

	private const float ArmHitboxThreshold = 0.1025f;

	private const float TreatmentDurationSeconds = 7f;

	private const byte TreatmentSlownessIntensity = 35;

	private const float HealPerSecondAmount = 3.5f;

	private const float RegularBleedingLifetimeSeconds = 35f;

	private const float StrongBleedingLifetimeSeconds = 50f;

	private const float ExplosionBleedingEffectDurationSeconds = 45f;

	private const float PostTreatmentRegenPerSecondAmount = 2f;

	private const float PostTreatmentRegenDurationSeconds = 18f;

	private const float PostTreatmentRegenStartDelaySeconds = 1f;

	private readonly Dictionary<Player, CoroutineHandle> _menuCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, int> _selectedOption = new Dictionary<Player, int>();

	private readonly Dictionary<ushort, MedicalKitInventory> _medkitInventories = new Dictionary<ushort, MedicalKitInventory>();

	private readonly Dictionary<Player, CoroutineHandle> _realHealCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly string[] _menuOptions = new string[5] { "Жгут-турникет", "Гемостатическая повязка", "Компрессионная повязка", "Окклюзионная наклейка", "Щипцы для извлечения" };

	public string Name => "RealDemage";

	public string Author => "FUST";

	public Version Version => new Version(0, 1, 0);

	public static Plugin Instance { get; private set; }

	public void OnEnabled()
	{
		Instance = this;
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.UsedItem += (CustomEventHandler<UsedItemEventArgs>)OnUsedItem;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.ChangingSpectatedPlayer += (CustomEventHandler<ChangingSpectatedPlayerEventArgs>)OnChangingSpectatedPlayer;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.CancellingItemUse += (CustomEventHandler<CancellingItemUseEventArgs>)OnCancellingItem;
		KeybindManager.OnObjectInteraction += OnKeybindPressed;
		Player.ChangedItem += (CustomEventHandler<ChangedItemEventArgs>)OnChangedItem;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
	}

	public void OnDisabled()
	{
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.UsedItem -= (CustomEventHandler<UsedItemEventArgs>)OnUsedItem;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.ChangingSpectatedPlayer -= (CustomEventHandler<ChangingSpectatedPlayerEventArgs>)OnChangingSpectatedPlayer;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.CancellingItemUse -= (CustomEventHandler<CancellingItemUseEventArgs>)OnCancellingItem;
		KeybindManager.OnObjectInteraction -= OnKeybindPressed;
		Player.ChangedItem -= (CustomEventHandler<ChangedItemEventArgs>)OnChangedItem;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		ClearAllData();
		foreach (CoroutineHandle value in _menuCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		foreach (CoroutineHandle value2 in _realHealCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
		}
		foreach (CoroutineHandle value3 in _postTreatmentRegenCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value3 });
		}
		_menuCoroutines.Clear();
		_selectedOption.Clear();
		_medkitInventories.Clear();
		_realHealCoroutines.Clear();
		_postTreatmentRegenCoroutines.Clear();
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config();
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		CustomItem obj = CustomItem.Get(1u);
		if (obj != null && obj.Check(ev.Item))
		{
			return;
		}
		CustomItem obj2 = CustomItem.Get(2u);
		if (obj2 != null && obj2.Check(ev.Item))
		{
			return;
		}
		CustomItem obj3 = CustomItem.Get(3u);
		if (obj3 != null && obj3.Check(ev.Item))
		{
			return;
		}
		CustomItem obj4 = CustomItem.Get(4u);
		if (obj4 != null && obj4.Check(ev.Item))
		{
			return;
		}
		CustomItem obj5 = CustomItem.Get(5u);
		if (obj5 != null && obj5.Check(ev.Item))
		{
			return;
		}
		CustomItem obj6 = CustomItem.Get(6u);
		if (obj6 != null && obj6.Check(ev.Item))
		{
			return;
		}
		CustomItem obj7 = CustomItem.Get(7u);
		if (obj7 != null && obj7.Check(ev.Item))
		{
			return;
		}
		CustomItem obj8 = CustomItem.Get(8u);
		if (obj8 != null && obj8.Check(ev.Item))
		{
			return;
		}
		CustomItem obj9 = CustomItem.Get(9u);
		if (obj9 != null && obj9.Check(ev.Item))
		{
			return;
		}
		CustomItem obj10 = CustomItem.Get(10u);
		if (obj10 != null && obj10.Check(ev.Item))
		{
			return;
		}
		CustomItem obj11 = CustomItem.Get(11u);
		if (obj11 != null && obj11.Check(ev.Item))
		{
			return;
		}
		CustomItem obj12 = CustomItem.Get(12u);
		if (obj12 != null && obj12.Check(ev.Item))
		{
			return;
		}
		CustomItem obj13 = CustomItem.Get(13u);
		if (obj13 != null && obj13.Check(ev.Item))
		{
			return;
		}
		CustomItem obj14 = CustomItem.Get(14u);
		if (obj14 != null && obj14.Check(ev.Item))
		{
			return;
		}
		CustomItem obj15 = CustomItem.Get(15u);
		if (obj15 != null && obj15.Check(ev.Item))
		{
			return;
		}
		CustomItem obj16 = CustomItem.Get(16u);
		if (obj16 != null && obj16.Check(ev.Item))
		{
			return;
		}
		CustomItem obj17 = CustomItem.Get(17u);
		if (obj17 != null && obj17.Check(ev.Item))
		{
			return;
		}
		CustomItem obj18 = CustomItem.Get(18u);
		if (obj18 != null && obj18.Check(ev.Item))
		{
			return;
		}
		CustomItem obj19 = CustomItem.Get(19u);
		if (obj19 == null || !obj19.Check(ev.Item))
		{
			Item item = ev.Item;
			if (item != null && (int)item.Type == 14)
			{
				ev.IsAllowed = false;
			}
		}
	}

	private void OnCancellingItem(CancellingItemUseEventArgs ev)
	{
		CustomItem obj = CustomItem.Get(1u);
		if (obj != null && obj.Check(ev.Item))
		{
			return;
		}
		CustomItem obj2 = CustomItem.Get(2u);
		if (obj2 != null && obj2.Check(ev.Item))
		{
			return;
		}
		CustomItem obj3 = CustomItem.Get(3u);
		if (obj3 != null && obj3.Check(ev.Item))
		{
			return;
		}
		CustomItem obj4 = CustomItem.Get(4u);
		if (obj4 != null && obj4.Check(ev.Item))
		{
			return;
		}
		CustomItem obj5 = CustomItem.Get(5u);
		if (obj5 != null && obj5.Check(ev.Item))
		{
			return;
		}
		CustomItem obj6 = CustomItem.Get(6u);
		if (obj6 != null && obj6.Check(ev.Item))
		{
			return;
		}
		CustomItem obj7 = CustomItem.Get(7u);
		if (obj7 != null && obj7.Check(ev.Item))
		{
			return;
		}
		CustomItem obj8 = CustomItem.Get(8u);
		if (obj8 != null && obj8.Check(ev.Item))
		{
			return;
		}
		CustomItem obj9 = CustomItem.Get(9u);
		if (obj9 != null && obj9.Check(ev.Item))
		{
			return;
		}
		CustomItem obj10 = CustomItem.Get(10u);
		if (obj10 != null && obj10.Check(ev.Item))
		{
			return;
		}
		CustomItem obj11 = CustomItem.Get(11u);
		if (obj11 != null && obj11.Check(ev.Item))
		{
			return;
		}
		CustomItem obj12 = CustomItem.Get(12u);
		if (obj12 != null && obj12.Check(ev.Item))
		{
			return;
		}
		CustomItem obj13 = CustomItem.Get(13u);
		if (obj13 != null && obj13.Check(ev.Item))
		{
			return;
		}
		CustomItem obj14 = CustomItem.Get(14u);
		if (obj14 != null && obj14.Check(ev.Item))
		{
			return;
		}
		CustomItem obj15 = CustomItem.Get(15u);
		if (obj15 != null && obj15.Check(ev.Item))
		{
			return;
		}
		CustomItem obj16 = CustomItem.Get(16u);
		if (obj16 != null && obj16.Check(ev.Item))
		{
			return;
		}
		CustomItem obj17 = CustomItem.Get(17u);
		if (obj17 != null && obj17.Check(ev.Item))
		{
			return;
		}
		CustomItem obj18 = CustomItem.Get(18u);
		if (obj18 != null && obj18.Check(ev.Item))
		{
			return;
		}
		CustomItem obj19 = CustomItem.Get(19u);
		if (obj19 != null && obj19.Check(ev.Item))
		{
			return;
		}
		Item item = ev.Item;
		if (item != null && (int)item.Type == 14)
		{
			ev.Player.DisableEffect<Slowness>();
			if (_realHealCoroutines.TryGetValue(ev.Player, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_realHealCoroutines.Remove(ev.Player);
				ShowMeowHint(ev.Player, 3f, "Лечение прервано!");
			}
		}
	}

	private void OnChangedItem(ChangedItemEventArgs ev)
	{
		CustomItem obj = CustomItem.Get(1u);
		if (obj != null && obj.Check(ev.Item))
		{
			return;
		}
		CustomItem obj2 = CustomItem.Get(2u);
		if (obj2 != null && obj2.Check(ev.Item))
		{
			return;
		}
		CustomItem obj3 = CustomItem.Get(3u);
		if (obj3 != null && obj3.Check(ev.Item))
		{
			return;
		}
		CustomItem obj4 = CustomItem.Get(4u);
		if (obj4 != null && obj4.Check(ev.Item))
		{
			return;
		}
		CustomItem obj5 = CustomItem.Get(5u);
		if (obj5 != null && obj5.Check(ev.Item))
		{
			return;
		}
		CustomItem obj6 = CustomItem.Get(6u);
		if (obj6 != null && obj6.Check(ev.Item))
		{
			return;
		}
		CustomItem obj7 = CustomItem.Get(7u);
		if (obj7 != null && obj7.Check(ev.Item))
		{
			return;
		}
		CustomItem obj8 = CustomItem.Get(8u);
		if (obj8 != null && obj8.Check(ev.Item))
		{
			return;
		}
		CustomItem obj9 = CustomItem.Get(9u);
		if (obj9 != null && obj9.Check(ev.Item))
		{
			return;
		}
		CustomItem obj10 = CustomItem.Get(10u);
		if (obj10 != null && obj10.Check(ev.Item))
		{
			return;
		}
		CustomItem obj11 = CustomItem.Get(11u);
		if (obj11 != null && obj11.Check(ev.Item))
		{
			return;
		}
		CustomItem obj12 = CustomItem.Get(12u);
		if (obj12 != null && obj12.Check(ev.Item))
		{
			return;
		}
		CustomItem obj13 = CustomItem.Get(13u);
		if (obj13 != null && obj13.Check(ev.Item))
		{
			return;
		}
		CustomItem obj14 = CustomItem.Get(14u);
		if (obj14 != null && obj14.Check(ev.Item))
		{
			return;
		}
		CustomItem obj15 = CustomItem.Get(15u);
		if (obj15 != null && obj15.Check(ev.Item))
		{
			return;
		}
		CustomItem obj16 = CustomItem.Get(16u);
		if (obj16 != null && obj16.Check(ev.Item))
		{
			return;
		}
		CustomItem obj17 = CustomItem.Get(17u);
		if (obj17 != null && obj17.Check(ev.Item))
		{
			return;
		}
		CustomItem obj18 = CustomItem.Get(18u);
		if (obj18 != null && obj18.Check(ev.Item))
		{
			return;
		}
		CustomItem obj19 = CustomItem.Get(19u);
		if (obj19 != null && obj19.Check(ev.Item))
		{
			return;
		}
		Item item = ev.Item;
		if (item == null || (int)item.Type != 14)
		{
			StopMedkitMenu(ev.Player);
			return;
		}
		ushort serial = ev.Item.Serial;
		if (!_medkitInventories.ContainsKey(serial))
		{
			_medkitInventories[serial] = new MedicalKitInventory();
			if (!_menuCoroutines.ContainsKey(ev.Player))
			{
				_selectedOption[ev.Player] = 0;
				_menuCoroutines[ev.Player] = Timing.RunCoroutine(ShowMedkitMenuCoroutine(ev.Player, serial));
				ShowMeowHint(ev.Player, 3f, "Меню аптечки открыто");
			}
		}
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		CustomItem obj = CustomItem.Get(1u);
		if (obj != null && obj.Check(ev.Item))
		{
			return;
		}
		CustomItem obj2 = CustomItem.Get(2u);
		if (obj2 != null && obj2.Check(ev.Item))
		{
			return;
		}
		CustomItem obj3 = CustomItem.Get(3u);
		if (obj3 != null && obj3.Check(ev.Item))
		{
			return;
		}
		CustomItem obj4 = CustomItem.Get(4u);
		if (obj4 != null && obj4.Check(ev.Item))
		{
			return;
		}
		CustomItem obj5 = CustomItem.Get(5u);
		if (obj5 != null && obj5.Check(ev.Item))
		{
			return;
		}
		CustomItem obj6 = CustomItem.Get(6u);
		if (obj6 != null && obj6.Check(ev.Item))
		{
			return;
		}
		CustomItem obj7 = CustomItem.Get(7u);
		if (obj7 != null && obj7.Check(ev.Item))
		{
			return;
		}
		CustomItem obj8 = CustomItem.Get(8u);
		if (obj8 != null && obj8.Check(ev.Item))
		{
			return;
		}
		CustomItem obj9 = CustomItem.Get(9u);
		if (obj9 != null && obj9.Check(ev.Item))
		{
			return;
		}
		CustomItem obj10 = CustomItem.Get(10u);
		if (obj10 != null && obj10.Check(ev.Item))
		{
			return;
		}
		CustomItem obj11 = CustomItem.Get(11u);
		if (obj11 != null && obj11.Check(ev.Item))
		{
			return;
		}
		CustomItem obj12 = CustomItem.Get(12u);
		if (obj12 != null && obj12.Check(ev.Item))
		{
			return;
		}
		CustomItem obj13 = CustomItem.Get(13u);
		if (obj13 != null && obj13.Check(ev.Item))
		{
			return;
		}
		CustomItem obj14 = CustomItem.Get(14u);
		if (obj14 != null && obj14.Check(ev.Item))
		{
			return;
		}
		CustomItem obj15 = CustomItem.Get(15u);
		if (obj15 != null && obj15.Check(ev.Item))
		{
			return;
		}
		CustomItem obj16 = CustomItem.Get(16u);
		if (obj16 != null && obj16.Check(ev.Item))
		{
			return;
		}
		CustomItem obj17 = CustomItem.Get(17u);
		if (obj17 != null && obj17.Check(ev.Item))
		{
			return;
		}
		CustomItem obj18 = CustomItem.Get(18u);
		if (obj18 != null && obj18.Check(ev.Item))
		{
			return;
		}
		CustomItem obj19 = CustomItem.Get(19u);
		if (obj19 == null || !obj19.Check(ev.Item))
		{
			Item item = ev.Item;
			if (item != null && (int)item.Type == 14)
			{
				StopMedkitMenu(ev.Player);
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
		if (val2 == (Player)null || !val2.IsAlive || val2.IsScp)
		{
			return;
		}
		Item currentItem = val2.CurrentItem;
		if (currentItem == null || (int)currentItem.Type != 14)
		{
			return;
		}
		ushort serial = val2.CurrentItem.Serial;
		if (!_medkitInventories.ContainsKey(serial))
		{
			return;
		}
		switch (((ServerSpecificSettingBase)val).SettingId)
		{
		case 81:
			if (!_menuCoroutines.ContainsKey(val2))
			{
				_selectedOption[val2] = 0;
				_menuCoroutines[val2] = Timing.RunCoroutine(ShowMedkitMenuCoroutine(val2, serial));
				ShowMeowHint(val2, 3f, "Меню аптечки открыто");
			}
			else
			{
				StopMedkitMenu(val2);
				ShowMeowHint(val2, 3f, "Меню аптечки закрыто");
			}
			break;
		case 82:
			if (_menuCoroutines.ContainsKey(val2))
			{
				_selectedOption[val2] = (_selectedOption[val2] - 1 + _menuOptions.Length) % _menuOptions.Length;
				UpdateMedkitMenu(val2, serial);
			}
			break;
		case 83:
			if (_menuCoroutines.ContainsKey(val2))
			{
				_selectedOption[val2] = (_selectedOption[val2] + 1) % _menuOptions.Length;
				UpdateMedkitMenu(val2, serial);
			}
			break;
		case 84:
			if (_menuCoroutines.ContainsKey(val2))
			{
				ExecuteMedkitAction(val2, serial);
			}
			break;
		}
	}

	[IteratorStateMachine(typeof(ShowMedkitMenuCoroutine_d_48))]
	private IEnumerator<float> ShowMedkitMenuCoroutine(Player p, ushort s)
	{
		return new ShowMedkitMenuCoroutine_d_48(0)
		{
			__4__this = this,
			p = p,
			s = s
		};
	}

	private void UpdateMedkitMenu(Player p, ushort s)
	{
		ShowMedkitMenu(p, _medkitInventories[s]);
	}

	private void ShowMedkitMenu(Player p, MedicalKitInventory inv)
	{
		string text = "<size=29><b><color=#61616193>『</color></size> <size=21>Меню аптечки</size> <size=29><b><color=#61616193>』</color></size>\n";
		for (int i = 0; i < _menuOptions.Length; i++)
		{
			string text2 = ((_selectedOption[p] == i) ? "<color=#61616193>→</color>" : "");
			string text3 = ((_selectedOption[p] == i) ? "<color=#61616193>←</color>" : "");
			if (1 == 0)
			{
			}
			string text4 = i switch
			{
				0 => $"[<color=#15c239>{inv.Tourniquets}шт</color>]", 
				1 => $"[<color=#15c239>{inv.HemostaticBandages}шт</color>]", 
				2 => $"[<color=#15c239>{inv.CompressionBandages}шт</color>]", 
				3 => $"[<color=#15c239>{inv.OcclusivePatches}шт</color>]", 
				4 => "[<color=#15c239>∞шт</color>]", 
				_ => "", 
			};
			if (1 == 0)
			{
			}
			string text5 = text4;
			text = text + "<size=16><b>" + text2 + " " + _menuOptions[i] + " " + text5 + " " + text3 + "</b></size>\n";
		}
		p.ShowMeowHint(0.2f, text, (HintVerticalAlign)0, 870, 0, (HintAlignment)1);
	}

	private void ExecuteMedkitAction(Player p, ushort s)
	{
		if (!_medkitInventories.TryGetValue(s, out var value))
		{
			return;
		}
		int num = _selectedOption[p];
		if (!IsAvailable(num, value))
		{
			ShowMeowHint(p, 3f, "Предмет недоступен!");
			return;
		}
		Player target = GetTarget(p);
		if (target == (Player)null)
		{
			ShowMeowHint(p, 3f, "Цель не найдена!");
		}
		else
		{
			ApplyTreatment(p, target, num, value, s);
		}
	}

	private bool IsAvailable(int i, MedicalKitInventory inv)
	{
		if (1 == 0)
		{
		}
		bool result = i switch
		{
			0 => inv.Tourniquets > 0, 
			1 => inv.HemostaticBandages > 0, 
			2 => inv.CompressionBandages > 0, 
			3 => inv.OcclusivePatches > 0, 
			4 => inv.Forceps, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private Player GetTarget(Player p)
	{
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(p.CameraTransform.position, p.CameraTransform.forward, ref val, 1.5f, LayerMask.GetMask(new string[1] { "Player" })))
		{
			Player val2 = Player.Get(((Component)((RaycastHit)(ref val)).collider).gameObject);
			if (val2 != (Player)null && val2 != p && val2.IsAlive && !val2.IsScp)
			{
				return val2;
			}
		}
		return p;
	}

	private void ApplyTreatment(Player p, Player t, int type, MedicalKitInventory inv, ushort s)
	{
		StopMedkitMenu(p);
		if (!IsAvailable(type, inv))
		{
			ShowMeowHint(p, 5f, "Предмет недоступен!");
		}
		else if (!ApplyEffect(t, type))
		{
			ShowMeowHint(p, 5f, "Нечего лечить");
		}
		else
		{
			_realHealCoroutines[p] = Timing.RunCoroutine(TreatmentCoroutine(p, t, type, s));
		}
	}

	[IteratorStateMachine(typeof(TreatmentCoroutine_d_55))]
	private IEnumerator<float> TreatmentCoroutine(Player p, Player t, int type, ushort s)
	{
		return new TreatmentCoroutine_d_55(0)
		{
			__4__this = this,
			p = p,
			t = t,
			type = type,
			s = s
		};
	}

	private void CompleteTreatment(Player p, Player t, int type, MedicalKitInventory medicalKit, ushort s)
	{
		if (!_playerInjuries.TryGetValue(t, out var value))
		{
			return;
		}
		bool flag = false;
		switch (type)
		{
		case 0:
		{
			Timing.RunCoroutine(HealingCoroutine(t));
			Injury injury4 = value.FirstOrDefault((Injury i) => i.Type == "Сильное кровотечение" && (i.BodyPart.Contains("рука") || i.BodyPart.Contains("нога")));
			if (injury4 != null)
			{
				value.Remove(injury4);
				flag = true;
			}
			break;
		}
		case 1:
		{
			Timing.RunCoroutine(HealingCoroutine(t));
			Injury injury3 = value.FirstOrDefault((Injury i) => i.Type == "Глубокая рана");
			if (injury3 != null)
			{
				value.Remove(injury3);
				flag = true;
			}
			break;
		}
		case 2:
		{
			Timing.RunCoroutine(HealingCoroutine(t));
			Injury injury2 = value.FirstOrDefault((Injury i) => i.Type == "Кровотечение" && (i.BodyPart.Contains("рука") || i.BodyPart.Contains("нога")));
			if (injury2 != null)
			{
				value.Remove(injury2);
				flag = true;
			}
			break;
		}
		case 3:
		{
			Timing.RunCoroutine(HealingCoroutine(t));
			Injury injury5 = value.FirstOrDefault((Injury i) => i.Type == "Кровотечение" && (i.BodyPart == "Туловище" || i.BodyPart == "Грудь"));
			if (injury5 != null)
			{
				value.Remove(injury5);
				flag = true;
			}
			break;
		}
		case 4:
		{
			Injury injury = value.FirstOrDefault((Injury i) => i.Type == "Осколки от гранаты");
			if (injury != null)
			{
				value.Remove(injury);
				value.Add(new Injury
				{
					BodyPart = injury.BodyPart,
					Type = "Глубокая рана",
					DamagePerSecond = 0.5f,
					Timestamp = Time.time
				});
				value.Add(new Injury
				{
					BodyPart = injury.BodyPart,
					Type = "Кровотечение",
					DamagePerSecond = 0.1f,
					Timestamp = Time.time
				});
				ApplyEffects(t, value);
				flag = true;
			}
			break;
		}
		}
		if (flag)
		{
			if (_medkitInventories.TryGetValue(s, out var _))
			{
				UpdateMedkitMenu(p, s);
			}
			StartPostTreatmentRegen(t);
			string msg = ((t == p) ? ("Вы применили " + GetName(type)) : ("Вы применили " + GetName(type) + " на " + t.DisplayNickname));
			ShowMeowHint(p, 5f, msg);
			if (t != p)
			{
				ShowMeowHint(t, 5f, p.DisplayNickname + " применил " + GetName(type));
			}
		}
		else
		{
			ShowMeowHint(p, 5f, "Лечение не нужно");
		}
		if (_medkitInventories.TryGetValue(s, out var value3) && value3.Tourniquets <= 0 && value3.HemostaticBandages <= 0 && value3.CompressionBandages <= 0 && value3.OcclusivePatches <= 0)
		{
			ShowMeowHint(p, 5f, "Аптечка исчерпана!");
		}
	}

	private string GetName(int t)
	{
		if (1 == 0)
		{
		}
		string result = t switch
		{
			0 => "Жгут-турникет", 
			1 => "Гемостатическая повязка", 
			2 => "Компрессионная повязка", 
			3 => "Окклюзионная наклейка", 
			4 => "Щипцы для осколков", 
			_ => "Лечение", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private bool ApplyEffect(Player t, int type)
	{
		if (!_playerInjuries.TryGetValue(t, out var value))
		{
			return false;
		}
		bool flag = false;
		switch (type)
		{
		case 0:
			flag = value.Any((Injury i) => (i.Type == "Сильное кровотечение" && (i.BodyPart.Contains("рука") || i.BodyPart.Contains("нога"))) || i.BodyPart.Contains("Туловище") || i.BodyPart.Contains("Грудь"));
			break;
		case 1:
			flag = value.Any((Injury i) => i.Type == "Глубокая рана");
			break;
		case 2:
			flag = value.Any((Injury i) => (i.Type == "Кровотечение" && (i.BodyPart.Contains("рука") || i.BodyPart.Contains("нога"))) || i.BodyPart.Contains("Туловище") || i.BodyPart.Contains("Грудь"));
			break;
		case 3:
			flag = value.Any((Injury i) => (i.Type == "Кровотечение" && (i.BodyPart.Contains("рука") || i.BodyPart.Contains("нога"))) || i.BodyPart.Contains("Туловище") || i.BodyPart.Contains("Грудь"));
			break;
		case 4:
			flag = value.Any((Injury i) => i.Type == "Осколки от гранаты");
			break;
		}
		if (!flag)
		{
			return false;
		}
		t.DisableAllEffects();
		return true;
	}

	private void StopMedkitMenu(Player p)
	{
		if (_menuCoroutines.TryGetValue(p, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_menuCoroutines.Remove(p);
		}
		if (_realHealCoroutines.TryGetValue(p, out var value2))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
			_realHealCoroutines.Remove(p);
			p.DisableEffect((EffectType)43);
		}
		_selectedOption.Remove(p);
	}

	private void ShowMeowHint(Player p, float time, string msg)
	{
		p.ShowMeowHint(time, "<size=29><b><color=#61616193>|</color></size> <size=19>" + msg + "</size> <size=29><b><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
	}

	private void OnShot(ShotEventArgs ev)
	{
		CustomItem val = default(CustomItem);
		if (_config.IsEnabled && !(ev.Player == (Player)null) && !(ev.Target == (Player)null) && ev.Target.IsHuman && ev.Item != null && !((Object)(object)ev.Hitbox == (Object)null) && (IsWork || !CustomItem.TryGet(ev.Item, ref val) || val.Id == 1))
		{
			CustomItem obj = CustomItem.Get(16u);
			if (obj == null || !obj.Check(ev.Item))
			{
				Item item = ev.Item;
				Player target = ev.Target;
				HitboxType hitboxType = ev.Hitbox.HitboxType;
				RaycastHit raycastHit = ev.RaycastHit;
				HandleShot(item, target, hitboxType, ((RaycastHit)(ref raycastHit)).point);
			}
		}
	}

	private void HandleAdrenalineOveruse(Player player)
	{
		if (!_adrenalineUses.ContainsKey(player))
		{
			return;
		}
		int num = _adrenalineUses[player];
		if (num == 1)
		{
			if (_playerInjuries.ContainsKey(player) && _playerInjuries[player].Any((Injury i) => i.Type == "Кровотечение"))
			{
				List<Injury> list = _playerInjuries[player].Where((Injury i) => i.Type == "Кровотечение").ToList();
				foreach (Injury item3 in list)
				{
					item3.DamagePerSecond *= 1.5f;
				}
			}
		}
		else if (num == 2)
		{
			if (_playerInjuries.ContainsKey(player) && _playerInjuries[player].Any((Injury i) => i.Type == "Кровотечение"))
			{
				List<Injury> list2 = _playerInjuries[player].Where((Injury i) => i.Type == "Кровотечение").ToList();
				foreach (Injury item4 in list2)
				{
					item4.DamagePerSecond *= 2f;
				}
			}
			else
			{
				if (!_playerInjuries.ContainsKey(player))
				{
					_playerInjuries[player] = new List<Injury>();
				}
				Injury item = new Injury
				{
					BodyPart = "Тело",
					Type = "Кровотечение",
					DamagePerSecond = 0.1f,
					Timestamp = Time.time
				};
				_playerInjuries[player].Add(item);
				ApplyEffects(player, new List<Injury> { item });
			}
		}
		else if (num >= 3)
		{
			player.EnableEffect((EffectType)29, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)16, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)2, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)1, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)43, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)43, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)6, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)21, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)42, (byte)8, 180f, false);
			player.Health -= 5f;
			if (_playerInjuries.ContainsKey(player) && _playerInjuries[player].Any((Injury i) => i.Type == "Кровотечение"))
			{
				List<Injury> list3 = _playerInjuries[player].Where((Injury i) => i.Type == "Кровотечение").ToList();
				foreach (Injury item5 in list3)
				{
					item5.DamagePerSecond *= 3f;
				}
			}
			else
			{
				Injury item2 = new Injury
				{
					BodyPart = "Тело",
					Type = "Кровотечение",
					DamagePerSecond = 0.3f,
					Timestamp = Time.time
				};
				_playerInjuries[player].Add(item2);
				ApplyEffects(player, new List<Injury> { item2 });
			}
			ResetAdrenalineUses(player);
		}
		else if (num >= 4)
		{
			player.EnableEffect((EffectType)30, byte.MaxValue, 180f, false);
			ResetAdrenalineUses(player);
		}
		if (_playerInjuries.ContainsKey(player) && !_damageCoroutines.ContainsKey(player))
		{
			_damageCoroutines[player] = Timing.RunCoroutine(DamageCoroutine(player));
		}
		if (_playerInjuries.ContainsKey(player) && !_hintCoroutines.ContainsKey(player))
		{
			_hintCoroutines[player] = Timing.RunCoroutine(HintCoroutine(player));
		}
	}

	private void OnUsedItem(UsedItemEventArgs ev)
	{
		if (_config.IsEnabled && !(ev.Player == (Player)null) && ev.Item != null)
		{
			if ((int)ev.Item.Type == 14)
			{
				HandleHealing(ev.Player, (ItemType)14);
			}
			else if ((int)ev.Item.Type == 33)
			{
				HandleHealing(ev.Player, (ItemType)33);
				ev.Player.DisableEffect((EffectType)16);
				ev.Player.ArtificialHealth = 0f;
				ev.Player.EnableEffect((EffectType)29, byte.MaxValue, 60f, false);
			}
			else if ((int)ev.Item.Type == 34)
			{
				HandlePainkillers(ev.Player);
				ev.Player.EnableEffect((EffectType)22, (byte)100, 60f, false);
			}
			else if ((int)ev.Item.Type == 17)
			{
				ev.Player.DisableAllEffects();
			}
		}
	}

	private void HandlePainkillers(Player player)
	{
		if (!_medkitUses.ContainsKey(player))
		{
			_medkitUses[player] = 0;
		}
		_medkitUses[player]++;
		if (_medkitUses[player] >= 3)
		{
			player.EnableEffect((EffectType)2, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)21, byte.MaxValue, 180f, false);
			player.EnableEffect((EffectType)18, (byte)1, 180f, false);
			player.EnableEffect((EffectType)42, (byte)4, 180f, false);
			player.Health -= 3f;
		}
		if (_medkitUses[player] >= 4)
		{
			player.EnableEffect((EffectType)30, byte.MaxValue, 180f, false);
			player.Health -= 3f;
			_medkitUses[player] = 0;
		}
	}

	private void OnDying(DyingEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			ClearPlayerData(ev.Player);
		}
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		if (!_config.IsEnabled || ev.Player == (Player)null || ev.DamageHandler == null || !IsWork)
		{
			return;
		}
		if ((int)((DamageHandlerBase)ev.DamageHandler).Type == 25)
		{
			List<Injury> list = new List<Injury>();
			string bodyPart = ((Random.Range(0, 2) == 0) ? "Бедро" : "Грудь");
			list.Add(new Injury
			{
				BodyPart = bodyPart,
				Type = "Глубокая рана",
				DamagePerSecond = 0f
			});
			ev.Player.EnableEffect((EffectType)26, (byte)1, 0f, false);
			if (Random.value <= 0.7f)
			{
				ev.Player.EnableEffect((EffectType)30, (byte)1, 0f, false);
			}
			ev.Player.Hurt(25f, (DamageType)0, "");
			ApplyInjuries(ev.Player, list);
		}
		else if ((int)((DamageHandlerBase)ev.DamageHandler).Type == 11)
		{
			List<Injury> list2 = new List<Injury>();
			string bodyPart2 = ((Random.Range(0, 2) == 0) ? "Бедро" : "Грудь");
			list2.Add(new Injury
			{
				BodyPart = bodyPart2,
				Type = "Осколки от гранаты",
				DamagePerSecond = 0f
			});
			ev.Player.EnableEffect((EffectType)26, byte.MaxValue, 5f, false);
			ev.Player.EnableEffect((EffectType)7, byte.MaxValue, 15f, false);
			ev.Player.EnableEffect((EffectType)4, (byte)3, 45f, false);
			ApplyInjuries(ev.Player, list2);
		}
		if ((int)((DamageHandlerBase)ev.DamageHandler).Type == 1)
		{
			HandleFallDamage(ev.Player);
		}
	}

	private void ApplyInjuries(Player player, List<Injury> injuries)
	{
		if (!_playerInjuries.ContainsKey(player))
		{
			_playerInjuries[player] = new List<Injury>();
		}
		foreach (Injury injury in injuries)
		{
			if (!_playerInjuries[player].Any((Injury i) => i.BodyPart == injury.BodyPart && i.Type == injury.Type))
			{
				injury.Timestamp = Time.time;
				_playerInjuries[player].Add(injury);
			}
		}
		ApplyEffects(player, injuries);
		if (!_hintCoroutines.ContainsKey(player))
		{
			_hintCoroutines[player] = Timing.RunCoroutine(HintCoroutine(player));
		}
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			ClearPlayerData(ev.Player);
		}
	}

	private void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && !(ev.NewTarget == (Player)null))
		{
			Player player = ev.Player;
			Player newTarget = ev.NewTarget;
			if (_spectatedPlayers.ContainsKey(player))
			{
				_spectatedPlayers[player] = newTarget;
			}
			else
			{
				_spectatedPlayers.Add(player, newTarget);
			}
		}
	}

	private void OnPlayerSpawned(SpawnedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && _spectatedPlayers.ContainsKey(ev.Player))
		{
			_spectatedPlayers.Remove(ev.Player);
		}
	}

	private void HandleShot(Item item, Player target, HitboxType hitbox, Vector3 hit)
	{
		if (item == null || target == (Player)null || _config == null || target.Role == (Role)null)
		{
			return;
		}
		BoneType preciseHitBone = GetPreciseHitBone(hitbox, hit, target.Position);
		string bodyPart = GetBoneName(preciseHitBone);
		if (bodyPart == null || target.IsGodModeEnabled)
		{
			return;
		}
		Role role = target.Role;
		if (role != null && (int)role.Team == 0)
		{
			return;
		}
		if (!_playerInjuries.ContainsKey(target))
		{
			_playerInjuries[target] = new List<Injury>();
		}
		List<Injury> list = GenerateInjuries(hitbox, bodyPart, target);
		foreach (Injury injury in list)
		{
			if (!_playerInjuries[target].Any((Injury i) => i.BodyPart == bodyPart && i.Type == injury.Type))
			{
				injury.Timestamp = Time.time;
				_playerInjuries[target].Add(injury);
			}
		}
		ApplyEffects(target, list);
		if (!_damageCoroutines.ContainsKey(target))
		{
			_damageCoroutines[target] = Timing.RunCoroutine(DamageCoroutine(target));
		}
		if (!_hintCoroutines.ContainsKey(target))
		{
			_hintCoroutines[target] = Timing.RunCoroutine(HintCoroutine(target));
		}
	}

	private string GetBoneName(BoneType bone)
	{
		if (1 == 0)
		{
		}
		string result = bone switch
		{
			BoneType.Head => "Голова", 
			BoneType.Body => "Туловище", 
			BoneType.LeftArm => "Левая рука", 
			BoneType.RightArm => "Правая рука", 
			BoneType.LeftLeg => "Левая нога", 
			BoneType.RightLeg => "Правая нога", 
			_ => "Тело", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private void HandleFallDamage(Player player)
	{
		if (player == (Player)null || player.IsGodModeEnabled)
		{
			return;
		}
		Role role = player.Role;
		if ((role == null || (int)role.Team != 0) && _config != null)
		{
			if (!_playerInjuries.ContainsKey(player))
			{
				_playerInjuries[player] = new List<Injury>();
			}
			string bodyPart = ((Random.Range(0, 2) == 0) ? "Левая нога" : "Правая нога");
			Injury injury = new Injury
			{
				BodyPart = bodyPart,
				Type = "Перелом",
				DamagePerSecond = 0.3f,
				Timestamp = Time.time
			};
			if (!_playerInjuries[player].Any((Injury i) => i.BodyPart == bodyPart && i.Type == injury.Type))
			{
				_playerInjuries[player].Add(injury);
				ApplyEffects(player, new List<Injury> { injury });
			}
			if (!_damageCoroutines.ContainsKey(player))
			{
				_damageCoroutines[player] = Timing.RunCoroutine(DamageCoroutine(player));
			}
			if (!_hintCoroutines.ContainsKey(player))
			{
				_hintCoroutines[player] = Timing.RunCoroutine(HintCoroutine(player));
			}
		}
	}

	private void HandleHealing(Player player, ItemType itemType)
	{
		if (player == (Player)null || _config == null)
		{
			return;
		}
		if ((int)itemType == 14)
		{
			if (!_medkitUses.ContainsKey(player))
			{
				_medkitUses[player] = 0;
			}
			_medkitUses[player]++;
			if (_medkitUses[player] >= 3)
			{
				ResetAdrenalineUses(player);
				_medkitUses[player] = 0;
			}
			if (!_healingCoroutines.ContainsKey(player))
			{
				_healingCoroutines[player] = Timing.RunCoroutine(HealingCoroutine(player));
			}
		}
		else if ((int)itemType == 33)
		{
			if (!_adrenalineUses.ContainsKey(player))
			{
				_adrenalineUses[player] = 0;
				_adrenalineResetCoroutines[player] = Timing.RunCoroutine(AdrenalineResetCoroutine(player));
			}
			_adrenalineUses[player]++;
			HandleAdrenalineOveruse(player);
		}
	}

	private void ClearPlayerData(Player player)
	{
		if (player == (Player)null)
		{
			return;
		}
		_playerInjuries.Remove(player);
		if (_damageCoroutines.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_damageCoroutines.Remove(player);
		}
		if (_hintCoroutines.TryGetValue(player, out var value2))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
			_hintCoroutines.Remove(player);
		}
		if (_healingCoroutines.TryGetValue(player, out var value3))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value3 });
			_healingCoroutines.Remove(player);
		}
		if (_postTreatmentRegenCoroutines.TryGetValue(player, out var value4))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value4 });
			_postTreatmentRegenCoroutines.Remove(player);
		}
		_adrenalineUses.Remove(player);
		_medkitUses.Remove(player);
		if (_adrenalineResetCoroutines.TryGetValue(player, out var value5))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value5 });
			_adrenalineResetCoroutines.Remove(player);
		}
		_spectatedPlayers.Remove(player);
		foreach (KeyValuePair<Player, Player> item in _spectatedPlayers.Where((KeyValuePair<Player, Player> p) => p.Value == player).ToList())
		{
			_spectatedPlayers.Remove(item.Key);
		}
	}

	private void ClearAllData()
	{
		foreach (Player item in _playerInjuries.Keys.ToList())
		{
			ClearPlayerData(item);
		}
		_spectatedPlayers.Clear();
	}

	[IteratorStateMachine(typeof(AdrenalineResetCoroutine_d_77))]
	private IEnumerator<float> AdrenalineResetCoroutine(Player player)
	{
		return new AdrenalineResetCoroutine_d_77(0)
		{
			__4__this = this,
			player = player
		};
	}

	private void ResetAdrenalineUses(Player player)
	{
		_adrenalineUses[player] = 0;
		if (_adrenalineResetCoroutines.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_adrenalineResetCoroutines.Remove(player);
		}
	}

	private void StartPostTreatmentRegen(Player player)
	{
		if (!(player == (Player)null) && player.IsAlive)
		{
			if (_postTreatmentRegenCoroutines.TryGetValue(player, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_postTreatmentRegenCoroutines.Remove(player);
			}
			_postTreatmentRegenCoroutines[player] = Timing.RunCoroutine(PostTreatmentRegenCoroutine(player));
		}
	}

	[IteratorStateMachine(typeof(PostTreatmentRegenCoroutine_d_80))]
	private IEnumerator<float> PostTreatmentRegenCoroutine(Player player)
	{
		return new PostTreatmentRegenCoroutine_d_80(0)
		{
			__4__this = this,
			player = player
		};
	}

	private List<Injury> GenerateInjuries(HitboxType hitbox, string bodyPart, Player target)
	{
		List<Injury> list = new List<Injury>();
		if ((int)hitbox == 1)
		{
			if (Random.value < 0.35f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Перелом",
					DamagePerSecond = 0.3f
				});
			}
			if (Random.value < 0.31f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Сильное кровотечение",
					DamagePerSecond = 0.15f
				});
			}
			if (Random.value < 0.4f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Кровотечение",
					DamagePerSecond = 0.08f
				});
			}
			if (Random.value < 0.2f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Растяжение",
					DamagePerSecond = 0.1f
				});
			}
			if (Random.value < 0.15f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Глубокая рана",
					DamagePerSecond = 0.5f
				});
			}
		}
		else if ((int)hitbox == 0)
		{
			if (Random.value < 0.4f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Кровотечение",
					DamagePerSecond = 0.08f
				});
			}
			if (Random.value < 0.35f)
			{
				list.Add(new Injury
				{
					BodyPart = bodyPart,
					Type = "Сильное кровотечение",
					DamagePerSecond = 0.15f
				});
			}
		}
		else if ((int)hitbox == 2 && Random.value < 0.7f)
		{
			list.Add(new Injury
			{
				BodyPart = bodyPart,
				Type = "Пробой черепа",
				DamagePerSecond = target.Health / 5f
			});
		}
		return list;
	}

	public void ApplyEffects(Player player, List<Injury> injuries)
	{
		if (player == (Player)null || injuries == null)
		{
			return;
		}
		foreach (Injury injury in injuries)
		{
			switch (injury.Type)
			{
			case "Сильное кровотечение":
				player.EnableEffect((EffectType)4, (byte)3, 0f, false);
				player.EnableEffect((EffectType)13, (byte)1, 0f, false);
				break;
			case "Кровотечение":
				player.EnableEffect((EffectType)4, (byte)1, 0f, false);
				player.EnableEffect((EffectType)13, (byte)1, 0f, false);
				break;
			case "Перелом":
				player.EnableEffect((EffectType)43, (byte)(player.IsInjuredFromFall(injury) ? 45 : ((injury.BodyPart == "Грудь") ? 33 : 63)), 0f, false);
				player.EnableEffect((EffectType)26, (byte)1, 0f, false);
				break;
			case "Растяжение":
				player.EnableEffect((EffectType)43, (byte)20, 0f, false);
				break;
			case "Глубокая рана":
				player.EnableEffect((EffectType)26, (byte)1, 0f, false);
				break;
			case "Сотрясение мозга":
				player.EnableEffect((EffectType)5, (byte)1, 0f, false);
				break;
			case "Осколочное ранение":
				player.EnableEffect((EffectType)26, (byte)1, 0f, false);
				player.EnableEffect((EffectType)13, (byte)1, 0f, false);
				break;
			case "Контузия":
				player.EnableEffect((EffectType)9, 10f, false);
				if (injury.DamagePerSecond > 0.15f)
				{
					player.EnableEffect((EffectType)5, 5f, false);
					player.EnableEffect((EffectType)11, 3f, false);
				}
				break;
			case "Повреждение от взрывной волны":
				player.EnableEffect((EffectType)15, (byte)1, 0f, false);
				player.EnableEffect((EffectType)13, (byte)1, 0f, false);
				break;
			}
		}
	}

	private void RemoveExpiredBleedingInjuries(Player player, List<Injury> injuries)
	{
		if (player == (Player)null || injuries == null || injuries.Count == 0)
		{
			return;
		}
		float now = Time.time;
		bool flag = false;
		flag |= injuries.RemoveAll((Injury i) => i.Type == "Кровотечение" && now - i.Timestamp >= 35f) > 0;
		flag |= injuries.RemoveAll((Injury i) => i.Type == "Сильное кровотечение" && now - i.Timestamp >= 50f) > 0;
		if (!injuries.Any((Injury i) => i.IsBleeding))
		{
			player.DisableEffect((EffectType)4);
			player.DisableEffect((EffectType)13);
		}
		if (flag && injuries.Count == 0)
		{
			if (_damageCoroutines.TryGetValue(player, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_damageCoroutines.Remove(player);
			}
			if (_hintCoroutines.TryGetValue(player, out var value2))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
				_hintCoroutines.Remove(player);
				player.ShowHint(string.Empty, 0.1f);
			}
			_playerInjuries.Remove(player);
		}
	}

	[IteratorStateMachine(typeof(DamageCoroutine_d_84))]
	private IEnumerator<float> DamageCoroutine(Player player)
	{
		return new DamageCoroutine_d_84(0)
		{
			__4__this = this,
			player = player
		};
	}

	[IteratorStateMachine(typeof(HintCoroutine_d_85))]
	private IEnumerator<float> HintCoroutine(Player player)
	{
		return new HintCoroutine_d_85(0)
		{
			__4__this = this,
			player = player
		};
	}

	[IteratorStateMachine(typeof(HealingCoroutine_d_86))]
	public IEnumerator<float> HealingCoroutine(Player player)
	{
		return new HealingCoroutine_d_86(0)
		{
			__4__this = this,
			player = player
		};
	}

	private BoneType GetPreciseHitBone(HitboxType hitboxType, Vector3 hitPosition, Vector3 playerPosition)
	{
		Vector3 val = hitPosition - playerPosition;
		if (1 == 0)
		{
		}
		BoneType result = (int)hitboxType switch
		{
			2 => BoneType.Head, 
			0 => (!(Mathf.Abs(val.x) > 0.1025f)) ? BoneType.Body : ((val.x < 0f) ? BoneType.LeftArm : BoneType.RightArm), 
			1 => (!(val.y <= 0.3f)) ? BoneType.Body : ((val.x < 0f) ? BoneType.LeftLeg : BoneType.RightLeg), 
			_ => BoneType.Unknown, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private string FormatInjuryHint(List<Injury> injuries)
	{
		if (injuries == null || injuries.Count == 0)
		{
			return string.Empty;
		}
		string text = BuildSmartStickman(injuries);
		IEnumerable<string> values = from g in (from i in injuries
				orderby i.Timestamp descending
				group i by i.BodyPart).Take(4)
			select "<size=17><color=#2e2e2ef5><b>╠</b></color></size><size=19>" + g.Key + ": <color=#7a1e1ef5><b>" + string.Join(", ", g.Select((Injury i) => i.Type).Distinct()) + "</b></color></size>";
		return text + "\n<size=17><color=#2e2e2ef5><b>╔</b></color></size><size=21><b> Ранения</b></size>\n" + string.Join("\n", values);
	}

	private string BuildSmartStickman(List<Injury> injuries)
	{
		int partSeverity = GetPartSeverity(injuries, "Голова");
		int partSeverity2 = GetPartSeverity(injuries, "Туловище", "Грудь", "Тело");
		int partSeverity3 = GetPartSeverity(injuries, "Левая рука");
		int partSeverity4 = GetPartSeverity(injuries, "Правая рука");
		int num = GetPartSeverity(injuries, "Левая нога", "Левое бедро");
		int num2 = GetPartSeverity(injuries, "Правая нога", "Правое бедро");
		int partSeverity5 = GetPartSeverity(injuries, "Бедро");
		if (partSeverity5 > 0)
		{
			num = Math.Max(num, partSeverity5);
			num2 = Math.Max(num2, partSeverity5);
		}
		string text = "<size=24>" + Segment("o", partSeverity) + "</size>";
		string text2 = "<size=24>" + Segment("/", partSeverity3) + Segment("|", partSeverity2) + Segment("\\", partSeverity4) + "</size>";
		string text3 = "<size=24>" + Segment("/", num) + " " + Segment("\\", num2) + "</size>";
		return string.Join("\n", "<size=18><color=#2e2e2ef5><b>Состояние тела</b></color></size>", text, text2, text3);
	}

	private int GetPartSeverity(IEnumerable<Injury> injuries, params string[] bodyParts)
	{
		int num = 0;
		foreach (Injury injury in injuries)
		{
			if (bodyParts.Any((string part) => injury.BodyPart != null && injury.BodyPart.AsSpan().Contains(part.AsSpan(), StringComparison.OrdinalIgnoreCase)))
			{
				num = Math.Max(num, GetInjurySeverity(injury));
			}
		}
		return num;
	}

	private int GetInjurySeverity(Injury injury)
	{
		if (injury == null || string.IsNullOrWhiteSpace(injury.Type))
		{
			return 0;
		}
		if (injury.Type.Contains("Сильное кровотечение") || injury.Type.Contains("Перелом") || injury.Type.Contains("Пневмоторакс") || injury.Type.Contains("Глубокая рана") || injury.Type.Contains("Контузия"))
		{
			return 2;
		}
		return 1;
	}

	private string Segment(string symbol, int severity)
	{
		if (1 == 0)
		{
		}
		string text = severity switch
		{
			2 => "#d63b3b", 
			1 => "#d8a531", 
			_ => "#6f6f6fb6", 
		};
		if (1 == 0)
		{
		}
		string text2 = text;
		return "<color=" + text2 + "><b>" + symbol + "</b></color>";
	}
}
