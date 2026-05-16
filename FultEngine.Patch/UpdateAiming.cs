using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;

namespace FultEngine.Patch;

[HarmonyPatch(typeof(Scp173TeleportAbility), "UpdateAiming")]
internal static class UpdateAiming
{
	internal static bool Prefix(Scp173TeleportAbility __instance, bool wantsToTeleport)
	{
		return Scp173BlinkPatch.CanTeleport(__instance);
	}
}
