using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;

namespace FultEngine.Module.Logs.Event;

public static class Server
{
	[CompilerGenerated]
	private static class __O
	{
		public static CustomEventHandler _0__OnRoundStarted;

		public static CustomEventHandler<RoundEndedEventArgs> _1__OnRoundEnded;

		public static CustomEventHandler _2__OnRestartingRound;
	}

	public static void Enabled()
	{
		Event roundStarted = Server.RoundStarted;
		object obj = __O._0__OnRoundStarted;
		if (obj == null)
		{
			CustomEventHandler val = OnRoundStarted;
			__O._0__OnRoundStarted = val;
			obj = (object)val;
		}
		Server.RoundStarted = roundStarted + (CustomEventHandler)obj;
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Event restartingRound = Server.RestartingRound;
		object obj2 = __O._2__OnRestartingRound;
		if (obj2 == null)
		{
			CustomEventHandler val2 = OnRestartingRound;
			__O._2__OnRestartingRound = val2;
			obj2 = (object)val2;
		}
		Server.RestartingRound = restartingRound + (CustomEventHandler)obj2;
		Event waitingForPlayers = Server.WaitingForPlayers;
		object obj3 = __O._2__OnRestartingRound;
		if (obj3 == null)
		{
			CustomEventHandler val3 = OnRestartingRound;
			__O._2__OnRestartingRound = val3;
			obj3 = (object)val3;
		}
		Server.WaitingForPlayers = waitingForPlayers + (CustomEventHandler)obj3;
	}

	private static void OnRoundStarted()
	{
		Plugin.LogToFile("Запустился раунд", Plugin.LogType.Server);
	}

	private static void OnRoundEnded(RoundEndedEventArgs ev)
	{
		LeadingTeam leadingTeam = ev.LeadingTeam;
		Plugin.LogToFile("Раунд завершен.\n______________________________\nПобедили: " + ((object)(LeadingTeam)(ref leadingTeam)).ToString() + "\n" + $"Время рестарта: {ev.TimeToRestart}", Plugin.LogType.Server);
	}

	private static void OnRestartingRound()
	{
		Plugin.LogToFile("Перезапустился раунд", Plugin.LogType.Server);
	}
}
