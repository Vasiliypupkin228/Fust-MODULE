using HarmonyLib;
using InventorySystem.Items.Usables.Scp244;
using UnityEngine;

namespace CustomItems.Item;

[HarmonyPatch(typeof(Scp244DeployablePickup), "FogPercentForPoint")]
internal static class Scp244Patch1
{
	private static bool Prefix(ref float __result, Scp244DeployablePickup __instance)
	{
		if (((Component)__instance).transform.localScale == Vector3.zero)
		{
			__result = 0f;
			return false;
		}
		return true;
	}
}
