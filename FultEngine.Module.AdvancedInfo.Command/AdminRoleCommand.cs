using System;
using CommandSystem;
using Exiled.API.Features;

namespace FultEngine.Module.AdvancedInfo.Command;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AdminRoleCommand : ICommand
{
	public string Command => "arole";

	public string[] Aliases => new string[1] { "adminrole" };

	public string Description => "Устанавливает пользовательскую роль для игрока.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Config config = (Plugin.GetInstance() as Plugin)?._config;
		if (config == null || !config.IsEnabled || !config.EnableCommands)
		{
			response = "Команда отключена или плагин неактивен.";
			return false;
		}
		if (arguments.Count < 2)
		{
			response = "Использование: arole <ID игрока> <роль>";
			return false;
		}
		Player val = Player.Get(CollectionExtensions.At<string>(arguments, 0));
		if (val == (Player)null)
		{
			response = "Игрок не найден.";
			return false;
		}
		string newValue = CollectionExtensions.At<string>(arguments, 1);
		string newValue2 = "";
		if (!string.IsNullOrEmpty(val.CustomInfo))
		{
			string[] array = val.CustomInfo.Split(new char[1] { '\n' });
			if (array.Length >= 4)
			{
				newValue2 = array[3].Replace("<size=13><color=#FFFFFF>", "").Replace("</color></size>", "").Trim();
			}
		}
		val.CustomInfo = config.AdminRoleFormat.Replace("{PlayerId}", val.Id.ToString()).Replace("{Nickname}", val.Nickname).Replace("{Role}", newValue)
			.Replace("{CustomInfo}", newValue2);
		val.ShowHint(val.CustomInfo, config.ShowHintDuration);
		response = "Пользовательская роль для " + val.Nickname + " обновлена.";
		return true;
	}
}
