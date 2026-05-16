using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.Cassie;
using MEC;
using UnityEngine;

namespace FultEngine.Module;

public class HackManager
{
	[CompilerGenerated]
	private sealed class HackingCoroutine_d_11 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Door door;

		public bool isScpDoor;

		public ZoneType zone;

		public Vector3 initialPos;

		public int itemSerial;

		public HackManager __4__this;

		private float duration;

		private float elapsed;

		private string error;

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
		public HackingCoroutine_d_11(int __1__state)
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
				duration = (isScpDoor ? 25f : 12f);
				elapsed = 0f;
				break;
			case 1:
				__1__state = -1;
				error = null;
				break;
			}
			if (elapsed < duration)
			{
				if (!__4__this.ValidateHackingState(player, door, initialPos, 1.8f, out error))
				{
					__4__this._hackingUI.ShowMessage(player, "Взлом прерван: " + error);
					__4__this.CleanupHacking(player);
					return false;
				}
				elapsed += 1f;
				__4__this._hackingUI.UpdateHackingUI(player, Mathf.Clamp(Mathf.FloorToInt(elapsed / duration * 17f), 0, 17), 18);
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			}
			__4__this.CompleteHacking(player, door, isScpDoor, zone, itemSerial);
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

	private readonly Dictionary<int, Dictionary<ZoneType, int>> _doorsHackedPerZone = new Dictionary<int, Dictionary<ZoneType, int>>();

	private readonly Dictionary<int, Dictionary<ZoneType, int>> _requiredDoorsPerZone = new Dictionary<int, Dictionary<ZoneType, int>>();

	private readonly Dictionary<Player, CoroutineHandle> _activeHacks = new Dictionary<Player, CoroutineHandle>();

	private readonly HashSet<string> _cassieMessagesSent = new HashSet<string>();

	private readonly HackingUI _hackingUI = new HackingUI();

	public void HandleDoorInteraction(Player player, Door door, int itemSerial)
	{
		if (_activeHacks.ContainsKey(player))
		{
			_hackingUI.ShowMessage(player, "Вы уже взламываете другие двери!");
			return;
		}
		ZoneType zoneType = GetZoneType(door);
		if (!HasZoneAccess(itemSerial, zoneType))
		{
			AudioManager.CreateForPlayer(player, "hackDoor");
			StartHackingProcess(player, door, isScpDoor: false, zoneType, itemSerial);
		}
	}

	public void CleanupPlayer(Player player)
	{
		if (_activeHacks.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_activeHacks.Remove(player);
		}
	}

	public void CleanupAllHacks()
	{
		foreach (CoroutineHandle value in _activeHacks.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		_activeHacks.Clear();
		_doorsHackedPerZone.Clear();
		_requiredDoorsPerZone.Clear();
		_cassieMessagesSent.Clear();
	}

	private ZoneType GetZoneType(Door door)
	{
		Vector3 position = door.Position;
		if ((Object)(object)door.Room != (Object)null)
		{
			return door.Room.Zone;
		}
		if (position.y > 900f)
		{
			return (ZoneType)2;
		}
		if (position.y > 0f)
		{
			return (ZoneType)1;
		}
		return (ZoneType)4;
	}

	public bool HasZoneAccess(int itemSerial, ZoneType zone)
	{
		if (!_doorsHackedPerZone.ContainsKey(itemSerial))
		{
			_doorsHackedPerZone[itemSerial] = new Dictionary<ZoneType, int>();
			_requiredDoorsPerZone[itemSerial] = new Dictionary<ZoneType, int>();
		}
		if (!_doorsHackedPerZone[itemSerial].ContainsKey(zone))
		{
			_doorsHackedPerZone[itemSerial][zone] = 0;
			_requiredDoorsPerZone[itemSerial][zone] = 1;
		}
		return _doorsHackedPerZone[itemSerial][zone] >= _requiredDoorsPerZone[itemSerial][zone];
	}

	public void StartHackingProcess(Player player, Door door, bool isScpDoor, ZoneType zone, int itemSerial)
	{
		if (!_activeHacks.ContainsKey(player))
		{
			Vector3 position = player.Position;
			AudioManager.CreateForPlayer(player, "hackProcess");
			_activeHacks[player] = Timing.RunCoroutine(HackingCoroutine(player, door, isScpDoor, zone, position, itemSerial));
		}
	}

	[IteratorStateMachine(typeof(HackingCoroutine_d_11))]
	private IEnumerator<float> HackingCoroutine(Player player, Door door, bool isScpDoor, ZoneType zone, Vector3 initialPos, int itemSerial)
	{
		return new HackingCoroutine_d_11(0)
		{
			__4__this = this,
			player = player,
			door = door,
			isScpDoor = isScpDoor,
			zone = zone,
			initialPos = initialPos,
			itemSerial = itemSerial
		};
	}

	private bool ValidateHackingState(Player player, Door door, Vector3 initialPos, float tolerance, out string error)
	{
		if (player == (Player)null || !player.IsAlive)
		{
			error = "Игрок недоступен!";
			return false;
		}
		if (Vector3.Distance(player.Position, initialPos) > tolerance)
		{
			AudioManager.DestroyForPlayer(player);
			error = "Вы отошли слишком далеко!";
			return false;
		}
		if (player.CurrentItem == null || (int)player.CurrentItem.Type != 10)
		{
			AudioManager.DestroyForPlayer(player);
			error = "Вы не держите устройство взлома в руках!";
			return false;
		}
		if (door == null)
		{
			AudioManager.DestroyForPlayer(player);
			error = "Дверь недоступна!";
			return false;
		}
		error = null;
		return true;
	}

	private void CompleteHacking(Player player, Door door, bool isScpDoor, ZoneType zone, int itemSerial)
	{
		_doorsHackedPerZone[itemSerial][zone]++;
		door.IsOpen = true;
		int num = _requiredDoorsPerZone[itemSerial][zone] - _doorsHackedPerZone[itemSerial][zone];
		if ((int)door.Type == 23 && !_cassieMessagesSent.Contains("GateA"))
		{
			CassieRussianHelper.Message("<b><size=21><color=#006d8f>A.R.E.S.S</color> | <color=#ba1818>[Аварийная тревога]</color></size></b>\n<split><b><size=20>Зафиксирован несанкционированный доступ к Гермоворотам [<color=#7f0e16>Альфа</color>] неизвестным устройством.</size></b>\n<split><b><size=20>На территории Комплекса активирован код [<color=#727472>Серый</color>].</size></b>\n<split><b><size=20><color=#727472>Службе Безопасности</color>: приоритетная задача изменена на <color=#ba1818>нейтрализацию злоумышленников</color>.</size></b>", "pitch_0.27 .g4 pitch_0.25 .g4 . pitch_0.92 emergency alarm . detected unauthorized access to gate alpha with unknown software . pitch_0.92 .g5 .g4 attention . on site activated code grey . .g5 .g2 security service . your priority task has been changed to neutralize unauthorized intruders . pitch_0.2 .g4 pitch_0.15 .g4", isHeld: false, isNoisy: false);
			_cassieMessagesSent.Add("GateA");
		}
		else if ((int)door.Type == 24 && !_cassieMessagesSent.Contains("GateB"))
		{
			CassieRussianHelper.Message("<b><size=21><color=#006d8f>A.R.E.S.S</color> | <color=#ba1818>[Аварийная тревога]</color></size></b>\n<split><b><size=20>Зафиксирован несанкционированный доступ к Гермоворотам [<color=green>Браво</color>] неизвестным устройством.</size></b>\n<split><b><size=20>На территории Комплекса активирован код [<color=#727472>Серый</color>].</size></b>\n<split><b><size=20><color=#727472>Службе Безопасности</color>: приоритетная задача изменена на <color=#ba1818>нейтрализацию злоумышленников</color>.</size></b>", "pitch_0.27 .g4 pitch_0.25 .g4 . pitch_0.92 emergency alarm . detected unauthorized access to gate bravo with unknown software . pitch_0.92 .g5 .g4 attention . on site activated code grey . .g5 .g2 security service . your priority task has been changed to neutralize unauthorized intruders . pitch_0.2 .g4 pitch_0.15 .g4", isHeld: false, isNoisy: false);
			_cassieMessagesSent.Add("GateB");
		}
		if (num > 0)
		{
			_hackingUI.ShowMessage(player, $"Дверь взломана! Осталось взломать {num} дверей в зоне {zone}.");
		}
		else
		{
			AudioManager.DestroyForPlayer(player);
			AudioManager.CreateForPlayer(player, "hackWIN");
			string item = $"Zone_{zone}";
			if ((int)door.Type != 23 && (int)door.Type != 24 && !_cassieMessagesSent.Contains(item) && SendZoneCassie(zone))
			{
				_cassieMessagesSent.Add(item);
			}
			if ((int)zone == 4)
			{
				_hackingUI.ShowMessage(player, "Доступ к Административной зоне получен!");
			}
			else if ((int)zone == 8)
			{
				_hackingUI.ShowMessage(player, "Доступ к Наземной зоне получен!");
			}
			else if ((int)zone == 2)
			{
				_hackingUI.ShowMessage(player, "Доступ к Тяжёлой зоне содержания получен!");
			}
			else if ((int)zone == 1)
			{
				_hackingUI.ShowMessage(player, "Доступ к Лёгкой зоне содержания получен!");
			}
		}
		CleanupHacking(player);
	}

	private bool SendZoneCassie(ZoneType zone)
	{
		switch (zone - 1)
		{
		case 0:
			CassieRussianHelper.Message("<b><size=20><color=#8B8B05>ВНИМАНИЕ</color> | Всему персоналу:</size></b>\n<split><b><size=19><color=#556B2F>Несанкционированный персонал</color> обнаружен в «<color=blue>Лёгкой зоне содержания</color>».</size></b>", "attention . unauthorized personnel detected in light containment zone .", isHeld: false, isNoisy: false);
			return true;
		case 1:
			CassieRussianHelper.Message("<b><size=20><color=#8B8B05>ВНИМАНИЕ</color> | Всему персоналу:</size></b>\n<split><b><size=19><color=#556B2F>Несанкционированный персонал</color> обнаружен в «<color=red>Тяжёлой зоне содержания</color>».</size></b>", "attention . unauthorized personnel detected in heavy containment zone .", isHeld: false, isNoisy: false);
			return true;
		case 3:
			CassieRussianHelper.Message("<b><size=20><color=#8B8B05>ВНИМАНИЕ</color> | Всему персоналу:</size></b>\n<split><b><size=19><color=#556B2F>Несанкционированный персонал</color> обнаружен в «<color=yellow>Административной зоне</color>».</size></b>", "attention . unauthorized personnel detected in entrance zone .", isHeld: false, isNoisy: false);
			return true;
		default:
			return false;
		}
	}

	private void CleanupHacking(Player player)
	{
		if (_activeHacks.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_activeHacks.Remove(player);
		}
	}
}
