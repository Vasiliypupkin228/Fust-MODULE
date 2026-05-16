using Exiled.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems;

public class DroneComponent : MonoBehaviour
{
	public Player Owner { get; set; }

	public float Health { get; set; }

	public float MaxHealth { get; set; }

	public FPV FPVInstance { get; set; }

	public void TakeDamage(float damage)
	{
		Health -= damage;
		if (Health <= 0f)
		{
			Health = 0f;
			if (Owner != (Player)null && FPVInstance != null)
			{
				FPVInstance.StopFlight(Owner, forced: true, exploded: true, teleportBack: true);
			}
		}
	}
}
