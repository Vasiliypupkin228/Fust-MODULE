using System;
using System.IO;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace FultEngine.API.Libraries.Audio;

public static class AudioManager
{
	private static readonly string[] SubDirs = new string[23]
	{
		"SoundItem", "DestroyCam", "SFire", "Tizer", "Recontaim/Cell", "Recontaim/Mask", "Recontaim/D-106B", "Alert", "Hack", "Reanimate",
		"NVG", "RPG", "Disguise", "CPP", "Scp", "Fight", "Shield", "Drone", "Donate", "Christmas",
		"Squid", "Ventilation", "Elevator"
	};

	public static bool SafeGlobalMode => AudioPlayerFactory.SafeGlobalMode;

	public static void RegisterAudio()
	{
		string baseAudioPath = AudioClipRegistry.BaseAudioPath;
		string[] subDirs = SubDirs;
		foreach (string path in subDirs)
		{
			string text = Path.Combine(baseAudioPath, path);
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
			}
			catch (Exception ex)
			{
				Log.Warn("[FULT-ENGINE.Audio] Не удалось подготовить папку " + text + ": " + ex.Message);
			}
		}
		AudioClipRegistry.RegisterClips();
	}

	public static void CreateGlobalAudio(string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if (string.IsNullOrWhiteSpace(clipName))
		{
			Log.Error("[FULT-ENGINE.Audio] Имя глобального клипа пустое");
		}
		else
		{
			AudioPlayerFactory.CreateGlobal(clipName, volume, loop, destroyOnEnd);
		}
	}

	public static void CreateForPlayer(Player player, string clipName, float minDistance = 0f, float maxDistance = 15f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Audio] Игрок null");
		}
		else
		{
			AudioPlayerFactory.CreateForPlayer(player, clipName, minDistance, maxDistance, volume, loop, destroyOnEnd);
		}
	}

	public static bool CreateLocalForPlayer(Player player, string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true, float fallbackMaxDistance = 0.1f)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Audio] Игрок null для local audio");
			return false;
		}
		return AudioPlayerFactory.CreateLocalForPlayer(player, clipName, volume, loop, destroyOnEnd, fallbackMaxDistance);
	}

	public static bool CreateLocal2DForPlayer(Player player, string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Audio] Игрок null для local2d audio");
			return false;
		}
		return AudioPlayerFactory.CreateLocal2DForPlayer(player, clipName, volume, loop, destroyOnEnd);
	}

	public static void CreateForSchematic(SchematicObject schematic, string clipName, float minDistance = 0f, float maxDistance = 15f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if ((Object)(object)schematic == (Object)null)
		{
			Log.Error("[FULT-ENGINE.Audio] Schematic null");
		}
		else
		{
			AudioPlayerFactory.CreateForSchematic(schematic, clipName, minDistance, maxDistance, volume, loop, destroyOnEnd);
		}
	}

	public static void CreateForGameObject(GameObject gameObject, string clipName, float minDistance = 5f, float maxDistance = 7f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if ((Object)(object)gameObject == (Object)null)
		{
			Log.Error("[FULT-ENGINE.Audio] GameObject null");
		}
		else
		{
			AudioPlayerFactory.CreateForGameObject(gameObject, clipName, minDistance, maxDistance, volume, loop, destroyOnEnd);
		}
	}

	public static void DestroyForPlayer(Player player)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Audio] Игрок null");
		}
		else
		{
			AudioPlayerFactory.DestroyForPlayer(player);
		}
	}

	public static void DestroyClipForPlayer(Player player, string clipName)
	{
		if (!(player == (Player)null))
		{
			AudioPlayerFactory.DestroyClipForPlayer(player, clipName);
		}
	}

	public static void DestroyForGlobal(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			Log.Error("[FULT-ENGINE.Audio] Название global clip пустое");
		}
		else
		{
			AudioPlayerFactory.DestroyForGlobal(AudioClipRegistry.ResolveClipName(name));
		}
	}

	public static void DestroyForGameObject(GameObject gameObject)
	{
		if ((Object)(object)gameObject == (Object)null)
		{
			Log.Error("[FULT-ENGINE.Audio] GameObject null");
		}
		else
		{
			AudioPlayerFactory.DestroyForGameObject(gameObject);
		}
	}

	public static void SetSafeGlobalMode(bool enabled)
	{
		AudioPlayerFactory.SetSafeGlobalMode(enabled);
	}

	public static string GetRuntimeStatus()
	{
		return AudioPlayerFactory.GetRuntimeStatus();
	}
}
