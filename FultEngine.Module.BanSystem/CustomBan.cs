using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace FultEngine.Module.BanSystem;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CustomBan : ICommand
{
	public string Command => "customban";

	public string[] Aliases => new string[2] { "cb", "cban" };

	public string Description => "Банит игрока и сохраняет информацию в файл. Использование: customban <ID игрока> [время в минутах] <причина бана>";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (arguments.Count < 1)
		{
			response = "Используйте: customban <ID игрока> [время бана в минутах] <причина бана>";
			return false;
		}
		string text = CollectionExtensions.At<string>(arguments, 0);
		int num = 60;
		string text2 = "Не указана";
		int num2 = 1;
		if (arguments.Count > 1 && int.TryParse(CollectionExtensions.At<string>(arguments, 1), out var result))
		{
			num = result;
			num2 = 2;
		}
		if (arguments.Count > num2)
		{
			text2 = string.Join(" ", arguments.Skip(num2));
		}
		Player val = Player.Get(text);
		if (val == (Player)null)
		{
			response = "Игрок с ID " + text + " не найден.";
			return false;
		}
		PlayerCommandSender val2 = (PlayerCommandSender)(object)((sender is PlayerCommandSender) ? sender : null);
		if (val2 == null)
		{
			response = "Команду может выполнить только игрок-админ.";
			return false;
		}
		Player val3 = Player.Get(((CommandSender)val2).SenderId);
		if (val3 == (Player)null)
		{
			response = "Не удалось определить админа.";
			return false;
		}
		try
		{
			if (BanSystem.Instance == null)
			{
				response = "Плагин BanSystem не инициализирован.";
				return false;
			}
			BanSystem.Instance.AddBan(val, text2, num, val3);
			response = $"Игрок {val.Nickname} забанен на {num} минут. Причина: {text2}";
			return true;
		}
		catch (Exception ex)
		{
			response = "Ошибка при бане: " + ex.Message;
			return false;
		}
	}
}
