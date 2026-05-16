using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.Module.Reanimate;

public class ReanimationManager
{
	[CompilerGenerated]
	private sealed class ReanimateCoroutine_d_5 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Ragdoll ragdoll;

		public Config config;

		public ReanimationManager __4__this;

		private float elapsed;

		private Player target;

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
		public ReanimateCoroutine_d_5(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			target = null;
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
				target = ragdoll.Owner;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (elapsed < config.ReanimationTime)
			{
				if (!__4__this.IsReanimationActive(player, ragdoll, config.ReanimationRadiusSqr))
				{
					__4__this.ShowError(player, "Реанимация прервана: отошёл, умер или убрал аптечку!", 5f);
					AudioManager.DestroyForGameObject(ragdoll.GameObject);
					__4__this._activeReanimations.Remove(player.Id);
					Log.Debug("Реанимация прервана для " + player.Nickname + ".");
					return false;
				}
				__4__this.UpdateProgressUI(player, elapsed / config.ReanimationTime);
				elapsed += 0.5f;
				__2__current = Timing.WaitForSeconds(0.5f);
				__1__state = 1;
				return true;
			}
			__4__this.CompleteReanimation(player, ragdoll, target, config);
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

	private readonly Dictionary<int, (Ragdoll Ragdoll, string CustomInfo)> _activeReanimations = new Dictionary<int, (Ragdoll, string)>();

	public void TryStartReanimation(Player player, Config config)
	{
		if (!CanReanimate(player))
		{
			ShowError(player, "Для реанимации нужна аптечка!");
			return;
		}
		if (_activeReanimations.ContainsKey(player.Id))
		{
			ShowError(player, "Ты уже реанимируешь кого-то!");
			return;
		}
		Ragdoll val = FindClosestRagdoll(player, config.ReanimationRadiusSqr);
		if (val == null)
		{
			ShowError(player, "Поблизости нет тел для реанимации.");
		}
		else if (IsValidRagdoll(player, val))
		{
			float num = (float)(DateTime.Now - val.CreationTime).TotalSeconds;
			if (num > config.MaxReanimationDelay)
			{
				ShowError(player, "Это тело слишком давно мертво!");
				return;
			}
			_activeReanimations[player.Id] = (val, val.Owner.CustomInfo);
			Timing.RunCoroutine(ReanimateCoroutine(player, val, config));
			AudioManager.CreateForGameObject(val.GameObject, "ReanimateStart");
			ShowSuccess(player, "Начало реанимации... Держись рядом!");
		}
	}

	private bool CanReanimate(Player player)
	{
		int result;
		if (player != null && player.IsAlive && !player.IsScp)
		{
			Item currentItem = player.CurrentItem;
			result = ((currentItem != null && (int)currentItem.Type == 14) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private bool IsValidRagdoll(Player player, Ragdoll ragdoll)
	{
		Player owner = ragdoll.Owner;
		if (owner == (Player)null || owner == player)
		{
			ShowError(player, "Нельзя реанимировать себя.");
			return false;
		}
		if (owner.IsScp)
		{
			ShowError(player, "Нельзя реанимировать SCP.");
			return false;
		}
		if (_activeReanimations.ContainsValue((ragdoll, null)))
		{
			ShowError(player, "Это тело уже реанимируется!");
			return false;
		}
		return true;
	}

	private Ragdoll FindClosestRagdoll(Player player, float radiusSqr)
	{
		return (from r in Ragdoll.List
			where Vector3.SqrMagnitude(player.Position - r.Position) <= radiusSqr
			orderby Vector3.SqrMagnitude(player.Position - r.Position)
			select r).FirstOrDefault();
	}

	[IteratorStateMachine(typeof(ReanimateCoroutine_d_5))]
	private IEnumerator<float> ReanimateCoroutine(Player player, Ragdoll ragdoll, Config config)
	{
		return new ReanimateCoroutine_d_5(0)
		{
			__4__this = this,
			player = player,
			ragdoll = ragdoll,
			config = config
		};
	}

	private bool IsReanimationActive(Player player, Ragdoll ragdoll, float radiusSqr)
	{
		int result;
		if (player.IsAlive)
		{
			Item currentItem = player.CurrentItem;
			if (currentItem != null && (int)currentItem.Type == 14)
			{
				result = ((Vector3.SqrMagnitude(player.Position - ragdoll.Position) <= radiusSqr) ? 1 : 0);
				goto IL_003f;
			}
		}
		result = 0;
		goto IL_003f;
		IL_003f:
		return (byte)result != 0;
	}

	private void UpdateProgressUI(Player player, float progress)
	{
		int num = Mathf.FloorToInt(progress * 17f);
		string text = string.Concat("<color=black>", string.Concat(Enumerable.Repeat("<color=#78ffc792>⎯</color>", num)), string.Concat(Enumerable.Repeat("⎯", 17 - num)));
		string text2 = "<size=19><b><color=#ffffff6d>Реанимация игрока</color></b></size>\n<size=51><b>" + text + "</b></size>";
		try
		{
			player.ClearBroadcasts();
			player.Broadcast((ushort)1, text2, (BroadcastFlags)0, false);
			player.ShowHitMarker(1f);
		}
		catch (Exception ex)
		{
			Log.Error("Ошибка отображения интерфейса для " + player.Nickname + ": " + ex.Message);
		}
	}

	private void CompleteReanimation(Player player, Ragdoll ragdoll, Player target, Config config)
	{
		int id = player.Id;
		if (Random.value < config.SuccessChance && (int)target.Role.Team == 5)
		{
			string item = _activeReanimations[id].CustomInfo;
			target.Role.Set(ragdoll.Role, (RoleSpawnFlags)0);
			Timing.CallDelayed(0.7f, (Action)delegate
			{
				target.Position = player.Position;
				target.Health = target.MaxHealth * 0.3f;
				target.ClearInventory(true);
				player.RemoveItem(player.CurrentItem, true);
				ragdoll.Destroy();
				ShowSuccess(player, "Успешно реанимировал " + target.Nickname + "!", 5f);
				AudioManager.CreateForPlayer(target, "ReanimateEnd");
				ShowSuccess(target, "Тебя реанимировали!", 5f);
			});
			Log.Debug("Реанимация успешна для " + player.Nickname + " на " + target.Nickname + ".");
		}
		else
		{
			ShowError(player, "Реанимация не удалась!", 5f);
			AudioManager.CreateForGameObject(ragdoll.GameObject, "ReanimateGameOver");
			Log.Debug("Реанимация не удалась для " + player.Nickname + ".");
		}
		_activeReanimations.Remove(id);
	}

	private void ShowError(Player player, string message, float duration = 3f)
	{
		if (player == (Player)null)
		{
			return;
		}
		try
		{
			player.ShowMeowHint(duration, "<b><size=25><color=#d61c3ea4>⚠\ufe0f " + message + " ⚠\ufe0f</color></size></b>", (HintVerticalAlign)0, 775, 0, (HintAlignment)2);
		}
		catch (Exception ex)
		{
			Log.Error("Ошибка отображения ошибки для " + player.Nickname + ": " + ex.Message);
		}
	}

	private void ShowSuccess(Player player, string message, float duration = 3f)
	{
		if (player == (Player)null)
		{
			return;
		}
		try
		{
			player.ShowMeowHint(duration, "<b><size=25><color=#34e009a4>✔\ufe0f " + message + " ✔\ufe0f</color></size></b>", (HintVerticalAlign)0, 775, 0, (HintAlignment)2);
		}
		catch (Exception ex)
		{
			Log.Error("Ошибка отображения успеха для " + player.Nickname + ": " + ex.Message);
		}
	}

	public void Clear()
	{
		_activeReanimations.Clear();
	}
}
