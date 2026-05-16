using Exiled.API.Features;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace ProjectMER.Features.Extensions;

public static class ProjectMerExiledCullingExtensions
{
	public static void SpawnSchematic(this Player player, SchematicObject schematic)
	{
		if (!(player == (Player)null) && !((Object)(object)schematic == (Object)null))
		{
			Player val = Player.Get(player.ReferenceHub);
			if (val != null)
			{
				CullingExtensions.SpawnSchematic(val, schematic);
			}
		}
	}

	public static void DestroySchematic(this Player player, SchematicObject schematic)
	{
		if (!(player == (Player)null) && !((Object)(object)schematic == (Object)null))
		{
			Player val = Player.Get(player.ReferenceHub);
			if (val != null)
			{
				CullingExtensions.DestroySchematic(val, schematic);
			}
		}
	}
}
