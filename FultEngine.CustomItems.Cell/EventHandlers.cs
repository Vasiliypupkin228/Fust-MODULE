using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.SSBinds;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.CustomItems.Cell;

public class EventHandlers
{
	private readonly ContainmentSequence _containmentSequence;

	private readonly Cell _cell;

	public EventHandlers(Cell cell)
	{
		_cell = cell;
		_containmentSequence = new ContainmentSequence();
	}

	public void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Scp173.BlinkingRequest += (CustomEventHandler<BlinkingRequestEventArgs>)OnBlinkingRequest;
		Map.PickupAdded += (CustomEventHandler<PickupAddedEventArgs>)OnMapPickupAdded;
		Map.ExplodingGrenade += (CustomEventHandler<ExplodingGrenadeEventArgs>)OnExplodingGrenade;
		KeybindManager.OnObjectInteraction += OnKeybindPressed;
		KeybindManager.AddCustomKeybind(31, "╔ <color=#ff5d00b1>\ud83d\udd12</color> Перетащите клетку SCP-173", (KeyCode)98, preventInteractionOnGUI: false, "");
	}

	public void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Scp173.BlinkingRequest -= (CustomEventHandler<BlinkingRequestEventArgs>)OnBlinkingRequest;
		Map.PickupAdded -= (CustomEventHandler<PickupAddedEventArgs>)OnMapPickupAdded;
		Map.ExplodingGrenade -= (CustomEventHandler<ExplodingGrenadeEventArgs>)OnExplodingGrenade;
		KeybindManager.OnObjectInteraction -= OnKeybindPressed;
	}

	private void OnBlinkingRequest(BlinkingRequestEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && _containmentSequence._scp173Cell)
		{
			ev.IsAllowed = false;
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (ev.Item != null && !(ev.Player == (Player)null))
		{
			CustomItem val = CustomItem.Get(5u);
			if (val != null && val.Check(ev.Item))
			{
				ev.IsAllowed = false;
				Timing.RunCoroutine(_containmentSequence.RunContainmentSequence(ev.Player, ev.Item));
			}
		}
	}

	private void OnMapPickupAdded(PickupAddedEventArgs ev)
	{
		if (ev.Pickup == null || (Object)(object)ev.Pickup.Transform == (Object)null)
		{
			return;
		}
		CustomItem val = CustomItem.Get(5u);
		if (val == null || !val.Check(ev.Pickup))
		{
			return;
		}
		Rigidbody component = ev.Pickup.GameObject.GetComponent<Rigidbody>();
		if ((Object)(object)component != (Object)null)
		{
			component.freezeRotation = true;
		}
		SchematicObject val2 = ObjectSpawner.SpawnSchematic("CellPacket", ev.Pickup.Position, (Quaternion?)Quaternion.identity, (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		if (!((Object)(object)val2 == (Object)null))
		{
			GameObject gameObject = ((Component)val2).gameObject;
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider val3 in componentsInChildren)
			{
				val3.enabled = false;
			}
			gameObject.transform.parent = ev.Pickup.Transform;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && CustomItem.Get(5u).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Клетка\n<size=19>Используется для вус SCP-173</size>");
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && CustomItem.Get(5u).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Клетка\n<size=19>Используется для вус SCP-173</size>");
		}
	}

	private void OnShot(ShotEventArgs ev)
	{
		if (ev.Player == (Player)null || ev.Item == null)
		{
			return;
		}
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val, ref val2, 100f, LayerMask.GetMask(new string[2] { "Default", "Interactable" })))
		{
			SchematicObject componentInParent = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<SchematicObject>();
			if ((Object)(object)componentInParent != (Object)null && componentInParent.Name == "173cage")
			{
				AudioManager.CreateForSchematic(componentInParent, "popal1", 7f);
				float damage = 10f;
				_containmentSequence.DamageCage(damage, ev.Player);
			}
		}
	}

	private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
	{
		if ((int)((Pickup)ev.Projectile).Type != 26)
		{
			_containmentSequence.HandleGrenadeExplosion(ev.Position, ev.Player);
		}
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && (int)ev.Player.Role.Type <= 0)
		{
			_containmentSequence.CheckCageUnloaded(ev.Player);
		}
	}

	private void OnKeybindPressed(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val != null && val.SyncIsPressed)
		{
			Player val2 = Player.Get(hub);
			if (!(val2 == (Player)null))
			{
				_containmentSequence.OnKeybindPressed(val2, ((ServerSpecificSettingBase)val).SettingId);
			}
		}
	}
}
