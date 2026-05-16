using UnityEngine;

namespace FultEngine.Module.Pets;

public class PetData
{
	public string SchematicName { get; set; }

	public string Description { get; set; }

	public Vector3 Offset { get; set; } = Vector3.up;


	public Quaternion Rotation { get; set; } = Quaternion.identity;


	public Vector3 Scale { get; set; } = Vector3.one;


	public PetData(string schematic, string desc)
	{
		SchematicName = schematic;
		Description = desc;
	}
}
