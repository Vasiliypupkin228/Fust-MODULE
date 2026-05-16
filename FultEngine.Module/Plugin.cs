using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.Module;

public class Plugin : IFultEngineModule
{
	public static readonly ConcurrentDictionary<Player, SubClassData> PlayerSubclasses = new ConcurrentDictionary<Player, SubClassData>();

	private readonly Dictionary<string, SubClassGroup> _subclassGroups = new Dictionary<string, SubClassGroup>(StringComparer.OrdinalIgnoreCase);

	private SubClassConfig _config;

	private bool _isAssigningSubclass;

	public string Name { get; } = "Subclass";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 0, 3);


	public static Plugin Instance { get; private set; }

	public SubClassConfig Config => _config;

	public void OnEnabled()
	{
		Instance = this;
		InitializeSpawnGroups();
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnPlayerDied;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Server.EndingRound += (CustomEventHandler<EndingRoundEventArgs>)OnEndingRound;
	}

	public void OnDisabled()
	{
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnPlayerDied;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Server.EndingRound -= (CustomEventHandler<EndingRoundEventArgs>)OnEndingRound;
		PlayerSubclasses.Clear();
		_subclassGroups.Clear();
		Instance = null;
	}

	public Type GetConfigType()
	{
		return typeof(SubClassConfig);
	}

	public void SetConfig(object config)
	{
		_config = (SubClassConfig)config;
		InitializeSpawnGroups();
	}

	public object GetDefaultConfig()
	{
		return new SubClassConfig();
	}

	private void InitializeSpawnGroups()
	{
		_subclassGroups.Clear();
		if (_config?.AvailableSubclasses == null || _config.AvailableSubclasses.Count == 0)
		{
			return;
		}
		foreach (IGrouping<string, SubClassData> item in from sc in _config.AvailableSubclasses.Values
			group sc by string.IsNullOrWhiteSpace(sc.SpawnGroup) ? "default" : sc.SpawnGroup)
		{
			List<SubClassData> subClasses = item.OrderByDescending((SubClassData sc) => sc.SpawnPriority).ThenBy<SubClassData, string>((SubClassData sc) => sc.Id, StringComparer.OrdinalIgnoreCase).ToList();
			_subclassGroups[item.Key] = new SubClassGroup
			{
				Name = item.Key,
				SubClasses = subClasses
			};
		}
	}

	private void OnPlayerDied(DiedEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null))
		{
			ev.Player.CustomInfo = string.Empty;
			RemoveSubclass(ev.Player);
		}
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (!(((ev != null) ? ev.Player : null) == (Player)null) && !_isAssigningSubclass)
		{
			ev.Player.CustomInfo = string.Empty;
			RemoveSubclass(ev.Player);
		}
	}

	private void OnPlayerSpawned(SpawnedEventArgs ev)
	{
		Timing.CallDelayed(0.5f, (Action)delegate
		{
			SpawnedEventArgs obj = ev;
			if (!(((obj != null) ? obj.Player : null) == (Player)null) && ev.Player.IsAlive && !PlayerSubclasses.ContainsKey(ev.Player))
			{
				Player player = ev.Player;
				player.InfoArea = (PlayerInfoArea)(player.InfoArea & -59);
				SubClassGroup subClassGroup = FindBestGroupForRole(ev.Player.Role.Type);
				if (subClassGroup != null && subClassGroup.SubClasses.Count != 0)
				{
					foreach (SubClassData subclass in subClassGroup.SubClasses)
					{
						if (subclass.AutoSpawn)
						{
							if (!_config.IgnoreSubclassLimits && subclass.MaxPlayers > 0)
							{
								int num = PlayerSubclasses.Count((KeyValuePair<Player, SubClassData> p) => p.Value.Id == subclass.Id && p.Key != (Player)null && p.Key.IsAlive);
								if (num >= subclass.MaxPlayers)
								{
									continue;
								}
							}
							if (!(Random.Range(0f, 100f) > subclass.SpawnChance) && AssignSubclass(ev.Player, subclass))
							{
								break;
							}
						}
					}
				}
			}
		});
	}

	private void OnEndingRound(EndingRoundEventArgs ev)
	{
		try
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (Player item in Player.List)
			{
				if (item == (Player)null || !item.IsAlive)
				{
					continue;
				}
				string playerRoundTeam = GetPlayerRoundTeam(item);
				if (!string.IsNullOrWhiteSpace(playerRoundTeam))
				{
					hashSet.Add(playerRoundTeam);
					if (hashSet.Count > 1)
					{
						ev.IsAllowed = false;
						return;
					}
				}
			}
			ev.IsAllowed = true;
		}
		catch (Exception arg)
		{
			Log.Error($"[Subclass] Ошибка в OnEndingRound: {arg}");
		}
	}

	private string GetPlayerRoundTeam(Player player)
	{
		if (player == (Player)null || !player.IsAlive)
		{
			return string.Empty;
		}
		if (PlayerSubclasses.TryGetValue(player, out var value) && value != null)
		{
			if (!string.IsNullOrWhiteSpace(value.RoundTeam))
			{
				return value.RoundTeam.Trim();
			}
			return GetDefaultRoundTeam(value.BaseRole);
		}
		return GetDefaultRoundTeam(player.Role.Type);
	}

	private string GetDefaultRoundTeam(RoleTypeId role)
	{
		if (1 == 0)
		{
		}
		string result;
		switch ((int)role)
		{
		case 1:
			result = "Д-Класс";
			break;
		case 6:
			result = "Учёные";
			break;
		case 15:
			result = "Служба Безопасности";
			break;
		case 4:
		case 11:
		case 12:
		case 13:
			result = "МОГ";
			break;
		case 8:
		case 18:
		case 19:
		case 20:
			result = "Повстанцы Хаоса";
			break;
		case 0:
		case 3:
		case 5:
		case 7:
		case 9:
		case 10:
		case 16:
		case 23:
			result = "SCP";
			break;
		case 14:
			result = "Обучение";
			break;
		case 2:
		case 21:
		case 22:
			result = "Наблюдатели";
			break;
		default:
			result = "Другое";
			break;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private SubClassGroup FindBestGroupForRole(RoleTypeId role)
	{
		List<SubClassGroup> source = (from g in _subclassGroups.Values
			where g.SubClasses.Any((SubClassData sc) => AreInSameSpawnFamily(sc.BaseRole, role))
			orderby g.SubClasses.Count((SubClassData sc) => AreInSameSpawnFamily(sc.BaseRole, role)) descending
			select g).ThenBy<SubClassGroup, string>((SubClassGroup g) => g.Name, StringComparer.OrdinalIgnoreCase).ToList();
		return source.FirstOrDefault();
	}

	private bool AreInSameSpawnFamily(RoleTypeId subclassRole, RoleTypeId playerRole)
	{
		if (subclassRole == playerRole)
		{
			return true;
		}
		return GetSpawnFamily(subclassRole) == GetSpawnFamily(playerRole);
	}

	private string GetSpawnFamily(RoleTypeId role)
	{
		if (1 == 0)
		{
		}
		string result;
		switch (role - 1)
		{
		case 3:
		case 10:
		case 11:
		case 12:
			result = "MTF";
			break;
		case 7:
		case 17:
		case 18:
		case 19:
			result = "CHAOS";
			break;
		case 14:
			result = "GUARD";
			break;
		case 0:
			result = "CLASSD";
			break;
		case 5:
			result = "SCIENTIST";
			break;
		default:
			result = ((object)(RoleTypeId)(ref role)).ToString();
			break;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	public bool AssignCustomSubclassById(Player player, string subclassId, bool preservePosition = false)
	{
		if (player == (Player)null || string.IsNullOrEmpty(subclassId))
		{
			return false;
		}
		SubClassData subClassData = _config.AvailableSubclasses.Values.FirstOrDefault((SubClassData sc) => sc.Id.Equals(subclassId, StringComparison.OrdinalIgnoreCase));
		if (subClassData == null)
		{
			return false;
		}
		return AssignSubclass(player, subClassData, preservePosition);
	}

	public bool AssignSubclass(Player player, SubClassData subclass, bool preservePosition = false)
	{
		if (player == (Player)null || subclass == null)
		{
			return false;
		}
		try
		{
			_isAssigningSubclass = true;
			PlayerSubclasses[player] = subclass;
			player.Role.Set(subclass.BaseRole, (RoleSpawnFlags)0);
			player.ClearInventory(true);
			foreach (ItemType item in subclass.Items ?? new List<ItemType>())
			{
				player.AddItem(item);
			}
			foreach (ushort customItemId in subclass.CustomItems ?? new List<ushort>())
			{
				CustomItem val = ((IEnumerable<CustomItem>)CustomItem.Registered).FirstOrDefault((Func<CustomItem, bool>)((CustomItem ci) => ci.Id == customItemId));
				if (val != null)
				{
					val.Give(player, true);
				}
			}
			Timing.CallDelayed(1.2f, (Action)delegate
			{
				if (!(player == (Player)null) && player.IsConnected)
				{
					string customInfo = (subclass.CustomInfo ?? subclass.Id ?? string.Empty).Replace("[", "｢").Replace("]", "｣");
					player.CustomInfo = customInfo;
					Player obj2 = player;
					obj2.InfoArea = (PlayerInfoArea)(obj2.InfoArea & -59);
				}
			});
			if (!preservePosition && !string.IsNullOrWhiteSpace(subclass.SpawnRoom) && !subclass.SpawnRoom.Equals("No", StringComparison.OrdinalIgnoreCase) && Enum.TryParse<RoomType>(subclass.SpawnRoom, ignoreCase: true, out RoomType result))
			{
				Room val2 = Room.Get(result);
				if ((Object)(object)val2 != (Object)null)
				{
					player.Position = val2.Position + Vector3.up;
				}
			}
			player.MaxHealth = subclass.Health;
			player.Health = subclass.Health;
			foreach (SubClassEffect item2 in subclass.Effects ?? new List<SubClassEffect>())
			{
				player.EnableEffect(item2.Type, item2.Duration, false);
				player.ChangeEffectIntensity(item2.Type, item2.Intensity, 0f);
			}
			return true;
		}
		catch (Exception arg)
		{
			string id = subclass.Id;
			Player obj = player;
			Log.Error($"Ошибка при назначении подкласса {id} игроку {((obj != null) ? obj.Nickname : null)}: {arg}");
			if (player != (Player)null)
			{
				PlayerSubclasses.TryRemove(player, out var _);
			}
			return false;
		}
		finally
		{
			_isAssigningSubclass = false;
		}
	}

	public bool RemoveSubclass(Player player)
	{
		SubClassData value;
		return player != (Player)null && PlayerSubclasses.TryRemove(player, out value);
	}

	public int GetAvailableSlotsForSubclass(string subclassId)
	{
		SubClassData subclass = _config?.AvailableSubclasses?.Values.FirstOrDefault((SubClassData sc) => sc.Id.Equals(subclassId, StringComparison.OrdinalIgnoreCase));
		if (subclass == null)
		{
			return 0;
		}
		if (_config.IgnoreSubclassLimits || subclass.MaxPlayers <= 0)
		{
			return int.MaxValue;
		}
		int num = PlayerSubclasses.Count((KeyValuePair<Player, SubClassData> p) => p.Value.Id == subclass.Id && p.Key != (Player)null && p.Key.IsAlive);
		return Math.Max(0, subclass.MaxPlayers - num);
	}
}
