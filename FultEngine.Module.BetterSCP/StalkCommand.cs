using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Core;
using Exiled.API.Features.Roles;
using MEC;
using UnityEngine;

namespace FultEngine.Module.BetterSCP;

[CommandHandler(typeof(ClientCommandHandler))]
public class StalkCommand : ICommand
{
	private static readonly float CooldownDuration = 180f;

	private static readonly Dictionary<string, float> LastUsed = new Dictionary<string, float>();

	public string Command { get; } = "stalk";


	public string Description { get; } = "Телепортирует SCP-106 к случайному игроку.";


	public string[] Aliases { get; } = new string[1] { "st" };


	public static void ClearCooldowns()
	{
		LastUsed.Clear();
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
		if (LastUsed.TryGetValue(player.UserId, out var value) && Time.time < value + CooldownDuration)
		{
			response = $"Ошибка: команда на кулдауне. Подождите {Mathf.CeilToInt(value + CooldownDuration - Time.time)} секунд.";
			return false;
		}
		List<Player> list = Player.List.Where((Player p) => p.IsAlive && (int)p.Role.Type != 3 && (int)p.Role.Type != 2).ToList();
		if (list.Count == 0)
		{
			response = "Ошибка: нет доступных игроков для телепортации.";
			return false;
		}
		Player target = list[Random.Range(0, list.Count)];
		Scp106Role val = ((TypeCastObject<Role>)(object)player.Role).As<Scp106Role>();
		if ((Role)(object)val != (Role)null)
		{
			val.IsSubmerged = true;
		}
		Timing.CallDelayed(1f, (Action)delegate
		{
			if (!(player == (Player)null) && player.IsAlive && (int)player.Role.Type == 3 && !(target == (Player)null) && target.IsAlive)
			{
				player.Position = target.Position;
			}
		});
		LastUsed[player.UserId] = Time.time;
		response = "Телепортирован к " + target.Nickname + "!";
		return true;
	}
}
