namespace FultEngineMvp;

public sealed class PlayerRoundStats
{
	public string UserKey { get; set; }

	public string Nickname { get; set; }

	public int Kills { get; set; }

	public int HumanKills { get; set; }

	public int ScpKills { get; set; }

	public int Deaths { get; set; }

	public float DamageDealt { get; set; }

	public float DamageTaken { get; set; }

	public bool Escaped { get; set; }

	public bool Survived { get; set; }
}
