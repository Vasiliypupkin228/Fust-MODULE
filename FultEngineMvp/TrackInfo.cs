namespace FultEngineMvp;

public sealed class TrackInfo
{
	public int Number { get; }

	public string DisplayName { get; }

	public string ClipName { get; }

	public string FilePath { get; }

	public TrackInfo(int number, string displayName, string clipName, string filePath)
	{
		Number = number;
		DisplayName = displayName;
		ClipName = clipName;
		FilePath = filePath;
	}
}
