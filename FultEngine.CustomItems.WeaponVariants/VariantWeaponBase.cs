using System;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using UnityEngine;

namespace FultEngine.CustomItems.WeaponVariants;

public abstract class VariantWeaponBase : CustomWeapon
{
	protected abstract string WeaponSubtitle { get; }

	protected abstract string FeatureDescription { get; }

	protected abstract string AttachmentsDescription { get; }

	protected virtual string AccentColor => "#ffffff";

	protected override void SubscribeEvents()
	{
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		((CustomWeapon)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		((CustomWeapon)this).UnsubscribeEvents();
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		try
		{
			if (ev != null && !(ev.Attacker == (Player)null) && !(ev.Player == (Player)null) && !(ev.Attacker == ev.Player) && ev.Attacker.CurrentItem != null && ((CustomItem)this).Check(ev.Attacker.CurrentItem))
			{
				OnVariantHurting(ev);
			}
		}
		catch (Exception arg)
		{
			Log.Error($"[{((CustomItem)this).Name}] Ошибка в OnHurting: {arg}");
		}
	}

	protected virtual void OnVariantHurting(HurtingEventArgs ev)
	{
	}

	protected static bool IsTargetArmored(Player player)
	{
		return player != (Player)null && (player.CurrentArmor != null || player.ArtificialHealth > 0f);
	}

	protected static float Distance(Player a, Player b)
	{
		return (a == (Player)null || b == (Player)null) ? 0f : Vector3.Distance(a.Position, b.Position);
	}
}
