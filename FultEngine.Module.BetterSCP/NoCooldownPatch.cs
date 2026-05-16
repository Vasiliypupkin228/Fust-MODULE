using HarmonyLib;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Subroutines;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp3114Disguise), "ServerComplete")]
internal class NoCooldownPatch
{
	private static bool Prefix(Scp3114Disguise __instance)
	{
		return ((StandardSubroutine<Scp3114Role>)(object)__instance).CastRole.Disguised = true;
	}
}
