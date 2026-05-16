using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.Module.BackGun;

public class Plugin : IFultEngineModule
{
	private Config _config;

	private readonly Dictionary<Player, Dictionary<ItemType, SchematicObject>> _playerBackGuns = new Dictionary<Player, Dictionary<ItemType, SchematicObject>>();

	public string Name => "BackGun";

	public string Author => "FUST";

	public Version Version => new Version(1, 1, 0);

	public void OnEnabled()
	{
		SubscribeEvents();
	}

	public void OnDisabled()
	{
		UnsubscribeEvents();
		foreach (KeyValuePair<Player, Dictionary<ItemType, SchematicObject>> item in _playerBackGuns.ToList())
		{
			ClearPlayerSchematics(item.Key);
		}
		_playerBackGuns.Clear();
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
		_config = (config as Config) ?? new Config();
	}

	private void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnInventoryChanged;
		Player.ItemRemoved += (CustomEventHandler<ItemRemovedEventArgs>)OnInventoryChanged;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
	}

	private void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnInventoryChanged;
		Player.ItemRemoved -= (CustomEventHandler<ItemRemovedEventArgs>)OnInventoryChanged;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
	}

	private void OnVerified(VerifiedEventArgs ev)
	{
		VerifiedEventArgs obj = ev;
		if (!(((obj != null) ? obj.Player : null) == (Player)null))
		{
			Timing.CallDelayed(1f, (Action)delegate
			{
				RefreshPlayerBackGuns(ev.Player);
			});
		}
	}

	private void OnSpawned(SpawnedEventArgs ev)
	{
		SpawnedEventArgs obj = ev;
		if (!(((obj != null) ? obj.Player : null) == (Player)null))
		{
			Timing.CallDelayed(0.6f, (Action)delegate
			{
				RefreshPlayerBackGuns(ev.Player);
			});
		}
	}

	private void OnLeft(LeftEventArgs ev)
	{
		if (!(((ev != null) ? ((JoinedEventArgs)ev).Player : null) == (Player)null))
		{
			ClearPlayerSchematics(((JoinedEventArgs)ev).Player);
			_playerBackGuns.Remove(((JoinedEventArgs)ev).Player);
		}
	}

	private void OnDied(DiedEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			ClearPlayerSchematics(ev.Player);
		}
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			ClearPlayerSchematics(ev.Player);
		}
	}

	private void OnInventoryChanged(ItemAddedEventArgs ev)
	{
		ItemAddedEventArgs obj = ev;
		if (!(((obj != null) ? obj.Player : null) == (Player)null))
		{
			Timing.CallDelayed(0.05f, (Action)delegate
			{
				RefreshPlayerBackGuns(ev.Player);
			});
		}
	}

	private void OnInventoryChanged(ItemRemovedEventArgs ev)
	{
		ItemRemovedEventArgs obj = ev;
		if (!(((obj != null) ? obj.Player : null) == (Player)null))
		{
			Timing.CallDelayed(0.05f, (Action)delegate
			{
				RefreshPlayerBackGuns(ev.Player);
			});
		}
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		DroppingItemEventArgs obj = ev;
		if (!(((obj != null) ? obj.Player : null) == (Player)null))
		{
			Timing.CallDelayed(0.05f, (Action)delegate
			{
				RefreshPlayerBackGuns(ev.Player);
			});
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		ChangingItemEventArgs obj = ev;
		if (!(((obj != null) ? obj.Player : null) == (Player)null))
		{
			Timing.CallDelayed(0.05f, (Action)delegate
			{
				RefreshPlayerBackGuns(ev.Player);
			});
		}
	}

	private void RefreshPlayerBackGuns(Player player)
	{
		try
		{
			if (player == (Player)null || !player.IsConnected || player.ReferenceHub == (ReferenceHub)null || !player.IsAlive)
			{
				ClearPlayerSchematics(player);
				return;
			}
			if (!_playerBackGuns.ContainsKey(player))
			{
				_playerBackGuns[player] = new Dictionary<ItemType, SchematicObject>();
			}
			HashSet<ItemType> hashSet = (from item in player.Items
				where item != null && IsSupportedGun(item.Type)
				select item.Type).Distinct().ToHashSet();
			ItemType val = (ItemType)((player.CurrentItem == null) ? (-1) : ((int)player.CurrentItem.Type));
			foreach (ItemType item in _playerBackGuns[player].Keys.ToList())
			{
				if (!hashSet.Contains(item) || item == val)
				{
					RemoveGunFromBack(player, item);
				}
			}
			foreach (ItemType item2 in hashSet)
			{
				if (item2 != val && !_playerBackGuns[player].ContainsKey(item2))
				{
					AttachGunToBack(player, item2);
				}
			}
		}
		catch (Exception arg)
		{
			Log.Error($"[BackGun] Ошибка обновления оружия на спине у {((player != null) ? player.Nickname : null)}: {arg}");
		}
	}

	private bool IsSupportedGun(ItemType itemType)
	{
		return (int)itemType == 20 || (int)itemType == 52 || (int)itemType == 23 || (int)itemType == 21 || (int)itemType == 24 || (int)itemType == 40 || (int)itemType == 30 || (int)itemType == 39 || (int)itemType == 41;
	}

	private void AttachGunToBack(Player player, ItemType gunType)
	{
		if (player == (Player)null)
		{
			return;
		}
		string schematicNameForGun = GetSchematicNameForGun(gunType);
		if (string.IsNullOrWhiteSpace(schematicNameForGun))
		{
			return;
		}
		SchematicObject val = ObjectSpawner.SpawnSchematic(schematicNameForGun, player.Position, (Quaternion?)player.Rotation, (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		if ((Object)(object)val == (Object)null)
		{
			if (_config.Debug)
			{
				Log.Warn($"[BackGun] Не удалось заспавнить схему '{schematicNameForGun}' для {gunType}. Проверь название схематики в MER.");
			}
			return;
		}
		try
		{
			if ((Object)(object)((Component)val).gameObject == (Object)null)
			{
				if (_config.Debug)
				{
					Log.Warn("[BackGun] У схемы '" + schematicNameForGun + "' пустой gameObject.");
				}
				return;
			}
			Collider[] componentsInChildren = ((Component)val).gameObject.GetComponentsInChildren<Collider>(true);
			foreach (Collider val2 in componentsInChildren)
			{
				val2.enabled = false;
			}
			((Component)val).transform.SetParent(player.Transform);
			(Vector3, Quaternion, Vector3) attachmentTransform = GetAttachmentTransform(gunType);
			((Component)val).transform.localPosition = attachmentTransform.Item1;
			((Component)val).transform.localRotation = attachmentTransform.Item2;
			((Component)val).transform.localScale = attachmentTransform.Item3;
			CullingExtensions.DestroySchematic(player, val);
			_playerBackGuns[player][gunType] = val;
			if (_config.Debug)
			{
				Log.Info($"[BackGun] Прикреплено оружие '{gunType}' на спину игроку {player.Nickname} через схему '{schematicNameForGun}'.");
			}
		}
		catch (Exception arg)
		{
			Log.Error($"[BackGun] Ошибка прикрепления схемы '{schematicNameForGun}' к {((player != null) ? player.Nickname : null)}: {arg}");
			TryDestroySchematic(val);
		}
	}

	private void RemoveGunFromBack(Player player, ItemType gunType)
	{
		if (!(player == (Player)null) && _playerBackGuns.TryGetValue(player, out var value) && value.TryGetValue(gunType, out var value2))
		{
			TryDestroySchematic(value2);
			value.Remove(gunType);
			if (value.Count == 0)
			{
				_playerBackGuns.Remove(player);
			}
		}
	}

	private void ClearPlayerSchematics(Player player)
	{
		if (player == (Player)null || !_playerBackGuns.TryGetValue(player, out var value))
		{
			return;
		}
		foreach (SchematicObject item in value.Values.ToList())
		{
			TryDestroySchematic(item);
		}
		value.Clear();
		_playerBackGuns.Remove(player);
	}

	private void TryDestroySchematic(SchematicObject schematic)
	{
		try
		{
			if ((Object)(object)schematic != (Object)null && (Object)(object)((Component)schematic).gameObject != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)schematic).gameObject);
			}
		}
		catch (Exception ex)
		{
			if (_config.Debug)
			{
				Log.Warn("[BackGun] Ошибка удаления схемы: " + ex.Message);
			}
		}
	}

	private (Vector3 position, Quaternion rotation, Vector3 scale) GetAttachmentTransform(ItemType gunType)
	{
		if ((int)gunType <= 30)
		{
			switch (gunType - 20)
			{
			default:
				if ((int)gunType != 30)
				{
					break;
				}
				return (position: new Vector3(0.18f, 0.16f, -0.14f), rotation: Quaternion.Euler(0f, 0f, -25f), scale: Vector3.one);
			case 0:
				return (position: new Vector3(0.12f, 0.35f, -0.22f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
			case 3:
				return (position: new Vector3(0.16f, 0.22f, -0.18f), rotation: Quaternion.Euler(0f, 0f, -35f), scale: Vector3.one);
			case 1:
				return (position: new Vector3(0.14f, 0.28f, -0.18f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
			case 4:
				return (position: new Vector3(-0.22f, 0.3f, -0.24f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
			case 2:
				break;
			}
		}
		else
		{
			switch (gunType - 39)
			{
			case 1:
				return (position: new Vector3(0.1f, 0.33f, -0.22f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
			case 0:
				return (position: new Vector3(-0.18f, 0.16f, -0.14f), rotation: Quaternion.Euler(0f, 0f, 25f), scale: Vector3.one);
			case 2:
				return (position: new Vector3(-0.12f, 0.32f, -0.21f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
			}
			if ((int)gunType == 52)
			{
				return (position: new Vector3(-0.18f, 0.34f, -0.22f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
			}
		}
		return (position: new Vector3(0.12f, 0.3f, -0.2f), rotation: Quaternion.Euler(90f, 0f, -90f), scale: Vector3.one);
	}

	private string GetSchematicNameForGun(ItemType gunType)
	{
		if ((int)gunType <= 30)
		{
			switch (gunType - 20)
			{
			default:
				if ((int)gunType != 30)
				{
					break;
				}
				return _config.Com18SchematicName;
			case 0:
				return _config.E11SchematicName;
			case 3:
				return _config.FSP9SchematicName;
			case 1:
				return _config.CrossvecSchematicName;
			case 4:
				return _config.LogicerSchematicName;
			case 2:
				break;
			}
		}
		else
		{
			switch (gunType - 39)
			{
			case 1:
				return _config.AKSchematicName;
			case 0:
				return _config.RevolverSchematicName;
			case 2:
				return _config.ShotgunSchematicName;
			}
			if ((int)gunType == 52)
			{
				return _config.FRMG0SchematicName;
			}
		}
		return null;
	}
}
