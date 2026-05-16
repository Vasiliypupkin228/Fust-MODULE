using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;

namespace FultEngine.Module.Logs.Event;

public static class Admin
{
	public static void Enabled()
	{
		Player.Kicking += (CustomEventHandler<KickingEventArgs>)OnKicking;
		Player.IssuingMute += (CustomEventHandler<IssuingMuteEventArgs>)OnIssuingMute;
		Player.RevokingMute += (CustomEventHandler<RevokingMuteEventArgs>)OnRevokingMute;
	}

	private static void OnKicking(KickingEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && !(ev.Target == (Player)null))
		{
			Plugin.LogToFile("\nАдминистратор " + ev.Player.Nickname + " кикнул " + ev.Target.Nickname + "\n______________________________\nПричина: " + ev.Reason + "\n______________________________\nАдминистратор " + ev.Player.Nickname + ":\nIP/SteamID: " + ev.Player.IPAddress + "/" + ev.Player.UserId + "\n______________________________\nИгрок " + ev.Target.Nickname + ":\nIP/SteamID: " + ev.Target.IPAddress + "/" + ev.Target.UserId, Plugin.LogType.Admin);
		}
	}

	private static void OnIssuingMute(IssuingMuteEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			string text = "Unknown";
			string text2 = "Unknown";
			Plugin.LogToFile("\nАдминистратор " + text + " замутил " + ev.Player.Nickname + "\n______________________________\nАдминистратор:\nIP/SteamID: Unknown/" + text2 + "\n______________________________\nИгрок " + ev.Player.Nickname + ":\nIP/SteamID: " + ev.Player.IPAddress + "/" + ev.Player.UserId, Plugin.LogType.Admin);
		}
	}

	private static void OnRevokingMute(RevokingMuteEventArgs ev)
	{
		if (!(((IssuingMuteEventArgs)ev).Player == (Player)null))
		{
			string text = "Unknown";
			string text2 = "Unknown";
			Plugin.LogToFile("\nАдминистратор " + text + " РАзамутил " + ((IssuingMuteEventArgs)ev).Player.Nickname + "\n______________________________\nАдминистратор:\nIP/SteamID: Unknown/" + text2 + "\n______________________________\nИгрок " + ((IssuingMuteEventArgs)ev).Player.Nickname + ":\nIP/SteamID: " + ((IssuingMuteEventArgs)ev).Player.IPAddress + "/" + ((IssuingMuteEventArgs)ev).Player.UserId, Plugin.LogType.Admin);
		}
	}
}
