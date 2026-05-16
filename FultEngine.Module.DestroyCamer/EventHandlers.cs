using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using UnityEngine;

namespace FultEngine.Module.DestroyCamer;

public static class EventHandlers
{
	public static void OnShooting(ShootingEventArgs ev)
	{
		if (ev.Player == (Player)null || !ev.Player.IsAlive)
		{
			return;
		}
		Vector3 position = ev.Player.Position;
		Vector3 forward = ev.Player.CameraTransform.forward;
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(position, forward);
		Plugin plugin = DestroyCamerAPI.GetPlugin();
		Config config = plugin.GetConfig();
		foreach (Camera item in Camera.List)
		{
			if (!((Object)(object)((Component)item.Base).gameObject != (Object)null))
			{
				continue;
			}
			GameObject gameObject = ((Component)item.Base).gameObject;
			Transform val2 = gameObject.transform.Find("Base");
			Transform val3 = gameObject.transform.Find("Arm");
			Transform val4 = gameObject.transform.Find("CamModel");
			if ((Object)(object)val2 != (Object)null || (Object)(object)val3 != (Object)null || (Object)(object)val4 != (Object)null)
			{
				Vector3 position2 = item.Position;
				Vector3 origin = ((Ray)(ref val)).origin;
				Vector3 val5 = ((Ray)(ref val)).direction * Vector3.Distance(((Ray)(ref val)).origin, position2);
				Vector3 direction = ((Ray)(ref val)).direction;
				Vector3 val6 = position2 - ((Ray)(ref val)).origin;
				Vector3 val7 = origin + val5 * Vector3.Dot(direction, ((Vector3)(ref val6)).normalized);
				if (Vector3.Distance(val7, position2) <= config.BreakRadius && Random.value <= config.BreakChance)
				{
					CameraManager.AddBrokenCamera(position2, item.GameObject, config);
				}
			}
		}
	}

	public static void OnScp079ChangingCamera(ChangingCameraEventArgs ev)
	{
		Vector3 cameraPos = ev.Camera.Position;
		if (CameraManager.GetBrokenCameras().Any((Vector3 brokenPos) => Vector3.Distance(brokenPos, cameraPos) <= 0.1f))
		{
			ev.IsAllowed = false;
		}
	}

	public static void OnWaitingForPlayers()
	{
		CameraManager.Clear();
	}
}
