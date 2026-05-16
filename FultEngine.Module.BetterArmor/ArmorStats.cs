namespace FultEngine.Module.BetterArmor;

public class ArmorStats
{
	public float ArtificialHealth { get; set; }

	public float MaxArtificialHealth { get; set; }

	public ArmorStats(float ahp)
	{
		ArtificialHealth = ahp;
		MaxArtificialHealth = ahp;
	}
}
