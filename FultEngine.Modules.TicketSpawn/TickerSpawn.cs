using System;
using CommandSystem;

namespace FultEngine.Modules.TicketSpawn;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TickerSpawn : ICommand
{
	public string Command => "TickerSpawn";

	public string[] Aliases => new string[2] { "ts", "spawnticket" };

	public string Description => "";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!Plugin._ticket)
		{
			Plugin._ticket = true;
			response = "Спавн выключен.";
		}
		else
		{
			Plugin._ticket = false;
			response = "Спавн включен.";
		}
		return true;
	}
}
