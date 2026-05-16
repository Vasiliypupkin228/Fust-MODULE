using System;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

namespace FultEngine.Plugins;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CreateZoneYums : ICommand, IUsageProvider
{
	public string Command { get; } = "createzoneyums";


	public string[] Aliases { get; } = new string[2] { "crzoneyums", "crzy" };


	public string Description { get; } = "Создать зону, где начисляются юмы по близости.";


	public string[] Usage { get; } = new string[3] { "Yumas", "size", "[Y_offset]" };


	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player val = Player.Get(sender);
		if (val == (Player)null)
		{
			response = "Команда может быть выполнена только игроком.";
			return false;
		}
		if (arguments.Count < 2)
		{
			response = "Использование: " + Command + " <Юмы> <Размер> [смещение Y]";
			return false;
		}
		if (!int.TryParse(CollectionExtensions.At<string>(arguments, 0), out var result) || result <= 0)
		{
			response = "Неверное количество Юмов (> 0).";
			return false;
		}
		if (!float.TryParse(CollectionExtensions.At<string>(arguments, 1), out var result2) || result2 <= 0f)
		{
			response = "Неверный размер зоны (радиус > 0).";
			return false;
		}
		float result3 = 0f;
		if (arguments.Count == 3 && !float.TryParse(CollectionExtensions.At<string>(arguments, 2), out result3))
		{
			response = "Неверное значение смещения по высоте.";
			return false;
		}
		Vector3 position = val.Position;
		YumsZoneManager.YumsZone item = new YumsZoneManager.YumsZone
		{
			Position = position,
			Radius = result2,
			YumsAmount = result
		};
		YumsZoneManager.Zones.Add(item);
		response = $"✅ Зона Юмов создана: Юмов: {result}, Радиус: {result2}м, Позиция: {position}. Всего зон: {YumsZoneManager.Zones.Count}.";
		return true;
	}
}
