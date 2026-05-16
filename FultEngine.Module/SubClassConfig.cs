using System.Collections.Generic;

namespace FultEngine.Module;

public class SubClassConfig
{
	public bool IgnoreSubclassLimits { get; set; } = true;


	public Dictionary<string, SubClassData> AvailableSubclasses { get; set; } = new Dictionary<string, SubClassData>();

}
