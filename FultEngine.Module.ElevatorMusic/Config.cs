using System.Collections.Generic;

namespace FultEngine.Module.ElevatorMusic;

public class Config
{
	public bool IsEnabled { get; set; } = true;


	public string DefaultClipName { get; set; } = "ElevatorMusic";


	public List<ElevatorTrackChance> TrackPool { get; set; } = new List<ElevatorTrackChance>
	{
		new ElevatorTrackChance
		{
			ClipName = "ElevatorMusic",
			Chance = 98.0
		},
		new ElevatorTrackChance
		{
			ClipName = "ElevatorMusic_Easter1",
			Chance = 1.0
		},
		new ElevatorTrackChance
		{
			ClipName = "ElevatorMusic_Easter2",
			Chance = 1.0
		}
	};


	public Dictionary<string, string> ClipByElevatorName { get; set; } = new Dictionary<string, string>();


	public float Volume { get; set; } = 1f;


	public bool Loop { get; set; } = true;


	public bool UseLocal2DForPlayers { get; set; } = true;


	public bool UseGlobalFallbackIfLocalFails { get; set; } = false;


	public float PollInterval { get; set; } = 0.25f;


	public float ElevatorRefreshInterval { get; set; } = 7.5f;


	public float DetectionRadius { get; set; } = 4.25f;


	public float HeightTolerance { get; set; } = 5.5f;


	public float BoundsExpand { get; set; } = 1.15f;


	public float StopGraceSeconds { get; set; } = 0.65f;


	public bool TryMuteNativeElevatorAudioSources { get; set; } = true;


	public bool AggressiveNativeMute { get; set; } = false;


	public List<string> MovingSequenceKeywords { get; set; } = new List<string> { "moving", "move", "transit", "closing", "closed", "opening", "arriving", "arrive" };


	public bool Debug { get; set; } = false;

}
