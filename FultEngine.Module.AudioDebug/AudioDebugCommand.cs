using System;
using System.Linq;
using System.Reflection;
using CommandSystem;
using Exiled.API.Features;
using FultEngine.API.Libraries.Audio;

namespace FultEngine.Module.AudioDebug;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class AudioDebugCommand : ICommand
{
	public string Command => "fult_audio";

	public string[] Aliases => new string[2] { "faudio", "audiofix" };

	public string Description => "Диагностика и режимы FULT-ENGINE.Audio v13.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		string[] array = Enumerable.ToArray(arguments);
		if (array.Length == 0)
		{
			response = Help();
			return true;
		}
		string text = array[0].ToLowerInvariant();
		string text2 = ((array.Length >= 2) ? array[1] : "TakeCard");
		try
		{
			switch (text)
			{
			case "status":
				response = Status();
				return true;
			case "reload":
				AudioClipRegistry.RegisterClips();
				response = Status();
				return true;
			case "inspect":
				response = Inspect(text2);
				return true;
			case "mode":
				response = Mode(array);
				return true;
			case "global":
			case "rawglobal":
			case "nuclear":
			{
				float volume2 = ParseFloat(array, 2, 3f);
				bool flag4 = AudioPlayerFactory.PlayNuclearGlobalPublic(text2, volume2);
				response = "GLOBAL2D " + (flag4 ? "запущен" : "не запущен") + ": " + text2 + ", volume=" + volume2;
				return flag4;
			}
			case "self2d":
			case "local":
			case "local2d":
			{
				Player val3 = Player.Get(sender);
				if (val3 == (Player)null)
				{
					response = "Команду self2d/local2d нужно запускать от игрока в RA.";
					return false;
				}
				float volume3 = ParseFloat(array, 2, 3f);
				bool flag5 = AudioPlayerFactory.PlayLocal2DPublic(val3, text2, volume3);
				response = "LOCAL2D " + (flag5 ? "запущен" : "не запущен") + ": " + text2 + ", volume=" + volume3;
				return flag5;
			}
			case "self3d":
			case "raw3d":
			case "spatial":
			{
				Player val4 = Player.Get(sender);
				if (val4 == (Player)null)
				{
					response = "Команду self3d/raw3d нужно запускать от игрока в RA.";
					return false;
				}
				float volume4 = ParseFloat(array, 2, 3f);
				bool flag6 = AudioPlayerFactory.PlaySpatial3DPublic(val4, text2, 0f, 15f, volume4);
				response = "SPATIAL3D test " + (flag6 ? "запущен" : "не запущен") + ": " + text2 + ", volume=" + volume4;
				return flag6;
			}
			case "play":
			{
				string text3 = ((array.Length >= 3) ? array[2].ToLowerInvariant() : "global");
				float volume = ParseFloat(array, 3, 3f);
				if (text3 == "self2d" || text3 == "local" || text3 == "local2d")
				{
					Player val = Player.Get(sender);
					if (val == (Player)null)
					{
						response = "play self2d нужно запускать от игрока в RA.";
						return false;
					}
					bool flag = AudioPlayerFactory.PlayLocal2DPublic(val, text2, volume);
					response = "PLAY LOCAL2D " + (flag ? "ok" : "fail") + ": " + text2;
					return flag;
				}
				if (text3 == "self3d" || text3 == "raw3d" || text3 == "spatial")
				{
					Player val2 = Player.Get(sender);
					if (val2 == (Player)null)
					{
						response = "play self3d нужно запускать от игрока в RA.";
						return false;
					}
					bool flag2 = AudioPlayerFactory.PlaySpatial3DPublic(val2, text2, 0f, 15f, volume);
					response = "PLAY SPATIAL3D " + (flag2 ? "ok" : "fail") + ": " + text2;
					return flag2;
				}
				bool flag3 = AudioPlayerFactory.PlayNuclearGlobalPublic(text2, volume);
				response = "PLAY GLOBAL2D " + (flag3 ? "ok" : "fail") + ": " + text2;
				return flag3;
			}
			case "clear":
			case "purge":
				AudioPlayerFactory.RemoveAllGlobalClips();
				response = "Global2D queue очищена.";
				return true;
			default:
				response = Help();
				return false;
			}
		}
		catch (Exception ex)
		{
			response = "Ошибка fult_audio: " + ex;
			return false;
		}
	}

	private static string Mode(string[] args)
	{
		if (args.Length < 2)
		{
			return "Current mode: " + (AudioPlayerFactory.SafeGlobalMode ? "safe" : "spatial") + "\nfult_audio mode safe — рабочий режим v12.1, все 3D-вызовы уходят в Global2D\nfult_audio mode spatial — экспериментальный настоящий 3D";
		}
		string text = args[1].ToLowerInvariant();
		if (text == "safe" || text == "global" || text == "global2d")
		{
			AudioPlayerFactory.SetSafeGlobalMode(enabled: true);
			return "Audio mode: safe/global2d.";
		}
		if (text == "spatial" || text == "3d" || text == "normal")
		{
			AudioPlayerFactory.SetSafeGlobalMode(enabled: false);
			return "Audio mode: spatial experimental. Если опять тишина — верни: fult_audio mode safe";
		}
		return "Неизвестный mode. Используй: safe или spatial.";
	}

	private static string Help()
	{
		return "FULT Audio Debug v13:\nfult_audio status\nfult_audio reload\nfult_audio inspect <clip>\nfult_audio mode [safe|spatial]\nfult_audio global <clip> [volume]\nfult_audio self2d <clip> [volume]\nfult_audio self3d <clip> [volume]\nfult_audio play <clip> [global|self2d|self3d] [volume]\nfult_audio purge";
	}

	private static string Status()
	{
		int num = -1;
		try
		{
			num = ((AudioClipStorage.AudioClips == null) ? (-1) : AudioClipStorage.AudioClips.Count);
		}
		catch
		{
		}
		return "FULT Audio Status: storage=" + num + ", runtime={" + AudioPlayerFactory.GetRuntimeStatus() + "}";
	}

	private static string Inspect(string clipName)
	{
		clipName = AudioClipRegistry.ResolveClipName(clipName);
		string fullPath;
		bool flag = AudioClipRegistry.TryGetPath(clipName, out fullPath);
		bool flag2 = AudioClipRegistry.EnsureClipLoaded(clipName);
		object obj = null;
		bool flag3 = false;
		try
		{
			flag3 = AudioClipStorage.AudioClips != null && AudioClipStorage.AudioClips.ContainsKey(clipName);
			if (flag3)
			{
				obj = AudioClipStorage.AudioClips[clipName];
			}
		}
		catch
		{
		}
		string text = "clip=" + clipName + "\npathOk=" + flag + "\npath=" + fullPath + "\nloaded=" + flag2 + "\nstorageHas=" + flag3 + "\n";
		if (obj != null)
		{
			text += DumpObject(obj);
		}
		return text;
	}

	private static string DumpObject(object obj)
	{
		if (obj == null)
		{
			return "clipObject=null\n";
		}
		Type type = obj.GetType();
		string text = "type=" + type.FullName + "\n";
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.GetIndexParameters().Length == 0)
			{
				try
				{
					object value = propertyInfo.GetValue(obj, null);
					text = ((value == null) ? (text + propertyInfo.Name + "=null\n") : ((!(value is Array array)) ? (text + propertyInfo.Name + "=" + value?.ToString() + "\n") : (text + propertyInfo.Name + "=Array[" + array.Length + "]\n")));
				}
				catch
				{
				}
			}
		}
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			try
			{
				object value2 = fieldInfo.GetValue(obj);
				if (value2 is Array array2)
				{
					text = text + fieldInfo.Name + "=Array[" + array2.Length + "]\n";
				}
				else if (value2 != null && (value2.GetType().IsPrimitive || value2 is string))
				{
					text = text + fieldInfo.Name + "=" + value2?.ToString() + "\n";
				}
			}
			catch
			{
			}
		}
		return text;
	}

	private static float ParseFloat(string[] args, int index, float fallback)
	{
		if (args.Length > index && float.TryParse(args[index], out var result))
		{
			return result;
		}
		return fallback;
	}
}
