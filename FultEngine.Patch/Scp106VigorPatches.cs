using System;
using System.Reflection;
using Exiled.API.Features;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Subroutines;
using UnityEngine;

namespace FultEngine.Patch;

public static class Scp106VigorPatches
{
	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	public static class VigorAmountSetterPatch
	{
		public static bool Prefix(Scp106VigorAbilityBase __instance, ref float value)
		{
			try
			{
				value = Math.Min(value, 1f);
				return true;
			}
			catch (Exception arg)
			{
				Log.Error($"Error in VigorAmountSetterPatch: {arg}");
				return true;
			}
		}
	}

	[HarmonyPatch]
	public static class VigorUpdatePatch
	{
		[HarmonyTargetMethod]
		public static MethodBase TargetMethod(Harmony instance)
		{
			return AccessTools.Method(typeof(Scp106VigorAbilityBase), "Update", (Type[])null, (Type[])null);
		}

		public static void Postfix(Scp106VigorAbilityBase __instance)
		{
			try
			{
				ReferenceHub owner = ((StandardSubroutine<Scp106Role>)(object)__instance).Owner;
				object obj;
				if (owner == null)
				{
					obj = null;
				}
				else
				{
					PlayerRoleManager roleManager = owner.roleManager;
					obj = ((roleManager != null) ? roleManager.CurrentRole : null);
				}
				PlayerRoleBase val = (PlayerRoleBase)obj;
				if ((Object)(object)val == (Object)null || (int)val.RoleTypeId != 3)
				{
					return;
				}
				PropertyInfo propertyInfo = AccessTools.Property(typeof(Scp106VigorAbilityBase), "VigorAmount");
				if (propertyInfo == null)
				{
					Log.Error("VigorAmount property not found in Scp106VigorAbilityBase.");
					return;
				}
				object value = propertyInfo.GetValue(__instance);
				if (value == null)
				{
					Log.Error("VigorAmount returned null in Scp106VigorAbilityBase.");
					return;
				}
				float num;
				try
				{
					num = Convert.ToSingle(value);
				}
				catch (Exception arg)
				{
					Log.Error($"Failed to convert VigorAmount to float: {arg}");
					return;
				}
				if (num < 1f)
				{
					propertyInfo.SetValue(__instance, 1f);
				}
			}
			catch (Exception arg2)
			{
				Log.Error($"Error in VigorUpdatePatch: {arg2}");
			}
		}
	}

	public const float MaxVigor = 1f;
}
