using UnityEngine;

namespace FultEngine.Module.Ventilation;

public class Vector3Config
{
	public float X { get; set; }

	public float Y { get; set; }

	public float Z { get; set; }

	public Vector3Config()
	{
	}

	public Vector3Config(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public Vector3 ToVector3()
	{
		return new Vector3(X, Y, Z);
	}
}
