using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using MapEditorReborn.API;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.CustomItems.Cell;

public class ContainmentSequence
{
	[CompilerGenerated]
	private sealed class __c__DisplayClass8_0
	{
		public Player scp173;

		internal bool RunContainmentSequence_b_1(Player p)
		{
			return p.IsAlive && (Object)(object)p.CurrentRoom == (Object)(object)scp173.CurrentRoom && (int)p.Role.Team != 5 && (int)p.Role.Team > 0;
		}
	}

	[CompilerGenerated]
	private sealed class ContainmentProcess_d_9 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Item item;

		public Player scp173;

		public Vector3 initialPosition;

		public ContainmentSequence __4__this;

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
		public ContainmentProcess_d_9(int __1__state)
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
				__4__this._cageSchematic = ObjectSpawner.SpawnSchematic("173cage", scp173.Position + new Vector3(0f, 0.4f, 0f), (Quaternion?)Quaternion.Euler(scp173.Transform.eulerAngles), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
				if ((Object)(object)__4__this._cageSchematic == (Object)null)
				{
					__4__this.ShowContainmentMessage(player, "Ошибка: не удалось создать клетку!", 3f);
					AudioManager.DestroyForGameObject(scp173.GameObject);
					__4__this.CleanupContainment(player);
					return false;
				}
				__4__this._cageHealth = 551f;
				__4__this._scp173Cell = true;
				segment = 0;
				break;
			case 1:
				__1__state = -1;
				error = null;
				segment++;
				break;
			}
			if (segment < 25)
			{
				if (!__4__this.ValidateContainment(player, item, initialPosition, 1.5f, out error))
				{
					AudioManager.DestroyForGameObject(scp173.GameObject);
					__4__this._scp173Cell = false;
					if ((Object)(object)__4__this._cageSchematic != (Object)null)
					{
						((MapEditorObject)__4__this._cageSchematic).Destroy();
					}
					__4__this.ShowContainmentMessage(player, "Прервано: <color=red>" + error + "</color>", 3f);
					__4__this.CleanupContainment(player);
					return false;
				}
				progress = (float)(segment + 1) / 25f;
				__4__this.ShowProgress(player, scp173, progress);
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			}
			__4__this.CompleteContainment(player, scp173, item);
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
	private sealed class RunContainmentSequence_d_8 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Item item;

		public ContainmentSequence __4__this;

		private __c__DisplayClass8_0 __8__1;

