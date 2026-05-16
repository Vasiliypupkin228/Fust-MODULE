using HarmonyLib;
using InventorySystem.Items.Usables;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp268), "EquipUpdate")]
public static class NoScp268EquipCooldownPatch
{
	public static bool Prefix(Scp268 __instance)
	{
		bool value = Traverse.Create((object)__instance).Property("IsLocalPlayer", (object[])null).GetValue<bool>();
		bool value2 = Traverse.Create((object)__instance).Property("IsWorn", (object[])null).GetValue<bool>();
		bool value3 = Traverse.Create((object)__instance).Field("IsUsing").GetValue<bool>();
		if (value && value2 && value3)
		{
			Traverse.Create((object)__instance).Field("IsUsing").SetValue((object)false);
		}
		return false;
	}
}
