using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;

namespace FultEngine.Module.Logs.Event;

public static class Player
{
	public static void Enabled()
	{
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Player.Joined += (CustomEventHandler<JoinedEventArgs>)OnJoined;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.Hurt += (CustomEventHandler<HurtEventArgs>)OnHurt;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Player.Escaped += (CustomEventHandler<EscapedEventArgs>)OnEscaped;
		Player.Handcuffing += (CustomEventHandler<HandcuffingEventArgs>)OnHandcuffing;
		Player.RemovingHandcuffs += (CustomEventHandler<RemovingHandcuffsEventArgs>)OnRemovingHandcuffs;
		Player.PickingUpItem += (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.DroppedItem += (CustomEventHandler<DroppedItemEventArgs>)OnDroppedItem;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.InteractingElevator += (CustomEventHandler<InteractingElevatorEventArgs>)OnInteractingElevator;
		Player.EnteringPocketDimension += (CustomEventHandler<EnteringPocketDimensionEventArgs>)OnEnteringPocketDimension;
		Player.EscapingPocketDimension += (CustomEventHandler<EscapingPocketDimensionEventArgs>)OnEscapingPocketDimension;
		Player.ActivatingWarheadPanel += (CustomEventHandler<ActivatingWarheadPanelEventArgs>)OnActivatingWarheadPanel;
		Player.UnlockingGenerator += (CustomEventHandler<UnlockingGeneratorEventArgs>)OnUnlockingGenerator;
		Player.ActivatingGenerator += (CustomEventHandler<ActivatingGeneratorEventArgs>)OnActivatingGenerator;
		Player.StoppingGenerator += (CustomEventHandler<StoppingGeneratorEventArgs>)OnStoppingGenerator;
		Player.IntercomSpeaking += (CustomEventHandler<IntercomSpeakingEventArgs>)OnIntercomSpeaking;
		Player.ThrownProjectile += (CustomEventHandler<ThrownProjectileEventArgs>)OnThrownProjectile;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.TogglingNoClip += (CustomEventHandler<TogglingNoClipEventArgs>)OnTogglingNoClip;
	}

	private static void OnVerified(VerifiedEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " был верифицирован\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnJoined(JoinedEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " подключается к серверу\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnLeft(LeftEventArgs ev)
	{
		if (!(((ev != null) ? ((JoinedEventArgs)ev).Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (((JoinedEventArgs)ev).Player.Nickname ?? "неизвестный") + " покинул сервер\n______________________________\nIP/SteamID: " + (((JoinedEventArgs)ev).Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnHurt(HurtEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			string[] obj = new string[6]
			{
				"Игрок ",
				ev.Player.Nickname ?? "неизвестный",
				" получил урон\n______________________________\nIP/SteamID: ",
				ev.Player.UserId ?? "N/A",
				"\nАтакующий:\nНик: ",
				null
			};
			CustomDamageHandler damageHandler = ev.DamageHandler;
			object obj2;
			if (damageHandler == null)
			{
				obj2 = null;
			}
			else
			{
				Player attacker = ((DamageHandler)damageHandler).Attacker;
				obj2 = ((attacker != null) ? attacker.Nickname : null);
			}
			if (obj2 == null)
			{
				obj2 = "неизвестный";
			}
			obj[5] = (string)obj2;
			Plugin.LogToFile(string.Concat(obj), Plugin.LogType.Player);
		}
	}

	private static void OnDied(DiedEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " умер\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile(string.Format("Игрок {0} меняет роль на {1}\n", ev.Player.Nickname ?? "неизвестный", ev.NewRole) + "______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnEscaped(EscapedEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " сбежал\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnHandcuffing(HandcuffingEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && !(((ev != null) ? ev.Target : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " надевает наручники на " + (ev.Target.Nickname ?? "неизвестный") + "\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A") + "\nЦель:\nIP/SteamID: " + (ev.Target.IPAddress ?? "N/A") + "/" + (ev.Target.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && !(((ev != null) ? ev.Target : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " снимает наручники с " + (ev.Target.Nickname ?? "неизвестный") + "\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A") + "\nЦель:\nIP/SteamID: " + (ev.Target.IPAddress ?? "N/A") + "/" + (ev.Target.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnPickingUpItem(PickingUpItemEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ((ev != null) ? ev.Pickup : null) != null)
		{
			Plugin.LogToFile(string.Format("Игрок {0} подобрал предмет {1}\n", ev.Player.Nickname ?? "неизвестный", ev.Pickup.Type) + "______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnDroppedItem(DroppedItemEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ((ev != null) ? ev.Pickup : null) != null)
		{
			Plugin.LogToFile(string.Format("Игрок {0} выбросил предмет {1}\n", ev.Player.Nickname ?? "неизвестный", ev.Pickup.Type) + "______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnUsingItem(UsingItemEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ((ev != null) ? ev.Item : null) != null)
		{
			Plugin.LogToFile(string.Format("Игрок {0} использует предмет {1}\n", ev.Player.Nickname ?? "неизвестный", ev.Item.Type) + "______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnInteractingElevator(InteractingElevatorEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " взаимодействует с лифтом\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " вошёл в карманное измерение\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " пытается сбежать из карманного измерения\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " активирует панель ядерной боеголовки\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " разблокирует генератор\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
	{
		if (((ev != null) ? ev.Player : null) == (Player)null)
		{
			Log.Error("OnActivatingGenerator: Player is null");
		}
		else
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " активирует генератор\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnStoppingGenerator(StoppingGeneratorEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " останавливает генератор\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " говорит по интеркому\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnThrownProjectile(ThrownProjectileEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ((ev != null) ? ev.Projectile : null) != null && ((int)((Pickup)ev.Projectile).Type == 25 || (int)((Pickup)ev.Projectile).Type == 26))
		{
			Plugin.LogToFile(string.Format("Игрок {0} бросил гранату {1}\n", ev.Player.Nickname ?? "неизвестный", ((Pickup)ev.Projectile).Type) + "______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && ((ev != null) ? ev.Item : null) != null)
		{
			Plugin.LogToFile(string.Format("Игрок {0} меняет предмет на {1}\n", ev.Player.Nickname ?? "неизвестный", ev.Item.Type) + "______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}

	private static void OnTogglingNoClip(TogglingNoClipEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			Plugin.LogToFile("Игрок " + (ev.Player.Nickname ?? "неизвестный") + " переключает режим NoClip\n______________________________\nIP/SteamID: " + (ev.Player.UserId ?? "N/A"), Plugin.LogType.Player);
		}
	}
}
