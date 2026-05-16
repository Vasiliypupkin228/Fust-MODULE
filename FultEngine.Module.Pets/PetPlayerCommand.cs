using System;
using CommandSystem;
using Exiled.API.Features;

namespace FultEngine.Module.Pets;

[CommandHandler(typeof(ClientCommandHandler))]
internal class PetPlayerCommand : ICommand
{
	public string Command { get; } = "pet";


	public string Description { get; } = "Attach, unload, or list assigned pets.";


	public string[] Aliases { get; } = new string[1] { "p" };


	public bool SanitizeResponse { get; } = false;


	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player val = Player.Get(sender);
		if (val == (Player)null)
		{
			response = "\n<color=#871212c6>Ошибка:</color>\n<color=#fffa>Игрок не найден.</color>";
			return false;
		}
		if (arguments.Count == 0 || (arguments.Count == 1 && CollectionExtensions.At<string>(arguments, 0).ToLower() == "list"))
		{
			if (Plugin.Instance.PlayerPets.TryGetValue(val.UserId, out var value) && Plugin.Instance.PetsData.TryGetValue(value, out var value2))
			{
				response = "\n<color=#871212c6>Доступные питомцы:</color>\n<color=#fffa>" + value + " - " + value2.Description + "</color>";
			}
			else
			{
				response = "\n<color=#871212c6>Нет доступных питомцев.</color>\n<color=#03b3ffa9>Попробуйте: .pet <petname> для прикрепления питомца или .pet unload для снятия.</color>";
			}
			return true;
		}
		if (arguments.Count == 1 && CollectionExtensions.At<string>(arguments, 0).ToLower() == "unload")
		{
			if (Plugin.Instance.AttachedPets.ContainsKey(val))
			{
				Plugin.Instance.DetachPet(val);
				response = "\n<color=#871212c6>Питомец снят.</color>";
				return true;
			}
			response = "\n<color=#871212c6>У вас нет активного питомца.</color>\n<color=#03b3ffa9>Попробуйте: .pet list для просмотра доступных питомцев.</color>";
			return false;
		}
		if (arguments.Count != 1)
		{
			response = "\n<color=#871212c6>Ошибка:</color>\n<color=#fffa>Использование: .pet <petname> | .pet list | .pet unload</color>";
			return false;
		}
		string text = CollectionExtensions.At<string>(arguments, 0);
		if (Plugin.Instance.PlayerPets.TryGetValue(val.UserId, out var value3) && value3 == text && Plugin.Instance.PetsData.ContainsKey(text))
		{
			Plugin.Instance.DetachPet(val);
			Plugin.Instance.AttachPet(val);
			response = "\n<color=#871212c6>Поздравляю! Вы прикрепили схематику:</color>\n<color=#fffa>" + text + "</color>\n<color=#03b3ffa9>Используйте .pet unload для снятия питомца.</color>";
			return true;
		}
		response = "\n<color=#871212c6>Питомец не назначен или недействителен.</color>\n<color=#03b3ffa9>Попробуйте: .pet list для просмотра доступных питомцев.</color>";
		return false;
	}
}
