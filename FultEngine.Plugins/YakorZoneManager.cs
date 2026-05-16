using System.Collections.Generic;
using UnityEngine;

namespace FultEngine.Plugins;

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

}
