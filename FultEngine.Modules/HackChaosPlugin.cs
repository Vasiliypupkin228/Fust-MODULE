using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using FultEngine.Module;

namespace FultEngine.Modules;

public class HackChaosPlugin : IFultEngineModule
{
	private readonly HackManager _hackManager;

	public string Name => "HackChaos";

	public string Author => "FUST";

	public Version Version => new Version(0, 0, 1);

	public HackChaosPlugin()
	{
		_hackManager = new HackManager();
	}

	public void OnEnabled()
	{
		Player.InteractingDoor += (CustomEventHandler<InteractingDoorEventArgs>)OnInteractingDoor;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
	}

	public void OnDisabled()
	{
		Player.InteractingDoor -= (CustomEventHandler<InteractingDoorEventArgs>)OnInteractingDoor;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		_hackManager.CleanupAllHacks();
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

	private void OnPlayerLeft(LeftEventArgs ev)
	{
		_hackManager.CleanupPlayer(((JoinedEventArgs)ev).Player);
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		_hackManager.CleanupAllHacks();
	}

	private void OnInteractingDoor(InteractingDoorEventArgs ev)
	{
		if (ev.Player == (Player)null || ev.Door == null || !ev.Door.IsKeycardDoor || ev.Player.CurrentItem == null || (int)ev.Player.CurrentItem.Type != 10)
		{
			return;
		}
		CustomItem obj = CustomItem.Get(1u);
		if (obj != null && obj.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj2 = CustomItem.Get(2u);
		if (obj2 != null && obj2.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj3 = CustomItem.Get(3u);
		if (obj3 != null && obj3.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj4 = CustomItem.Get(4u);
		if (obj4 != null && obj4.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj5 = CustomItem.Get(5u);
		if (obj5 != null && obj5.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj6 = CustomItem.Get(6u);
		if (obj6 != null && obj6.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj7 = CustomItem.Get(7u);
		if (obj7 != null && obj7.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj8 = CustomItem.Get(8u);
		if (obj8 != null && obj8.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj9 = CustomItem.Get(9u);
		if (obj9 != null && obj9.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj10 = CustomItem.Get(10u);
		if (obj10 != null && obj10.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj11 = CustomItem.Get(11u);
		if (obj11 != null && obj11.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj12 = CustomItem.Get(12u);
		if (obj12 != null && obj12.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj13 = CustomItem.Get(13u);
		if (obj13 != null && obj13.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj14 = CustomItem.Get(14u);
		if (obj14 != null && obj14.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj15 = CustomItem.Get(15u);
		if (obj15 != null && obj15.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj16 = CustomItem.Get(16u);
		if (obj16 != null && obj16.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj17 = CustomItem.Get(17u);
		if (obj17 != null && obj17.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj18 = CustomItem.Get(18u);
		if (obj18 != null && obj18.Check(ev.Player.CurrentItem))
		{
			return;
		}
		CustomItem obj19 = CustomItem.Get(19u);
		if (obj19 == null || !obj19.Check(ev.Player.CurrentItem))
		{
			ZoneType zone = ev.Door.Zone;
			int serial = ev.Player.CurrentItem.Serial;
			if (!_hackManager.HasZoneAccess(serial, zone))
			{
				ev.IsAllowed = false;
				ev.Door.IsOpen = false;
				_hackManager.HandleDoorInteraction(ev.Player, ev.Door, serial);
			}
		}
	}
}
