using HarmonyLib;
using PlayerRoles.PlayableScps.Scp3114;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp3114Disguise), "OnKeyUp")]
internal class NoCancelPatch
{
	private static bool Prefix()
	{
		return false;
	}
}
