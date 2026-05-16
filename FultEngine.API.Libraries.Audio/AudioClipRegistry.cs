using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;

namespace FultEngine.API.Libraries.Audio;

public static class AudioClipRegistry
{
	private static readonly object Sync = new object();

	private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		["hackProcess"] = "hackDoor",
		["HackProcess"] = "hackDoor"
	};

	private static readonly (string SubDir, string FileName, string ClipName)[] KnownClips = new(string, string, string)[57]
	{
		("SoundItem", "DropArmor.ogg", "DropArmor"),
		("SoundItem", "TakeArmor.ogg", "TakeArmor"),
		("SoundItem", "TakeCard.ogg", "TakeCard"),
		("SoundItem", "FlashGun.ogg", "FlashGun"),
		("SoundItem", "TakeGun.ogg", "TakeGun"),
		("SoundItem", "DropGun.ogg", "DropGun"),
		("SoundItem", "ammodt.ogg", "Ammodt"),
		("SoundItem", "Castrulya.ogg", "Castrulya"),
		("SoundItem", "Shot.ogg", "Shot"),
		("SoundItem", "Cpc.ogg", "Cpc"),
		("DestroyCam", "ElectricCamera.ogg", "CamDestroy"),
		("SFire", "error.ogg", "error"),
		("SFire", "next.ogg", "next"),
		("Tizer", "FireTizer.ogg", "FireTizer"),
		("Tizer", "Tizer.ogg", "Tizer"),
		("Recontaim/Cell", "cell.ogg", "cell"),
		("Recontaim/Cell", "popal1.ogg", "popal1"),
		("Recontaim/Cell", "destroy.ogg", "destroy"),
		("Recontaim/Mask", "bag.ogg", "bag"),
		("Recontaim/D-106B", "IntercomON.ogg", "ion"),
		("Recontaim/D-106B", "IntercomOFF.ogg", "ioff"),
		("Recontaim/D-106B", "MagnetUp.ogg", "mu"),
		("Recontaim/D-106B", "MagnetDown.ogg", "md"),
		("Recontaim/D-106B", "FemurBreaker.ogg", "fb"),
		("Alert", "alert.ogg", "alert"),
		("Hack", "WinHack.ogg", "hackWIN"),
		("Hack", "HackDoor.ogg", "hackDoor"),
		("Reanimate", "Start.ogg", "ReanimateStart"),
		("Reanimate", "End.ogg", "ReanimateEnd"),
		("Reanimate", "StopHeardChancdeOver.ogg", "ReanimateGameOver"),
		("NVG", "Enable.ogg", "EnableNVG"),
		("NVG", "Disable.ogg", "DisableNVG"),
		("NVG", "Recoil.ogg", "RecoilNVG"),
		("RPG", "ShotRPG.ogg", "ShotRPG"),
		("RPG", "Svist.ogg", "Svist"),
		("Disguise", "Masking.ogg", "Masking"),
		("CPP", "CPPFail.ogg", "CPPFail"),
		("CPP", "CPPFull.ogg", "CPPFull"),
		("Scp", "Whisper035.ogg", "Whisper035"),
		("Fight", "OverUdar.ogg", "OverUdar"),
		("Fight", "Udar.ogg", "Udar"),
		("Fight", "Talk.ogg", "Talk"),
		("Shield", "Shield.ogg", "shield"),
		("Shield", "UpShield.ogg", "UpShield"),
		("Christmas", "JingleBelsRock.ogg", "Christmas"),
		("Drone", "drone.ogg", "drone"),
		("Squid", "Green.ogg", "SquidGreen"),
		("Squid", "Kukla.ogg", "SquidKukla"),
		("Squid", "Leave.ogg", "SquidLeave"),
		("Squid", "MinglHard.ogg", "SquidMinglHard"),
		("Squid", "Noch.ogg", "SquidNoch"),
		("Squid", "Red.ogg", "SquidRed"),
		("Squid", "Shot.ogg", "SquidShot"),
		("Squid", "Skakalka.ogg", "SquidSkakalka"),
		("Squid", "StartGlobal.ogg", "SquidStartGlobal"),
		("Squid", "WaitRoom.ogg", "SquidWaitRoom"),
		("Ventilation", "VentAmbience.ogg", "VentAmbience")
	};

	private static readonly Dictionary<string, string> ClipPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private static string _baseAudioPath;

	private static bool _initialized;

	public static string BaseAudioPath
	{
		get
		{
			EnsureInitialized();
			return _baseAudioPath;
		}
	}

	public static IReadOnlyDictionary<string, string> RegisteredClips
	{
		get
		{
			EnsureInitialized();
			return ClipPaths;
		}
	}

	public static void RegisterClips()
	{
		RegisterClips(preloadExistingClips: true);
	}

	public static void RegisterClips(bool preloadExistingClips)
	{
		lock (Sync)
		{
			_baseAudioPath = ResolveAudioPath();
			Directory.CreateDirectory(_baseAudioPath);
			ClipPaths.Clear();
			int num = 0;
			(string, string, string)[] knownClips = KnownClips;
			for (int i = 0; i < knownClips.Length; i++)
			{
				(string, string, string) tuple = knownClips[i];
				string text = Path.Combine(_baseAudioPath, tuple.Item1, tuple.Item2);
				if (File.Exists(text))
				{
					ClipPaths[tuple.Item3] = text;
					num++;
				}
			}
			int num2 = RegisterAllOggFilesDynamically();
			int num3 = 0;
			int num4 = 0;
			_initialized = true;
			if (preloadExistingClips)
			{
				foreach (string item in ClipPaths.Keys.ToList())
				{
					if (EnsureClipLoadedInternal(item, logMissing: true))
					{
						num3++;
					}
					else
					{
						num4++;
					}
				}
			}
			Log.Info($"[FULT-ENGINE.Audio] Каталог клипов готов. База: {_baseAudioPath}. Known: {num}/{KnownClips.Length}. Dynamic: {num2}. Загружено в storage: {num3}. Ошибок: {num4}. Сейчас в storage: {AudioClipStorage.AudioClips.Count}");
			if (ClipPaths.TryGetValue("VentAmbience", out var value))
			{
				Log.Info(string.Format("[FULT-ENGINE.Audio] VentAmbience найден: {0}. В storage: {1}", value, AudioClipStorage.AudioClips.ContainsKey("VentAmbience")));
			}
		}
	}

	public static string ResolveClipName(string clipName)
	{
		if (string.IsNullOrWhiteSpace(clipName))
		{
			return clipName;
		}
		string value;
		return Aliases.TryGetValue(clipName, out value) ? value : clipName;
	}

	public static bool EnsureClipLoaded(string clipName)
	{
		EnsureInitialized();
		return EnsureClipLoadedInternal(ResolveClipName(clipName), logMissing: true);
	}

	public static bool TryGetPath(string clipName, out string fullPath)
	{
		EnsureInitialized();
		return ClipPaths.TryGetValue(ResolveClipName(clipName), out fullPath);
	}

	public static string BuildStatus()
	{
		EnsureInitialized();
		string[] value = ClipPaths.Keys.OrderBy<string, string>((string x) => x, StringComparer.OrdinalIgnoreCase).Take(20).ToArray();
		return "BaseAudioPath: " + _baseAudioPath + "\n" + $"Registered paths: {ClipPaths.Count}\n" + $"Storage clips: {AudioClipStorage.AudioClips.Count}\n" + "First registered: " + string.Join(", ", value);
	}

	public static string InspectClip(string clipName)
	{
		EnsureInitialized();
		string text = ResolveClipName(clipName);
		if (string.IsNullOrWhiteSpace(text))
		{
			return "Clip name is empty.";
		}
		string value;
		bool flag = ClipPaths.TryGetValue(text, out value);
		bool flag2 = flag && File.Exists(value);
		bool flag3 = AudioClipStorage.AudioClips.ContainsKey(text);
		string text2 = "Clip: " + text + "\n" + $"Path known: {flag}\n" + "Path: " + (flag ? value : "-") + "\n" + $"File exists: {flag2}\n" + $"Storage loaded: {flag3}\n";
		if (!flag3)
		{
			return text2;
		}
		try
		{
			object obj = AudioClipStorage.AudioClips[text];
			if (obj == null)
			{
				return text2 + "Storage object: null\n";
			}
			Type type = obj.GetType();
			text2 = text2 + "Storage object type: " + type.FullName + "\n";
			PropertyInfo[] array = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where p.GetIndexParameters().Length == 0
				select p).Take(40).ToArray();
			PropertyInfo[] array2 = array;
			foreach (PropertyInfo propertyInfo in array2)
			{
				try
				{
					object value2 = propertyInfo.GetValue(obj, null);
					if (value2 != null)
					{
						string text3 = value2.ToString();
						if (text3.Length > 120)
						{
							text3 = text3.Substring(0, 120) + "...";
						}
						text2 = text2 + "Property " + propertyInfo.Name + ": " + text3 + "\n";
					}
				}
				catch
				{
				}
			}
			FieldInfo[] array3 = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Take(40).ToArray();
			FieldInfo[] array4 = array3;
			foreach (FieldInfo fieldInfo in array4)
			{
				try
				{
					object value3 = fieldInfo.GetValue(obj);
					if (value3 != null)
					{
						string text4 = value3.ToString();
						if (text4.Length > 120)
						{
							text4 = text4.Substring(0, 120) + "...";
						}
						text2 = text2 + "Field " + fieldInfo.Name + ": " + text4 + "\n";
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception ex)
		{
			text2 = text2 + "Inspect reflection error: " + ex.Message + "\n";
		}
		return text2;
	}

	private static int RegisterAllOggFilesDynamically()
	{
		if (!Directory.Exists(_baseAudioPath))
		{
			return 0;
		}
		int num = 0;
		string[] files = Directory.GetFiles(_baseAudioPath, "*.ogg", SearchOption.AllDirectories);
		foreach (string text in files)
		{
			if (!ShouldSkipDynamicAudioFile(text))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension) && !ClipPaths.ContainsKey(fileNameWithoutExtension))
				{
					ClipPaths[fileNameWithoutExtension] = text;
					num++;
				}
			}
		}
		return num;
	}

	private static bool ShouldSkipDynamicAudioFile(string file)
	{
		if (string.IsNullOrWhiteSpace(file))
		{
			return false;
		}
		string text = file.Replace('\\', '/');
		return text.IndexOf("/MVPMusic/", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("/MVP_Music/", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("/MVP-Music/", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private static bool EnsureClipLoadedInternal(string clipName, bool logMissing)
	{
		if (string.IsNullOrWhiteSpace(clipName))
		{
			return false;
		}
		if (AudioClipStorage.AudioClips.ContainsKey(clipName))
		{
			return true;
		}
		if (!ClipPaths.TryGetValue(clipName, out var value))
		{
			if (logMissing)
			{
				Log.Error("[FULT-ENGINE.Audio] Аудиоклип '" + clipName + "' не зарегистрирован. База: " + _baseAudioPath);
			}
			return false;
		}
		if (!File.Exists(value))
		{
			Log.Error("[FULT-ENGINE.Audio] Файл клипа '" + clipName + "' не найден: " + value);
			return false;
		}
		try
		{
			AudioClipStorage.LoadClip(value, clipName);
			bool flag = AudioClipStorage.AudioClips.ContainsKey(clipName);
			if (!flag)
			{
				Log.Error("[FULT-ENGINE.Audio] AudioClipStorage не сохранил '" + clipName + "' после LoadClip: " + value);
			}
			return flag;
		}
		catch (Exception arg)
		{
			Log.Error($"[FULT-ENGINE.Audio] Ошибка загрузки '{clipName}' из {value}: {arg}");
			return false;
		}
	}

	private static void EnsureInitialized()
	{
		if (!_initialized)
		{
			RegisterClips(preloadExistingClips: true);
		}
	}

	private static string ResolveAudioPath()
	{
		string[] array = new string[5]
		{
			Path.Combine(Paths.Plugins, "FULT-ENGINE", "Audio"),
			Path.Combine(Paths.Plugins, "Fust-MODULE", "Audio"),
			Path.Combine(Paths.Plugins, "FultEngine", "Audio"),
			Path.Combine(Paths.Plugins, "FultModule", "Audio"),
			Path.Combine(Paths.Plugins, "RedModule", "Audio")
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (Directory.Exists(text))
			{
				return text;
			}
		}
		return array[0];
	}
}
