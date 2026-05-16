using UnityEngine;

namespace FultEngine.Module.Pets;

public class Config
{
	public bool IsEnabled { get; set; } = true;


	public float SpawnDelay { get; set; } = 0.1f;


	public Vector3? PetOffset { get; set; } = Vector3.up;


	public Quaternion? PetRotation { get; set; } = Quaternion.identity;


	public Vector3? PetScale { get; set; } = Vector3.one;

}
