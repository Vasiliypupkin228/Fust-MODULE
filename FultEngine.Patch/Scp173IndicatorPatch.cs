using System.Reflection;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;

namespace FultEngine.Patch;

[HarmonyPatch(typeof(Scp173TeleportIndicator), "UpdateVisibility")]
internal static class Scp173IndicatorPatch
{
	private static readonly FieldInfo AbilityInfo = typeof(Scp173TeleportIndicator).GetField("_teleportAbility", BindingFlags.Instance | BindingFlags.NonPublic);

	internal static bool Prefix(Scp173TeleportIndicator __instance, ref bool isVisible)
	{
		Scp173TeleportAbility ability = (Scp173TeleportAbility)AbilityInfo.GetValue(__instance);
		if (Scp173BlinkPatch.CanTeleport(ability))
		{
			return true;
		}
		isVisible = false;
		return true;
	}
}
