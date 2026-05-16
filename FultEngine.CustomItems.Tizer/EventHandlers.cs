using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CustomEffectsAndItems.Effects.Camer;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.CustomItems.Tizer;

public class EventHandlers
{
	[CompilerGenerated]
	private sealed class PlayTizerAudio_d_9 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player target;

		public float duration;

		public EventHandlers __4__this;

		private float startTime;

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
		public PlayTizerAudio_d_9(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
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
				startTime = Time.time;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < startTime + duration && target != (Player)null)
			{
				try
				{
					AudioManager.CreateForPlayer(target, "Tizer", 1f, 7f, 100f);
				}
				catch (Exception ex)
				{
					ex = ex;
					Log.Error("Tazer: Ошибка при воспроизведении звука Tizer: " + ex.Message + "\n" + ex.StackTrace);
				}
				__2__current = Timing.WaitForSeconds(4.9f);
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

	private readonly Taizer _tazer;

	private readonly ConcurrentDictionary<Player, CoroutineHandle> _tazedPlayers = new ConcurrentDictionary<Player, CoroutineHandle>();

	public EventHandlers(Taizer tazer)
	{
		_tazer = tazer;
	}

	public void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
	}

	public void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)_tazer).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)_tazer).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnShot(ShotEventArgs ev)
	{
		if (!((CustomItem)_tazer).Check(ev.Player.CurrentItem) || ev.Player == (Player)null || ev.Target == (Player)null)
		{
			return;
		}
		ev.CanHurt = false;
		AudioManager.CreateForGameObject(ev.Player.GameObject, "FireTizer");
		Vector3 val = ev.Player.CameraTransform.position + ev.Player.CameraTransform.forward;
		Vector3 forward = ev.Player.CameraTransform.forward;
		Vector3 normalized = ((Vector3)(ref forward)).normalized;
		try
		{
			SchematicObject schematic = ObjectSpawner.SpawnSchematic("TizerElectric", val, (Quaternion?)Quaternion.LookRotation(normalized), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			Timing.CallDelayed(0.9f, (Action)delegate
			{
				SchematicObject obj = schematic;
				if (obj != null)
				{
					((MapEditorObject)obj).Destroy();
				}
			});
		}
		catch (Exception ex)
		{
			ev.Player.ShowHint("<color=red>Ошибка при спавне эффекта тазера</color>", 3f);
			Log.Error("Tazer: Ошибка при спавне схемы TizerElectric: " + ex.Message + "\n" + ex.StackTrace);
		}
		if ((int)ev.Target.Role.Team != 0 && !(Vector3.Distance(ev.Player.Position, ev.Target.Position) >= 9f) && !_tazedPlayers.ContainsKey(ev.Target))
		{
			ApplyTazerEffect(ev.Target);
		}
	}

	private void ApplyTazerEffect(Player target)
	{
		AudioManager.CreateForGameObject(target.GameObject, "Tizer");
		float num = ((Random.value < 0.33f) ? 5f : 15f);
		target.ShowMeowHint(num, "<size=29><b><color=#61616193>【</color></size> <size=23>Вас оглушили тайзером</size> <size=29><b><color=#61616193>】</color></size>", (HintVerticalAlign)0, 1000, 0, (HintAlignment)2);
		target.EnableEffect((EffectType)43, (byte)75, num, false);
		target.EnableEffect((EffectType)47, num, false);
		target.EnableEffect((EffectType)5, num, false);
		target.EnableEffect((EffectType)18, byte.MaxValue, num, false);
		CameraShake.ShakeCamera(target, num, 3f);
		CoroutineHandle value = Timing.RunCoroutine(PlayTizerAudio(target, num));
		_tazedPlayers[target] = value;
		Timing.CallDelayed(num, (Action)delegate
		{
			RestorePlayer(target);
		});
	}

	[IteratorStateMachine(typeof(PlayTizerAudio_d_9))]
	private IEnumerator<float> PlayTizerAudio(Player target, float duration)
	{
		return new PlayTizerAudio_d_9(0)
		{
			__4__this = this,
			target = target,
			duration = duration
		};
	}

	private void RestorePlayer(Player player)
	{
		if (!_tazedPlayers.TryGetValue(player, out var value))
		{
			return;
		}
		player.DisableEffect((EffectType)43);
		if (((CoroutineHandle)(ref value)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			try
			{
				AudioManager.DestroyForPlayer(player);
			}
			catch (Exception ex)
			{
				Log.Error("Tazer: Ошибка при остановке звука Tizer: " + ex.Message + "\n" + ex.StackTrace);
			}
		}
		_tazedPlayers.TryRemove(player, out var _);
	}

	private void OnDying(DyingEventArgs ev)
	{
		RestorePlayerIfTazed(ev.Player);
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		RestorePlayerIfTazed(ev.Player);
	}

	private void OnPlayerLeft(LeftEventArgs ev)
	{
		RestorePlayerIfTazed(((JoinedEventArgs)ev).Player);
	}

	private void RestorePlayerIfTazed(Player player)
	{
		if (_tazedPlayers.ContainsKey(player))
		{
			RestorePlayer(player);
		}
	}

	private void ShowItemHint(Player player)
	{
		CIMessage.SendMessage(player, "Тайзер");
	}
}
