using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using Mirror;
using UnityEngine;

namespace FultEngine.CustomItems.NVG;

public static class NVGCollection
{
	[CompilerGenerated]
	private sealed class PlayNVGAudio_d_1 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player target;

		private IEnumerator<Player> __s__1;

		private Player player;

		private SchematicObject schematic;

		private Exception ex;

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
		public PlayNVGAudio_d_1(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = __1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					__m__Finally1();
				}
			}
			__s__1 = null;
			player = null;
			schematic = null;
			ex = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				int num = __1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					__1__state = -3;
					goto IL_0058;
				}
				__1__state = -1;
				__s__1 = Player.List.GetEnumerator();
				__1__state = -3;
				goto IL_0104;
				IL_0058:
				if (NVGData.ActiveSchematics.TryGetValue(player, out schematic))
				{
					try
					{
						AudioManager.CreateForPlayer(target, "RecoilNVG", 1f, 5f, 100f);
					}
					catch (Exception ex)
					{
						ex = ex;
						Log.Error("NVG: Ошибка при воспроизведении звука: " + ex.Message + "\n" + ex.StackTrace);
					}
					__2__current = Timing.WaitForSeconds(3f);
					__1__state = 1;
					return true;
				}
				player = null;
				goto IL_0104;
				IL_0104:
				if (__s__1.MoveNext())
				{
					player = __s__1.Current;
					goto IL_0058;
				}
				__m__Finally1();
				__s__1 = null;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		private void __m__Finally1()
		{
			__1__state = -1;
			if (__s__1 != null)
			{
				__s__1.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	public static bool EnableNVG(Player player)
	{
		if ((Object)(object)((player != null) ? player.CurrentRoom : null) == (Object)null || NVGData.Cooldown.Contains(player))
		{
			return false;
		}
		ShowHint(player, "ВКЛЮЧЕНО", "#16e0426d");
		player.EnableEffect((EffectType)14, 1f, false);
		try
		{
			SchematicObject val = ObjectSpawner.SpawnSchematic("NVG", player.Position, (Quaternion?)Quaternion.Euler(player.CameraTransform.eulerAngles), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			if ((Object)(object)val == (Object)null)
			{
				return false;
			}
			((Component)val).transform.parent = player.Transform;
			((Component)val).transform.localPosition = new Vector3(0f, 1.1f, 0f);
			NetworkServer.Spawn(((Component)val).gameObject, player.Connection);
			AudioManager.CreateForPlayer(player, "EnableNVG", 1f, 7f, 90f);
			foreach (Player item in Player.List)
			{
				if (item != player)
				{
					CullingExtensions.DestroySchematic(item, val);
				}
			}
			Timing.RunCoroutine(PlayNVGAudio(player));
			NVGData.ActiveSchematics[player] = val;
			return true;
		}
		catch
		{
			return false;
		}
	}

	[IteratorStateMachine(typeof(PlayNVGAudio_d_1))]
	private static IEnumerator<float> PlayNVGAudio(Player target)
	{
		return new PlayNVGAudio_d_1(0)
		{
			target = target
		};
	}

	public static void DisableNVG(Player player)
	{
		if (NVGData.ActiveSchematics.TryGetValue(player, out var value))
		{
			AudioManager.CreateForPlayer(player, "DisableNVG", 1f, 7f, 90f);
			ShowHint(player, "ВЫКЛЮЧЕНО", "#e8070768");
			player.EnableEffect((EffectType)5, 1f, false);
			CullingExtensions.DestroySchematic(player, value);
			NVGData.ActiveSchematics.Remove(player);
			NVGData.Cooldown.Add(player);
			Timing.CallDelayed(3f, (Action)delegate
			{
				NVGData.Cooldown.Remove(player);
			});
		}
	}

	private static void ShowHint(Player player, string status, string color)
	{
		player.ShowMeowHint(5f, "<size=29><b><color=#61616193>|</color></size> <size=19><color=" + color + ">" + status + "</color></size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
	}
}
