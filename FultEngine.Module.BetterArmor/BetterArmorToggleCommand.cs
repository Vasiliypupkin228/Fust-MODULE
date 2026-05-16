using System;
using CommandSystem;
using Exiled.API.Features;

namespace FultEngine.Module.BetterArmor;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class BetterArmorToggleCommand : ICommand
{
	public string Command => "betterarmor";

	public string[] Aliases => new string[1] { "barmor" };

	public string Description => "Включение/выключение системы улучшенной брони";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (arguments.Count < 1 || (CollectionExtensions.At<string>(arguments, 0).ToLower() != "on" && CollectionExtensions.At<string>(arguments, 0).ToLower() != "off"))
		{
			response = "Использование: betterarmor [on/off]";
			return false;
		}
		Player val = Player.Get(sender);
		if (val == (Player)null)
		{
			response = "Команда доступна только игрокам.";
			return false;
		}
		bool flag = CollectionExtensions.At<string>(arguments, 0).ToLower() == "on";
		if (BetterArmorModule.IsEnabled == flag)
		{
			response = "Система улучшенной брони уже " + (flag ? "включена" : "выключена") + ".";
			return false;
		}
		BetterArmorModule.IsEnabled = flag;
		response = "Система улучшенной брони " + (flag ? "включена" : "выключена") + ".";
		return true;
	}
}
