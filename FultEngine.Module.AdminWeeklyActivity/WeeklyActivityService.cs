using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using Newtonsoft.Json;

namespace FultEngine.Module.AdminWeeklyActivity;

public sealed class WeeklyActivityService
{
	private readonly Plugin _plugin;

	private readonly object _sync = new object();

	private readonly Dictionary<string, DateTime> _activeSessionsUtc = new Dictionary<string, DateTime>();

	private WeeklyActivityStore _store = new WeeklyActivityStore();

	private TimeZoneInfo _timeZone;

	private string _dataPath = string.Empty;

	public string DataPath => _dataPath;

	public TimeZoneInfo TimeZone => _timeZone;

	public WeeklyActivityService(Plugin plugin)
	{
		_plugin = plugin;
		_timeZone = ResolveTimeZone(plugin.ModuleConfig.TimeZoneId);
		_dataPath = Path.Combine(plugin.ModuleDirectoryPath, plugin.ModuleConfig.DataFileName);
	}

	public void Initialize()
	{
		Directory.CreateDirectory(_plugin.ModuleDirectoryPath);
		_timeZone = ResolveTimeZone(_plugin.ModuleConfig.TimeZoneId);
		_dataPath = Path.Combine(_plugin.ModuleDirectoryPath, _plugin.ModuleConfig.DataFileName);
		Load();
		TouchWeek();
		Save();
	}

