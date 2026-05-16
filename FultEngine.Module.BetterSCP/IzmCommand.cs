using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core;
using Exiled.API.Features.Roles;
using MEC;
using UnityEngine;

namespace FultEngine.Module.BetterSCP;

[CommandHandler(typeof(ClientCommandHandler))]
public class IzmCommand : ICommand
{
	private static readonly Dictionary<ReferenceHub, Vector3> _lastPositions = new Dictionary<ReferenceHub, Vector3>();

	public string Command { get; } = "izm";


	public string Description { get; } = "Телепортирует SCP-106 в Измерение или обратно.";


	public string[] Aliases { get; } = new string[1] { "pocket" };


	public static void ClearStaticState()
	{
		ClearPositions();
		StalkCommand.ClearCooldowns();
	}

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player player = Player.Get(sender);
		if (player == (Player)null)
		{
			response = "Ошибка: игрок не найден.";
			return false;
		}
		if ((int)player.Role.Type != 3)
		{
			response = "Ошибка: вы должны быть SCP-106.";
			return false;
		}
		if ((int)player.CurrentRoom.Type == 45)
		{
			if (_lastPositions.TryGetValue(player.ReferenceHub, out var returnPosition))
			{
				Scp106Role val = ((TypeCastObject<Role>)(object)player.Role).As<Scp106Role>();
				if ((Role)(object)val != (Role)null)
				{
					val.IsSubmerged = true;
				}
				Timing.CallDelayed(0.5f, (Action)delegate
				{
					player.Position = returnPosition;
				});
				_lastPositions.Remove(player.ReferenceHub);
				response = "Вы телепортированы обратно.";
				return true;
			}
			response = "Ошибка: позиция возврата не найдена.";
			return false;
		}
		Scp106Role val2 = ((TypeCastObject<Role>)(object)player.Role).As<Scp106Role>();
		if ((Role)(object)val2 != (Role)null)
		{
			val2.IsSubmerged = true;
		}
		Room val3 = Room.Get((RoomType)45);
		if ((Object)(object)val3 == (Object)null)
		{
			response = "Ошибка: Pocket Dimension не найден на карте.";
			return false;
		}
		_lastPositions[player.ReferenceHub] = player.Position;
		player.Position = val3.Position + Vector3.up;
		response = "Вы телепортированы в Измерение.";
		return true;
	}

	public static void ClearPositions()
	{
		_lastPositions.Clear();
	}
}