		private List<Player> playersInRoom;

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
		public RunContainmentSequence_d_8(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__8__1 = null;
			playersInRoom = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			if (__1__state != 0)
			{
				return false;
			}
			__1__state = -1;
			__8__1 = new __c__DisplayClass8_0();
			if (player == (Player)null || item == null)
			{
				__4__this.CleanupContainment(player);
				return false;
			}
			if (__4__this._activeContainments.ContainsKey(player))
			{
				__4__this.ShowContainmentMessage(player, "Сдерживание уже запущено!", 3f);
				__4__this.CleanupContainment(player);
				return false;
			}
			__8__1.scp173 = ((IEnumerable<Player>)Player.List).FirstOrDefault((Func<Player, bool>)((Player p) => p.Role == (RoleTypeId)0));
			if (__8__1.scp173 == (Player)null)
			{
				__4__this.CleanupContainment(player);
				__4__this.ShowContainmentMessage(player, "SCP-173 не найден", 3f);
				return false;
			}
			playersInRoom = Player.List.Where((Player p) => p.IsAlive && (Object)(object)p.CurrentRoom == (Object)(object)__8__1.scp173.CurrentRoom && (int)p.Role.Team != 5 && (int)p.Role.Team > 0).ToList();
			if (playersInRoom.Count < 2)
			{
				__4__this.CleanupContainment(player);
				__4__this.ShowContainmentMessage(player, "Для сдерживания нужно 2 или более игроков в комнате!", 3f);
				return false;
			}
			AudioManager.CreateForGameObject(__8__1.scp173.GameObject, "cell", 5f, 9f);
			initialPosition = player.Position;
			__4__this._activeContainments[player] = Timing.RunCoroutine(__4__this.ContainmentProcess(player, item, __8__1.scp173, initialPosition));
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
	private sealed class TeleportCageCoroutine_d_21 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public SchematicObject schematic;

		public Player player;

		public ContainmentSequence __4__this;

		private Vector3 localPosition;

		private Quaternion localRotation;

		private bool isPositionClear;

		private Vector3 targetWorldPosition;

		private Vector3 currentWorldPosition;

		private IEnumerator<Player> __s__6;

		private Player p;

		private Collider[] colliders;

		private Collider[] __s__9;

		private int __s__10;

		private Collider collider;

		private Vector3 targetLocalPosition;

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
		public TeleportCageCoroutine_d_21(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__6 = null;
			p = null;
			colliders = null;
			__s__9 = null;
			collider = null;
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
				((Component)schematic).transform.SetParent(player.Transform);
				localPosition = new Vector3(0f, 0f, 3f);
				localRotation = Quaternion.identity;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if ((Object)(object)schematic != (Object)null && __4__this._followingPlayer == player && player.IsAlive)
			{
				isPositionClear = true;
				targetWorldPosition = player.Transform.TransformPoint(localPosition);
				currentWorldPosition = ((Component)schematic).transform.position;
				targetWorldPosition.y = currentWorldPosition.y;
				__s__6 = Player.List.GetEnumerator();
				try
				{
					while (__s__6.MoveNext())
					{
						p = __s__6.Current;
						if (p != player && Vector3.Distance(p.Position, targetWorldPosition) < 1f)
						{
							isPositionClear = false;
							break;
						}
						p = null;
					}
				}
				finally
				{
					if (__s__6 != null)
					{
						__s__6.Dispose();
					}
				}
				__s__6 = null;
				if (isPositionClear)
				{
					colliders = Physics.OverlapSphere(targetWorldPosition, 1f, LayerMask.GetMask(new string[2] { "Default", "Interactable" }));
					__s__9 = colliders;
					for (__s__10 = 0; __s__10 < __s__9.Length; __s__10++)
					{
						collider = __s__9[__s__10];
						if ((Object)(object)((Component)collider).gameObject != (Object)(object)player.GameObject && (Object)(object)((Component)collider).gameObject != (Object)(object)((Component)schematic).gameObject)
						{
							isPositionClear = false;
							break;
						}
						collider = null;
					}
					__s__9 = null;
					colliders = null;
				}
				if (isPositionClear)
				{
					targetLocalPosition = new Vector3(localPosition.x, ((Component)schematic).transform.localPosition.y, localPosition.z);
					((Component)schematic).transform.localPosition = Vector3.Lerp(((Component)schematic).transform.localPosition, targetLocalPosition, 0.2f);
					((Component)schematic).transform.localRotation = localRotation;
				}
				__2__current = Timing.WaitForSeconds(0.1f);
				__1__state = 1;
				return true;
			}
			((Component)schematic).transform.SetParent((Transform)null);
			__4__this.StopTeleporting(schematic);
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

	public bool _scp173Cell;

	private readonly Dictionary<Player, CoroutineHandle> _activeContainments = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<SchematicObject, CoroutineHandle> _teleportCoroutines = new Dictionary<SchematicObject, CoroutineHandle>();

	private float _cageHealth = 551f;

	private SchematicObject _cageSchematic;

	private Player _followingPlayer;

	private static bool TryGetSchematic(string schematicName, out SchematicObject schematic)
	{
		schematic = API.SpawnedObjects.OfType<SchematicObject>().FirstOrDefault((Func<SchematicObject, bool>)((SchematicObject s) => s.Name.Equals(schematicName, StringComparison.OrdinalIgnoreCase)));
		return (Object)(object)schematic != (Object)null;
	}

	private void CleanupAllCages()
	{
		foreach (SchematicObject item in from s in API.SpawnedObjects.OfType<SchematicObject>()
			where s.Name.Equals("173cage", StringComparison.OrdinalIgnoreCase)
			select s)
		{
			if (_teleportCoroutines.TryGetValue(item, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_teleportCoroutines.Remove(item);
			}
			((MapEditorObject)item).Destroy();
		}
		_cageSchematic = null;
		_scp173Cell = false;
	}

	[IteratorStateMachine(typeof(RunContainmentSequence_d_8))]
	public IEnumerator<float> RunContainmentSequence(Player player, Item item)
	{
		return new RunContainmentSequence_d_8(0)
		{
			__4__this = this,
			player = player,
			item = item
		};
	}

	[IteratorStateMachine(typeof(ContainmentProcess_d_9))]
	private IEnumerator<float> ContainmentProcess(Player player, Item item, Player scp173, Vector3 initialPosition)
	{
		return new ContainmentProcess_d_9(0)
		{
			__4__this = this,
			player = player,
			item = item,
			scp173 = scp173,
			initialPosition = initialPosition
		};
	}

	private bool ValidateContainment(Player player, Item item, Vector3 initialPos, float tolerance, out string error)
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
		Player val = ((IEnumerable<Player>)Player.List).FirstOrDefault((Func<Player, bool>)((Player p) => p.Role == (RoleTypeId)0));
		if (val != (Player)null && (double)Vector3.Distance(player.Position, val.Position) > 1.9)
		{
			CleanupContainment(player);
			error = "Нарушение позиции!";
			return false;
		}
		List<Player> list = Player.List.Where((Player p) => p.IsAlive && (Object)(object)p.CurrentRoom == (Object)(object)player.CurrentRoom && (int)p.Role.Team != 5 && (int)p.Role.Team > 0).ToList();
		if (list.Count < 2)
		{
			CleanupContainment(player);
			error = "Для сдерживания нужно 2 или более игроков в комнате!";
			return false;
		}
		error = null;
		return true;
	}

	private void ShowProgress(Player player, Player target, float progress)
	{
		int num = 18;
		int num2 = Mathf.RoundToInt(progress * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt(progress * 100f);
		string arg = "<color=#ffffff>" + new string('▒', num2) + "</color><color=#616161>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>Сдерживание 173</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>Прогресс: {num3}%</size> <size=29><color=#61616193>|</color></b></size>";
		player.ShowMeowHint(1f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}

	private void CompleteContainment(Player player, Player scp173, Item item)
	{
		_scp173Cell = false;
		AudioManager.DestroyForGameObject(scp173.GameObject);
		scp173.Role.Set((RoleTypeId)2, (SpawnReason)7);
		player.RemoveItem(item, true);
		ShowContainmentMessage(player, "SCP-173 успешно сдержан", 5f);
		_followingPlayer = player;
		_teleportCoroutines[_cageSchematic] = Timing.RunCoroutine(TeleportCageCoroutine(_cageSchematic, player));
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
		StopTeleporting(_cageSchematic);
	}

	public void DamageCage(float damage, Player shooter)
	{
		if ((Object)(object)_cageSchematic == (Object)null)
		{
			return;
		}
		List<Player> source = Player.List.Where((Player p) => p.Role == (RoleTypeId)2).ToList();
		if (!source.Any())
		{
			ShowContainmentMessage(shooter, "Клетка не может быть разрушена: нет наблюдателей!", 3f);
			return;
		}
		_cageHealth -= damage;
		if (_cageHealth <= 0f)
		{
			BreakCage();
		}
		else
		{
			shooter.ShowHitMarker(1f);
		}
	}

	public void HandleGrenadeExplosion(Vector3 explosionPosition, Player thrower)
	{
		if (!((Object)(object)_cageSchematic == (Object)null))
		{
			List<Player> source = Player.List.Where((Player p) => p.Role == (RoleTypeId)2).ToList();
			if (!source.Any())
			{
				ShowContainmentMessage(thrower, "Клетка не может быть разрушена: нет наблюдателей!", 3f);
			}
			else
			{
				BreakCage();
			}
		}
	}

	private void BreakCage()
	{
		if ((Object)(object)_cageSchematic == (Object)null)
		{
			return;
		}
		List<Player> list = Player.List.Where((Player p) => p.Role == (RoleTypeId)2).ToList();
		if (list.Any())
		{
			Vector3 position = ((MapEditorObject)_cageSchematic).Position;
			Player val = list[Random.Range(0, list.Count)];
			val.Role.Set((RoleTypeId)0, (SpawnReason)7);
			val.Position = position;
			((MapEditorObject)_cageSchematic).Destroy();
			_cageSchematic = null;
			_scp173Cell = false;
			SchematicObject val2 = ObjectSpawner.SpawnSchematic("173cage_destroyed", val.Position + new Vector3(0f, 0.4f, 0f), (Quaternion?)Quaternion.Euler(val.Transform.eulerAngles), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			if ((Object)(object)val2 != (Object)null)
			{
				AudioManager.CreateForGameObject(((Component)val2).gameObject, "destroy", 9f, 17f, 155f);
			}
		}
	}

	public void CheckCageUnloaded(Player scp173)
	{
		if (!((Object)(object)_cageSchematic == (Object)null) || !_scp173Cell)
		{
			return;
		}
		List<Player> list = Player.List.Where((Player p) => p.Role == (RoleTypeId)2).ToList();
		if (list.Any())
		{
			Vector3 position = scp173.Position;
			Player val = list[Random.Range(0, list.Count)];
			val.Role.Set((RoleTypeId)0, (SpawnReason)7);
			val.Position = position;
			_scp173Cell = false;
			SchematicObject val2 = ObjectSpawner.SpawnSchematic("173cage_destroyed", val.Position + new Vector3(0f, 0.4f, 0f), (Quaternion?)Quaternion.Euler(val.Transform.eulerAngles), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			if ((Object)(object)val2 != (Object)null)
			{
				AudioManager.CreateForGameObject(((Component)val2).gameObject, "destroy", 9f, 17f, 155f);
			}
		}
	}

	public void OnKeybindPressed(Player player, int keybindId)
	{
		if ((Object)(object)_cageSchematic == (Object)null || !player.IsAlive || player.IsScp || keybindId != 31)
		{
			return;
		}
		if (_followingPlayer == player)
		{
			StopTeleporting(_cageSchematic);
			ShowContainmentMessage(player, "Клетка больше не следует за вами!", 3f);
			return;
		}
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(player.CameraTransform.position, player.CameraTransform.forward);
		RaycastHit val2 = default(RaycastHit);
		if (!Physics.Raycast(val, ref val2, 2f, LayerMask.GetMask(new string[2] { "Default", "Interactable" })))
		{
			return;
		}
		SchematicObject componentInParent = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<SchematicObject>();
		if ((Object)(object)componentInParent != (Object)null && componentInParent.Name == "173cage")
		{
			if (_followingPlayer != (Player)null)
			{
				StopTeleporting(_cageSchematic);
			}
			_followingPlayer = player;
			_teleportCoroutines[_cageSchematic] = Timing.RunCoroutine(TeleportCageCoroutine(_cageSchematic, player));
			ShowContainmentMessage(player, "Клетка теперь следует за вами!", 3f);
		}
	}

	[IteratorStateMachine(typeof(TeleportCageCoroutine_d_21))]
	private IEnumerator<float> TeleportCageCoroutine(SchematicObject schematic, Player player)
	{
		return new TeleportCageCoroutine_d_21(0)
		{
			__4__this = this,
			schematic = schematic,
			player = player
		};
	}

	private void StopTeleporting(SchematicObject schematic)
	{
		if ((Object)(object)schematic != (Object)null && _teleportCoroutines.TryGetValue(schematic, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_teleportCoroutines.Remove(schematic);
		}
		if ((Object)(object)schematic != (Object)null)
		{
			((Component)schematic).transform.SetParent((Transform)null);
		}
		if (_followingPlayer != (Player)null)
		{
			_followingPlayer = null;
		}
	}
}
