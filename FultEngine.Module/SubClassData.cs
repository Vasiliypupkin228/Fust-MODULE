using System.Collections.Generic;
using PlayerRoles;

namespace FultEngine.Module;

public class SubClassData
{
	public string Id { get; set; }

	public string CustomInfo { get; set; }

	public RoleTypeId BaseRole { get; set; }

	public List<ItemType> Items { get; set; } = new List<ItemType>();


	public List<ushort> CustomItems { get; set; } = new List<ushort>();


	public string SpawnRoom { get; set; }

	public float Health { get; set; } = 100f;


	public List<SubClassEffect> Effects { get; set; } = new List<SubClassEffect>();


	public string SpawnGroup { get; set; } = "default";


	public string RoundTeam { get; set; } = string.Empty;


	public int MaxPlayers { get; set; } = int.MaxValue;


	public int SpawnPriority { get; set; } = 1;


	public bool AutoSpawn { get; set; } = true;


	public float SpawnChance { get; set; } = 100f;

}
