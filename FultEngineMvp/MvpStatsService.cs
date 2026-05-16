using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using PlayerRoles;

namespace FultEngineMvp;

public sealed class MvpStatsService
{
	private readonly FultEngineMvpPlugin _plugin;

	private readonly Dictionary<string, PlayerRoundStats> _stats = new Dictionary<string, PlayerRoundStats>(StringComparer.OrdinalIgnoreCase);

	public MvpRoundResult CurrentMvp { get; private set; }

	public bool HasRoundMvp => CurrentMvp != null && CurrentMvp.HasWinner;

	public MvpStatsService(FultEngineMvpPlugin plugin)
	{
		_plugin = plugin;
	}

	public void RegisterEvents()
	{
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Escaping += (CustomEventHandler<EscapingEventArgs>)OnEscaping;
	}

	public void UnregisterEvents()
	{
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Escaping -= (CustomEventHandler<EscapingEventArgs>)OnEscaping;
	}

	public void ResetRound()
	{
		_stats.Clear();
		CurrentMvp = null;
	}

	public void HandleRoundEnded()
	{
		foreach (Player item in Player.List)
		{
			if (IsValidRealPlayer(item))
			{
				PlayerRoundStats orCreate = GetOrCreate(item);
				orCreate.Nickname = item.Nickname;
				if (item.IsAlive)
				{
					orCreate.Survived = true;
				}
			}
		}
		CurrentMvp = CalculateMvp();
		if (_plugin.Config.Debug && CurrentMvp != null && CurrentMvp.HasWinner)
		{
			Log.Info($"[FultEngineMvp] MVP: {CurrentMvp.Nickname} | Score: {CurrentMvp.Score} | {CurrentMvp.Reason}");
		}
	}

	public bool IsRoundMvp(Player player)
	{
		if (player == (Player)null || !HasRoundMvp)
		{
			return false;
		}
		return string.Equals(GetPlayerKey(player), CurrentMvp.UserKey, StringComparison.OrdinalIgnoreCase);
	}

	public string GetReasonFor(Player player)
	{
		if (player == (Player)null)
		{
			return _plugin.Config.ManualMvpReasonFallback;
		}
		if (HasRoundMvp && IsRoundMvp(player))
		{
			return CurrentMvp.ReasonWithDetails;
		}
		PlayerRoundStats playerRoundStats = TryGet(player);
		if (playerRoundStats == null)
		{
			return _plugin.Config.ManualMvpReasonFallback;
		}
		return BuildReason(playerRoundStats, CalculateScore(playerRoundStats)).ReasonWithDetails;
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		if (ev == null || ev.Attacker == (Player)null || ev.Player == (Player)null || !ev.IsAllowed)
		{
			return;
		}
		Player attacker = ev.Attacker;
		Player player = ev.Player;
		if (IsValidRealPlayer(attacker) && IsValidRealPlayer(player) && !(attacker == player))
		{
			float num = Math.Max(0f, ev.Amount);
			if (!(num <= 0f))
			{
				PlayerRoundStats orCreate = GetOrCreate(attacker);
				PlayerRoundStats orCreate2 = GetOrCreate(player);
				orCreate.DamageDealt += num;
				orCreate2.DamageTaken += num;
			}
		}
	}

	private void OnDied(DiedEventArgs ev)
	{
		if (ev == null || ev.Player == (Player)null)
		{
			return;
		}
		Player player = ev.Player;
		if (IsValidRealPlayer(player))
		{
			PlayerRoundStats orCreate = GetOrCreate(player);
			orCreate.Deaths++;
			orCreate.Nickname = player.Nickname;
		}
		Player attacker = ev.Attacker;
		if (IsValidRealPlayer(attacker) && !(attacker == player))
		{
			PlayerRoundStats orCreate2 = GetOrCreate(attacker);
			orCreate2.Kills++;
			if (IsScp(player))
			{
				orCreate2.ScpKills++;
			}
			else
			{
				orCreate2.HumanKills++;
			}
		}
	}

	private void OnEscaping(EscapingEventArgs ev)
	{
		if (ev != null && !(ev.Player == (Player)null) && ev.IsAllowed && IsValidRealPlayer(ev.Player))
		{
			PlayerRoundStats orCreate = GetOrCreate(ev.Player);
			orCreate.Escaped = true;
		}
	}

