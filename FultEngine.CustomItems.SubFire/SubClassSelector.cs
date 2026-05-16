using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using FultEngine.Module;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.CustomItems.SubFire;

public class SubClassSelector
{
	private readonly ConcurrentDictionary<Player, (int categoryIndex, int roleIndex)> _playerIndices = new ConcurrentDictionary<Player, (int, int)>();

	private Dictionary<RoleTypeId, List<SubClassData>> _groupedSubclasses;

	private void EnsureInitialized()
	{
		if (_groupedSubclasses != null)
		{
			return;
		}
		if (Plugin.Instance?.Config?.AvailableSubclasses == null)
		{
			_groupedSubclasses = new Dictionary<RoleTypeId, List<SubClassData>>();
			return;
		}
		_groupedSubclasses = (from x in Plugin.Instance.Config.AvailableSubclasses.Values
			where x != null
			group x by x.BaseRole).ToDictionary((IGrouping<RoleTypeId, SubClassData> g) => g.Key, (IGrouping<RoleTypeId, SubClassData> g) => g.OrderBy<SubClassData, string>((SubClassData x) => x.Id, StringComparer.OrdinalIgnoreCase).ToList());
	}

	public void CycleCategory(Player player)
	{
		EnsureInitialized();
		if (!(player == (Player)null) && _groupedSubclasses.Count != 0)
		{
			int item = (_playerIndices.GetOrAdd(player, (0, 0)).categoryIndex + 1) % _groupedSubclasses.Count;
			_playerIndices[player] = (item, 0);
			ShowInfo(player);
		}
	}

	public void CycleRole(Player player)
	{
		EnsureInitialized();
		if (!(player == (Player)null) && _groupedSubclasses.Count != 0)
		{
			(int, int) orAdd = _playerIndices.GetOrAdd(player, (0, 0));
			List<SubClassData> value = _groupedSubclasses.ElementAt(orAdd.Item1).Value;
			if (value.Count != 0)
			{
				int item = (orAdd.Item2 + 1) % value.Count;
				_playerIndices[player] = (orAdd.Item1, item);
				ShowInfo(player);
			}
		}
	}

	public void ShowInfo(Player player)
	{
		EnsureInitialized();
		if (!(player == (Player)null) && _groupedSubclasses.Count != 0)
		{
			(int, int) orAdd = _playerIndices.GetOrAdd(player, (0, 0));
			KeyValuePair<RoleTypeId, List<SubClassData>> keyValuePair = _groupedSubclasses.ElementAt(orAdd.Item1);
			if (keyValuePair.Value.Count != 0)
			{
				SubClassData subClassData = keyValuePair.Value[Mathf.Clamp(orAdd.Item2, 0, keyValuePair.Value.Count - 1)];
				Color color = RoleExtensions.GetColor(keyValuePair.Key);
				string text = ColorUtility.ToHtmlStringRGB(color);
				player.ShowHint("<size=19><b>┏ Категория: <color=#" + text + ">" + GetRoleDisplayName(keyValuePair.Key) + "</color>\n┠ ID: <color=#" + text + ">" + subClassData.Id + "</color>\n┗ Роль: <color=#" + text + ">" + subClassData.Id + "</color></b></size>", 5f);
			}
		}
	}

	public void TryAssign(Player admin, Player target)
	{
		EnsureInitialized();
		if (!(admin == (Player)null) && !(target == (Player)null) && target.IsAlive && _groupedSubclasses.Count != 0 && _playerIndices.TryGetValue(admin, out (int, int) value))
		{
			SubClassData subClassData = _groupedSubclasses.ElementAt(value.Item1).Value.ElementAtOrDefault(value.Item2);
			if (subClassData != null)
			{
				bool flag = Plugin.Instance != null && Plugin.Instance.AssignCustomSubclassById(target, subClassData.Id, preservePosition: true);
				string text = ColorUtility.ToHtmlStringRGB(RoleExtensions.GetColor(subClassData.BaseRole));
				admin.ShowHint(flag ? ("<size=19><b>┏ Выдано игроку: " + target.Nickname + "\n┠ ID: <color=#" + text + ">" + subClassData.Id + "</color>\n┗ Роль: <color=#" + text + ">" + subClassData.Id + "</color></b></size>") : ("<size=19><b><color=#ff4d4d>Не удалось выдать подкласс игроку " + target.Nickname + "</color></b></size>"), 5f);
			}
		}
	}

	public void ClearPlayer(Player player)
	{
		if (player != (Player)null)
		{
			_playerIndices.TryRemove(player, out (int, int) _);
		}
	}

	private static string GetRoleDisplayName(RoleTypeId role)
	{
		if (1 == 0)
		{
		}
		string result = (role - 1) switch
		{
			0 => "Д-Класс", 
			5 => "Учёные", 
			14 => "Охрана комплекса", 
			12 => "МОГ — Рядовой", 
			10 => "МОГ — Сержант", 
			11 => "МОГ — Капитан", 
			3 => "МОГ — Специалист", 
			7 => "Повстанцы Хаоса — Рекрут", 
			17 => "Повстанцы Хаоса — Стрелок", 
			18 => "Повстанцы Хаоса — Мародёр", 
			19 => "Повстанцы Хаоса — Подавитель", 
			13 => "Обучение", 
			_ => ((object)(RoleTypeId)(ref role)).ToString(), 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
