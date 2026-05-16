using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace FultEngine.Module.AdminWeeklyActivity.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class WeeklyAdminActivityCommand : ICommand
{
	[CompilerGenerated]
	private sealed class EnumerateWeekDays_d_12 : IEnumerable<DateTime>, IEnumerable, IEnumerator<DateTime>, IDisposable, IEnumerator
	{
		private int __1__state;

		private DateTime __2__current;

		private int __l__initialThreadId;

		private DateTime start;

		public DateTime __3__start;

		private DateTime end;

		public DateTime __3__end;

		private DateTime date;

		DateTime IEnumerator<DateTime>.Current
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
		public EnumerateWeekDays_d_12(int __1__state)
		{
			this.__1__state = __1__state;
			__l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
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
				date = start.Date;
				break;
			case 1:
				__1__state = -1;
				date = date.AddDays(1.0);
				break;
			}
			if (date <= end.Date)
			{
				__2__current = date;
				__1__state = 1;
				return true;
			}
			return false;
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

		[DebuggerHidden]
		IEnumerator<DateTime> IEnumerable<DateTime>.GetEnumerator()
		{
			EnumerateWeekDays_d_12 EnumerateWeekDaysd__;
			if (__1__state == -2 && __l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				__1__state = 0;
				EnumerateWeekDaysd__ = this;
			}
			else
			{
				EnumerateWeekDaysd__ = new EnumerateWeekDays_d_12(0);
			}
			EnumerateWeekDaysd__.start = __3__start;
			EnumerateWeekDaysd__.end = __3__end;
			return EnumerateWeekDaysd__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<DateTime>)this).GetEnumerator();
		}
	}

	public string Command => "weeklyadmin";

	public string[] Aliases => new string[3] { "wa", "adminweek", "adminsweek" };

	public string Description => "Показывает активность админов за текущую неделю.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Plugin instance = Plugin.Instance;
		if (instance == null || instance.Service == null || !instance.ModuleConfig.IsEnabled)
		{
			response = "Модуль WeeklyActivity сейчас отключён.";
			return false;
		}
		if (!Permissions.CheckPermission(sender, instance.ModuleConfig.ViewPermission))
		{
			response = "Недостаточно прав. Нужен пермишен: " + instance.ModuleConfig.ViewPermission;
			return false;
		}
		string[] array = Enumerable.ToArray(arguments);
		if (array.Length == 0)
		{
			response = BuildHelp(instance);
			return true;
		}
		switch (array[0].ToLowerInvariant())
		{
		case "help":
			response = BuildHelp(instance);
			return true;
		case "top":
			return ExecuteTop(instance, array, out response);
		case "player":
		case "user":
			return ExecutePlayer(instance, array, out response);
		case "online":
			return ExecuteOnline(instance, out response);
		case "reset":
			return ExecuteReset(instance, sender, out response);
		default:
			response = BuildHelp(instance);
			return false;
		}
	}

	private bool ExecuteTop(Plugin plugin, string[] args, out string response)
	{
		int result = 1;
		if (args.Length >= 2 && (!int.TryParse(args[1], out result) || result < 1))
		{
			response = "Номер страницы должен быть положительным числом.";
			return false;
		}
		List<WeeklyActivityRecord> sortedPreview = plugin.Service.GetSortedPreview();
		if (sortedPreview.Count == 0)
		{
			response = "За текущую неделю пока нет записей по отслеживаемым администраторам.";
			return true;
		}
		int num = Math.Max(1, plugin.ModuleConfig.TopPageSize);
		int num2 = (int)Math.Ceiling((double)sortedPreview.Count / (double)num);
		if (result > num2)
		{
			result = num2;
		}
		(string, DateTime, DateTime) weekInfo = plugin.Service.GetWeekInfo();
		List<string> list = new List<string>();
		list.Add($"Активность админов за неделю {FormatDate(weekInfo.Item2)} - {FormatDate(weekInfo.Item3)} | страница {result}/{num2}");
		list.Add(string.Empty);
		List<string> list2 = list;
		List<WeeklyActivityRecord> list3 = sortedPreview.Skip((result - 1) * num).Take(num).ToList();
		for (int i = 0; i < list3.Count; i++)
		{
			WeeklyActivityRecord weeklyActivityRecord = list3[i];
			int num3 = (result - 1) * num + i + 1;
			list2.Add($"#{num3} {weeklyActivityRecord.LastNickname} [{weeklyActivityRecord.UserId}] — {FormatDuration(weeklyActivityRecord.TotalSeconds)}");
		}
		response = string.Join("\n", list2);
		return true;
	}

	private bool ExecutePlayer(Plugin plugin, string[] args, out string response)
	{
		if (args.Length < 2)
		{
			response = "Использование: weeklyadmin player <userid|часть ника>";
			return false;
		}
		string query = string.Join(" ", args.Skip(1));
		List<WeeklyActivityRecord> sortedPreview = plugin.Service.GetSortedPreview();
		WeeklyActivityRecord weeklyActivityRecord = sortedPreview.FirstOrDefault((WeeklyActivityRecord x) => x.UserId.Equals(query, StringComparison.OrdinalIgnoreCase)) ?? sortedPreview.FirstOrDefault((WeeklyActivityRecord x) => x.LastNickname.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
		if (weeklyActivityRecord == null)
		{
			response = "Игрок в статистике за текущую неделю не найден.";
			return false;
		}
		(string, DateTime, DateTime) weekInfo = plugin.Service.GetWeekInfo();
		List<string> list = new List<string>
		{
			"Игрок: " + weeklyActivityRecord.LastNickname,
			"UserId: " + weeklyActivityRecord.UserId,
			"Неделя: " + FormatDate(weekInfo.Item2) + " - " + FormatDate(weekInfo.Item3),
			"Всего за неделю: " + FormatDuration(weeklyActivityRecord.TotalSeconds),
			string.Empty,
			"По датам:"
		};
		foreach (DateTime item in EnumerateWeekDays(weekInfo.Item2, weekInfo.Item3))
		{
			string key = item.ToString("yyyy-MM-dd");
			weeklyActivityRecord.DailySeconds.TryGetValue(key, out var value);
			list.Add($"- {item:dd.MM.yyyy}: {FormatDuration(value)}");
		}
		response = string.Join("\n", list);
		return true;
	}

	private bool ExecuteOnline(Plugin plugin, out string response)
	{
		List<WeeklyActivityRecord> list = (from p in Player.List.Where(plugin.Service.IsPlayerTracked)
			select plugin.Service.GetRecordPreview(p.UserId) into r
			where r != null
			orderby r.TotalSeconds descending
			select r).ToList();
		if (list.Count == 0)
		{
			response = "Сейчас на сервере нет отслеживаемых администраторов.";
			return true;
		}
		List<string> list2 = new List<string>
		{
			"Отслеживаемые администраторы онлайн:",
			string.Empty
		};
		foreach (WeeklyActivityRecord item in list)
		{
			list2.Add("- " + item.LastNickname + " [" + item.UserId + "] — " + FormatDuration(item.TotalSeconds) + " за неделю");
		}
		response = string.Join("\n", list2);
		return true;
	}

	private bool ExecuteReset(Plugin plugin, ICommandSender sender, out string response)
	{
		if (!Permissions.CheckPermission(sender, plugin.ModuleConfig.ResetPermission))
		{
			response = "Недостаточно прав для сброса. Нужен пермишен: " + plugin.ModuleConfig.ResetPermission;
			return false;
		}
		plugin.Service.ResetCurrentWeek("Команда от " + sender.LogName);
		response = "Статистика текущей недели сброшена.";
		return true;
	}

	private string BuildHelp(Plugin plugin)
	{
		(string, DateTime, DateTime) weekInfo = plugin.Service.GetWeekInfo();
		return string.Join("\n", "WeeklyAdmin — активность админов за неделю " + FormatDate(weekInfo.Item2) + " - " + FormatDate(weekInfo.Item3), "weeklyadmin top [страница] — топ по времени", "weeklyadmin player <userid|часть ника> — подробная статистика игрока по датам", "weeklyadmin online — кто из отслеживаемых админов сейчас онлайн", "weeklyadmin reset — сбросить текущую неделю");
	}

	[IteratorStateMachine(typeof(EnumerateWeekDays_d_12))]
	private static IEnumerable<DateTime> EnumerateWeekDays(DateTime start, DateTime end)
	{
		return new EnumerateWeekDays_d_12(-2)
		{
			__3__start = start,
			__3__end = end
		};
	}

	private static string FormatDuration(double totalSeconds)
	{
		if (totalSeconds < 0.0)
		{
			totalSeconds = 0.0;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
		int num = (int)timeSpan.TotalHours;
		return $"{num:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}

	private static string FormatDate(DateTime date)
	{
		return date.ToString("dd.MM.yyyy");
	}
}
