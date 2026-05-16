using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems;

public static class YakorZoneManager
{
	public class YakorZone
	{
		public Vector3 Position { get; set; }

		public float Radius { get; set; } = 15f;

	}

	public const int MinSuppressedYums = 1;

	public const int MaxSuppressedYums = 5;

	public static List<YakorZone> ActiveYakors { get; } = new List<YakorZone>();


	public static bool IsPlayerInSuppressionZone(Player player)
	{
		if (player == (Player)null || (Object)(object)player.GameObject == (Object)null)
		{
			return false;
		}
		Vector3 position = player.Position;
		foreach (YakorZone activeYakor in ActiveYakors)
		{
			if (Vector3.Distance(position, activeYakor.Position) <= activeYakor.Radius)
			{
				return true;
			}
		}
		return false;
	}
}