	private MvpRoundResult CalculateMvp()
	{
		List<PlayerRoundStats> list = (from s in _stats.Values
			where !string.IsNullOrWhiteSpace(s.UserKey)
			where CalculateScore(s) > 0 || s.Kills > 0 || s.ScpKills > 0 || s.Escaped || s.Survived
			select s).ToList();
		if (list.Count == 0)
		{
			return MvpRoundResult.Empty;
		}
		PlayerRoundStats playerRoundStats = list.OrderByDescending(CalculateScore).ThenByDescending((PlayerRoundStats s) => s.ScpKills).ThenByDescending((PlayerRoundStats s) => s.Kills)
			.ThenByDescending((PlayerRoundStats s) => s.DamageDealt)
			.ThenByDescending((PlayerRoundStats s) => s.Escaped)
			.FirstOrDefault();
		if (playerRoundStats == null)
		{
			return MvpRoundResult.Empty;
		}
		int score = CalculateScore(playerRoundStats);
		return BuildReason(playerRoundStats, score);
	}

	private MvpRoundResult BuildReason(PlayerRoundStats stats, int score)
	{
		string reason = ((stats.ScpKills > 0) ? $"уничтожил SCP: {stats.ScpKills}" : ((stats.Kills > 0) ? $"набрал убийств: {stats.Kills}" : (stats.Escaped ? "успешно сбежал" : ((stats.DamageDealt >= _plugin.Config.MinDamageToShowInReason) ? $"нанёс много урона: {stats.DamageDealt:0}" : ((!stats.Survived) ? "активность в раунде" : "дожил до конца раунда")))));
		List<string> list = new List<string>();
		if (stats.Kills > 0)
		{
			list.Add($"убийств: {stats.Kills}");
		}
		if (stats.ScpKills > 0)
		{
			list.Add($"SCP: {stats.ScpKills}");
		}
		if (stats.DamageDealt >= _plugin.Config.MinDamageToShowInReason)
		{
			list.Add($"урон: {stats.DamageDealt:0}");
		}
		if (stats.Escaped)
		{
			list.Add("побег: да");
		}
		if (stats.Survived)
		{
			list.Add("выжил");
		}
		string details = ((list.Count > 0) ? string.Join(" | ", list) : "без подробной статистики");
		return new MvpRoundResult(hasWinner: true, stats.UserKey, stats.Nickname, score, reason, details);
	}

	private int CalculateScore(PlayerRoundStats stats)
	{
		if (stats == null)
		{
			return 0;
		}
		int num = 0;
		num += stats.Kills * _plugin.Config.MvpPointsPerKill;
		num += stats.ScpKills * _plugin.Config.MvpBonusPerScpKill;
		num += (int)Math.Floor(stats.DamageDealt / 100f) * _plugin.Config.MvpPointsPer100Damage;
		if (stats.Escaped)
		{
			num += _plugin.Config.MvpBonusForEscape;
		}
		if (stats.Survived)
		{
			num += _plugin.Config.MvpBonusForSurvival;
		}
		num -= stats.Deaths * _plugin.Config.MvpPenaltyPerDeath;
		return Math.Max(0, num);
	}

	private PlayerRoundStats GetOrCreate(Player player)
	{
		string playerKey = GetPlayerKey(player);
		if (!_stats.TryGetValue(playerKey, out var value))
		{
			value = new PlayerRoundStats
			{
				UserKey = playerKey,
				Nickname = (((player != null) ? player.Nickname : null) ?? "Unknown")
			};
			_stats[playerKey] = value;
		}
		if (player != (Player)null)
		{
			value.Nickname = player.Nickname;
		}
		return value;
	}

	private PlayerRoundStats TryGet(Player player)
	{
		if (player == (Player)null)
		{
			return null;
		}
		PlayerRoundStats value;
		return _stats.TryGetValue(GetPlayerKey(player), out value) ? value : null;
	}

	private static bool IsValidRealPlayer(Player player)
	{
		return player != (Player)null && player.IsConnected && !player.IsHost;
	}

	private static bool IsScp(Player player)
	{
		if (player == (Player)null)
		{
			return false;
		}
		RoleTypeId type = player.Role.Type;
		return ((object)(RoleTypeId)(ref type)).ToString().StartsWith("Scp", StringComparison.OrdinalIgnoreCase);
	}

	private static string GetPlayerKey(Player player)
	{
		if (player == (Player)null)
		{
			return string.Empty;
		}
		return (!string.IsNullOrWhiteSpace(player.UserId)) ? player.UserId : player.Id.ToString();
	}
}
