using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp018Projectile), "Start")]
public static class Scp018ProjectilePatch
{
	public static void Postfix(Scp018Projectile __instance)
	{
		AccessTools.Field(typeof(TimeGrenade), "_fuseTime").SetValue(__instance, 600f);
	}
}
