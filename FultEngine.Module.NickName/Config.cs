using System.Collections.Generic;

namespace FultEngine.Module.NickName;

public class Config
{
	public bool EnableRPMode { get; set; } = true;


	public Dictionary<string, string> NameFormats { get; set; } = new Dictionary<string, string>();


	public int ClassDNumberRangeMin { get; set; } = 4000;


	public int ClassDNumberRangeMax { get; set; } = 5000;


	public List<string> Callsigns { get; set; } = new List<string>();


	public List<string> FirstNames { get; set; } = new List<string>();


	public List<string> LastNames { get; set; } = new List<string>();

}
