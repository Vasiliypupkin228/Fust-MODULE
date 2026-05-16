using Exiled.API.Enums;

namespace FultEngine.Module;

public class SubClassEffect
{
	public EffectType Type { get; set; }

	public float Duration { get; set; }

	public byte Intensity { get; set; } = 1;


	public SubClassEffect()
	{
	}

	public SubClassEffect(EffectType type, byte intensity, float duration = 0f)
	{
		Type = type;
		Intensity = intensity;
		Duration = duration;
	}
}
