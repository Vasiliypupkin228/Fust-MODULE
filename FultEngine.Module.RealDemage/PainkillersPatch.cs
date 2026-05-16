using HarmonyLib;
using InventorySystem.Items.Usables;

namespace FultEngine.Module.RealDemage;

[HarmonyPatch(typeof(Painkillers), "OnEffectsActivated")]
public static class PainkillersPatch
{
	[HarmonyPrefix]
	public static bool Prefix()
	{
		return false;
	}
}
