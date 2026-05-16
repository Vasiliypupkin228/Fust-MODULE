using System;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;

namespace FultEngine.Module.BetterArmor;

public class BetterArmorModule : IFultEngineModule
{
	public static bool IsEnabled = true;

	private Config _config;

	public string Name => "BetterArmor";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 0);

	public void OnEnabled()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.Hurt += (CustomEventHandler<HurtEventArgs>)OnHurt;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Server.WaitingForPlayers += new CustomEventHandler(OnWaitingForPlayers);
		Player.PickingUpItem += (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
	}

	public void OnDisabled()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.Hurt -= (CustomEventHandler<HurtEventArgs>)OnHurt;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Server.WaitingForPlayers -= new CustomEventHandler(OnWaitingForPlayers);
		Player.PickingUpItem -= (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		ArmorUtils.ClearAllStats();
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config
		{
			IsEnabled = true,
			Debug = false,
			LightArmorAHP = 10f,
			CombatArmorAHP = 30f,
			HeavyArmorAHP = 45f
		};
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (IsEnabled && !(ev.Player == (Player)null) && ev.Item != null && ev.Item.IsArmor)
		{
			Timing.CallDelayed(0.2f, (Action)delegate
			{
				ArmorUtils.ApplyArmorStats(ev.Player, ev.Item, _config);
			});
		}
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		if (IsEnabled && !(ev.Player == (Player)null) && ev.Item != null && ev.IsAllowed && ev.Item.IsArmor)
		{
			Timing.CallDelayed(0.1f, (Action)delegate
			{
				ArmorUtils.DropArmor(ev.Player, ev.Item);
			});
		}
	}

	private void OnPickingUpItem(PickingUpItemEventArgs ev)
	{
		if (!IsEnabled || ev.Player == (Player)null || ev.Pickup == null || !ev.IsAllowed)
		{
			return;
		}
		Item item = Item.Get(ev.Pickup.Serial);
		if (item != null && item.IsArmor)
		{
			Timing.CallDelayed(0.1f, (Action)delegate
			{
				ArmorUtils.PickupArmor(ev.Player, item);
			});
		}
	}

	private void OnHurt(HurtEventArgs ev)
	{
		if (IsEnabled && !(ev.Player == (Player)null) && ev.Player.CurrentArmor != null && (int)((DamageHandlerBase)ev.DamageHandler).Type != 1)
		{
			ArmorUtils.DamageArmor((Item)(object)ev.Player.CurrentArmor, ev.Amount, ev.Player);
		}
	}

	private void OnDied(DiedEventArgs ev)
	{
		if (IsEnabled && !(ev.Player == (Player)null))
		{
			ArmorUtils.StopAHPCoroutine(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (IsEnabled && !(ev.Player == (Player)null) && ev.Item != null)
		{
			if (ev.Item.IsArmor)
			{
				ArmorUtils.StartAHPCoroutine(ev.Player);
			}
			else
			{
				ArmorUtils.StopAHPCoroutine(ev.Player);
			}
		}
	}

	private void OnWaitingForPlayers()
	{
		if (IsEnabled)
		{
			ArmorUtils.ClearAllStats();
		}
	}
}
