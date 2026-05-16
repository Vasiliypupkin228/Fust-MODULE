using Exiled.API.Features;

namespace FultEngine.Module.RealDemage;

public static class PlayerExtensions
{
	public static bool IsInjuredFromFall(this Player player, Injury injury)
	{
		return injury.Type == "Перелом" && (injury.BodyPart == "Левая нога" || injury.BodyPart == "Правая нога") && injury.DamagePerSecond == 0.3f;
	}
}
