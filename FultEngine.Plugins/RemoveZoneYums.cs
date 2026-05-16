using System;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

namespace FultEngine.Plugins;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class RemoveZoneYums : ICommand, IUsageProvider
{
	public string Command => "removezoneyums";

	public string[] Aliases => new string[2] { "rmzoneyums", "rmzy" };

	public string Description => "Удалить ближайшую зону с юмами или все зоны.";

	public string[] Usage => new string[1] { "all/nearest" };

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player val = Player.Get(sender);
		if (val == (Player)null)
		{
			response = "Команда может быть выполнена только игроком.";
			return false;
		}
		if (arguments.Count != 1)
		{
			response = "Использование: " + Command + " <all/nearest>";
			return false;
		}
		string text = CollectionExtensions.At<string>(arguments, 0).ToLower();
		if (text == "all")
		{
			int count = YumsZoneManager.Zones.Count;
			YumsZoneManager.Zones.Clear();
			response = $"✅ Удалено {count} зон Юмов.";
			return true;
		}
		if (text == "nearest")
		{
			YumsZoneManager.YumsZone yumsZone = null;
			float num = float.MaxValue;
			Vector3 position = val.Position;
			foreach (YumsZoneManager.YumsZone zone in YumsZoneManager.Zones)
			{
				float num2 = Vector3.SqrMagnitude(position - zone.Position);
				if (num2 < num)
				{
					num = num2;
					yumsZone = zone;
				}
			}
			if (yumsZone != null)
			{
				YumsZoneManager.Zones.Remove(yumsZone);
				response = $"✅ Ближайшая зона Юмов (Радиус: {yumsZone.Radius}, Юмов: {yumsZone.YumsAmount}) удалена.";
				return true;
			}
			response = "Не найдено ближайших зон Юмов.";
			return false;
		}
		response = "Неверный аргумент. Использование: <all/nearest>";
		return false;
	}
}
