using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;
using PlayerRoles.Subroutines;

namespace FultEngine.Module.BetterSCP;

[HarmonyPatch(typeof(Scp3114Disguise), "OnProgressSet")]
internal class InfiniteDisguisePatch
{
	private static bool Prefix(Scp3114Disguise __instance)
	{
		if (!((RagdollAbilityBase<Scp3114Role>)(object)__instance).IsInProgress)
		{
			return true;
		}
		StolenIdentity curIdentity = ((StandardSubroutine<Scp3114Role>)(object)__instance).CastRole.CurIdentity;
		__instance._equipSkinSound.Play();
		curIdentity.Ragdoll = ((RagdollAbilityBase<Scp3114Role>)(object)__instance).CurRagdoll;
		curIdentity.UnitNameId = (byte)(__instance._prevUnitIds.TryGetValue(((RagdollAbilityBase<Scp3114Role>)(object)__instance).CurRagdoll, out var value) ? value : 0);
		curIdentity.Status = (DisguiseStatus)1;
		((StandardSubroutine<Scp3114Role>)(object)__instance).CastRole.Disguised = true;
		BasicRagdoll curRagdoll = ((RagdollAbilityBase<Scp3114Role>)(object)__instance).CurRagdoll;
		DynamicRagdoll val = (DynamicRagdoll)(object)((curRagdoll is DynamicRagdoll) ? curRagdoll : null);
		if (val != null)
		{
			Scp3114RagdollToBonesConverter.ServerConvertNew(((StandardSubroutine<Scp3114Role>)(object)__instance).CastRole, val);
		}
		return false;
	}
}
