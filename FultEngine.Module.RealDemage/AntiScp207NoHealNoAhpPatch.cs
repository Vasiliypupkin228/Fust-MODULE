using CustomPlayerEffects;
using HarmonyLib;

namespace FultEngine.Module.RealDemage;

[HarmonyPatch(typeof(AntiScp207), "OnTick")]
public static class AntiScp207NoHealNoAhpPatch
{
	[HarmonyPrefix]
	public static bool Prefix()
	{
		return false;
	}
}
