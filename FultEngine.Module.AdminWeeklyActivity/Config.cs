using System.Collections.Generic;

namespace FultEngine.Module.AdminWeeklyActivity;

public class Config
{
	public bool IsEnabled { get; set; } = true;


	public bool TrackOnlyRemoteAdminUsers { get; set; } = true;


	public bool IgnoreNorthwoodStaff { get; set; } = true;


	public int AutosaveSeconds { get; set; } = 60;


	public int TopPageSize { get; set; } = 10;


	public string TimeZoneId { get; set; } = "Europe/Chisinau";


	public string DataFileName { get; set; } = "AdminWeeklyActivityData.json";


	public string ViewPermission { get; set; } = "fultengine.weeklyactivity.view";


	public string ResetPermission { get; set; } = "fultengine.weeklyactivity.reset";


	public List<string> ExcludedUserIds { get; set; } = new List<string>();

}
