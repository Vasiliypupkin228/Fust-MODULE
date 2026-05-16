namespace FultEngine.Module.Reanimate;

public class Config
{
	public bool IsEnabled { get; set; } = true;


	public float ReanimationRadiusSqr { get; set; } = 3f;


	public float ReanimationTime { get; set; } = 15f;


	public float SuccessChance { get; set; } = 0.7f;


	public float MaxReanimationDelay { get; set; } = 59f;

}
