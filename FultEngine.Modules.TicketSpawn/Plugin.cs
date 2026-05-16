using System;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;

namespace FultEngine.Modules.TicketSpawn;

public class Plugin : IFultEngineModule
{
	public static bool _ticket;

	public string Name => "TicketSpawn";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 1);

	public static Plugin Instance { get; private set; }

	public void OnEnabled()
	{
		Instance = this;
		Server.RespawningTeam += (CustomEventHandler<RespawningTeamEventArgs>)OnRespawningTeam;
	}

	public void OnDisabled()
	{
		Instance = null;
		Server.RespawningTeam -= (CustomEventHandler<RespawningTeamEventArgs>)OnRespawningTeam;
	}

	private void OnRespawningTeam(RespawningTeamEventArgs ev)
	{
		if (ev.Players != null && _ticket)
		{
			ev.IsAllowed = false;
		}
	}

	public Type GetConfigType()
	{
		return null;
	}

	public object GetDefaultConfig()
	{
		return null;
	}

	public void SetConfig(object configObj)
	{
	}

	public object GetConfig()
	{
		return null;
	}
}
