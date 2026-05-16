using System.Collections.Generic;
using UnityEngine;

namespace FultEngine.Module.DestroyCamer;

public static class DestroyCamerAPI
{
	private static Plugin _plugin;

	public static void Initialize(Plugin plugin)
	{
		_plugin = plugin;
	}

	public static void Deinitialize()
	{
		_plugin = null;
	}

	public static HashSet<Vector3> GetBrokenCameras()
	{
		return (_plugin != null) ? CameraManager.GetBrokenCameras() : new HashSet<Vector3>();
	}

	public static void RepairCamera(Vector3 cameraPos)
	{
		if (_plugin != null)
		{
			CameraManager.RepairCamera(cameraPos);
		}
	}

	public static Plugin GetPlugin()
	{
		return _plugin;
	}
}
