using System;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.LoaderModule;

namespace FultEngine.Module.CustomSound;

public class Plugin : IFultEngineModule
{
	public string Name => "CustomSound";

	public string Author => "FUST";

	public Version Version => new Version(0, 0, 1);

	public void OnEnabled()
	{
		Player.PickingUpItem += (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.DroppedItem += (CustomEventHandler<DroppedItemEventArgs>)OnDroppedItem;
		Player.TogglingWeaponFlashlight += (CustomEventHandler<TogglingWeaponFlashlightEventArgs>)OnTogglingWeaponFlashlight;
	}

	public void OnDisabled()
	{
		Player.PickingUpItem -= (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.DroppedItem -= (CustomEventHandler<DroppedItemEventArgs>)OnDroppedItem;
		Player.TogglingWeaponFlashlight -= (CustomEventHandler<TogglingWeaponFlashlightEventArgs>)OnTogglingWeaponFlashlight;
	}

	public Type GetConfigType()
	{
		return null;
	}

	public object GetDefaultConfig()
	{
		return null;
	}

	public void SetConfig(object config)
	{
	}

	public void OnPickingUpItem(PickingUpItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Pickup != null && ev.IsAllowed)
		{
			if (ItemExtensions.IsKeycard(ev.Pickup.Type))
			{
				PlayLocalSfx(ev.Player, "TakeCard");
			}
			else if (ItemExtensions.IsArmor(ev.Pickup.Type))
			{
				PlayLocalSfx(ev.Player, "TakeArmor");
			}
			else if (ItemExtensions.IsWeapon(ev.Pickup.Type, true))
			{
				PlayLocalSfx(ev.Player, "TakeGun");
			}
			else if (ItemExtensions.IsAmmo(ev.Pickup.Type))
			{
				PlayLocalSfx(ev.Player, "Ammodt");
			}
		}
	}

	public void OnDroppedItem(DroppedItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Pickup != null)
		{
			if (ItemExtensions.IsArmor(ev.Pickup.Type))
			{
				PlayLocalSfx(ev.Player, "DropArmor");
			}
			else if (ItemExtensions.IsWeapon(ev.Pickup.Type, true))
			{
				PlayLocalSfx(ev.Player, "DropGun");
			}
			else if (ItemExtensions.IsAmmo(ev.Pickup.Type))
			{
				PlayLocalSfx(ev.Player, "Ammodt");
			}
		}
	}

	private static void PlayLocalSfx(Player player, string clipName)
	{
		if (!(player == (Player)null) && !string.IsNullOrWhiteSpace(clipName) && !AudioManager.CreateLocal2DForPlayer(player, clipName))
		{
			Log.Warn("[CustomSound] Не удалось проиграть локальный звук '" + clipName + "' для " + player.Nickname + ".");
		}
	}

	public void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			PlayLocalSfx(ev.Player, "FlashGun");
		}
	}
}
