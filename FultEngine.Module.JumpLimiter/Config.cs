namespace FultEngine.Module.JumpLimiter;

public sealed class Config
{
	public bool OnlyHumans { get; set; } = true;


	public int AllowedJumps { get; set; } = 3;


	public float ComboResetSeconds { get; set; } = 2.25f;


	public float Damage { get; set; } = 5f;


	public string DamageReason { get; set; } = "Выдохся от прыжков";


	public float PenaltySeconds { get; set; } = 1.5f;


	public float StaminaUsageMultiplierWhilePunished { get; set; } = 2f;


	public float StaminaRegenMultiplierWhilePunished { get; set; } = 0.15f;


	public bool ShowHint { get; set; } = true;


	public float HintDuration { get; set; } = 2.5f;


	public string HintText { get; set; } = "<color=#ff5555><b>Перепрыгался.</b></color>\n<size=20>-{damage} HP и стамина в ноль</size>";

}
