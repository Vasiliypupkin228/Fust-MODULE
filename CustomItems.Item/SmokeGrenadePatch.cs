using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables.Scp244;
using MEC;
using UnityEngine;

namespace CustomItems.Item;

[HarmonyPatch(typeof(TimeGrenade), "ServerFuseEnd")]
internal static class SmokeGrenadePatch
{
	[CompilerGenerated]
	private sealed class SmokeRoutine_d_1 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Scp244Pickup pickup;

		public Vector3 lockedPos;

		public int totalDuration;

		public float fadeTime;

		private Scp244DeployablePickup basePickup;

		private int fadeSteps;

		private float sizePerStep;

		private float holdTime;

		private float elapsed;

		private int i;

		private int i;

		float IEnumerator<float>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public SmokeRoutine_d_1(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			basePickup = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			switch (__1__state)
			{
			default:
				return false;
			case 0:
				__1__state = -1;
				__2__current = float.NegativeInfinity;
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				if (pickup == null || (Object)(object)pickup.Base == (Object)null || pickup.IsBroken)
				{
					return false;
				}
				basePickup = pickup.Base;
				fadeSteps = Mathf.Max(1, (int)(fadeTime / 0.05f));
				sizePerStep = 1f / (float)fadeSteps;
				i = 0;
				goto IL_017e;
			case 2:
				__1__state = -1;
				i++;
				goto IL_017e;
			case 3:
				__1__state = -1;
				goto IL_0242;
			case 4:
				{
					__1__state = -1;
					i++;
					break;
				}
				IL_0242:
				if (elapsed < holdTime)
				{
					if (pickup == null || pickup.IsBroken)
					{
						return false;
					}
					LockPosition(pickup, lockedPos);
					elapsed += 0.1f;
					__2__current = Timing.WaitForSeconds(0.1f);
					__1__state = 3;
					return true;
				}
				i = 0;
				break;
				IL_017e:
				if (i < fadeSteps)
				{
					if (pickup == null || pickup.IsBroken)
					{
						return false;
					}
					LockPosition(pickup, lockedPos);
					pickup.CurrentSizePercent = Mathf.Min(1f, pickup.CurrentSizePercent + sizePerStep);
					basePickup.Network_syncSizePercent = (byte)Mathf.RoundToInt(pickup.CurrentSizePercent * 255f);
					__2__current = Timing.WaitForSeconds(0.05f);
					__1__state = 2;
					return true;
				}
				pickup.CurrentSizePercent = 1f;
				basePickup.Network_syncSizePercent = byte.MaxValue;
				holdTime = (float)totalDuration - 2f * fadeTime;
				elapsed = 0f;
				goto IL_0242;
			}
			if (i < fadeSteps)
			{
				if (pickup == null || pickup.IsBroken)
				{
					return false;
				}
				LockPosition(pickup, lockedPos);
				pickup.CurrentSizePercent = Mathf.Max(0f, pickup.CurrentSizePercent - sizePerStep);
				basePickup.Network_syncSizePercent = (byte)Mathf.RoundToInt(pickup.CurrentSizePercent * 255f);
				__2__current = Timing.WaitForSeconds(0.05f);
				__1__state = 4;
				return true;
			}
			pickup.CurrentSizePercent = 0f;
			basePickup.Network_syncSizePercent = 0;
			((Pickup)pickup).Destroy();
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private static bool Prefix(TimeGrenade __instance)
	{
		CustomItem val = default(CustomItem);
		if (!CustomItem.TryGet(8u, ref val) || !(val is SmokeGrenade smokeGrenade))
		{
			return true;
		}
		if (!((CustomItem)smokeGrenade).TrackedSerials.Contains(((ItemPickupBase)__instance).Info.Serial))
		{
			return true;
		}
		Pickup obj = Pickup.Create((ItemType)44);
		Scp244Pickup val2 = (Scp244Pickup)(object)((obj is Scp244Pickup) ? obj : null);
		if (val2 == null)
		{
			return true;
		}
		val2.State = (Scp244State)1;
		val2.MaxDiameter = smokeGrenade.MaxDiameter;
		((Pickup)val2).PreviousOwner = Server.Host;
		((Pickup)val2).IsLocked = true;
		Rigidbody component = ((Pickup)val2).GameObject.GetComponent<Rigidbody>();
		if ((Object)(object)component != (Object)null)
		{
			component.isKinematic = true;
			component.useGravity = false;
		}
		Vector3 position = ((ItemPickupBase)__instance).Position;
		((Pickup)val2).Spawn(position, (Quaternion?)null, (Player)null);
		Timing.RunCoroutine(SmokeRoutine(val2, position, smokeGrenade.TotalSmokingTime, smokeGrenade.FadeTime));
		Object.Destroy((Object)(object)((Component)__instance).gameObject);
		return false;
	}

	[IteratorStateMachine(typeof(SmokeRoutine_d_1))]
	private static IEnumerator<float> SmokeRoutine(Scp244Pickup pickup, Vector3 lockedPos, int totalDuration, float fadeTime)
	{
		return new SmokeRoutine_d_1(0)
		{
			pickup = pickup,
			lockedPos = lockedPos,
			totalDuration = totalDuration,
			fadeTime = fadeTime
		};
	}

	private static void LockPosition(Scp244Pickup pickup, Vector3 pos)
	{
		if (!((Object)(object)((pickup != null) ? ((Pickup)pickup).GameObject : null) == (Object)null))
		{
			((Pickup)pickup).Position = pos;
			((Pickup)pickup).Rotation = Quaternion.identity;
		}
	}
}
