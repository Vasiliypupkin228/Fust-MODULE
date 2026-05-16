using System;
using CommandSystem;

namespace FultEngine.Plugins;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetYumsCommand : ICommand, IUsageProvider
{
	public string Command => "setyums";

	public string[] Aliases => new string[1] { "sy" };

	public string Description => "Устанавливает количество юмов (1-5000) для игрока.";

	public string[] Usage => new string[2] { "id игрока", "количество юмов" };

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		response = "Функционал SetYums не реализован в этом примере. Добавьте логику установки юмов игроку.";
		return false;
	}
}