	public bool ShouldTrack(Player player)
	{
		if (player == (Player)null || string.IsNullOrWhiteSpace(player.UserId))
		{
			return false;
		}
		if (_plugin.ModuleConfig.ExcludedUserIds.Any((string x) => string.Equals(x, player.UserId, StringComparison.OrdinalIgnoreCase)))
		{
			return false;
		}
		if (_plugin.ModuleConfig.IgnoreNorthwoodStaff && player.UserId.EndsWith("@northwood", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (_plugin.ModuleConfig.TrackOnlyRemoteAdminUsers)
		{
			return player.RemoteAdminAccess;
		}
		return true;
	}

	public void StartSession(Player player)
	{
		if (!ShouldTrack(player))
		{
			return;
		}
		lock (_sync)
		{
			TouchWeek_NoLock();
			EnsureRecord_NoLock(player.UserId, player.Nickname);
			if (!_activeSessionsUtc.ContainsKey(player.UserId))
			{
				_activeSessionsUtc[player.UserId] = DateTime.UtcNow;
			}
		}
	}

	public void StopSession(Player player)
	{
		if (player == (Player)null || string.IsNullOrWhiteSpace(player.UserId))
		{
			return;
		}
		lock (_sync)
		{
			TouchWeek_NoLock();
			if (_activeSessionsUtc.TryGetValue(player.UserId, out var value))
			{
				AddPlayedTime_NoLock(player.UserId, player.Nickname, value, DateTime.UtcNow);
				_activeSessionsUtc.Remove(player.UserId);
			}
		}
	}

	public void CommitAndSave()
	{
		lock (_sync)
		{
			TouchWeek_NoLock();
			CommitLiveSessions_NoLock();
			Save_NoLock();
		}
	}

	public void Save()
	{
		lock (_sync)
		{
			Save_NoLock();
		}
	}

	public void ResetCurrentWeek(string reason = "Ручной сброс")
	{
		lock (_sync)
		{
			(string, DateTime, DateTime) currentWeekInfo = GetCurrentWeekInfo();
			_store = new WeeklyActivityStore
			{
				WeekKey = currentWeekInfo.Item1,
				WeekStartLocal = currentWeekInfo.Item2,
				WeekEndLocal = currentWeekInfo.Item3,
				Players = new Dictionary<string, WeeklyActivityRecord>()
			};
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(currentWeekInfo.Item2, _timeZone);
			List<string> list = _activeSessionsUtc.Keys.ToList();
			foreach (string item in list)
			{
				_activeSessionsUtc[item] = ((utcNow > dateTime) ? dateTime : utcNow);
			}
			Log.Info("[AdminWeeklyActivity] Выполнен сброс текущей недели. Причина: " + reason);
			Save_NoLock();
		}
	}

	public WeeklyActivityRecord GetRecordPreview(string userId)
	{
		lock (_sync)
		{
			TouchWeek_NoLock();
			if (!_store.Players.TryGetValue(userId, out var value))
			{
				return null;
			}
			WeeklyActivityRecord weeklyActivityRecord = CloneRecord(value);
			ApplyActivePreview_NoLock(weeklyActivityRecord);
			return weeklyActivityRecord;
		}
	}

	public List<WeeklyActivityRecord> GetSortedPreview()
	{
		lock (_sync)
		{
			TouchWeek_NoLock();
			List<WeeklyActivityRecord> list = _store.Players.Values.Select(CloneRecord).ToList();
			foreach (WeeklyActivityRecord item in list)
			{
				ApplyActivePreview_NoLock(item);
			}
			return list.OrderByDescending((WeeklyActivityRecord x) => x.TotalSeconds).ThenBy<WeeklyActivityRecord, string>((WeeklyActivityRecord x) => x.LastNickname, StringComparer.OrdinalIgnoreCase).ToList();
		}
	}

	public (string weekKey, DateTime weekStartLocal, DateTime weekEndLocal) GetWeekInfo()
	{
		lock (_sync)
		{
			TouchWeek_NoLock();
			return (weekKey: _store.WeekKey, weekStartLocal: _store.WeekStartLocal, weekEndLocal: _store.WeekEndLocal);
		}
	}

	public bool IsPlayerTracked(Player player)
	{
		return ShouldTrack(player);
	}

	private void Load()
	{
		lock (_sync)
		{
			if (!File.Exists(_dataPath))
			{
				_store = CreateEmptyStore();
				return;
			}
			try
			{
				string text = File.ReadAllText(_dataPath);
				_store = JsonConvert.DeserializeObject<WeeklyActivityStore>(text) ?? CreateEmptyStore();
				WeeklyActivityStore store = _store;
				if (store.Players == null)
				{
					Dictionary<string, WeeklyActivityRecord> dictionary2 = (store.Players = new Dictionary<string, WeeklyActivityRecord>());
				}
			}
			catch (Exception arg)
			{
				Log.Error($"[AdminWeeklyActivity] Не удалось загрузить файл статистики: {arg}");
				_store = CreateEmptyStore();
			}
		}
	}

	private WeeklyActivityStore CreateEmptyStore()
	{
		(string, DateTime, DateTime) currentWeekInfo = GetCurrentWeekInfo();
		return new WeeklyActivityStore
		{
			WeekKey = currentWeekInfo.Item1,
			WeekStartLocal = currentWeekInfo.Item2,
			WeekEndLocal = currentWeekInfo.Item3,
			Players = new Dictionary<string, WeeklyActivityRecord>()
		};
	}

	private void Save_NoLock()
	{
		try
		{
			Directory.CreateDirectory(_plugin.ModuleDirectoryPath);
			string contents = JsonConvert.SerializeObject((object)_store, (Formatting)1);
			File.WriteAllText(_dataPath, contents);
		}
		catch (Exception arg)
		{
			Log.Error($"[AdminWeeklyActivity] Не удалось сохранить файл статистики: {arg}");
		}
	}

	private void TouchWeek()
	{
		lock (_sync)
		{
			TouchWeek_NoLock();
		}
	}

	private void TouchWeek_NoLock()
	{
		(string, DateTime, DateTime) currentWeekInfo = GetCurrentWeekInfo();
		if (string.Equals(_store.WeekKey, currentWeekInfo.Item1, StringComparison.Ordinal))
		{
			return;
		}
		Log.Info("[AdminWeeklyActivity] Началась новая неделя: " + _store.WeekKey + " -> " + currentWeekInfo.Item1);
		DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(currentWeekInfo.Item2, _timeZone);
		List<string> list = _activeSessionsUtc.Keys.ToList();
		_store = new WeeklyActivityStore
		{
			WeekKey = currentWeekInfo.Item1,
			WeekStartLocal = currentWeekInfo.Item2,
			WeekEndLocal = currentWeekInfo.Item3,
			Players = new Dictionary<string, WeeklyActivityRecord>()
		};
		foreach (string item in list)
		{
			if (_activeSessionsUtc[item] < dateTime)
			{
				_activeSessionsUtc[item] = dateTime;
			}
		}
	}

	private void CommitLiveSessions_NoLock()
	{
		DateTime utcNow = DateTime.UtcNow;
		List<string> list = _activeSessionsUtc.Keys.ToList();
		foreach (string userId in list)
		{
			Player val = ((IEnumerable<Player>)Player.List).FirstOrDefault((Func<Player, bool>)((Player p) => p.UserId == userId));
			string nickname = ((val != null) ? val.Nickname : null) ?? userId;
			AddPlayedTime_NoLock(userId, nickname, _activeSessionsUtc[userId], utcNow);
			_activeSessionsUtc[userId] = utcNow;
		}
	}

	private void EnsureRecord_NoLock(string userId, string nickname)
	{
		if (_store.Players.TryGetValue(userId, out var value))
		{
			value.LastNickname = nickname;
			value.LastUpdateUtc = DateTime.UtcNow;
			return;
		}
		_store.Players[userId] = new WeeklyActivityRecord
		{
			UserId = userId,
			LastNickname = nickname,
			TotalSeconds = 0.0,
			DailySeconds = new Dictionary<string, double>(),
			FirstSeenUtc = DateTime.UtcNow,
			LastUpdateUtc = DateTime.UtcNow
		};
	}

	private void AddPlayedTime_NoLock(string userId, string nickname, DateTime startUtc, DateTime endUtc)
	{
		if (!(endUtc <= startUtc))
		{
			EnsureRecord_NoLock(userId, nickname);
			WeeklyActivityRecord weeklyActivityRecord = _store.Players[userId];
			DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(_store.WeekStartLocal, _timeZone);
			DateTime dateTime2 = TimeZoneInfo.ConvertTimeToUtc(_store.WeekEndLocal.AddDays(1.0), _timeZone);
			DateTime dateTime3 = ((startUtc < dateTime) ? dateTime : startUtc);
			DateTime dateTime4 = ((endUtc > dateTime2) ? dateTime2 : endUtc);
			if (!(dateTime4 <= dateTime3))
			{
				AddDailyBreakdown_NoLock(weeklyActivityRecord, dateTime3, dateTime4);
				weeklyActivityRecord.LastNickname = nickname;
				weeklyActivityRecord.LastUpdateUtc = DateTime.UtcNow;
			}
		}
	}

	private void AddDailyBreakdown_NoLock(WeeklyActivityRecord record, DateTime startUtc, DateTime endUtc)
	{
		DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(startUtc, _timeZone);
		DateTime dateTime2 = TimeZoneInfo.ConvertTimeFromUtc(endUtc, _timeZone);
		DateTime dateTime3 = dateTime;
		while (dateTime3 < dateTime2)
		{
			DateTime dateTime4 = dateTime3.Date.AddDays(1.0);
			DateTime dateTime5 = ((dateTime2 < dateTime4) ? dateTime2 : dateTime4);
			double totalSeconds = (dateTime5 - dateTime3).TotalSeconds;
			if (totalSeconds > 0.0)
			{
				string key = dateTime3.ToString("yyyy-MM-dd");
				if (!record.DailySeconds.ContainsKey(key))
				{
					record.DailySeconds[key] = 0.0;
				}
				record.DailySeconds[key] += totalSeconds;
				record.TotalSeconds += totalSeconds;
			}
			dateTime3 = dateTime5;
		}
	}

	private void ApplyActivePreview_NoLock(WeeklyActivityRecord record)
	{
		if (_activeSessionsUtc.TryGetValue(record.UserId, out var value))
		{
			WeeklyActivityRecord weeklyActivityRecord = CloneRecord(record);
			AddPlayedTime_NoLock(weeklyActivityRecord.UserId, weeklyActivityRecord.LastNickname, value, DateTime.UtcNow);
			record.TotalSeconds = weeklyActivityRecord.TotalSeconds;
			record.DailySeconds = weeklyActivityRecord.DailySeconds;
			record.LastUpdateUtc = weeklyActivityRecord.LastUpdateUtc;
		}
	}

	private WeeklyActivityRecord CloneRecord(WeeklyActivityRecord source)
	{
		return new WeeklyActivityRecord
		{
			UserId = source.UserId,
			LastNickname = source.LastNickname,
			TotalSeconds = source.TotalSeconds,
			DailySeconds = source.DailySeconds.ToDictionary((KeyValuePair<string, double> x) => x.Key, (KeyValuePair<string, double> x) => x.Value),
			FirstSeenUtc = source.FirstSeenUtc,
			LastUpdateUtc = source.LastUpdateUtc
		};
	}

	private (string weekKey, DateTime weekStartLocal, DateTime weekEndLocal) GetCurrentWeekInfo()
	{
		DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);
		int num = (int)(dateTime.DayOfWeek + 6) % 7;
		DateTime dateTime2 = dateTime.Date.AddDays(-num);
		DateTime dateTime3 = dateTime2.AddDays(6.0);
		string item = $"{dateTime2:yyyy-MM-dd}_{dateTime3:yyyy-MM-dd}";
		return (weekKey: item, weekStartLocal: dateTime2, weekEndLocal: dateTime3);
	}

	private TimeZoneInfo ResolveTimeZone(string timeZoneId)
	{
		try
		{
			return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
		}
		catch
		{
			Log.Warn("[AdminWeeklyActivity] Часовой пояс '" + timeZoneId + "' не найден, используется UTC.");
			return TimeZoneInfo.Utc;
		}
	}
}
