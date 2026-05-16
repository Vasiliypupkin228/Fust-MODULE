using System;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using UnityEngine;

namespace FultEngine.Module.PhysicGraned;

public class Plugin : IFultEngineModule
{
	private Config _config;

	public string Name { get; } = "PhysicGraned";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 0, 1);


	public void OnEnabled()
	{
		Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
	}

	public void OnDisabled()
	{
		Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
	}

	public Type GetConfigType()
	{
		return null;
	}

	public object GetDefaultConfig()
	{
		return new Config();
	}

	public void SetConfig(object config)
	{
	}

	private void OnShot(ShotEventArgs ev)
	{
		RaycastHit raycastHit = ev.RaycastHit;
		if (!((Object)(object)((RaycastHit)(ref raycastHit)).transform != (Object)null))
		{
			return;
		}
		raycastHit = ev.RaycastHit;
		Vector3 point = ((RaycastHit)(ref raycastHit)).point;
		foreach (Pickup item in Pickup.List)
		{
			if ((int)item.Type == 25 || (int)item.Type == 26)
			{
				float num = Vector3.Distance(point, item.Position);
				if ((double)num <= 0.3)
				{
					ExplosiveGrenade val = (ExplosiveGrenade)Item.Create(item.Type, (Player)null);
					val.FuseTime = 0.1f;
					val.SpawnActive(item.Position, ev.Player);
					item.Destroy();
				}
			}
		}
	}
}
