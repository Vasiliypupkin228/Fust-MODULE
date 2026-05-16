using HarmonyLib;
using InventorySystem.Items.Usables.Scp244;
using UnityEngine;

namespace CustomItems.Item;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class Scp244Patch2
{
	private static bool Prefix(Scp244DeployablePickup __instance)
	{
		return ((Component)__instance).transform.localScale != Vector3.zero;
	}
}
