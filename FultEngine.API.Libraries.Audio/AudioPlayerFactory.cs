using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace FultEngine.API.Libraries.Audio;

public static class AudioPlayerFactory
{
	private const string GlobalKey = "GlobalAudioPlayer";

	private const string SpeakerName = "Main";

	private static readonly Dictionary<Vector3, GameObject> AudioObjects = new Dictionary<Vector3, GameObject>();

	private static readonly Dictionary<string, double> LastPlay = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

	private static bool _safeGlobalMode = false;

	public static bool SafeGlobalMode => _safeGlobalMode;

	public static void SetSafeGlobalMode(bool enabled)
	{
		_safeGlobalMode = enabled;
		Log.Warn("[FULT-ENGINE.Audio] MER restore audio работает в ECLIPSE-compatible режиме. SafeGlobalMode=" + enabled);
	}

	public static void CreateGlobal(string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		PlayGlobal2D(clipName, volume, loop, destroyOnEnd, "CreateGlobal");
	}

	public static void CreateGlobalAudio(string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		CreateGlobal(clipName, volume, loop, destroyOnEnd);
	}

	public static void CreateForPlayer(Player player, string clipName, float min = 0f, float max = 15f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Audio] CreateForPlayer: player null");
		}
		else
		{
			PlayForPlayerLegacy(player, clipName, min, max, volume, loop, destroyOnEnd, "CreateForPlayer");
		}
	}

	public static bool CreateLocal2DForPlayer(Player player, string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Audio] CreateLocal2DForPlayer: player null");
			return false;
		}
		return PlayForPlayerLegacy(player, clipName, 0f, 15f, volume, loop, destroyOnEnd, "CreateLocal2DForPlayer");
	}

	public static bool CreateLocalForPlayer(Player player, string clipName, float volume = 1f, bool loop = false, bool destroyOnEnd = true, float fallbackMaxDistance = 0.1f)
	{
		return CreateLocal2DForPlayer(player, clipName, volume, loop, destroyOnEnd);
	}

	public static void CreateForGameObject(GameObject gameObject, string clipName, float min = 5f, float max = 20f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if ((Object)(object)gameObject == (Object)null)
		{
			Log.Error("[FULT-ENGINE.Audio] CreateForGameObject: gameObject null");
		}
		else
		{
			PlayForGameObjectLegacy(gameObject, clipName, min, max, volume, loop, destroyOnEnd, "CreateForGameObject");
		}
	}

	public static void CreateForSchematic(SchematicObject schematic, string clipName, float min = 5f, float max = 20f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if ((Object)(object)schematic == (Object)null || (Object)(object)((Component)schematic).gameObject == (Object)null)
		{
			Log.Error("[FULT-ENGINE.Audio] CreateForSchematic: schematic null");
		}
		else
		{
			PlayForGameObjectLegacy(((Component)schematic).gameObject, clipName, min, max, volume, loop, destroyOnEnd, "CreateForSchematic:" + schematic.Name);
		}
	}

	public static void CreateForRoom(Room room, string clipName, float min = 5f, float max = 20f, float volume = 1f, bool loop = false, bool destroyOnEnd = true)
	{
		if ((Object)(object)room == (Object)null)
		{
			Log.Error("[FULT-ENGINE.Audio] CreateForRoom: room null");
		}
		else
		{
			PlayAtPositionLegacy("Room_" + room.Name, ((Component)room).transform.position, clipName, min, max, volume, loop, destroyOnEnd, "CreateForRoom");
		}
	}

	public static void PlayAudioAtPosition(Vector3 position, string clipName, float minDistance = 5f, float maxDistance = 20f, float volume = 1f)
	{
		clipName = AudioClipRegistry.ResolveClipName(clipName);
		if (EnsureReady(clipName, "PlayAudioAtPosition"))
		{
			GameObject val = new GameObject("TempAudio_" + clipName + "_" + ((object)(Vector3)(ref position)).GetHashCode());
			val.transform.position = position;
			AudioObjects[position] = val;
			PlayForGameObjectLegacy(val, clipName, minDistance, maxDistance, volume, loop: false, destroyOnEnd: true, "PlayAudioAtPosition");
		}
	}

	public static void StopAudioAtPosition(Vector3 position)
	{
		if (AudioObjects.TryGetValue(position, out var value) && !((Object)(object)value == (Object)null))
		{
			DestroyForGameObject(value);
			try
			{
				Object.Destroy((Object)(object)value);
			}
			catch
			{
			}
			AudioObjects.Remove(position);
		}
	}

	public static void DestroyForGlobal(string clipName)
	{
		RemoveClipFromKey("GlobalAudioPlayer", AudioClipRegistry.ResolveClipName(clipName));
	}

	public static void DestroyForPlayer(Player player)
	{
		if (!(player == (Player)null))
		{
			DestroyByKey("Player_" + GetPlayerKey(player));
			if ((Object)(object)player.GameObject != (Object)null)
			{
				DestroyByKey("Player_" + ((Object)player.GameObject).GetInstanceID());
			}
		}
	}

	public static void DestroyForGameObject(GameObject gameObject)
	{
		if (!((Object)(object)gameObject == (Object)null))
		{
			DestroyByKey("GameObject_" + ((Object)gameObject).GetInstanceID());
		}
	}

	public static void DestroyForSchematic(SchematicObject schematic)
	{
		if (!((Object)(object)schematic == (Object)null) && !((Object)(object)((Component)schematic).gameObject == (Object)null))
		{
			DestroyForGameObject(((Component)schematic).gameObject);
		}
	}

	public static void DestroyForRoom(Room room)
	{
		if (!((Object)(object)room == (Object)null))
		{
			DestroyByKey("Room_" + room.Name);
		}
	}

	public static void DestroyClipForPlayer(Player player, string clipName)
	{
		if (!(player == (Player)null))
		{
			clipName = AudioClipRegistry.ResolveClipName(clipName);
			RemoveClipFromKey("Player_" + GetPlayerKey(player), clipName);
			if ((Object)(object)player.GameObject != (Object)null)
			{
				RemoveClipFromKey("Player_" + ((Object)player.GameObject).GetInstanceID(), clipName);
			}
		}
	}

	public static void RemoveAllGlobalClips()
	{
		AudioPlayer val = default(AudioPlayer);
		if (AudioPlayer.TryGet("GlobalAudioPlayer", ref val) && !((Object)(object)val == (Object)null))
		{
			TryRemoveAllClips(val);
		}
	}

	public static string GetRuntimeStatus()
	{
		int num = -1;
		try
		{
			num = ((AudioClipStorage.AudioClips == null) ? (-1) : AudioClipStorage.AudioClips.Count);
		}
		catch
		{
		}
		return "mode=MER_ECLIPSE_COMPAT, storage=" + num + ", global={" + BuildPlayerStatus("GlobalAudioPlayer") + "}, base=" + AudioClipRegistry.BaseAudioPath;
	}

	public static bool PlayNuclearGlobalPublic(string clipName, float volume = 3f, bool loop = false, bool destroyOnEnd = true)
	{
		return PlayGlobal2D(clipName, volume, loop, destroyOnEnd, "RA.Global2D");
	}

	public static bool PlayLocal2DPublic(Player player, string clipName, float volume = 3f, bool loop = false, bool destroyOnEnd = true)
	{
		return CreateLocal2DForPlayer(player, clipName, volume, loop, destroyOnEnd);
	}

	public static bool PlaySpatial3DPublic(Player player, string clipName, float min = 0f, float max = 15f, float volume = 3f)
	{
		if (player == (Player)null || (Object)(object)player.GameObject == (Object)null)
		{
			return false;
		}
		return PlayForGameObjectLegacy(player.GameObject, clipName, min, max, volume, loop: false, destroyOnEnd: true, "RA.Spatial3D");
	}

	private static bool PlayGlobal2D(string clipName, float volume, bool loop, bool destroyOnEnd, string source)
	{
		clipName = AudioClipRegistry.ResolveClipName(clipName);
		if (!EnsureReady(clipName, source))
		{
			return false;
		}
		try
		{
			AudioPlayer val = AudioPlayer.CreateOrGet("GlobalAudioPlayer", clipName, (Action<AudioPlayer>)null, false, true, (List<ReferenceHub>)null, byte.MaxValue, (Action<AudioPlayer>)null, (Func<ReferenceHub, bool>)null);
			if ((Object)(object)val == (Object)null)
			{
				Log.Error("[FULT-ENGINE.Audio] " + source + ": CreateOrGet returned null");
				return false;
			}
			Speaker speaker = EnsureSpeaker(val, "Main", spatial: false, 0f, 5000f, volume);
			Configure2D(speaker, volume);
			AddClipSafe(val, clipName, volume, loop, destroyOnEnd);
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("[FULT-ENGINE.Audio] " + source + ": failed to play global clip " + clipName + ": " + ex);
			return false;
		}
	}

	private static bool PlayForPlayerLegacy(Player player, string clipName, float min, float max, float volume, bool loop, bool destroyOnEnd, string source)
	{
		clipName = AudioClipRegistry.ResolveClipName(clipName);
		if (!EnsureReady(clipName, source))
		{
			return false;
		}
		string text = "Player_" + GetPlayerKey(player);
		if (!loop && IsRateLimited(text + ":" + clipName, 0.05))
		{
			return false;
		}
		try
		{
			AudioPlayer val = AudioPlayer.CreateOrGet(text, (string)null, (Action<AudioPlayer>)null, false, true, (List<ReferenceHub>)null, byte.MaxValue, (Action<AudioPlayer>)delegate(AudioPlayer p)
			{
				float num = min;
				float num2 = max;
				Speaker val3 = p.AddSpeaker("Main", volume, false, num, num2);
				Configure2D(val3, volume);
				try
				{
					((Component)val3).transform.parent = player.Transform;
				}
				catch
				{
				}
			}, (Func<ReferenceHub, bool>)null);
			if ((Object)(object)val == (Object)null)
			{
				Log.Error("[FULT-ENGINE.Audio] " + source + ": CreateOrGet returned null for " + player.Nickname);
				return false;
			}
			Speaker val2 = EnsureSpeaker(val, "Main", spatial: false, min, max, volume);
			Configure2D(val2, volume);
			try
			{
				((Component)val2).transform.parent = player.Transform;
			}
			catch
			{
			}
			AddClipSafe(val, clipName, volume, loop, destroyOnEnd);
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("[FULT-ENGINE.Audio] " + source + ": failed to play player clip " + clipName + ": " + ex);
			return false;
		}
	}

	private static bool PlayForGameObjectLegacy(GameObject gameObject, string clipName, float min, float max, float volume, bool loop, bool destroyOnEnd, string source)
	{
		if ((Object)(object)gameObject == (Object)null)
		{
			return false;
		}
		clipName = AudioClipRegistry.ResolveClipName(clipName);
		if (!EnsureReady(clipName, source))
		{
			return false;
		}
		string text = "GameObject_" + ((Object)gameObject).GetInstanceID();
		if (!loop && IsRateLimited(text + ":" + clipName, 0.05))
		{
			return false;
		}
		try
		{
			AudioPlayer val = AudioPlayer.CreateOrGet(text, (string)null, (Action<AudioPlayer>)null, false, true, (List<ReferenceHub>)null, byte.MaxValue, (Action<AudioPlayer>)delegate(AudioPlayer p)
			{
				float num = min;
				float num2 = max;
				Speaker speaker2 = p.AddSpeaker("Main", volume, true, num, num2);
				ConfigureSpatial(speaker2, gameObject.transform, gameObject.transform.position, min, max, volume);
				AudioPositionTracker audioPositionTracker2 = gameObject.GetComponent<AudioPositionTracker>();
				if ((Object)(object)audioPositionTracker2 == (Object)null)
				{
					audioPositionTracker2 = gameObject.AddComponent<AudioPositionTracker>();
				}
				audioPositionTracker2.StartTracking(gameObject, p, speaker2);
			}, (Func<ReferenceHub, bool>)null);
			if ((Object)(object)val == (Object)null)
			{
				Log.Error("[FULT-ENGINE.Audio] " + source + ": CreateOrGet returned null for GameObject " + ((Object)gameObject).name);
				return false;
			}
			Speaker speaker = EnsureSpeaker(val, "Main", spatial: true, min, max, volume);
			ConfigureSpatial(speaker, gameObject.transform, gameObject.transform.position, min, max, volume);
			AudioPositionTracker audioPositionTracker = gameObject.GetComponent<AudioPositionTracker>();
			if ((Object)(object)audioPositionTracker == (Object)null)
			{
				audioPositionTracker = gameObject.AddComponent<AudioPositionTracker>();
			}
			audioPositionTracker.StartTracking(gameObject, val, speaker);
			AddClipSafe(val, clipName, volume, loop, destroyOnEnd);
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("[FULT-ENGINE.Audio] " + source + ": failed to play object clip " + clipName + ": " + ex);
			return false;
		}
	}

	private static bool PlayAtPositionLegacy(string key, Vector3 position, string clipName, float min, float max, float volume, bool loop, bool destroyOnEnd, string source)
	{
		clipName = AudioClipRegistry.ResolveClipName(clipName);
		if (!EnsureReady(clipName, source))
		{
			return false;
		}
		try
		{
			AudioPlayer val = AudioPlayer.CreateOrGet(key, (string)null, (Action<AudioPlayer>)null, false, true, (List<ReferenceHub>)null, byte.MaxValue, (Action<AudioPlayer>)delegate(AudioPlayer p)
			{
				((Component)p).transform.position = position;
				float num = min;
				float num2 = max;
				Speaker speaker2 = p.AddSpeaker("Main", volume, true, num, num2);
				ConfigureSpatial(speaker2, null, position, min, max, volume);
			}, (Func<ReferenceHub, bool>)null);
			if ((Object)(object)val == (Object)null)
			{
				return false;
			}
			((Component)val).transform.position = position;
			Speaker speaker = EnsureSpeaker(val, "Main", spatial: true, min, max, volume);
			ConfigureSpatial(speaker, null, position, min, max, volume);
			AddClipSafe(val, clipName, volume, loop, destroyOnEnd);
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("[FULT-ENGINE.Audio] " + source + ": failed to play position clip " + clipName + ": " + ex);
			return false;
		}
	}

	private static bool EnsureReady(string clipName, string source)
	{
		if (string.IsNullOrWhiteSpace(clipName))
		{
			Log.Error("[FULT-ENGINE.Audio] " + source + ": empty clip name");
			return false;
		}
		if (!AudioClipRegistry.EnsureClipLoaded(clipName))
		{
			Log.Error("[FULT-ENGINE.Audio] " + source + ": clip '" + clipName + "' not loaded");
			return false;
		}
		return true;
	}

	private static Speaker EnsureSpeaker(AudioPlayer audioPlayer, string speakerName, bool spatial, float min, float max, float volume)
	{
		try
		{
			if (audioPlayer.SpeakersByName != null && audioPlayer.SpeakersByName.TryGetValue(speakerName, out var value) && (Object)(object)value != (Object)null)
			{
				return value;
			}
		}
		catch
		{
		}
		bool flag = spatial;
		return audioPlayer.AddSpeaker(speakerName, volume, flag, min, max);
	}

	private static void Configure2D(Speaker speaker, float volume)
	{
		if ((Object)(object)speaker == (Object)null)
		{
			return;
		}
		try
		{
			AudioSource component = ((Component)speaker).GetComponent<AudioSource>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).enabled = true;
				component.spatialBlend = 0f;
				component.playOnAwake = false;
				component.volume = SafeVolume(volume);
			}
		}
		catch
		{
		}
	}

	private static void ConfigureSpatial(Speaker speaker, Transform parent, Vector3 position, float min, float max, float volume)
	{
		if ((Object)(object)speaker == (Object)null)
		{
			return;
		}
		try
		{
			if ((Object)(object)parent != (Object)null)
			{
				((Component)speaker).transform.parent = parent;
				((Component)speaker).transform.localPosition = Vector3.zero;
			}
			else
			{
				((Component)speaker).transform.position = position;
			}
		}
		catch
		{
		}
		try
		{
			AudioSource component = ((Component)speaker).GetComponent<AudioSource>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).enabled = true;
				component.spatialBlend = 1f;
				component.playOnAwake = false;
				component.volume = SafeVolume(volume);
				component.minDistance = min;
				component.maxDistance = max;
			}
		}
		catch
		{
		}
	}

	private static object AddClipSafe(AudioPlayer audioPlayer, string clipName, float volume, bool loop, bool destroyOnEnd)
	{
		if ((Object)(object)audioPlayer == (Object)null)
		{
			return null;
		}
		Type type = ((object)audioPlayer).GetType();
		object obj = TryInvokeAddClip(type, audioPlayer, new object[4]
		{
			clipName,
			SafeVolume(volume),
			loop,
			destroyOnEnd
		});
		if (obj != null)
		{
			return obj;
		}
		obj = TryInvokeAddClip(type, audioPlayer, new object[3] { clipName, loop, destroyOnEnd });
		if (obj != null)
		{
			return obj;
		}
		obj = TryInvokeAddClip(type, audioPlayer, new object[2] { clipName, destroyOnEnd });
		if (obj != null)
		{
			return obj;
		}
		obj = TryInvokeAddClip(type, audioPlayer, new object[1] { clipName });
		if (obj != null)
		{
			return obj;
		}
		Log.Error("[FULT-ENGINE.Audio] AddClip overload not found for clip " + clipName);
		return null;
	}

	private static object TryInvokeAddClip(Type type, AudioPlayer audioPlayer, object[] args)
	{
		try
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.Name != "AddClip")
				{
					continue;
				}
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length != args.Length)
				{
					continue;
				}
				object[] array = new object[args.Length];
				bool flag = true;
				for (int j = 0; j < args.Length; j++)
				{
					object obj = args[j];
					Type parameterType = parameters[j].ParameterType;
					if (obj == null || parameterType.IsInstanceOfType(obj))
					{
						array[j] = obj;
						continue;
					}
					try
					{
						array[j] = Convert.ChangeType(obj, parameterType);
					}
					catch
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return methodInfo.Invoke(audioPlayer, array);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Warn("[FULT-ENGINE.Audio] AddClip reflection failed: " + ex.Message);
		}
		return null;
	}

	private static void RemoveClipFromKey(string key, string clipName)
	{
		AudioPlayer val = default(AudioPlayer);
		if (string.IsNullOrWhiteSpace(key) || !AudioPlayer.TryGet(key, ref val) || (Object)(object)val == (Object)null)
		{
			return;
		}
		try
		{
			val.RemoveClipByName(clipName);
		}
		catch
		{
		}
	}

	private static void DestroyByKey(string key)
	{
		AudioPlayer val = default(AudioPlayer);
		if (string.IsNullOrWhiteSpace(key) || !AudioPlayer.TryGet(key, ref val) || (Object)(object)val == (Object)null)
		{
			return;
		}
		TryRemoveAllClips(val);
		try
		{
			if (val.SpeakersByName != null)
			{
				foreach (Speaker item in val.SpeakersByName.Values.ToList())
				{
					try
					{
						AudioSource component = ((Component)item).GetComponent<AudioSource>();
						if (component != null)
						{
							component.Stop();
						}
					}
					catch
					{
					}
					try
					{
						val.RemoveSpeaker(item.Name);
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			val.Destroy();
		}
		catch
		{
		}
	}

	private static void TryRemoveAllClips(AudioPlayer audioPlayer)
	{
		if ((Object)(object)audioPlayer == (Object)null)
		{
			return;
		}
		try
		{
			MethodInfo method = ((object)audioPlayer).GetType().GetMethod("RemoveAllClips", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(audioPlayer, null);
				return;
			}
		}
		catch
		{
		}
		try
		{
			if (audioPlayer.ClipsById == null)
			{
				return;
			}
			foreach (AudioClipPlayback item in audioPlayer.ClipsById.Values.ToList())
			{
				audioPlayer.RemoveClipById(item.Id);
			}
		}
		catch
		{
		}
	}

	private static bool IsRateLimited(string key, double seconds)
	{
		double num = Time.unscaledTime;
		if (LastPlay.TryGetValue(key, out var value) && num - value < seconds)
		{
			return true;
		}
		LastPlay[key] = num;
		return false;
	}

	private static float SafeVolume(float volume)
	{
		if (float.IsNaN(volume) || float.IsInfinity(volume) || volume <= 0f)
		{
			return 1f;
		}
		if (volume > 10f)
		{
			volume /= 100f;
		}
		return Mathf.Clamp(volume, 0.01f, 1.25f);
	}

	private static string GetPlayerKey(Player player)
	{
		if (player == (Player)null)
		{
			return "null";
		}
		string userId = player.UserId;
		if (!string.IsNullOrWhiteSpace(userId))
		{
			return userId.Replace("@", "_").Replace(":", "_");
		}
		return player.Nickname;
	}

	private static string BuildPlayerStatus(string key)
	{
		try
		{
			AudioPlayer val = default(AudioPlayer);
			if (!AudioPlayer.TryGet(key, ref val) || (Object)(object)val == (Object)null)
			{
				return "exists=False";
			}
			return "exists=True, clips=" + SafeCount(val.ClipsById) + ", speakers=" + SafeCount(val.SpeakersByName);
		}
		catch (Exception ex)
		{
			return "statusError=" + ex.Message;
		}
	}

	private static int SafeCount(object maybeCollection)
	{
		if (maybeCollection == null)
		{
			return -1;
		}
		try
		{
			PropertyInfo property = maybeCollection.GetType().GetProperty("Count");
			return (property == null) ? (-1) : Convert.ToInt32(property.GetValue(maybeCollection, null));
		}
		catch
		{
			return -1;
		}
	}
}
