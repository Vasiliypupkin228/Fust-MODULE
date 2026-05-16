using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;

namespace FultEngine.ModuleCommand.Admin;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SpawnGBR : ICommand
{
	public string Command => "Dspl";

	public string[] Aliases => new string[2] { "dspling", "dspls" };

	public string Description => "Команда для отображения подсказки всем игрокам с настраиваемыми параметрами";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (arguments.Count < 4)
		{
			response = "Ошибка: укажите время (в секундах), текст, X и Y координаты. Пример: !Hint 10 \"Welcome to GOC!\" 0 0";
			return false;
		}
		if (!float.TryParse(CollectionExtensions.At<string>(arguments, 0), out var result) || result <= 0f)
		{
			response = "Ошибка: время должно быть положительным числом (в секундах).";
			return false;
		}
		string text = string.Join(" ", arguments.Skip(1).Take(arguments.Count - 3)).Trim();
		if (string.IsNullOrEmpty(text))
		{
			response = "Ошибка: текст подсказки не может быть пустым.";
			return false;
		}
		if (!int.TryParse(CollectionExtensions.At<string>(arguments, arguments.Count - 2), out var result2))
		{
			response = "Ошибка: X-координата должна быть целым числом.";
			return false;
		}
		if (!int.TryParse(CollectionExtensions.At<string>(arguments, arguments.Count - 1), out var result3))
		{
			response = "Ошибка: Y-координата должна быть целым числом.";
			return false;
		}
		try
		{
			foreach (Player item in Player.List)
			{
				if (!(item == (Player)null))
				{
					item.ShowMeowHint(result, text, (HintVerticalAlign)0, result3, result2, (HintAlignment)2);
				}
			}
			response = $"Подсказка отправлена всем игрокам: \"{text}\" (длительность: {result} сек, X: {result2}, Y: {result3}).";
			return true;
		}
		catch (Exception ex)
		{
			response = "Ошибка при отправке подсказки: " + ex.Message;
			return false;
		}
	}
}
