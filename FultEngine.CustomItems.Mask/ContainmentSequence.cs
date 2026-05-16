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
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.CustomItems.Mask;

public class ContainmentSequence
{
	[CompilerGenerated]
	private sealed class ContainmentProcess_d_3 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Item item;

		public Player scp096;

		public Vector3 initialPosition;

		public ContainmentSequence __4__this;

		private float totalDuration;

		private int segment;

		private string error;

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
		public ContainmentProcess_d_3(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			error = null;
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
				totalDuration = 15f;
				segment = 0;
				break;
			case 1:
				__1__state = -1;
				error = null;
				segment++;
				break;
			}
			if (segment < 15)
			{
				if (!__4__this.ValidateContainment(player, item, scp096, initialPosition, 1.5f, out error))
				{
					AudioManager.DestroyForGameObject(scp096.GameObject);
					__4__this.ShowContainmentMessage(player, "Прервано: <color=red>" + error + "</color>", 3f);
					__4__this.CleanupContainment(player);
					return false;
				}
				progress = (float)(segment + 1) / 15f;
				__4__this.UpdateContainmentUI(player, segment, 15);
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			}
			__4__this.CompleteContainment(player, scp096, item);
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
	private sealed class RunContainmentSequence_d_2 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Item item;

		public ContainmentSequence __4__this;

		private Player scp096;

		private Vector3 initialPosition;

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
		public RunContainmentSequence_d_2(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			scp096 = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			if (__1__state != 0)
			{
				return false;
			}
			__1__state = -1;
			if (player == (Player)null || item == null)
			{
				return false;
			}
			scp096 = ((IEnumerable<Player>)Player.List).FirstOrDefault((Func<Player, bool>)((Player p) => p.Role == (RoleTypeId)9));
			if (scp096 == (Player)null)
			{
				__4__this.CleanupContainment(player);
				__4__this.ShowContainmentMessage(player, "SCP-096 Не найден", 3f);
				return false;
			}
			if (__4__this._scp096States.ContainsKey(scp096) && __4__this._scp096States[scp096].IsContained)
			{
				__4__this.CleanupContainment(player);
				__4__this.ShowContainmentMessage(player, "SCP-096 уже сдержан!", 3f);
				return false;
			}
			if (__4__this._activeContainments.ContainsKey(player))
			{
				__4__this.CleanupContainment(player);
				__4__this.ShowContainmentMessage(player, "Сдерживание уже запущено!", 3f);
				return false;
			}
			AudioManager.CreateForGameObject(scp096.GameObject, "bag", 5f, 9f);
			initialPosition = player.Position;
			__4__this._activeContainments[player] = Timing.RunCoroutine(__4__this.ContainmentProcess(player, item, scp096, initialPosition));
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

	private readonly Dictionary<Player, CoroutineHandle> _activeContainments = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, (bool IsContained, SchematicObject Mask)> _scp096States = new Dictionary<Player, (bool, SchematicObject)>();

	[IteratorStateMachine(typeof(RunContainmentSequence_d_2))]
	public IEnumerator<float> RunContainmentSequence(Player player, Item item)
	{
		return new RunContainmentSequence_d_2(0)
		{
			__4__this = this,
			player = player,
			item = item
		};
	}

	[IteratorStateMachine(typeof(ContainmentProcess_d_3))]
	private IEnumerator<float> ContainmentProcess(Player player, Item item, Player scp096, Vector3 initialPosition)
	{
		return new ContainmentProcess_d_3(0)
		{
			__4__this = this,
			player = player,
			item = item,
			scp096 = scp096,
			initialPosition = initialPosition
		};
	}

	private bool ValidateContainment(Player player, Item item, Player scp096, Vector3 initialPos, float tolerance, out string error)
	{
		if (!player.IsAlive)
		{
			CleanupContainment(player);
			error = "Смерть оператора!";
			return false;
		}
		if (player.CurrentItem != item)
		{
			CleanupContainment(player);
			error = "Предмет в руках не найден!";
			return false;
		}
		if (Vector3.Distance(player.Position, initialPos) > tolerance)
		{
			CleanupContainment(player);
			error = "Нарушение позиции!";
			return false;
		}
		if (scp096 == (Player)null || Vector3.Distance(player.Position, scp096.Position) > 3.5f)
		{
			CleanupContainment(player);
			error = "Нарушение позиции!";
			return false;
		}
		error = null;
		return true;
	}

	public void UpdateContainmentUI(Player player, int currentSegment, int totalSegments)
	{
		int num = 18;
		int num2 = Mathf.RoundToInt((float)(currentSegment + 1) / (float)totalSegments * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt((float)(currentSegment + 1) / (float)totalSegments * 100f);
		string arg = "<color=#ffffff>" + new string('▒', num2) + "</color><color=#616161>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>Сдерживание 096</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>Прогресс: {num3}%</size> <size=29><color=#61616193>|</color></b></size>";
		try
		{
			player.ShowMeowHint(1f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
		}
		catch (Exception ex)
		{
			Log.Error("ContainmentUI: Ошибка при показе подсказки игроку " + player.Nickname + ": " + ex.Message);
		}
	}

	private void CompleteContainment(Player player, Player scp096, Item item)
	{
		SchematicObject val = ObjectSpawner.SpawnSchematic("Mask", scp096.Position + new Vector3(0f, 0.935f, 0.211f), (Quaternion?)Quaternion.Euler(scp096.Transform.eulerAngles), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		((Component)val).transform.parent = scp096.Transform;
		_scp096States[scp096] = (true, val);
		AudioManager.DestroyForGameObject(scp096.GameObject);
		player.RemoveItem(item, true);
		ShowContainmentMessage(player, "SCP-096 Успешно сдержан", 5f);
		CleanupContainment(player);
	}

	public void ShowContainmentMessage(Player player, string message, float duration)
	{
		if (player == (Player)null)
		{
			return;
		}
		try
		{
			player.ShowMeowHint(duration, "<size=29><b><color=#61616193>|</color></size> <size=19>" + message + "</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
		}
		catch (Exception ex)
		{
			Log.Error("HintDisplay: Ошибка при показе подсказки игроку " + player.Nickname + ": " + ex.Message);
		}
	}

	private void CleanupContainment(Player player)
	{
		if (_activeContainments.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_activeContainments.Remove(player);
		}
	}

	public void CancelContainment(Player player)
	{
		CleanupContainment(player);
	}

	public void ClearState(Player scp096)
	{
		if (_scp096States.TryGetValue(scp096, out (bool, SchematicObject) value) && (Object)(object)value.Item2 != (Object)null)
		{
			((MapEditorObject)value.Item2).Destroy();
		}
		_scp096States.Remove(scp096);
	}

	public bool IsScp096Contained(Player scp096)
	{
		(bool, SchematicObject) value;
		return _scp096States.TryGetValue(scp096, out value) && value.Item1;
	}
}
