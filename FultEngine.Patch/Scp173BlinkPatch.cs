using System.Reflection;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;

namespace FultEngine.Patch;

[HarmonyPatch(typeof(Scp173TeleportAbility), "TryBlink")]
internal static class Scp173BlinkPatch
{
	private static readonly FieldInfo IndicatorInfo = typeof(Scp173TeleportAbility).GetField("_tpIndicator", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly FieldInfo TrackerInfo = typeof(Scp173TeleportAbility).GetField("_observersTracker", BindingFlags.Instance | BindingFlags.NonPublic);

	private const int ObserverFreezeThreshold = 4;

	internal static bool CanTeleport(Scp173TeleportAbility ability)
	{
		Scp173ObserversTracker val = (Scp173ObserversTracker)TrackerInfo.GetValue(ability);
		int currentObservers = val.CurrentObservers;
		return currentObservers < 4;
	}

	internal static bool Prefix(Scp173TeleportAbility __instance, float maxDis)
	{
		return CanTeleport(__instance);
	}
}
