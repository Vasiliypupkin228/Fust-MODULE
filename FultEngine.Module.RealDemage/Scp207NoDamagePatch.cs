using CustomPlayerEffects;
using HarmonyLib;

namespace FultEngine.Module.RealDemage;

[HarmonyPatch(typeof(Scp207), "OnTick")]
public static class Scp207NoDamagePatch
{
	[HarmonyPrefix]
	public static bool Prefix()
	{
		return false;
	}
}
