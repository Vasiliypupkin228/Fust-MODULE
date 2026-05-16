using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using FultEngine.API.Libraries.Audio;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.Module.DestroyCamer;

public static class CameraManager
{
	private static readonly HashSet<Vector3> BrokenCameras = new HashSet<Vector3>();

	private static readonly Dictionary<Vector3, CoroutineHandle> CameraAudioCoroutines = new Dictionary<Vector3, CoroutineHandle>();

	public static void RepairCamera(Vector3 cameraPos)
	{
		try
		{
			if (!BrokenCameras.Contains(cameraPos))
			{
				return;
			}
			Camera val = ((IEnumerable<Camera>)Camera.List).FirstOrDefault((Func<Camera, bool>)((Camera c) => Vector3.Distance(c.Position, cameraPos) <= 0.1f));
			if (val != null && (Object)(object)val.GameObject != (Object)null)
			{
				try
				{
					AudioManager.DestroyForGameObject(val.GameObject);
				}
				catch (Exception ex)
				{
					Log.Warn($"Failed to destroy audio for camera at {cameraPos}: {ex.Message}");
				}
			}
			BrokenCameras.Remove(cameraPos);
			if (CameraAudioCoroutines.TryGetValue(cameraPos, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				CameraAudioCoroutines.Remove(cameraPos);
			}
		}
		catch (Exception arg)
		{
			Log.Error($"Failed to repair camera at {cameraPos}: {arg}");
		}
	}

	public static HashSet<Vector3> GetBrokenCameras()
	{
		return BrokenCameras;
	}

	public static void AddBrokenCamera(Vector3 cameraPos, GameObject cameraGO, Config config)
	{
		if (!BrokenCameras.Contains(cameraPos))
		{
			SchematicObject val = ObjectSpawner.SpawnSchematic("DestroyCamPartcile", cameraPos, (Quaternion?)Quaternion.Euler(cameraGO.transform.eulerAngles), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			BrokenCameras.Add(cameraPos);
			if (CameraAudioCoroutines.ContainsKey(cameraPos))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { CameraAudioCoroutines[cameraPos] });
				CameraAudioCoroutines.Remove(cameraPos);
			}
		}
	}

	public static void Clear()
	{
		foreach (CoroutineHandle value in CameraAudioCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		CameraAudioCoroutines.Clear();
		BrokenCameras.Clear();
	}
}
