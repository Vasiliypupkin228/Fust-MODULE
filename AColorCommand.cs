using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AColorCommand : ICommand
{
	public string Command { get; } = "acolor";


	public string[] Aliases { get; } = Array.Empty<string>();


	public string Description { get; } = "Изменяет цвет освещения во всём комплексе (rgb или hex).";


	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player val = Player.Get(sender);
		if (val == (Player)null || !Permissions.CheckPermission(sender, "RedEngine.acolor"))
		{
			response = "<b><color=#ff3b00ce>❗ У вас нет прав для использования этой команды. <color=#00ff84ae>`RedEngine.acolor`</color> <color=#ff3b00ce>❗</color></b>";
			return false;
		}
		if (arguments.Count < 1)
		{
			response = "<b><color=#ff3b00ce>❗ Использование: acolor <R G B> или acolor <HEX> ❗</color></b>";
			return false;
		}
		if (arguments.ElementAt(0).StartsWith("#", StringComparison.OrdinalIgnoreCase))
		{
			Color val2 = default(Color);
			if (ColorUtility.TryParseHtmlString(arguments.ElementAt(0), ref val2))
			{
				Map.ChangeLightsColor(val2);
				response = "<b><color=#00ff1a70>Цвет освещения изменён на " + arguments.ElementAt(0) + ".</color></b>";
				return true;
			}
			response = "<b><color=#ff3b00ce>❗ Неверный HEX формат! Используй: #RRGGBB ❗</color></b>";
			return false;
		}
		if (arguments.Count >= 3)
		{
			if (byte.TryParse(arguments.ElementAt(0), out var result) && byte.TryParse(arguments.ElementAt(1), out var result2) && byte.TryParse(arguments.ElementAt(2), out var result3))
			{
				Color val2 = Color32.op_Implicit(new Color32(result, result2, result3, byte.MaxValue));
				Map.ChangeLightsColor(val2);
				response = $"<b><color=#00ff1a70>Цвет освещения изменён на RGB({result}, {result2}, {result3}).</color></b>";
				return true;
			}
			response = "<b><color=#ff3b00ce>❗ Неверные значения RGB! Используй числа от 0 до 255 ❗</color></b>";
			return false;
		}
		response = "<b><color=#ff3b00ce>❗ Использование: acolor <R G B> или acolor <HEX> ❗</color></b>";
		return false;
	}
}
