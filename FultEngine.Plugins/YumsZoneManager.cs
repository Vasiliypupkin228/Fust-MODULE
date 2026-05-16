using System.Collections.Generic;
using UnityEngine;

namespace FultEngine.Plugins;

public static class YumsZoneManager
{
	public class YumsZone
	{
		public Vector3 Position { get; set; }

		public float Radius { get; set; }

		public int YumsAmount { get; set; }
	}

	public static List<YumsZone> Zones { get; } = new List<YumsZone>();

}
