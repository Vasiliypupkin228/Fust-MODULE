using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using UnityEngine;

namespace FultEngine.CustomItems.Tranq;

public class EventHandlers
{
	[CompilerGenerated]
	private sealed class DisplayTimerRoutine_d_11 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player p;

		public float totalDuration;

		public EventHandlers __4__this;

		private float remaining;

		private int minutes;

		private int seconds;

		private string timerText;

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
		public DisplayTimerRoutine_d_11(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			timerText = null;
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
				remaining = totalDuration;
				break;
			case 1:
				__1__state = -1;
				timerText = null;
				break;
			}
			if (remaining > 0f)
			{
				if (!__4__this._originalPositions.ContainsKey(p))
				{
					return false;
				}
				minutes = Mathf.FloorToInt(remaining / 60f);
				seconds = Mathf.FloorToInt(remaining % 60f);
				timerText = $"<size=19>До возвращения: {minutes:00}:{seconds:00}</size>";
				p.ShowMeowHint(1f, "<size=29><b><color=#61616193>|</color></size> <size=19>Вы упали под действием транквилизатора</size> <size=29><color=#61616193>|</color></b></size>\n" + timerText, (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
				remaining -= 1f;
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
	private sealed class ReturnFromTranq_d_12 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player p;

		public Ragdoll rag;

		public EventHandlers __4__this;

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
		public ReturnFromTranq_d_12(int __1__state)
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
				__2__current = Timing.WaitForSeconds(350f);
				__1__state = 1;
				return true;
			case 1:
			{
				__1__state = -1;
				if (!__4__this._originalPositions.ContainsKey(p))
				{
					return false;
				}
				Ragdoll obj = rag;
				if ((Object)(object)((obj != null) ? obj.GameObject : null) != (Object)null)
				{
					Object.Destroy((Object)(object)rag.GameObject);
				}
				p.Position = __4__this._originalPositions[p];
				__4__this.CleanupPlayerState(p);
				return false;
			}
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

	private readonly Tranquilizer _tranq;

	private readonly Dictionary<Player, Vector3> _originalPositions = new Dictionary<Player, Vector3>();

	private readonly Dictionary<Player, CoroutineHandle> _teleportCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> _timerCoroutines = new Dictionary<Player, CoroutineHandle>();

	public EventHandlers(Tranquilizer tranq)
	{
		_tranq = tranq;
	}

	public void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
	}

	public void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)_tranq).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)_tranq).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void ShowItemHint(Player player)
	{
		CIMessage.SendMessage(player, "Транквилизатор");
	}

	private void OnShot(ShotEventArgs ev)
	{
		if (!((CustomItem)_tranq).Check(ev.Player.CurrentItem) || ev.Target == (Player)null)
		{
			return;
		}
		ev.CanHurt = false;
		if ((int)ev.Target.Role.Team == 0 || Vector3.Distance(ev.Player.Position, ev.Target.Position) > 12f)
		{
			return;
		}
		Player target = ev.Target;
		if (!_originalPositions.ContainsKey(target))
		{
			float num = Random.Range(8f, 15f);
			target.EnableEffect((EffectType)26, num, false);
			target.EnableEffect((EffectType)42, (byte)5, num + 2f, false);
			Timing.CallDelayed(9f, (Action)delegate
			{
				target.EnableEffect((EffectType)20, 0f, false);
				target.EnableEffect((EffectType)12, (byte)1, 0f, false);
				target.DropItems();
				Ragdoll rag = Ragdoll.CreateAndSpawn(target.Role.Type, target.DisplayNickname, "Упал под действием транквилизатора", target.Position, (Quaternion?)null, (Player)null);
				_originalPositions[target] = target.Position;
				target.Position = new Vector3(162.057f, 319.465f, -12.904f);
				_timerCoroutines[target] = Timing.RunCoroutine(DisplayTimerRoutine(target, 60f));
				_teleportCoroutines[target] = Timing.RunCoroutine(ReturnFromTranq(target, rag));
			});
			Timing.CallDelayed(num, (Action)delegate
			{
				WakeUp(target);
			});
		}
	}

	[IteratorStateMachine(typeof(DisplayTimerRoutine_d_11))]
	private IEnumerator<float> DisplayTimerRoutine(Player p, float totalDuration)
	{
		return new DisplayTimerRoutine_d_11(0)
		{
			__4__this = this,
			p = p,
			totalDuration = totalDuration
		};
	}

	[IteratorStateMachine(typeof(ReturnFromTranq_d_12))]
	private IEnumerator<float> ReturnFromTranq(Player p, Ragdoll rag)
	{
		return new ReturnFromTranq_d_12(0)
		{
			__4__this = this,
			p = p,
			rag = rag
		};
	}

	private void WakeUp(Player p)
	{
		CleanupPlayerState(p);
	}

	private void CleanupPlayerState(Player p)
	{
		if (_originalPositions.ContainsKey(p))
		{
			if (_timerCoroutines.TryGetValue(p, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_timerCoroutines.Remove(p);
			}
			if (_teleportCoroutines.TryGetValue(p, out var value2))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
				_teleportCoroutines.Remove(p);
			}
			p.DisableEffect((EffectType)42);
			p.DisableEffect((EffectType)26);
			p.DisableEffect((EffectType)20);
			p.DisableEffect((EffectType)12);
			_originalPositions.Remove(p);
		}
	}

	private void OnDying(DyingEventArgs ev)
	{
		CleanupPlayerState(ev.Player);
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		CleanupPlayerState(ev.Player);
	}

	private void OnLeft(LeftEventArgs ev)
	{
		CleanupPlayerState(((JoinedEventArgs)ev).Player);
	}
}
