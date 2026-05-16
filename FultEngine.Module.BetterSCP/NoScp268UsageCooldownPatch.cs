using HarmonyLib;
using InventorySystem.Items.Usables;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp268), "ServerOnUsingCompleted")]
public static class NoScp268UsageCooldownPatch
{
	public static bool Prefix(Scp268 __instance)
	{
		Traverse.Create((object)__instance).Field("IsUsing").SetValue((object)false);
		Traverse.Create((object)__instance).Property("IsWorn", (object[])null).SetValue((object)true);
		Traverse.Create((object)__instance).Method("SetState", new object[1] { true }).GetValue();
		Traverse.Create((object)__instance).Method("ServerSetPersonalCooldown", new object[1] { 0f }).GetValue();
		return false;
	}
}
