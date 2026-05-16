namespace FultEngineMvp;

public sealed class MvpRoundResult
{
	public static readonly MvpRoundResult Empty = new MvpRoundResult(hasWinner: false, string.Empty, string.Empty, 0, string.Empty, string.Empty);

	public bool HasWinner { get; }

	public string UserKey { get; }

	public string Nickname { get; }

	public int Score { get; }

	public string Reason { get; }

	public string Details { get; }

	public string ReasonWithDetails => string.IsNullOrWhiteSpace(Details) ? Reason : (Reason + " (" + Details + ")");

	public MvpRoundResult(bool hasWinner, string userKey, string nickname, int score, string reason, string details)
	{
		HasWinner = hasWinner;
		UserKey = userKey;
		Nickname = nickname;
		Score = score;
		Reason = reason;
		Details = details;
	}
}
