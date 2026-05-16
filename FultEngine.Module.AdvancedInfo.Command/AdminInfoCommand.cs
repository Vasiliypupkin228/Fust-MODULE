using System;
using System.Linq;
using System.Text.RegularExpressions;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;

namespace FultEngine.Module.AdvancedInfo.Command;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AdminInfoCommand : ICommand
{
	public string Command => "ainfo";

	public string[] Aliases => new string[1] { "admininfo" };

	public string Description => "Устанавливает пользовательскую информацию для игрока.";

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
			response = "Использование: ainfo <ID игрока> <информация>";
			return false;
		}
		Player val = Player.Get(CollectionExtensions.At<string>(arguments, 0));
		if (val == (Player)null)
		{
			response = "Игрок не найден.";
			return false;
		}
		string newValue = string.Join(" ", arguments.Skip(1));
		string text;
		if (!FultEngine.Module.Plugin.PlayerSubclasses.TryGetValue(val, out var value))
		{
			RoleTypeId type = val.Role.Type;
			text = ((object)(RoleTypeId)(ref type)).ToString();
		}
		else
		{
			text = value.Id;
		}
		string newValue2 = text;
		string text2 = Misc.ToHex(val.Role.Color);
		if (!text2.StartsWith("#"))
		{
			text2 = "#" + text2;
		}
		if (!Regex.IsMatch(text2, "^#[0-9A-Fa-f]{6}$"))
		{
			text2 = "#FFFFFF";
		}
		val.CustomInfo = config.AdminInfoFormat.Replace("{PlayerId}", val.Id.ToString()).Replace("{Nickname}", val.Nickname).Replace("{Role}", newValue2)
			.Replace("{CustomInfo}", newValue);
		val.ShowHint(val.CustomInfo, config.ShowHintDuration);
		response = "Пользовательская информация для " + val.Nickname + " обновлена.";
		return true;
	}
}
