using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using UnityEngine;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Scp1499 : CustomItem
{
	private readonly HashSet<Player> _activeUsers = new HashSet<Player>();

	private readonly Dictionary<Player, Vector3> _originalPositions = new Dictionary<Player, Vector3>();

	private readonly Dictionary<Player, List<Item>> _storedItems = new Dictionary<Player, List<Item>>();

	private readonly Dictionary<Player, CoroutineHandle> _dimensionTimers = new Dictionary<Player, CoroutineHandle>();

	public override uint Id { get; set; } = 13u;


	public override string Name { get; set; } = "<b><color=#14d979e2>SCP-1499</b></color>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 0.5f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
	{
		Limit = 1u,
		DynamicSpawnPoints = new List<DynamicSpawnPoint>()
	};


	[Description("Координаты для телепортации (указанные вами)")]
	public Vector3 AlternateDimensionPosition { get; set; } = new Vector3(162.057f, 319.465f, -12.904f);


	[Description("Длительность эффекта затемнения при телепортации (секунды)")]
	public float FadeDuration { get; set; } = 1.5f;


	[Description("Максимальное время в альтернативном измерении (секунды)")]
	public float DimensionTimeLimit { get; set; } = 39f;


	protected override void SubscribeEvents()
	{
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.Destroying += (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.Destroying -= (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		((CustomItem)this).UnsubscribeEvents();
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item) && _activeUsers.Contains(ev.Player) && !(ev.Player == (Player)null))
		{
			ReturnFromDimension(ev.Player);
		}
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Pickup))
		{
			CIMessage.SendMessage(ev.Player, "SCP-1499");
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "SCP-1499");
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
			ev.IsAllowed = false;
			if (_activeUsers.Contains(ev.Player))
			{
				ReturnFromDimension(ev.Player);
			}
			else
			{
				EnterDimension(ev.Player);
			}
		}
	}

	private void EnterDimension(Player player)
	{
		_originalPositions[player] = player.Position;
		player.EnableEffect((EffectType)14, FadeDuration, false);
		Timing.CallDelayed(FadeDuration, (Action)delegate
		{
			player.Position = AlternateDimensionPosition;
			_activeUsers.Add(player);
			player.ShowMeowHint(7f, "<size=29><b><color=#61616193>|</color></size> <size=19>Вы вошли в альтернативное измерение. Нажмите на маску снова, чтобы вернуться</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 95, 0, (HintAlignment)2);
			_dimensionTimers[player] = Timing.CallDelayed(DimensionTimeLimit, (Action)delegate
			{
				if (_activeUsers.Contains(player))
				{
					ReturnFromDimension(player);
				}
			});
		});
	}

	private void ReturnFromDimension(Player player)
	{
		if (!_activeUsers.Contains(player))
		{
			return;
		}
		player.EnableEffect((EffectType)14, FadeDuration, false);
		Timing.CallDelayed(FadeDuration, (Action)delegate
		{
			if (_originalPositions.TryGetValue(player, out var value))
			{
				player.Position = value;
			}
			_activeUsers.Remove(player);
			_originalPositions.Remove(player);
			_storedItems.Remove(player);
			if (_dimensionTimers.TryGetValue(player, out var value2))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
				_dimensionTimers.Remove(player);
			}
			player.ShowMeowHint(7f, "<size=29><b><color=#61616193>|</color></size> <size=19>Вы вернулись из альтернативного измерения</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 95, 0, (HintAlignment)2);
			AudioManager.DestroyForPlayer(player);
		});
	}

	private void OnDestroying(DestroyingEventArgs ev)
	{
		CleanupPlayer(ev.Player);
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		CleanupPlayer(ev.Player);
	}

	private void CleanupPlayer(Player player)
	{
		if (_activeUsers.Contains(player))
		{
			ReturnFromDimension(player);
		}
		_activeUsers.Remove(player);
		_originalPositions.Remove(player);
		_storedItems.Remove(player);
		if (_dimensionTimers.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_dimensionTimers.Remove(player);
		}
	}

	protected override void OnWaitingForPlayers()
	{
		_activeUsers.Clear();
		_originalPositions.Clear();
		_storedItems.Clear();
		_dimensionTimers.Clear();
		((CustomItem)this).OnWaitingForPlayers();
	}
}
