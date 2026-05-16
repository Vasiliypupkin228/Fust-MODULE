using System.Collections.Generic;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;

namespace FultEngine.CustomItems.NVG;

public static class NVGData
{
	public static HashSet<Player> Cooldown { get; } = new HashSet<Player>();


	public static Dictionary<Player, SchematicObject> ActiveSchematics { get; } = new Dictionary<Player, SchematicObject>();

}
