using System;

namespace FultEngine.Module.BanSystem;

public class BanData
{
	public string FileName { get; set; }

	public string SteamId { get; set; }

	public string Reason { get; set; }

	public DateTime EndTime { get; set; }

	public string Ip { get; set; }

	public string Admin { get; set; }

	public string BanNumber { get; set; }
}
