using CustomPlayerEffects;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerStatsSystem;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp3114Strangle), "OnAnyPlayerDied")]
public static class Scp3114StranglePatch
{
	public static void Prefix(ReferenceHub deadPly, DamageHandlerBase handler)
	{
		Scp3114DamageHandler val = (Scp3114DamageHandler)(object)((handler is Scp3114DamageHandler) ? handler : null);
		if (val != null && (int)val.Subtype == 1)
		{
			Strangled effect = deadPly.playerEffectsController.GetEffect<Strangled>();
			if (effect != null)
			{
				((StatusEffectBase)effect).Intensity = 50;
			}
		}
	}
}
