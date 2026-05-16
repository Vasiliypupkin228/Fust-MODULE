using System;
using System.Collections.Generic;

namespace FultEngine.Module.AdminWeeklyActivity;

public class WeeklyActivityRecord
{
	public string UserId { get; set; } = string.Empty;


	public string LastNickname { get; set; } = string.Empty;


	public double TotalSeconds { get; set; }

	public Dictionary<string, double> DailySeconds { get; set; } = new Dictionary<string, double>();


	public DateTime FirstSeenUtc { get; set; }

	public DateTime LastUpdateUtc { get; set; }
}
