using System;
using CommandSystem;
using Exiled.API.Features;

namespace FultEngine.Module.RealDemage;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class InjurySystemToggleCommand : ICommand
{
	public string Command => "injurysystem";

	public string[] Aliases => new string[1] { "isys" };

	public string Description => "Включение/выключение системы ранений";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (arguments.Count < 1 || (CollectionExtensions.At<string>(arguments, 0).ToLower() != "on" && CollectionExtensions.At<string>(arguments, 0).ToLower() != "off"))
		{
			response = "Использование: injurysystem [on/off]";
			return false;
		}
		Player val = Player.Get(sender);
		if (val == (Player)null)
		{
			response = "Команда доступна только игрокам.";
			return false;
		}
		bool flag = CollectionExtensions.At<string>(arguments, 0).ToLower() == "on";
		if (Plugin.IsWork == flag)
		{
			response = "Система ранений уже " + (flag ? "включена" : "выключена") + ".";
			return false;
		}
		Plugin.IsWork = flag;
		response = "Система ранений " + (flag ? "включена" : "выключена") + ".";
		return true;
	}
}
