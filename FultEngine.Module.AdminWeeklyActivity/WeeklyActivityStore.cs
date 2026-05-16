using System;
using System.Collections.Generic;

namespace FultEngine.Module.AdminWeeklyActivity;

public class WeeklyActivityStore
{
	public string WeekKey { get; set; } = string.Empty;


	public DateTime WeekStartLocal { get; set; }

	public DateTime WeekEndLocal { get; set; }

	public Dictionary<string, WeeklyActivityRecord> Players { get; set; } = new Dictionary<string, WeeklyActivityRecord>();

}
