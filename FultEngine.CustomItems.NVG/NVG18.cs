using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using MEC;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace FultEngine.CustomItems.NVG;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class NVG18 : CustomItem
{
	public override uint Id { get; set; } = 14u;


	public override string Name { get; set; } = "<b><color=#17d49b>Прибор ночного видения</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	protected override void SubscribeEvents()
	{
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ItemRemoved += (CustomEventHandler<ItemRemovedEventArgs>)OnItemRemoved;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnJoined;
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.ItemRemoved -= (CustomEventHandler<ItemRemovedEventArgs>)OnItemRemoved;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnJoined;
		((CustomItem)this).UnsubscribeEvents();
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Pickup != null && ((CustomItem)this).Check(ev.Pickup))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			ev.IsAllowed = false;
			if (NVGData.ActiveSchematics.ContainsKey(ev.Player))
			{
				NVGCollection.DisableNVG(ev.Player);
			}
			else
			{
				NVGCollection.EnableNVG(ev.Player);
			}
		}
	}

	private void OnItemRemoved(ItemRemovedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			NVGCollection.DisableNVG(ev.Player);
		}
	}

	private void OnDied(DiedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && NVGData.ActiveSchematics.TryGetValue(ev.Player, out var value))
		{
			AudioManager.CreateForPlayer(ev.Player, "DisableNVG", 1f, 7f, 90f);
			ev.Player.EnableEffect((EffectType)47, 3f, false);
			CullingExtensions.DestroySchematic(ev.Player, value);
			NVGData.ActiveSchematics.Remove(ev.Player);
			NVGData.Cooldown.Add(ev.Player);
			Timing.CallDelayed(3f, (Action)delegate
			{
				NVGData.Cooldown.Remove(ev.Player);
			});
		}
	}

	private void OnJoined(VerifiedEventArgs ev)
	{
		if (ev.Player == (Player)null)
		{
			return;
		}
		NVGCollection.DisableNVG(ev.Player);
		foreach (KeyValuePair<Player, SchematicObject> activeSchematic in NVGData.ActiveSchematics)
		{
			if (activeSchematic.Key != ev.Player && (Object)(object)activeSchematic.Value != (Object)null)
			{
				CullingExtensions.DestroySchematic(ev.Player, activeSchematic.Value);
			}
		}
	}

	private void ShowItemHint(Player player)
	{
		CIMessage.SendMessage(player, "Прибор ночного видения");
	}
}
