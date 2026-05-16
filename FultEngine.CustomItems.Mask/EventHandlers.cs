using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.CustomItems.Mask;

public class EventHandlers
{
	private readonly ContainmentSequence _containmentSequence;

	private readonly Bag _bag;

	public EventHandlers(Bag bag)
	{
		_bag = bag;
		_containmentSequence = new ContainmentSequence();
	}

	public void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Scp096.AddingTarget += (CustomEventHandler<AddingTargetEventArgs>)OnAddingTarget;
		Scp096.Enraging += (CustomEventHandler<EnragingEventArgs>)OnEnraging;
		Map.PickupAdded += (CustomEventHandler<PickupAddedEventArgs>)OnMapPickupAdded;
	}

	public void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
		Scp096.AddingTarget -= (CustomEventHandler<AddingTargetEventArgs>)OnAddingTarget;
		Scp096.Enraging -= (CustomEventHandler<EnragingEventArgs>)OnEnraging;
		Map.PickupAdded -= (CustomEventHandler<PickupAddedEventArgs>)OnMapPickupAdded;
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && _containmentSequence.IsScp096Contained(ev.Player))
		{
			_containmentSequence.ClearState(ev.Player);
		}
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && (int)ev.Player.Role.Type == 9 && _containmentSequence.IsScp096Contained(ev.Player))
		{
			_containmentSequence.ClearState(ev.Player);
		}
	}

	private void OnLeft(LeftEventArgs ev)
	{
		if (!(((JoinedEventArgs)ev).Player == (Player)null) && _containmentSequence.IsScp096Contained(((JoinedEventArgs)ev).Player))
		{
			_containmentSequence.ClearState(((JoinedEventArgs)ev).Player);
		}
	}

	private void OnEnraging(EnragingEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && _containmentSequence.IsScp096Contained(ev.Player))
		{
			_containmentSequence.ClearState(ev.Player);
		}
	}

	private void OnAddingTarget(AddingTargetEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && _containmentSequence.IsScp096Contained(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (ev.Item != null && !(ev.Player == (Player)null) && CustomItem.Get(6u).Check(ev.Item))
		{
			ev.IsAllowed = false;
			Timing.RunCoroutine(_containmentSequence.RunContainmentSequence(ev.Player, ev.Item));
		}
	}

	private void OnMapPickupAdded(PickupAddedEventArgs ev)
	{
		if (ev.Pickup == null || (Object)(object)ev.Pickup.Transform == (Object)null || !CustomItem.Get(6u).Check(ev.Pickup))
		{
			return;
		}
		Rigidbody component = ev.Pickup.GameObject.GetComponent<Rigidbody>();
		if ((Object)(object)component != (Object)null)
		{
			component.freezeRotation = true;
		}
		SchematicObject val = ObjectSpawner.SpawnSchematic("BagPacket", ev.Pickup.Position, (Quaternion?)Quaternion.identity, (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		if (!((Object)(object)val == (Object)null))
		{
			GameObject gameObject = ((Component)val).gameObject;
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider val2 in componentsInChildren)
			{
				val2.enabled = false;
			}
			gameObject.transform.parent = ev.Pickup.Transform;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && CustomItem.Get(6u).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Мешок\n<size=19>Используется для ВЗС SCP-096</size>");
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && CustomItem.Get(6u).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Мешок\n<size=19>Используется для ВЗС SCP-096</size>");
		}
	}
}
