using HarmonyLib;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class Scp3114SlapPatch
{
	public static void Postfix(ref float __result)
	{
		__result = 5f;
	}
}
