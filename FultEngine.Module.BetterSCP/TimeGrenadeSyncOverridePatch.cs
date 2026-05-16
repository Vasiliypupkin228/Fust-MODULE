using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch]
public static class TimeGrenadeSyncOverridePatch
{
	private static readonly FieldInfo syncVarDirtyBitsField = typeof(NetworkBehaviour).GetField("syncVarDirtyBits", BindingFlags.Instance | BindingFlags.NonPublic);

	private static ulong GetSyncVarDirtyBits(NetworkBehaviour nb)
	{
		return (ulong)syncVarDirtyBitsField.GetValue(nb);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	[HarmonyPatch(typeof(ItemPickupBase), "SerializeSyncVars")]
	private static void CallBase(ItemPickupBase _, NetworkWriter writer, bool forceAll)
	{
	}

	[HarmonyPatch(typeof(TimeGrenade), "SerializeSyncVars")]
	private static bool Prefix(TimeGrenade __instance, NetworkWriter writer, bool forceAll)
	{
		CallBase((ItemPickupBase)(object)__instance, writer, forceAll);
		if (forceAll)
		{
			NetworkWriterExtensions.WriteDouble(writer, double.MaxValue);
			return false;
		}
		ulong syncVarDirtyBits = GetSyncVarDirtyBits((NetworkBehaviour)(object)__instance);
		NetworkWriterExtensions.WriteULong(writer, syncVarDirtyBits);
		if ((syncVarDirtyBits & 2) != 0)
		{
			NetworkWriterExtensions.WriteDouble(writer, double.MaxValue);
		}
		return false;
	}
}
