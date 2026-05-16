using Achievements;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Usables;
using Mirror;
using PlayerStatsSystem;

namespace FultEngine.Module.RealDemage;

[HarmonyPatch(typeof(Scp500), "OnEffectsActivated")]
public static class Scp500NoHealPatch
{
	[HarmonyPrefix]
	public static bool Prefix(Scp500 __instance)
	{
		ReferenceHub owner = ((ItemBase)__instance).Owner;
		if (owner == (ReferenceHub)null)
		{
			return true;
		}
		HealthStat module = owner.playerStats.GetModule<HealthStat>();
		if (((StatBase)module).CurValue < 20f)
		{
			AchievementHandlerBase.ServerAchieve((NetworkConnection)(object)owner.networkIdentity.connectionToClient, (AchievementName)28);
		}
		owner.playerEffectsController.UseMedicalItem((ItemBase)(object)__instance);
		return false;
	}
}
