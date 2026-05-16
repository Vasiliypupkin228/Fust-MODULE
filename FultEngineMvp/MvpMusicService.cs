using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Permissions.Extensions;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngineMvp;

public sealed class MvpMusicService
{
	private const string PlayerSelectedSessionKey = "FultEngineMvp_SelectedTrack";

	private const string AudioPlayerName = "FultEngineMvp_GlobalAudioPlayer";

	private readonly FultEngineMvpPlugin _plugin;

	private readonly Dictionary<string, TrackInfo> _tracksByName = new Dictionary<string, TrackInfo>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, DateTime> _playerPlayCooldowns = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

	private readonly HashSet<string> _loadedClipNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	private readonly List<TrackInfo> _orderedTracks = new List<TrackInfo>();

	private DropdownSetting _dropdownSetting;

	private TwoButtonsSetting _playToggleSetting;

	private bool _roundEnded;

	private bool _playedThisRound;

	private bool _isStartingTrack;

	private DateTime _windowEndUtc;

	private DateTime _nextGlobalPlayUtc = DateTime.MinValue;

	private string _currentClipName;

	private CoroutineHandle _autoStopCoroutine;

	public IReadOnlyList<TrackInfo> Tracks => _orderedTracks;

	public string MusicDirectory
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(_plugin.Config.MusicFolderPath))
			{
				return _plugin.Config.MusicFolderPath;
			}
			return Path.Combine(Paths.Plugins, "FULT-ENGINE", "MVPMusic");
		}
	}

	public MvpMusicService(FultEngineMvpPlugin plugin)
	{
		_plugin = plugin;
	}

	public void LoadTracks()
	{
		_tracksByName.Clear();
		_orderedTracks.Clear();
		_playerPlayCooldowns.Clear();
		Directory.CreateDirectory(MusicDirectory);
		TryExtractMusicZip();
		string[] array = Directory.GetFiles(MusicDirectory, "*.ogg", SearchOption.TopDirectoryOnly).OrderBy<string, string>(Path.GetFileNameWithoutExtension, StringComparer.OrdinalIgnoreCase).ToArray();
		string[] array2 = array;
		foreach (string text in array2)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
			string text2 = BuildDisplayName(fileNameWithoutExtension);
			string text3 = BuildClipName(fileNameWithoutExtension);
			if (!IsTrackFileSafe(text, out var reason))
			{
				Log.Warn("[FultEngineMvp] Трек пропущен: " + Path.GetFileName(text) + " | " + reason);
				continue;
			}
			try
			{
				if (!_plugin.Config.LazyLoadAudioClips)
				{
					EnsureClipLoaded(text, text3);
				}
				TrackInfo trackInfo = new TrackInfo(_orderedTracks.Count + 1, text2, text3, text);
				_orderedTracks.Add(trackInfo);
				_tracksByName[text2] = trackInfo;
				if (_plugin.Config.Debug)
				{
					Log.Info("[FultEngineMvp] MVP track registered: " + text2 + " => " + text3);
				}
			}
			catch (Exception arg)
			{
				Log.Error($"[FultEngineMvp] Не удалось подготовить трек {text}: {arg}");
			}
		}
		Log.Info($"[FultEngineMvp] MVP треков найдено: {_orderedTracks.Count}. LazyLoad={_plugin.Config.LazyLoadAudioClips}. Папка: {MusicDirectory}");
	}

	public void RegisterServerSpecific()
	{
		UnregisterServerSpecific();
		if (_orderedTracks.Count == 0)
		{
			Log.Warn("[FultEngineMvp] Server Specific не зарегистрирован: нет .ogg треков в папке MVPMusic.");
			return;
		}
		string[] array = _orderedTracks.Select((TrackInfo t) => t.DisplayName).ToArray();
		_dropdownSetting = new DropdownSetting(_plugin.Config.MusicDropdownId, _plugin.Config.DropdownLabel, (IEnumerable<string>)array, 0, (DropdownEntryType)0, _plugin.Config.DropdownHint, byte.MaxValue, false, (HeaderSetting)null, (Action<Player, SettingBase>)delegate(Player player, SettingBase setting)
		{
			if (!(player == (Player)null))
			{
				DropdownSetting val2 = (DropdownSetting)(object)((setting is DropdownSetting) ? setting : null);
				if (val2 != null)
				{
					player.SessionVariables["FultEngineMvp_SelectedTrack"] = val2.SelectedOption;
					if (_plugin.Config.PlayOnDropdownChangeAfterRoundEnd && CanTryPlayNow(player, out var response2))
					{
						TryPlaySelected(player, out response2);
					}
				}
			}
		});
		_playToggleSetting = new TwoButtonsSetting(_plugin.Config.PlayToggleId, _plugin.Config.PlayToggleLabel, "Нет", "Да", false, _plugin.Config.PlayToggleHint, (HeaderSetting)null, (Action<Player, SettingBase>)delegate(Player player, SettingBase setting)
		{
			if (!(player == (Player)null))
			{
				TwoButtonsSetting val = (TwoButtonsSetting)(object)((setting is TwoButtonsSetting) ? setting : null);
				if (val != null && val.IsSecond)
				{
					TryPlaySelected(player, out var response);
					SendNotice(player, response, 5f);
				}
			}
		});
		SettingBase.Register((IEnumerable<SettingBase>)(object)new SettingBase[2]
		{
			(SettingBase)_dropdownSetting,
			(SettingBase)_playToggleSetting
		}, (Func<Player, bool>)null);
		Log.Info("[FultEngineMvp] Server Specific настройки MVP музыки зарегистрированы.");
	}

	public void UnregisterServerSpecific()
	{
		try
		{
			List<SettingBase> list = new List<SettingBase>();
			if (_dropdownSetting != null)
			{
				list.Add((SettingBase)(object)_dropdownSetting);
			}
			if (_playToggleSetting != null)
			{
				list.Add((SettingBase)(object)_playToggleSetting);
			}
			if (list.Count > 0)
			{
				SettingBase.Unregister((Func<Player, bool>)null, (IEnumerable<SettingBase>)list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Log.Warn("[FultEngineMvp] Ошибка при снятии Server Specific настроек: " + ex.Message);
		}
		finally
		{
			_dropdownSetting = null;
			_playToggleSetting = null;
		}
	}

	public void HandleRoundEnded()
	{
		_roundEnded = true;
		_playedThisRound = false;
		_nextGlobalPlayUtc = DateTime.MinValue;
		_playerPlayCooldowns.Clear();
		_windowEndUtc = ((_plugin.Config.RoundEndWindowSeconds <= 0) ? DateTime.MaxValue : DateTime.UtcNow.AddSeconds(_plugin.Config.RoundEndWindowSeconds));
		StopCurrentTrack();
		if (_plugin.Config.BroadcastMvpReasonOnRoundEnd)
		{
			BroadcastMvpReason();
		}
		foreach (Player item in Player.List)
		{
			if (!(item == (Player)null) && item.IsConnected && !item.IsHost && CanUseMvpMusic(item))
			{
				SendNotice(item, _plugin.Config.RoundEndMessage, 10f);
			}
		}
	}

	public void HandleRestartingRound()
	{
		_roundEnded = false;
		_playedThisRound = false;
		_isStartingTrack = false;
		_windowEndUtc = DateTime.MinValue;
		_nextGlobalPlayUtc = DateTime.MinValue;
		_playerPlayCooldowns.Clear();
		StopCurrentTrack();
	}

	public bool TryPlaySelected(Player player, out string response)
	{
		if (!CanTryPlayNow(player, out response))
		{
			return false;
		}
		TrackInfo selectedTrack = GetSelectedTrack(player);
		if (selectedTrack == null)
		{
			response = "❌ Трек не найден. Попробуй выбрать другой в Server Specific или напиши .mvp list";
			return false;
		}
		return TryPlayTrack(player, selectedTrack, out response);
	}

	public bool TryPlayByNumber(Player player, int number, out string response)
	{
		if (number <= 0 || number > _orderedTracks.Count)
		{
			response = $"❌ Нет трека с номером {number}. Напиши .mvp list";
			return false;
		}
		if (!CanTryPlayNow(player, out response))
		{
			return false;
		}
		return TryPlayTrack(player, _orderedTracks[number - 1], out response);
	}

	public bool TryPlayByName(Player player, string name, out string response)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			response = "❌ Укажи название или номер трека. Пример: .mvp 1";
			return false;
		}
		TrackInfo trackInfo = _orderedTracks.FirstOrDefault((TrackInfo t) => t.DisplayName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
		if (trackInfo == null)
		{
			response = "❌ Трек по названию '" + name + "' не найден. Напиши .mvp list";
			return false;
		}
		if (!CanTryPlayNow(player, out response))
		{
			return false;
		}
		return TryPlayTrack(player, trackInfo, out response);
	}

	public string BuildTrackListPage(int page, int pageSize = 15)
	{
		if (_orderedTracks.Count == 0)
		{
			return "MVP треки не загружены.";
		}
		int num = Math.Max(1, (int)Math.Ceiling((double)_orderedTracks.Count / (double)pageSize));
		page = Math.Max(1, Math.Min(num, page));
		IEnumerable<TrackInfo> source = _orderedTracks.Skip((page - 1) * pageSize).Take(pageSize);
		string text = string.Join("\n", source.Select((TrackInfo t) => $"{t.Number}. {t.DisplayName}"));
		return $"MVP Music [{page}/{num}]\n{text}\nКоманда: .mvp <номер> | .mvp list {Math.Min(num, page + 1)}";
	}

	public void StopCurrentTrack()
	{
		try
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _autoStopCoroutine });
			AudioPlayer val = default(AudioPlayer);
			if (AudioPlayer.TryGet("FultEngineMvp_GlobalAudioPlayer", ref val))
			{
				foreach (AudioClipPlayback item in val.ClipsById.Values.ToList())
				{
					val.RemoveClipById(item.Id);
				}
				foreach (Speaker item2 in val.SpeakersByName.Values.ToList())
				{
					try
					{
						AudioSource component = ((Component)item2).GetComponent<AudioSource>();
						if ((Object)(object)component != (Object)null)
						{
							component.Stop();
							component.clip = null;
							((Behaviour)component).enabled = false;
						}
					}
					catch
					{
					}
					val.RemoveSpeaker(item2.Name);
				}
				val.Destroy();
			}
			if (_plugin.Config.AggressiveAudioCleanup)
			{
				AggressiveCleanupByName();
			}
		}
		catch (Exception ex)
		{
			Log.Warn("[FultEngineMvp] Не удалось остановить MVP музыку: " + ex.Message);
		}
		finally
		{
			_currentClipName = null;
			_isStartingTrack = false;
		}
	}

	private bool TryPlayTrack(Player player, TrackInfo track, out string response)
	{
		if (_isStartingTrack)
		{
			response = "❌ MVP трек уже запускается. Не спамь кнопку.";
			return false;
		}
		DateTime utcNow = DateTime.UtcNow;
		if (utcNow < _nextGlobalPlayUtc)
		{
			response = "❌ Подожди пару секунд перед запуском MVP музыки.";
			return false;
		}
		string playerKey = GetPlayerKey(player);
		if (_playerPlayCooldowns.TryGetValue(playerKey, out var value) && utcNow < value)
		{
			response = "❌ Не спамь запуск MVP музыки.";
			return false;
		}
		_isStartingTrack = true;
		_nextGlobalPlayUtc = utcNow.AddSeconds(Math.Max(0.1f, _plugin.Config.GlobalPlayCooldownSeconds));
		_playerPlayCooldowns[playerKey] = utcNow.AddSeconds(Math.Max(0.1f, _plugin.Config.PlayerPlayCooldownSeconds));
		try
		{
			if (!EnsureClipLoaded(track.FilePath, track.ClipName))
			{
				response = "❌ Клип " + track.ClipName + " не загружен. Проверь файл: " + track.FilePath;
				return false;
			}
			if (_plugin.Config.StopPreviousTrackBeforePlay)
			{
				StopCurrentTrack();
			}
			AudioPlayer val = AudioPlayer.CreateOrGet("FultEngineMvp_GlobalAudioPlayer", (string)null, (Action<AudioPlayer>)null, false, true, (List<ReferenceHub>)null, byte.MaxValue, (Action<AudioPlayer>)delegate(AudioPlayer p)
			{
				Speaker val2 = p.AddSpeaker("Main", Mathf.Clamp(_plugin.Config.Volume, 0f, 1f), false, 0f, 5000f);
				AudioSource component = ((Component)val2).GetComponent<AudioSource>();
				if ((Object)(object)component != (Object)null)
				{
					component.Stop();
					component.loop = false;
					component.spatialBlend = 0f;
					component.volume = Mathf.Clamp(_plugin.Config.Volume, 0f, 1f);
					((Behaviour)component).enabled = true;
				}
			}, (Func<ReferenceHub, bool>)null);
			val.DestroyWhenAllClipsPlayed = _plugin.Config.DestroyPlayerWhenAllClipsPlayed;
			val.AddClip(track.ClipName, Mathf.Clamp(_plugin.Config.Volume, 0f, 1f), false, _plugin.Config.DestroyClipOnEnd);
			_currentClipName = track.ClipName;
			_playedThisRound = true;
			_isStartingTrack = false;
			ScheduleAutoStop();
			BroadcastTrackStarted(player, track);
			response = "✅ Запущен MVP трек: " + track.DisplayName;
			Log.Info("[FultEngineMvp] " + player.Nickname + " запустил MVP трек: " + track.DisplayName + " (" + track.ClipName + ")");
			return true;
		}
		catch (Exception ex)
		{
			response = "❌ Ошибка запуска MVP трека: " + ex.Message;
			Log.Error($"[FultEngineMvp] Ошибка запуска трека {track.DisplayName}: {ex}");
			StopCurrentTrack();
			return false;
		}
		finally
		{
			_isStartingTrack = false;
		}
	}

	private bool CanTryPlayNow(Player player, out string response)
	{
		if (player == (Player)null || !player.IsConnected)
		{
			response = "❌ Игрок не найден.";
			return false;
		}
		if (!CanUseMvpMusic(player))
		{
			response = (_plugin.Config.RestrictMusicToRoundMvp ? "❌ MVP музыку может включить только MVP раунда." : ("❌ Нет права: " + _plugin.Config.Permission));
			return false;
		}
		if (_orderedTracks.Count == 0)
		{
			response = "❌ MVP треки не загружены. Проверь папку MVPMusic.";
			return false;
		}
		if (_plugin.Config.OnlyAfterRoundEnd && !_roundEnded)
		{
			response = "❌ MVP музыку можно включить только после окончания раунда.";
			return false;
		}
		if (_plugin.Config.RoundEndWindowSeconds > 0 && DateTime.UtcNow > _windowEndUtc)
		{
			response = "❌ Время выбора MVP музыки уже вышло.";
			return false;
		}
		if (_plugin.Config.OneTrackPerRound && _playedThisRound)
		{
			response = "❌ MVP трек в этом раунде уже был запущен.";
			return false;
		}
		response = "OK";
		return true;
	}

	private void BroadcastMvpReason()
	{
		MvpRoundResult mvpRoundResult = _plugin.StatsService?.CurrentMvp;
		if (mvpRoundResult == null || !mvpRoundResult.HasWinner)
		{
			return;
		}
		string message = "<b><color=#ffd166>\ud83c\udfc6 MVP РАУНДА</color></b>\n<size=27><color=#70e000>" + EscapeRichText(mvpRoundResult.Nickname) + "</color></size>\n<size=22><color=#ffd166>За что:</color> <color=#ffffff>" + EscapeRichText(mvpRoundResult.Reason) + "</color></size>\n" + $"<size=19><color=#bde0fe>{EscapeRichText(mvpRoundResult.Details)}</color> <color=#ffffff80>| score: {mvpRoundResult.Score}</color></size>";
		foreach (Player item in Player.List)
		{
			if (item != (Player)null && item.IsConnected && !item.IsHost)
			{
				SendNotice(item, message, _plugin.Config.MvpReasonBroadcastSeconds);
			}
		}
	}

	private void BroadcastTrackStarted(Player player, TrackInfo track)
	{
		string text = ((!_plugin.Config.ShowMvpReasonWhenPlayingTrack) ? null : _plugin.StatsService?.GetReasonFor(player));
		string text2 = "<b><color=#ffd166>\ud83c\udfa7 MVP MUSIC</color></b>\n<size=25><color=#70e000>" + EscapeRichText(player.Nickname) + "</color> включил: <color=#00d4ff>" + EscapeRichText(track.DisplayName) + "</color></size>";
		if (!string.IsNullOrWhiteSpace(text))
		{
			text2 = text2 + "\n<size=22><color=#ffd166>\ud83c\udfc6 За что MVP:</color> <color=#ffffff>" + EscapeRichText(text) + "</color></size>";
		}
		foreach (Player item in Player.List)
		{
			if (item != (Player)null && item.IsConnected && !item.IsHost)
			{
				SendNotice(item, text2, 7f);
			}
		}
	}

	private bool CanUseMvpMusic(Player player)
	{
		if (player == (Player)null)
		{
			return false;
		}
		if (!_plugin.Config.RestrictMusicToRoundMvp)
		{
			return HasPermission(player);
		}
		if (_plugin.StatsService != null && _plugin.StatsService.IsRoundMvp(player))
		{
			return true;
		}
		if (_plugin.Config.PermissionCanBypassMvpRestriction && HasPermission(player))
		{
			return true;
		}
		return false;
	}

	private bool HasPermission(Player player)
	{
		if (player == (Player)null)
		{
			return false;
		}
		if (string.IsNullOrWhiteSpace(_plugin.Config.Permission))
		{
			return true;
		}
		return Permissions.CheckPermission(player, _plugin.Config.Permission);
	}

	private TrackInfo GetSelectedTrack(Player player)
	{
		string text = null;
		if (string.IsNullOrWhiteSpace(text) && player.SessionVariables.TryGetValue("FultEngineMvp_SelectedTrack", out var value))
		{
			text = value as string;
		}
		if (!string.IsNullOrWhiteSpace(text) && _tracksByName.TryGetValue(text, out var value2))
		{
			return value2;
		}
		return _orderedTracks.FirstOrDefault();
	}

	private bool EnsureClipLoaded(string filePath, string clipName)
	{
		if (AudioClipStorage.AudioClips.ContainsKey(clipName))
		{
			_loadedClipNames.Add(clipName);
			return true;
		}
		if (!File.Exists(filePath))
		{
			Log.Error("[FultEngineMvp] Аудиофайл не найден: " + filePath);
			return false;
		}
		if (!IsTrackFileSafe(filePath, out var reason))
		{
			Log.Error("[FultEngineMvp] Аудиофайл заблокирован: " + filePath + " | " + reason);
			return false;
		}
		AudioClipStorage.LoadClip(filePath, clipName);
		_loadedClipNames.Add(clipName);
		return AudioClipStorage.AudioClips.ContainsKey(clipName);
	}

	private bool IsTrackFileSafe(string filePath, out string reason)
	{
		reason = null;
		if (!File.Exists(filePath))
		{
			reason = "файл не найден";
			return false;
		}
		if (_plugin.Config.MaxTrackFileSizeMb > 0f)
		{
			long num = (long)(_plugin.Config.MaxTrackFileSizeMb * 1024f * 1024f);
			long length = new FileInfo(filePath).Length;
			if (length > num)
			{
				reason = $"слишком большой файл: {(float)length / 1024f / 1024f:0.00} MB > {_plugin.Config.MaxTrackFileSizeMb:0.00} MB";
				return false;
			}
		}
		return true;
	}

	private void ScheduleAutoStop()
	{
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _autoStopCoroutine });
		if (_plugin.Config.MaxTrackSeconds <= 0f)
		{
			return;
		}
		_autoStopCoroutine = Timing.CallDelayed(_plugin.Config.MaxTrackSeconds, (Action)delegate
		{
			if (!string.IsNullOrWhiteSpace(_currentClipName))
			{
				if (_plugin.Config.Debug)
				{
					Log.Info("[FultEngineMvp] AutoStop MVP audio: " + _currentClipName);
				}
				StopCurrentTrack();
			}
		});
	}

	private void AggressiveCleanupByName()
	{
		try
		{
			AudioPlayer val = default(AudioPlayer);
			if (AudioPlayer.TryGet("FultEngineMvp_GlobalAudioPlayer", ref val))
			{
				val.Destroy();
			}
		}
		catch
		{
		}
	}

	private void TryExtractMusicZip()
	{
		try
		{
			if (Directory.GetFiles(MusicDirectory, "*.ogg", SearchOption.TopDirectoryOnly).Length == 0)
			{
				string path = Path.Combine(Paths.Plugins, "FULT-ENGINE");
				string text = Path.Combine(path, _plugin.Config.MusicZipName);
				if (File.Exists(text))
				{
					Log.Warn("[FultEngineMvp] Found " + text + ", but auto-unzip is disabled. Extract .ogg files manually to " + MusicDirectory);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Warn("[FultEngineMvp] MVPMusic.zip check failed: " + ex.Message);
		}
	}

	private string BuildClipName(string rawFileName)
	{
		string text = new string(rawFileName.Select((char ch) => char.IsLetterOrDigit(ch) ? char.ToLowerInvariant(ch) : '_').ToArray());
		while (text.Contains("__"))
		{
			text = text.Replace("__", "_");
		}
		text = text.Trim(new char[1] { '_' });
		return _plugin.Config.ClipPrefix + text;
	}

	private static string BuildDisplayName(string rawFileName)
	{
		return rawFileName.Replace('_', ' ').Replace('-', ' ').Trim();
	}

	private void SendNotice(Player player, string message, float seconds)
	{
		if (!(player == (Player)null) && player.IsConnected && !player.IsHost)
		{
			ushort num = (ushort)Math.Max(1, Math.Min(65535, (int)Math.Ceiling(seconds)));
			player.Broadcast(num, message, (BroadcastFlags)0, false);
		}
	}

	private static string EscapeRichText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return text.Replace("<", "‹").Replace(">", "›");
	}

	private static string GetPlayerKey(Player player)
	{
		if (player == (Player)null)
		{
			return string.Empty;
		}
		return (!string.IsNullOrWhiteSpace(player.UserId)) ? player.UserId : player.Id.ToString();
	}
}
