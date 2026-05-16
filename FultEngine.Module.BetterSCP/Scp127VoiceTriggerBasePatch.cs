using HarmonyLib;
using InventorySystem.Items.Firearms.Modules.Scp127;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp127VoiceTriggerBase), "ServerPlayVoiceLineFromCollection")]
public static class Scp127VoiceTriggerBasePatch
{
	public static bool Prefix()
	{
		return false;
	}
}
