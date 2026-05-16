using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp049CallAbility), "ServerProcessCmd")]
public static class Scp049CallDisablePatch
{
	[HarmonyPrefix]
	public static bool Prefix()
	{
		return false;
	}
}
