using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.Plugins;
using HintServiceMeow.Core.Enum;
using MEC;
using UnityEngine;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Detector : CustomItem
{
	[CompilerGenerated]
	private sealed class DisplayHintRoutine_d_36 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Detector __4__this;

		private IEnumerator<Player> __s__1;

		private Player holder;

		private int yums;

		private float distance;

		private int finalYums;

		private string nearbyInfo;

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
		public DisplayHintRoutine_d_36(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__1 = null;
			holder = null;
			nearbyInfo = null;
			ex = null;
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
				break;
			case 1:
				__1__state = -1;
				break;
			}
			try
			{
				__s__1 = Player.List.GetEnumerator();
				try
				{
					while (__s__1.MoveNext())
					{
						holder = __s__1.Current;
						if (holder.IsAlive && holder.CurrentItem != null && ((CustomItem)__4__this).Check(holder.CurrentItem))
						{
							(int, float) tuple = __4__this.ScanNearestTarget(holder, __4__this.MaxRadius);
							yums = tuple.Item1;
							distance = tuple.Item2;
							finalYums = yums;
							finalYums = __4__this.ApplyYakorSuppression(holder, finalYums);
							nearbyInfo = ((finalYums == 0) ? "0 (нет целей)" : $"{finalYums}");
							holder.ShowMeowHint(1f, "<size=29><b><color=#61616193>|</color></size> <size=19>Ближайшие юмы: " + nearbyInfo + "</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 95, 0, (HintAlignment)2);
							nearbyInfo = null;
							holder = null;
						}
					}
				}
				finally
				{
					if (__s__1 != null)
					{
						__s__1.Dispose();
					}
				}
				__s__1 = null;
			}
			catch (Exception ex)
			{
				ex = ex;
				Log.Error($"Ошибка в DisplayHintRoutine: {ex}");
			}
			__2__current = Timing.WaitForSeconds(__4__this.HintUpdateInterval);
			__1__state = 1;
			return true;
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

	private CoroutineHandle _displayHintHandle;

	private static readonly Random Rng = new Random();

	public override uint Id { get; set; } = 15u;


	public override string Name { get; set; } = "<b><color=#00ff00df>Детектор Юмов</color></b>";


	public override string Description { get; set; } = "Отображает ближайшее скопление Юмов.";


	public override float Weight { get; set; } = 0.5f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
	{
		Limit = 1u,
		DynamicSpawnPoints = new List<DynamicSpawnPoint>()
	};


	public float MaxRadius { get; set; } = 30f;


	public float HintUpdateInterval { get; set; } = 0.5f;


	public float ScpComboBonus { get; set; } = 0f;


	protected override void SubscribeEvents()
	{
		if (((CoroutineHandle)(ref _displayHintHandle)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _displayHintHandle });
		}
		_displayHintHandle = Timing.RunCoroutine(DisplayHintRoutine());
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		if (((CoroutineHandle)(ref _displayHintHandle)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _displayHintHandle });
		}
		((CustomItem)this).UnsubscribeEvents();
	}

	[IteratorStateMachine(typeof(DisplayHintRoutine_d_36))]
	private IEnumerator<float> DisplayHintRoutine()
	{
		return new DisplayHintRoutine_d_36(0)
		{
			__4__this = this
		};
	}

	private int ApplyYakorSuppression(Player player, int currentYums)
	{
		Vector3 position = player.Position;
		foreach (YakorZoneManager.YakorZone activeYakor in YakorZoneManager.ActiveYakors)
		{
			float num = Vector3.Distance(position, activeYakor.Position);
			if (num <= activeYakor.Radius)
			{
				int val = Rng.Next(1, 6);
				return Math.Min(currentYums, val);
			}
		}
		return currentYums;
	}

	private (int yums, float distance) ScanNearestTarget(Player player, float maxRadius)
	{
		int item = 0;
		float num = maxRadius + 1f;
		YumsZoneManager.YumsZone yumsZone = null;
		if (player == (Player)null || (Object)(object)player.GameObject == (Object)null)
		{
			return (yums: 0, distance: 0f);
		}
		Vector3 position = player.Position;
		foreach (YumsZoneManager.YumsZone zone in YumsZoneManager.Zones)
		{
			float num2 = Vector3.Distance(position, zone.Position);
			if (num2 <= maxRadius && num2 < num)
			{
				num = num2;
				yumsZone = zone;
			}
		}
		if (yumsZone != null)
		{
			float num3 = Mathf.Clamp01(num / yumsZone.Radius);
			float num4 = 1f - num3;
			item = Mathf.CeilToInt(num4 * (float)yumsZone.YumsAmount);
		}
		return (yums: item, distance: num);
	}
}
