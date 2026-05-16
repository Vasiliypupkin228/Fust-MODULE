using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;

namespace FultEngine.Module.BetterArmor;

public static class ArmorUtils
{
	[CompilerGenerated]
	private sealed class ArmorAHPCoroutine_d_8 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		private Item armor;

		private ArmorStats stats;

		private float currentAHP;

		private string armorType;

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
		public ArmorAHPCoroutine_d_8(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			armor = null;
			stats = null;
			armorType = null;
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
				armor = null;
				stats = null;
				break;
			}
			if (player.IsAlive)
			{
				armor = (Item)(object)player.CurrentArmor;
				if (armor != null && ArmorStats.TryGetValue(armor.Serial, out stats))
				{
					currentAHP = stats.ArtificialHealth;
					if (currentAHP <= 0f)
					{
						player.ArtificialHealth = 0f;
						StopAHPCoroutine(player);
						return false;
					}
					armorType = GetArmorTypeName(armor.Type);
					if (player.ArtificialHealth != currentAHP)
					{
						player.ArtificialHealth = 0f;
						player.AddAhp(currentAHP, stats.MaxArtificialHealth, 0f, 0.7f, 0f, false);
					}
					armorType = null;
					__2__current = Timing.WaitForSeconds(1f);
					__1__state = 1;
					return true;
				}
				StopAHPCoroutine(player);
				return false;
			}
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

	private static readonly Dictionary<ushort, ArmorStats> ArmorStats = new Dictionary<ushort, ArmorStats>();

	private static readonly Dictionary<Player, CoroutineHandle> PlayerCoroutines = new Dictionary<Player, CoroutineHandle>();

	public static void ApplyArmorStats(Player player, Item item, Config config)
	{
		if (GetArmorAHP(item.Type, config, out var ahp))
		{
			if (!ArmorStats.ContainsKey(item.Serial))
			{
				ArmorStats.Add(item.Serial, new ArmorStats(ahp));
			}
			if (ArmorStats.TryGetValue(item.Serial, out var value))
			{
				player.ArtificialHealth = 0f;
				player.AddAhp(value.ArtificialHealth, value.MaxArtificialHealth, 0f, 0.7f, 0f, false);
				StartAHPCoroutine(player);
			}
		}
	}

	public static void DamageArmor(Item item, float damage, Player player)
	{
		if (ArmorStats.TryGetValue(item.Serial, out var value))
		{
			value.ArtificialHealth = Math.Max(0f, value.ArtificialHealth - damage);
			if (value.ArtificialHealth <= 0f)
			{
				player.ArtificialHealth = 0f;
				StopAHPCoroutine(player);
			}
			else
			{
				player.AddAhp(value.ArtificialHealth, value.MaxArtificialHealth, 0f, 0.7f, 0f, false);
			}
		}
	}

	public static void DropArmor(Player player, Item item)
	{
		if (item != null && item.IsArmor && ArmorStats.ContainsKey(item.Serial))
		{
			player.ArtificialHealth = 0f;
			StopAHPCoroutine(player);
		}
	}

	public static void PickupArmor(Player player, Item item)
	{
		if (item != null && item.IsArmor && ArmorStats.TryGetValue(item.Serial, out var value))
		{
			player.ArtificialHealth = 0f;
			player.AddAhp(value.ArtificialHealth, value.MaxArtificialHealth, 0f, 0.7f, 0f, false);
			StartAHPCoroutine(player);
		}
	}

	public static void StartAHPCoroutine(Player player)
	{
		if (PlayerCoroutines.ContainsKey(player))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { PlayerCoroutines[player] });
			PlayerCoroutines.Remove(player);
		}
		if (player.CurrentArmor != null && ((Item)player.CurrentArmor).IsArmor)
		{
			PlayerCoroutines[player] = Timing.RunCoroutine(ArmorAHPCoroutine(player));
		}
	}

	public static void StopAHPCoroutine(Player player)
	{
		if (PlayerCoroutines.ContainsKey(player))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { PlayerCoroutines[player] });
			PlayerCoroutines.Remove(player);
			player.ArtificialHealth = 0f;
		}
	}

	[IteratorStateMachine(typeof(ArmorAHPCoroutine_d_8))]
	private static IEnumerator<float> ArmorAHPCoroutine(Player player)
	{
		return new ArmorAHPCoroutine_d_8(0)
		{
			player = player
		};
	}

	public static bool GetArmorAHP(ItemType type, Config config, out float ahp)
	{
		switch (type - 36)
		{
		case 0:
			ahp = config.LightArmorAHP;
			return true;
		case 1:
			ahp = config.CombatArmorAHP;
			return true;
		case 2:
			ahp = config.HeavyArmorAHP;
			return true;
		default:
			ahp = 0f;
			return false;
		}
	}

	public static string GetArmorTypeName(ItemType type)
	{
		if (1 == 0)
		{
		}
		string result = (type - 36) switch
		{
			0 => "Легкая", 
			1 => "Средняя", 
			2 => "Тяжелая", 
			_ => "Неизвестная", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static void ClearAllStats()
	{
		ArmorStats.Clear();
		foreach (CoroutineHandle value in PlayerCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		PlayerCoroutines.Clear();
	}
}
