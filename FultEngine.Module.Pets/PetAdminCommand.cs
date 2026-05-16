using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

namespace FultEngine.Module.Pets;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class PetAdminCommand : ICommand
{
	public string Command => "pet";

	public string[] Aliases => new string[1] { "p" };

	public string Description => "Manage pets: create, give, remove, or list.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player val = Player.Get(sender);
		if (val == (Player)null || (val.GroupName != "owner" && val.GroupName != "director"))
		{
			response = "Только для высшей администрации.";
			return false;
		}
		if (arguments.Count < 1)
		{
			response = "Использование: .pet create <schematic> <petname> <description> | .pet give <id_игрока> <petname> | .pet remove <id_игрока> [petname|all] | .pet list";
			return false;
		}
		string text = CollectionExtensions.At<string>(arguments, 0).ToLower();
		if (text == "list")
		{
			string text2 = string.Join("\n", Plugin.Instance.PetsData.Select((KeyValuePair<string, PetData> kvp) => kvp.Key + " - " + kvp.Value.Description + " (" + kvp.Value.SchematicName + ")"));
			string text3 = string.Join("\n", Plugin.Instance.PlayerPets.Select((KeyValuePair<string, string> kvp) => kvp.Key + ": " + kvp.Value));
			response = "\n<color=#871212c6>1. Список информации из pets.txt:</color>\n<color=#fffa>" + text2 + "</color>\n\n<color=#871212c6>2. Список какие схематики доступны определенным игрокам:</color>\n<color=#fffa>" + text3 + "</color>";
			return true;
		}
		if (text == "create" && arguments.Count >= 3)
		{
			string schematic = CollectionExtensions.At<string>(arguments, 1);
			string text4 = CollectionExtensions.At<string>(arguments, 2);
			string desc = string.Join(" ", arguments.Skip(3));
			Plugin.Instance.PetsData[text4] = new PetData(schematic, desc);
			Plugin.Instance.SavePetsData();
			response = "Домашнее животное " + text4 + " создано.";
			return true;
		}
		if (text == "give" && arguments.Count == 3)
		{
			string text5 = CollectionExtensions.At<string>(arguments, 1);
			string text6 = CollectionExtensions.At<string>(arguments, 2);
			Player val2 = Player.Get(text5);
			if (val2 != null && Plugin.Instance.PetsData.ContainsKey(text6))
			{
				Plugin.Instance.PlayerPets[val2.UserId] = text6;
				Plugin.Instance.SavePlayerPets();
				response = "Дал " + text6 + " игроку " + val2.Nickname + ".";
				return true;
			}
			response = "Недействительный игрок или домашнее животное.";
			return false;
		}
		if (text == "remove" && arguments.Count >= 2)
		{
			string text7 = CollectionExtensions.At<string>(arguments, 1);
			Player val3 = Player.Get(text7);
			if (val3 != null)
			{
				if (!Plugin.Instance.PlayerPets.ContainsKey(val3.UserId))
				{
					response = "За игроком не закреплено домашнее животное.";
					return false;
				}
				if (arguments.Count == 2 || CollectionExtensions.At<string>(arguments, 2).ToLower() == "all" || Plugin.Instance.PlayerPets[val3.UserId] == CollectionExtensions.At<string>(arguments, 2))
				{
					Plugin.Instance.DetachPet(val3);
					Plugin.Instance.PlayerPets.Remove(val3.UserId);
					Plugin.Instance.SavePlayerPets();
					response = "Питомец удалён у " + val3.Nickname + ".";
					return true;
				}
			}
			response = "Не удалось удалить питомца.";
			return false;
		}
		response = "Неизвестная подкоманда.";
		return false;
	}
}
