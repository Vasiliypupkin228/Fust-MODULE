using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using FultEngine.Module.Logs;
using MEC;
using RemoteAdmin;

namespace FultEngine.Module.BanSystem;

public class BanSystem : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class UpdateBansCoroutine_d_31 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public BanSystem __4__this;

		private List<string> expiredBans;

		private Dictionary<string, BanData>.Enumerator __s__2;

		private KeyValuePair<string, BanData> ban;

		private List<string>.Enumerator __s__4;

		private string fileName;

		float IEnumerator<float>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public UpdateBansCoroutine_d_31(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			expiredBans = null;
			__s__2 = default(Dictionary<string, BanData>.Enumerator);
			ban = default(KeyValuePair<string, BanData>);
			__s__4 = default(List<string>.Enumerator);
			fileName = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			switch (__1__state)
			{
			default:
				return false;
			case 0:
				__1__state = -1;
				break;
			case 1:
				__1__state = -1;
				expiredBans = null;
				break;
			}
			expiredBans = new List<string>();
			__s__2 = __4__this._activeBans.GetEnumerator();
			try
			{
				while (__s__2.MoveNext())
				{
					ban = __s__2.Current;
					if ((ban.Value.EndTime - DateTime.Now).TotalSeconds <= 0.0)
					{
						expiredBans.Add(ban.Key);
					}
					ban = default(KeyValuePair<string, BanData>);
				}
			}
			finally
			{
				((IDisposable)__s__2).Dispose();
			}
			__s__2 = default(Dictionary<string, BanData>.Enumerator);
			__s__4 = expiredBans.GetEnumerator();
			try
			{
				while (__s__4.MoveNext())
				{
					fileName = __s__4.Current;
					__4__this.RemoveBan(fileName);
					fileName = null;
				}
			}
			finally
			{
				((IDisposable)__s__4).Dispose();
			}
			__s__4 = default(List<string>.Enumerator);
			__2__current = Timing.WaitForSeconds(1f);
			__1__state = 1;
			return true;
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private Config _config;

	public static readonly string ConfigPath = Path.Combine(Paths.Plugins, "FULT-ENGINE", "UserBan");

	private readonly Dictionary<string, BanData> _activeBans = new Dictionary<string, BanData>();

	private static int _banCounter;

	public static BanSystem Instance { get; private set; }

	public string Name { get; } = "CustomBanSystem";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 0, 1);


	public void OnEnabled()
	{
		Instance = this;
		Directory.CreateDirectory(ConfigPath);
		LoadBans();
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnPlayerVerified;
		Player.Banning += (CustomEventHandler<BanningEventArgs>)OnBanning;
		Player.Banned += (CustomEventHandler<BannedEventArgs>)OnBanned;
		Timing.RunCoroutine(UpdateBansCoroutine());
	}

	public void OnDisabled()
	{
		Instance = null;
		Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnPlayerVerified;
		Player.Banning -= (CustomEventHandler<BanningEventArgs>)OnBanning;
		Player.Banned -= (CustomEventHandler<BannedEventArgs>)OnBanned;
		Timing.KillCoroutines("UpdateBans");
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config();
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}

	private static void OnKicking(KickingEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && !(ev.Target == (Player)null))
		{
			FultEngine.Module.Logs.Plugin.LogToFile("\nАдминистратор " + ev.Player.Nickname + " кикнул " + ev.Target.Nickname + "\n______________________________\nПричина: " + ev.Reason + "\n______________________________\nАдминистратор " + ev.Player.Nickname + ":\nIP/SteamID: " + ev.Player.IPAddress + "/" + ev.Player.UserId + "\n______________________________\nИгрок " + ev.Target.Nickname + ":\nIP/SteamID: " + ev.Target.IPAddress + "/" + ev.Target.UserId, FultEngine.Module.Logs.Plugin.LogType.Admin);
		}
	}

	private void OnBanning(BanningEventArgs ev)
	{
		if (!(((KickingEventArgs)ev).Player == (Player)null) && !(((KickingEventArgs)ev).Target == (Player)null))
		{
			((KickingEventArgs)ev).IsAllowed = false;
			string reason = ((KickingEventArgs)ev).Reason;
			int durationMinutes = (int)(ev.Duration / 60);
			ICommandSender commandSender = ((KickingEventArgs)ev).CommandSender;
			ICommandSender obj = ((commandSender is PlayerCommandSender) ? commandSender : null);
			string text = ((obj != null) ? ((CommandSender)obj).Nickname : null) ?? "Server";
			AddBan(((KickingEventArgs)ev).Target, reason, durationMinutes, ((KickingEventArgs)ev).Player);
		}
	}

	private void OnBanned(BannedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && !(ev.Target == (Player)null))
		{
			string reason = ev.Details.Reason;
			int durationMinutes = (int)(ev.Details.IssuanceTime / 60);
			string text = ev.Player.Nickname ?? "Server";
			AddBan(ev.Target, reason, durationMinutes, ev.Player);
		}
	}

	private void OnPlayerVerified(VerifiedEventArgs ev)
	{
		if (!_config.IsEnabled || ev.Player == (Player)null)
		{
			return;
		}
		string steamId = ev.Player.UserId;
		BanData banData = _activeBans.Values.FirstOrDefault((BanData b) => b.SteamId == steamId);
		if (banData != null)
		{
			double totalSeconds = (banData.EndTime - DateTime.Now).TotalSeconds;
			if (totalSeconds > 0.0)
			{
				string arg = FormatTimeLeft(totalSeconds);
				string text = $"\nВы забанены.\nПричина: {banData.Reason}\nИстекает: {arg} | {banData.EndTime:yyyy-MM-dd HH:mm:ss}";
				ev.Player.Kick(text, (Player)null);
			}
			else
			{
				RemoveBan(banData.FileName);
			}
		}
	}

	private string FormatTimeLeft(double seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		return $"{timeSpan.Hours}ч:{timeSpan.Minutes:D2}м:{timeSpan.Seconds:D2}с";
	}

	private void LoadBans()
	{
		string path = Path.Combine(ConfigPath, "ban_counter.txt");
		if (File.Exists(path))
		{
			int.TryParse(File.ReadAllText(path), out _banCounter);
		}
		string[] files = Directory.GetFiles(ConfigPath, "*.txt");
		foreach (string text in files)
		{
			if (Path.GetFileName(text) == "ban_counter.txt")
			{
				continue;
			}
			try
			{
				string[] source = File.ReadAllLines(text);
				BanData banData = new BanData
				{
					FileName = Path.GetFileName(text),
					SteamId = source.FirstOrDefault((string l) => l.StartsWith("SteamID:"))?.Replace("SteamID: ", ""),
					Reason = source.FirstOrDefault((string l) => l.StartsWith("Reason:"))?.Replace("Reason: ", ""),
					EndTime = DateTime.Parse(source.FirstOrDefault((string l) => l.StartsWith("Time:"))?.Replace("Time: ", "") ?? DateTime.Now.ToString()),
					Ip = source.FirstOrDefault((string l) => l.StartsWith("Ip:"))?.Replace("Ip: ", ""),
					Admin = source.FirstOrDefault((string l) => l.StartsWith("Admin:"))?.Replace("Admin: ", ""),
					BanNumber = source.FirstOrDefault((string l) => l.StartsWith("NumberBan:"))?.Replace("NumberBan: ", "")
				};
				if ((banData.EndTime - DateTime.Now).TotalSeconds > 0.0)
				{
					_activeBans[banData.FileName] = banData;
				}
				else
				{
					File.Delete(text);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Ошибка загрузки бана " + text + ": " + ex.Message);
			}
		}
	}

	public void AddBan(Player player, string reason, int durationMinutes, Player admin)
	{
		_banCounter++;
		string text = $"#{_banCounter:D4}";
		string validFileName = GetValidFileName(player.Nickname);
		string path = Path.Combine(ConfigPath, validFileName);
		BanData banData = new BanData
		{
			FileName = validFileName,
			SteamId = player.UserId,
			Reason = reason,
			EndTime = DateTime.Now.AddMinutes(durationMinutes),
			Ip = player.IPAddress,
			Admin = admin.Nickname,
			BanNumber = text
		};
		string contents = $"Reason: {reason}\nTime: {banData.EndTime:yyyy-MM-dd HH:mm:ss}\nIp: {player.IPAddress}\nSteamID: {player.UserId}\nAdmin: {admin.Nickname}\nNumberBan: {text}";
		File.WriteAllText(path, contents);
		File.WriteAllText(Path.Combine(ConfigPath, "ban_counter.txt"), _banCounter.ToString());
		FultEngine.Module.Logs.Plugin.LogToFile("\nАдминистратор " + admin.Nickname + " забанил игрока " + player.Nickname + "\n______________________________\nПричина: " + reason + "\n______________________________\n" + $"Время: {banData.EndTime:yyyy-MM-dd HH:mm:ss}\n" + "______________________________\nАдминистратор " + admin.Nickname + ":\nIP/SteamID: " + admin.IPAddress + "/" + admin.UserId + "\n______________________________\nИгрок " + player.Nickname + ":\nIP/SteamID: " + player.IPAddress + "/" + player.UserId, FultEngine.Module.Logs.Plugin.LogType.Ban);
		_activeBans[validFileName] = banData;
		string arg = FormatTimeLeft(durationMinutes * 60);
		player.Kick($"Вы забанены.\nПричина: {reason}\nИстекает: {arg} | {banData.EndTime:yyyy-MM-dd HH:mm:ss}", (Player)null);
	}

	private string GetValidFileName(string nickname)
	{
		char[] invalidChars = Path.GetInvalidFileNameChars();
		string text = new string(nickname.Select((char c) => CollectionExtensions.Contains<char>(invalidChars, c) ? '_' : c).ToArray());
		text = ((text.Length > 50) ? text.Substring(0, 50) : text);
		string text2 = text + ".txt";
		string path = Path.Combine(ConfigPath, text2);
		int num = 1;
		while (File.Exists(path))
		{
			text2 = $"{text}_{num}.txt";
			path = Path.Combine(ConfigPath, text2);
			num++;
		}
		return text2;
	}

	private void RemoveBan(string fileName)
	{
		if (_activeBans.ContainsKey(fileName))
		{
			_activeBans.Remove(fileName);
			string path = Path.Combine(ConfigPath, fileName);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}

	[IteratorStateMachine(typeof(UpdateBansCoroutine_d_31))]
	private IEnumerator<float> UpdateBansCoroutine()
	{
		return new UpdateBansCoroutine_d_31(0)
		{
			__4__this = this
		};
	}
}
