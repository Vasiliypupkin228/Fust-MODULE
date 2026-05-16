using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.API.Libraries.SSBinds;
using FultEngine.LoaderModule;
using HintServiceMeow.Core.Enum;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.Module.Ventilation;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class LoadingCoroutine_d_40 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public string title;

		public Action action;

		public Plugin __4__this;

		private Vector3 startPosition;

		private float duration;

		private float interval;

		private float elapsed;

		private float maxMove;

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
		public LoadingCoroutine_d_40(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
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
				startPosition = player.Position;
				duration = Mathf.Max(0.05f, __4__this._config.LoadingSeconds);
				interval = Mathf.Clamp(__4__this._config.LoadingUpdateInterval, 0.05f, 0.5f);
				elapsed = 0f;
				break;
			case 1:
				__1__state = -1;
				elapsed += interval;
				break;
			}
			if (elapsed < duration)
			{
				if (player == (Player)null || !player.IsAlive || (!__4__this._config.AllowScp && player.IsScp))
				{
					__4__this._loadingCoroutines.Remove(player);
					return false;
				}
				if (__4__this._config.CancelLoadingOnMove)
				{
					maxMove = Mathf.Max(0.1f, __4__this._config.MaxMoveDuringLoading);
					Vector3 val = player.Position - startPosition;
					if (((Vector3)(ref val)).sqrMagnitude > maxMove * maxMove)
					{
						__4__this.ShowSmallHint(player, "Действие отменено: вы отошли");
						__4__this._loadingCoroutines.Remove(player);
						return false;
					}
				}
				__4__this.ShowLoadingHint(player, title, elapsed / duration);
				__2__current = Timing.WaitForSeconds(interval);
				__1__state = 1;
				return true;
			}
			__4__this.ShowLoadingHint(player, title, 1f);
			try
			{
				action();
			}
			catch (Exception ex)
			{
				ex = ex;
				Log.Error($"[FULT-ENGINE.VentilationSystem] Ошибка действия вентиляции: {ex}");
				__4__this.ShowSmallHint(player, "Ошибка вентиляции. Проверь консоль");
			}
			__4__this._loadingCoroutines.Remove(player);
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

	[CompilerGenerated]
	private sealed class MenuCoroutine_d_37 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Plugin __4__this;

		private float interval;

		private List<VentMenuOption> options;

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
		public MenuCoroutine_d_37(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			options = null;
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
				interval = Mathf.Max(0.25f, __4__this._config.MenuUpdateInterval);
				break;
			case 1:
				__1__state = -1;
				options = null;
				break;
			}
			if (player != (Player)null && player.IsAlive && (__4__this._config.AllowScp || !player.IsScp) && !__4__this._loadingCoroutines.ContainsKey(player))
			{
				options = __4__this.GetAvailableOptions(player);
				if (options.Count != 0)
				{
					if (!__4__this._selectedOption.ContainsKey(player) || __4__this._selectedOption[player] >= options.Count)
					{
						__4__this._selectedOption[player] = 0;
					}
					__4__this.ShowMenu(player, options);
					__2__current = Timing.WaitForSeconds(interval);
					__1__state = 1;
					return true;
				}
			}
			__4__this.CloseMenu(player);
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

	private VentilationConfig _config = new VentilationConfig();

	private readonly Dictionary<Player, CoroutineHandle> _menuCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, CoroutineHandle> _loadingCoroutines = new Dictionary<Player, CoroutineHandle>();

	private readonly Dictionary<Player, int> _selectedOption = new Dictionary<Player, int>();

	private readonly Dictionary<Player, float> _cooldowns = new Dictionary<Player, float>();

	private readonly Dictionary<string, Vector3> _returnPositions = new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, RoundExitTarget> _roundExitTargets = new Dictionary<string, RoundExitTarget>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, List<Transform>> _markerCache = new Dictionary<string, List<Transform>>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, float> _nearbyFallbackCooldowns = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

	private readonly HashSet<string> _playersWithVentAmbience = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	private readonly Random _random = new Random();

	private float _nextMarkerCacheRebuild;

	private bool _markerCacheBuilt;

	public string Name => "VentilationSystem";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 4);

	public void OnEnabled()
	{
		KeybindManager.OnObjectInteraction += OnKeybindPressed;
		Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
		Server.WaitingForPlayers += new CustomEventHandler(OnWaitingForPlayers);
		Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnPlayerDied;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		ResetRoundState();
		Timing.CallDelayed(Mathf.Max(0.5f, _config.InitialMarkerCacheDelay), (Action)RebuildMarkerCache);
	}

	public void OnDisabled()
	{
		KeybindManager.OnObjectInteraction -= OnKeybindPressed;
		Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
		Server.WaitingForPlayers -= new CustomEventHandler(OnWaitingForPlayers);
		Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnPlayerDied;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		KillAllCoroutines();
		_selectedOption.Clear();
		_cooldowns.Clear();
		_returnPositions.Clear();
		_roundExitTargets.Clear();
		_markerCache.Clear();
		StopAllVentAmbience();
		_nearbyFallbackCooldowns.Clear();
		_nextMarkerCacheRebuild = 0f;
		_markerCacheBuilt = false;
	}

	public Type GetConfigType()
	{
		return typeof(VentilationConfig);
	}

	public object GetDefaultConfig()
	{
		return new VentilationConfig();
	}

	public void SetConfig(object config)
	{
		_config = (config as VentilationConfig) ?? new VentilationConfig();
	}

	private void OnRoundStarted()
	{
		ResetRoundState();
		Timing.CallDelayed(_config.RoundExitSelectDelay, (Action)SelectRoundExitTargets);
		Timing.CallDelayed(Mathf.Max(0.5f, _config.InitialMarkerCacheDelay), (Action)RebuildMarkerCache);
	}

	private void OnWaitingForPlayers()
	{
		ResetRoundState();
	}

	private void OnRestartingRound()
	{
		ResetRoundState();
	}

	private void OnPlayerLeft(LeftEventArgs ev)
	{
		CleanupPlayer(((JoinedEventArgs)ev).Player);
	}

	private void OnPlayerDied(DiedEventArgs ev)
	{
		CleanupPlayer(ev.Player);
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		CleanupPlayer(ev.Player);
	}

	private void ResetRoundState()
	{
		KillAllCoroutines();
		_selectedOption.Clear();
		_cooldowns.Clear();
		_returnPositions.Clear();
		_roundExitTargets.Clear();
		_markerCache.Clear();
		StopAllVentAmbience();
		_nearbyFallbackCooldowns.Clear();
		_nextMarkerCacheRebuild = 0f;
		_markerCacheBuilt = false;
	}

	private void KillAllCoroutines()
	{
		foreach (CoroutineHandle value in _menuCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
		}
		foreach (CoroutineHandle value2 in _loadingCoroutines.Values)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value2 });
		}
		_menuCoroutines.Clear();
		_loadingCoroutines.Clear();
	}

	private void CleanupPlayer(Player player, bool removeReturnPosition = true)
	{
		if (!(player == (Player)null))
		{
			CloseMenu(player);
			StopLoading(player);
			_cooldowns.Remove(player);
			StopVentAmbience(player);
			if (removeReturnPosition)
			{
				_returnPositions.Remove(GetPlayerKey(player));
			}
		}
	}

	private void OnKeybindPressed(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		if (!_config.IsEnabled)
		{
			return;
		}
		SSKeybindSetting val = (SSKeybindSetting)(object)((setting is SSKeybindSetting) ? setting : null);
		if (val == null || !val.SyncIsPressed)
		{
			return;
		}
		Player val2 = Player.Get(hub);
		if (!(val2 == (Player)null) && val2.IsAlive && (_config.AllowScp || !val2.IsScp))
		{
			switch (((ServerSpecificSettingBase)val).SettingId)
			{
			case 81:
				ToggleMenu(val2);
				break;
			case 82:
				MoveSelection(val2, -1);
				break;
			case 83:
				MoveSelection(val2, 1);
				break;
			case 84:
				ExecuteSelected(val2);
				break;
			}
		}
	}

	private void ToggleMenu(Player player)
	{
		if (_loadingCoroutines.ContainsKey(player))
		{
			return;
		}
		if (_menuCoroutines.ContainsKey(player))
		{
			CloseMenu(player);
			ShowSmallHint(player, "Меню вентиляции закрыто");
			return;
		}
		List<VentMenuOption> availableOptions = GetAvailableOptions(player);
		if (availableOptions.Count == 0)
		{
			if (_config.ShowNoOptionsHint)
			{
				ShowSmallHint(player, "Рядом нет точки вентиляции");
			}
		}
		else
		{
			_selectedOption[player] = 0;
			_menuCoroutines[player] = Timing.RunCoroutine(MenuCoroutine(player));
			ShowMenu(player, availableOptions);
		}
	}

	private void MoveSelection(Player player, int direction)
	{
		if (_menuCoroutines.ContainsKey(player))
		{
			List<VentMenuOption> availableOptions = GetAvailableOptions(player);
			if (availableOptions.Count == 0)
			{
				CloseMenu(player);
				return;
			}
			int value;
			int num = (_selectedOption.TryGetValue(player, out value) ? value : 0);
			_selectedOption[player] = (num + direction + availableOptions.Count) % availableOptions.Count;
			ShowMenu(player, availableOptions);
		}
	}

	private void ExecuteSelected(Player player)
	{
		if (!_menuCoroutines.ContainsKey(player) || _loadingCoroutines.ContainsKey(player) || (_cooldowns.TryGetValue(player, out var value) && Time.time < value))
		{
			return;
		}
		List<VentMenuOption> availableOptions = GetAvailableOptions(player);
		if (availableOptions.Count == 0)
		{
			CloseMenu(player);
			return;
		}
		int value2;
		int num = (_selectedOption.TryGetValue(player, out value2) ? value2 : 0);
		if (num < 0 || num >= availableOptions.Count)
		{
			num = 0;
		}
		VentMenuOption option = availableOptions[num];
		CloseMenu(player);
		switch (option.Type)
		{
		case VentActionType.EnterVent:
			StartLoadingAction(player, "Вход в вентиляцию", delegate
			{
				EnterVentilation(player);
			});
			break;
		case VentActionType.ExitToRandomRoom:
			StartLoadingAction(player, "Открытие " + option.MarkerName, delegate
			{
				ExitVentilation(player, option.MarkerName);
			});
			break;
		case VentActionType.GoBackToSurface:
			StartLoadingAction(player, "Возврат к входу", delegate
			{
				GoBackToSurface(player);
			});
			break;
		}
	}

	[IteratorStateMachine(typeof(MenuCoroutine_d_37))]
	private IEnumerator<float> MenuCoroutine(Player player)
	{
		return new MenuCoroutine_d_37(0)
		{
			__4__this = this,
			player = player
		};
	}

	private List<VentMenuOption> GetAvailableOptions(Player player)
	{
		List<VentMenuOption> list = new List<VentMenuOption>();
		if (player == (Player)null || !player.IsAlive)
		{
			return list;
		}
		if (TryFindNearbyNamedTransform(player, _config.EntrySchematicName, _config.EntryInteractionRadius, out var found) || TryFindLookedNamedTransform(player, _config.EntrySchematicName, _config.EntryLookRayDistance, out found))
		{
			list.Add(new VentMenuOption
			{
				Type = VentActionType.EnterVent,
				Label = _config.EnterLabel,
				MarkerName = _config.EntrySchematicName
			});
		}
		foreach (string exitMarkerName in _config.ExitMarkerNames)
		{
			if (!string.IsNullOrWhiteSpace(exitMarkerName) && (TryFindNearbyNamedTransform(player, exitMarkerName, _config.ExitInteractionRadius, out found) || TryFindLookedNamedTransform(player, exitMarkerName, _config.ExitLookRayDistance, out found)))
			{
				string destinationLabel = GetDestinationLabel(exitMarkerName);
				list.Add(new VentMenuOption
				{
					Type = VentActionType.ExitToRandomRoom,
					Label = _config.ExitLabelPrefix + " " + exitMarkerName + " <color=#8a72ff>→</color> " + destinationLabel,
					MarkerName = exitMarkerName
				});
			}
		}
		if (TryFindNearbyNamedTransform(player, _config.GoBackMarkerName, _config.ExitInteractionRadius, out found) || TryFindLookedNamedTransform(player, _config.GoBackMarkerName, _config.ExitLookRayDistance, out found))
		{
			list.Add(new VentMenuOption
			{
				Type = VentActionType.GoBackToSurface,
				Label = _config.GoBackLabel,
				MarkerName = _config.GoBackMarkerName
			});
		}
		return list;
	}

	private void StartLoadingAction(Player player, string title, Action action)
	{
		if (!(player == (Player)null) && action != null)
		{
			StopLoading(player);
			_cooldowns[player] = Time.time + _config.LoadingSeconds + _config.ActionCooldown;
			_loadingCoroutines[player] = Timing.RunCoroutine(LoadingCoroutine(player, title, action));
		}
	}

	[IteratorStateMachine(typeof(LoadingCoroutine_d_40))]
	private IEnumerator<float> LoadingCoroutine(Player player, string title, Action action)
	{
		return new LoadingCoroutine_d_40(0)
		{
			__4__this = this,
			player = player,
			title = title,
			action = action
		};
	}

	private void StopLoading(Player player)
	{
		if (!(player == (Player)null) && _loadingCoroutines.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_loadingCoroutines.Remove(player);
		}
	}

	private void EnterVentilation(Player player)
	{
		if (!(player == (Player)null) && player.IsAlive)
		{
			string playerKey = GetPlayerKey(player);
			_returnPositions[playerKey] = player.Position + Vector3.up * _config.ReturnPositionYOffset;
			Vector3 position = _config.VentilationCenter.ToVector3();
			TeleportSafely(player, position, _config.EnterTeleportYOffset);
			_cooldowns[player] = Time.time + _config.ActionCooldown;
			StartVentAmbience(player);
			ShowSmallHint(player, "Вы вошли в вентиляцию");
		}
	}

	private void ExitVentilation(Player player, string markerName)
	{
		if (!(player == (Player)null) && player.IsAlive)
		{
			EnsureRoundTargetsSelected();
			if (!_roundExitTargets.TryGetValue(markerName, out var value))
			{
				ShowSmallHint(player, "Выход " + markerName + " ещё не настроен");
				return;
			}
			TeleportSafely(player, value.Position + Vector3.up * _config.ExitTeleportYOffset, 0f);
			_returnPositions.Remove(GetPlayerKey(player));
			StopVentAmbience(player);
			_cooldowns[player] = Time.time + _config.ActionCooldown;
			ShowSmallHint(player, "Вы вышли через " + markerName + ": " + value.Label);
		}
	}

	private void GoBackToSurface(Player player)
	{
		if (!(player == (Player)null) && player.IsAlive)
		{
			string playerKey = GetPlayerKey(player);
			if (!_returnPositions.TryGetValue(playerKey, out var value))
			{
				ShowSmallHint(player, "Точка входа не найдена");
				return;
			}
			TeleportSafely(player, value, _config.ExitTeleportYOffset);
			_returnPositions.Remove(playerKey);
			StopVentAmbience(player);
			_cooldowns[player] = Time.time + _config.ActionCooldown;
			ShowSmallHint(player, "Вы вернулись к месту входа");
		}
	}

	private void TeleportSafely(Player player, Vector3 position, float yOffset)
	{
		if (player == (Player)null || !player.IsAlive)
		{
			return;
		}
		Vector3 target = position + Vector3.up * yOffset;
		player.Position = target;
		if (!_config.DoubleTeleportFix)
		{
			return;
		}
		Timing.CallDelayed(0.15f, (Action)delegate
		{
			if (player != (Player)null && player.IsAlive)
			{
				player.Position = target;
			}
		});
	}

	private void StartVentAmbience(Player player)
	{
		if (player == (Player)null || !player.IsAlive || !_config.VentAmbienceEnabled || string.IsNullOrWhiteSpace(_config.VentAmbienceClipName))
		{
			return;
		}
		string playerKey = GetPlayerKey(player);
		if (string.IsNullOrWhiteSpace(playerKey) || _playersWithVentAmbience.Contains(playerKey))
		{
			return;
		}
		try
		{
			string text = AudioClipRegistry.ResolveClipName(_config.VentAmbienceClipName);
			if (!AudioClipRegistry.EnsureClipLoaded(text))
			{
				Log.Error("[FULT-ENGINE.VentilationSystem] Не удалось загрузить звук вентиляции: " + text + ". Проверь файл Ventilation/VentAmbience.ogg");
				return;
			}
			if (!AudioManager.CreateLocalForPlayer(player, text, Mathf.Clamp(_config.VentAmbienceVolume, 0f, 2f), loop: true, destroyOnEnd: false, Mathf.Max(0.05f, _config.VentAmbienceMaxDistance)))
			{
				Log.Error("[FULT-ENGINE.VentilationSystem] AudioManager не смог создать локальный звук для " + player.Nickname + ": " + text);
				return;
			}
			_playersWithVentAmbience.Add(playerKey);
			if (_config.Debug)
			{
				AudioClipRegistry.TryGetPath(text, out var fullPath);
				Log.Info("[FULT-ENGINE.VentilationSystem] Vent ambience started for " + player.Nickname + " | clip=" + text + " | path=" + fullPath);
			}
		}
		catch (Exception arg)
		{
			Log.Error($"[FULT-ENGINE.VentilationSystem] Не удалось запустить звук вентиляции для {player.Nickname}: {arg}");
		}
	}

	private void StopVentAmbience(Player player)
	{
		if (player == (Player)null)
		{
			return;
		}
		string playerKey = GetPlayerKey(player);
		if (string.IsNullOrWhiteSpace(playerKey))
		{
			return;
		}
		bool flag = _playersWithVentAmbience.Remove(playerKey);
		if ((!flag && !_config.ForceStopVentAmbienceOnCleanup) || string.IsNullOrWhiteSpace(_config.VentAmbienceClipName))
		{
			return;
		}
		try
		{
			AudioManager.DestroyClipForPlayer(player, _config.VentAmbienceClipName);
			if (_config.Debug && flag)
			{
				Log.Info("[FULT-ENGINE.VentilationSystem] Vent ambience stopped for " + player.Nickname);
			}
		}
		catch (Exception arg)
		{
			Log.Error($"[FULT-ENGINE.VentilationSystem] Не удалось остановить звук вентиляции для {player.Nickname}: {arg}");
		}
	}

	private void StopAllVentAmbience()
	{
		foreach (Player item in Player.List.ToList())
		{
			StopVentAmbience(item);
		}
		_playersWithVentAmbience.Clear();
	}

	private void SelectRoundExitTargets()
	{
		_roundExitTargets.Clear();
		List<RoundExitTarget> list = (_config.UseRandomMapRooms ? GetRandomRoomCandidates() : GetCustomExitCandidates());
		if (list.Count == 0)
		{
			Log.Warn("[FULT-ENGINE.VentilationSystem] Нет кандидатов для EXIT/EXIT2/EXIT3. Проверь конфиг VentilationSystem.yml.");
			return;
		}
		List<RoundExitTarget> list2 = new List<RoundExitTarget>(list);
		foreach (string item in _config.ExitMarkerNames.Where((string x) => !string.IsNullOrWhiteSpace(x)))
		{
			if (list2.Count == 0)
			{
				list2 = new List<RoundExitTarget>(list);
			}
			int index = _random.Next(list2.Count);
			RoundExitTarget value = list2[index];
			_roundExitTargets[item] = value;
			if (_config.UniqueRandomTargetsPerRound && list2.Count > 1)
			{
				list2.RemoveAt(index);
			}
		}
		if (!_config.Debug)
		{
			return;
		}
		foreach (KeyValuePair<string, RoundExitTarget> roundExitTarget in _roundExitTargets)
		{
			Log.Info($"[FULT-ENGINE.VentilationSystem] {roundExitTarget.Key} => {roundExitTarget.Value.Label} / {roundExitTarget.Value.Position}");
		}
	}

	private void EnsureRoundTargetsSelected()
	{
		if (_roundExitTargets.Count == 0)
		{
			SelectRoundExitTargets();
		}
	}

	private List<RoundExitTarget> GetRandomRoomCandidates()
	{
		List<RoundExitTarget> list = new List<RoundExitTarget>();
		foreach (Room item in Room.List)
		{
			if (!((Object)(object)item == (Object)null) && _config.AllowedRandomZones.Contains(item.Zone) && (int)item.Type != 45)
			{
				list.Add(new RoundExitTarget
				{
					Label = $"{item.Type} [{item.Zone}]",
					Position = item.Position + Vector3.up * _config.RoomCenterYOffset
				});
			}
		}
		return list;
	}

	private List<RoundExitTarget> GetCustomExitCandidates()
	{
		return (from x in _config.CustomExitTargets
			where x != null && x.Position != null
			select new RoundExitTarget
			{
				Label = (string.IsNullOrWhiteSpace(x.Name) ? "CustomExit" : x.Name),
				Position = x.Position.ToVector3()
			}).ToList();
	}

	private string GetDestinationLabel(string markerName)
	{
		EnsureRoundTargetsSelected();
		if (_roundExitTargets.TryGetValue(markerName, out var value))
		{
			return value.Label;
		}
		return "не выбран";
	}

	private bool TryFindLookedNamedTransform(Player player, string expectedName, float maxDistance, out Transform found)
	{
		found = null;
		if (maxDistance <= 0f || (Object)(object)((player != null) ? player.CameraTransform : null) == (Object)null || string.IsNullOrWhiteSpace(expectedName))
		{
			return false;
		}
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(player.CameraTransform.position, player.CameraTransform.forward);
		RaycastHit val2 = default(RaycastHit);
		if (!Physics.Raycast(val, ref val2, maxDistance, -1, (QueryTriggerInteraction)2))
		{
			return false;
		}
		Transform val3 = ((Component)((RaycastHit)(ref val2)).collider).transform;
		int num = 0;
		while ((Object)(object)val3 != (Object)null && num < _config.MaxParentScanDepth)
		{
			if (IsTransformMatch(val3, expectedName, includeParents: false, includeChildren: true, includeComponents: true))
			{
				found = val3;
				return true;
			}
			val3 = val3.parent;
			num++;
		}
		return false;
	}

	private bool TryFindNearbyNamedTransform(Player player, string expectedName, float maxDistance, out Transform found)
	{
		found = null;
		if (player == (Player)null || maxDistance <= 0f || string.IsNullOrWhiteSpace(expectedName))
		{
			return false;
		}
		float num = maxDistance * maxDistance;
		Vector3 position = player.Position;
		List<Transform> cachedMarkerTransforms = GetCachedMarkerTransforms(expectedName);
		Vector3 val;
		foreach (Transform item in cachedMarkerTransforms)
		{
			if (!((Object)(object)item == (Object)null))
			{
				val = item.position - position;
				float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
				if (!(sqrMagnitude > num))
				{
					num = sqrMagnitude;
					found = item;
				}
			}
		}
		if ((Object)(object)found != (Object)null)
		{
			return true;
		}
		if (!_config.EnableNearbyColliderFallback)
		{
			return false;
		}
		if (_config.UseNearbyFallbackOnlyWhenCacheMissing && cachedMarkerTransforms.Count > 0)
		{
			return false;
		}
		if (_nearbyFallbackCooldowns.TryGetValue(expectedName, out var value) && Time.time < value)
		{
			return false;
		}
		_nearbyFallbackCooldowns[expectedName] = Time.time + Mathf.Max(0.15f, _config.NearbyFallbackCooldown);
		Collider[] array = Physics.OverlapSphere(position, maxDistance, -1, (QueryTriggerInteraction)2);
		foreach (Collider val2 in array)
		{
			if (!((Object)(object)val2 == (Object)null) && !((Object)(object)((Component)val2).transform == (Object)null) && IsTransformMatch(((Component)val2).transform, expectedName, includeParents: true, includeChildren: true, includeComponents: true))
			{
				val = ((Component)val2).transform.position - position;
				float sqrMagnitude2 = ((Vector3)(ref val)).sqrMagnitude;
				if (!(sqrMagnitude2 > num))
				{
					num = sqrMagnitude2;
					found = ((Component)val2).transform;
				}
			}
		}
		return (Object)(object)found != (Object)null;
	}

	private List<Transform> GetCachedMarkerTransforms(string expectedName)
	{
		if (string.IsNullOrWhiteSpace(expectedName))
		{
			return new List<Transform>();
		}
		bool flag = !_markerCacheBuilt;
		if (_config.MarkerCacheRefreshSeconds > 0f && Time.time >= _nextMarkerCacheRebuild)
		{
			flag = true;
		}
		if (flag)
		{
			RebuildMarkerCache();
		}
		List<Transform> value;
		return _markerCache.TryGetValue(expectedName, out value) ? value : new List<Transform>();
	}

	private void RebuildMarkerCache()
	{
		_markerCache.Clear();
		_nearbyFallbackCooldowns.Clear();
		List<string> allMarkerNames = GetAllMarkerNames();
		foreach (string item in allMarkerNames)
		{
			_markerCache[item] = new List<Transform>();
		}
		if (allMarkerNames.Count == 0)
		{
			return;
		}
		Transform[] array = Object.FindObjectsOfType<Transform>();
		Dictionary<string, HashSet<int>> dictionary = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
		foreach (string item2 in allMarkerNames)
		{
			dictionary[item2] = new HashSet<int>();
		}
		Transform[] array2 = array;
		foreach (Transform val in array2)
		{
			if ((Object)(object)val == (Object)null)
			{
				continue;
			}
			foreach (string item3 in allMarkerNames)
			{
				if (IsNameMatch(((Object)val).name, item3))
				{
					AddMarkerToCache(item3, val, dictionary);
				}
			}
		}
		if (_config.EnableDeepSchematicScan)
		{
			List<Transform> value;
			List<string> list = allMarkerNames.Where((string name) => !_markerCache.TryGetValue(name, out value) || value.Count == 0).ToList();
			if (list.Count > 0 || !_config.DeepScanOnlyWhenMarkerMissing)
			{
				List<string> list2 = (_config.DeepScanOnlyWhenMarkerMissing ? list : allMarkerNames);
				Transform[] array3 = array;
				foreach (Transform val2 in array3)
				{
					if ((Object)(object)val2 == (Object)null || !HasAnySchematicComponent(val2))
					{
						continue;
					}
					foreach (string item4 in list2)
					{
						if (HasComponentNameMatch(val2, item4))
						{
							AddMarkerToCache(item4, val2, dictionary);
						}
					}
				}
			}
		}
		_markerCacheBuilt = true;
		_nextMarkerCacheRebuild = ((_config.MarkerCacheRefreshSeconds <= 0f) ? float.MaxValue : (Time.time + Mathf.Max(5f, _config.MarkerCacheRefreshSeconds)));
		if (!_config.Debug)
		{
			return;
		}
		foreach (KeyValuePair<string, List<Transform>> item5 in _markerCache)
		{
			Log.Info($"[FULT-ENGINE.VentilationSystem] Marker cache: {item5.Key} => {item5.Value.Count}");
		}
	}

	private void AddMarkerToCache(string expectedName, Transform transform, Dictionary<string, HashSet<int>> usedIds)
	{
		if (!string.IsNullOrWhiteSpace(expectedName) && !((Object)(object)transform == (Object)null))
		{
			if (!_markerCache.TryGetValue(expectedName, out var value))
			{
				value = new List<Transform>();
				_markerCache[expectedName] = value;
			}
			int instanceID = ((Object)transform).GetInstanceID();
			if (!usedIds.TryGetValue(expectedName, out var value2))
			{
				value2 = (usedIds[expectedName] = new HashSet<int>());
			}
			if (!value2.Contains(instanceID))
			{
				value2.Add(instanceID);
				value.Add(transform);
			}
		}
	}

	private List<string> GetAllMarkerNames()
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrWhiteSpace(_config.EntrySchematicName))
		{
			list.Add(_config.EntrySchematicName);
		}
		if (!string.IsNullOrWhiteSpace(_config.GoBackMarkerName))
		{
			list.Add(_config.GoBackMarkerName);
		}
		if (_config.ExitMarkerNames != null)
		{
			list.AddRange(_config.ExitMarkerNames.Where((string x) => !string.IsNullOrWhiteSpace(x)));
		}
		return list.Distinct<string>(StringComparer.OrdinalIgnoreCase).ToList();
	}

	private bool IsTransformMatch(Transform transform, string expectedName, bool includeParents, bool includeChildren, bool includeComponents)
	{
		if ((Object)(object)transform == (Object)null || string.IsNullOrWhiteSpace(expectedName))
		{
			return false;
		}
		if (IsNameMatch(((Object)transform).name, expectedName))
		{
			return true;
		}
		if (includeComponents && HasComponentNameMatch(transform, expectedName))
		{
			return true;
		}
		if (includeParents)
		{
			Transform parent = transform.parent;
			int num = 0;
			while ((Object)(object)parent != (Object)null && num < _config.MaxParentScanDepth)
			{
				if (IsNameMatch(((Object)parent).name, expectedName))
				{
					return true;
				}
				if (includeComponents && HasComponentNameMatch(parent, expectedName))
				{
					return true;
				}
				parent = parent.parent;
				num++;
			}
		}
		if (includeChildren)
		{
			Transform[] componentsInChildren = ((Component)transform).GetComponentsInChildren<Transform>(true);
			foreach (Transform val in componentsInChildren)
			{
				if (!((Object)(object)val == (Object)null) && !((Object)(object)val == (Object)(object)transform))
				{
					if (IsNameMatch(((Object)val).name, expectedName))
					{
						return true;
					}
					if (includeComponents && HasComponentNameMatch(val, expectedName))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool HasAnySchematicComponent(Transform transform)
	{
		if ((Object)(object)transform == (Object)null)
		{
			return false;
		}
		Component[] components = ((Component)transform).GetComponents<Component>();
		Component[] array = components;
		foreach (Component val in array)
		{
			if (!((Object)(object)val == (Object)null))
			{
				Type type = ((object)val).GetType();
				string text = type.FullName ?? type.Name;
				if (ContainsIgnoreCase(text, "Schematic") || ContainsIgnoreCase(text, "MapEditor") || ContainsIgnoreCase(text, "MER"))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool HasComponentNameMatch(Transform transform, string expectedName)
	{
		if ((Object)(object)transform == (Object)null || string.IsNullOrWhiteSpace(expectedName))
		{
			return false;
		}
		Component[] components = ((Component)transform).GetComponents<Component>();
		Component[] array = components;
		foreach (Component val in array)
		{
			if ((Object)(object)val == (Object)null)
			{
				continue;
			}
			Type type = ((object)val).GetType();
			string text = type.FullName ?? type.Name;
			if (ContainsIgnoreCase(text, "Schematic") || ContainsIgnoreCase(text, "MapEditor") || ContainsIgnoreCase(text, "MER"))
			{
				if (IsNameMatch(type.Name, expectedName))
				{
					return true;
				}
				if (TryReadNameFromObject(val, expectedName, 0))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool TryReadNameFromObject(object source, string expectedName, int depth)
	{
		if (source == null || depth > 1)
		{
			return false;
		}
		Type type = source.GetType();
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		PropertyInfo[] properties = type.GetProperties(bindingAttr);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.GetIndexParameters().Length == 0 && LooksLikeNameMember(propertyInfo.Name))
			{
				object value;
				try
				{
					value = propertyInfo.GetValue(source, null);
				}
				catch
				{
					continue;
				}
				if (IsMemberValueMatch(value, expectedName, depth))
				{
					return true;
				}
			}
		}
		FieldInfo[] fields = type.GetFields(bindingAttr);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (LooksLikeNameMember(fieldInfo.Name))
			{
				object value2;
				try
				{
					value2 = fieldInfo.GetValue(source);
				}
				catch
				{
					continue;
				}
				if (IsMemberValueMatch(value2, expectedName, depth))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsMemberValueMatch(object value, string expectedName, int depth)
	{
		if (value == null)
		{
			return false;
		}
		if (value is string currentName)
		{
			return IsNameMatch(currentName, expectedName);
		}
		Type type = value.GetType();
		if (type.IsPrimitive || type.IsEnum || value is Vector3)
		{
			return false;
		}
		string text = type.FullName ?? type.Name;
		if (!ContainsIgnoreCase(text, "Schematic") && !ContainsIgnoreCase(text, "MapEditor") && !ContainsIgnoreCase(text, "MER"))
		{
			return false;
		}
		return TryReadNameFromObject(value, expectedName, depth + 1);
	}

	private bool LooksLikeNameMember(string memberName)
	{
		if (string.IsNullOrWhiteSpace(memberName))
		{
			return false;
		}
		return ContainsIgnoreCase(memberName, "Name") || ContainsIgnoreCase(memberName, "Schematic");
	}

	private bool IsNameMatch(string currentName, string expectedName)
	{
		if (string.IsNullOrWhiteSpace(currentName) || string.IsNullOrWhiteSpace(expectedName))
		{
			return false;
		}
		string text = NormalizeTransformName(currentName);
		string text2 = NormalizeTransformName(expectedName);
		if (text.Equals(text2, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (text.StartsWith(text2 + "_", StringComparison.OrdinalIgnoreCase) || text.StartsWith(text2 + "-", StringComparison.OrdinalIgnoreCase) || text.StartsWith(text2 + " ", StringComparison.OrdinalIgnoreCase) || text.EndsWith("_" + text2, StringComparison.OrdinalIgnoreCase) || text.EndsWith("-" + text2, StringComparison.OrdinalIgnoreCase) || text.EndsWith(" " + text2, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (text2.Length >= 6 && ContainsIgnoreCase(text, text2))
		{
			return true;
		}
		return false;
	}

	private bool ContainsIgnoreCase(string text, string value)
	{
		if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value))
		{
			return false;
		}
		return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private string NormalizeTransformName(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return string.Empty;
		}
		string text = value.Trim();
		text = text.Replace("(Clone)", string.Empty).Trim();
		int num = text.IndexOf(" (", StringComparison.Ordinal);
		if (num > 0)
		{
			text = text.Substring(0, num).Trim();
		}
		return text;
	}

	private void ShowMenu(Player player, List<VentMenuOption> options)
	{
		if (!(player == (Player)null) && options != null && options.Count != 0)
		{
			int num = (_selectedOption.TryGetValue(player, out var value) ? value : 0);
			num = Mathf.Clamp(num, 0, options.Count - 1);
			string text = "<size=29><b><color=#4b4b4b93>『</color></b></size> <size=21><b><color=#dcdcff>" + _config.MenuTitle + "</color></b></size> <size=29><b><color=#4b4b4b93>』</color></b></size>\n";
			for (int i = 0; i < options.Count; i++)
			{
				bool flag = i == num;
				string text2 = (flag ? "<color=#8a72ff>→</color>" : " ");
				string text3 = (flag ? "<color=#8a72ff>←</color>" : "");
				text = text + "<size=19><b>" + text2 + " " + options[i].Label + " " + text3 + "</b></size>\n";
			}
			text = text + "<size=14><color=#ffffff55>" + _config.MenuFooter + "</color></size>";
			player.ShowMeowHint(Mathf.Max(0.4f, _config.MenuUpdateInterval + 0.15f), text, (HintVerticalAlign)0, _config.MenuY, _config.MenuX, (HintAlignment)2);
		}
	}

	private void ShowLoadingHint(Player player, string title, float progress)
	{
		if (!(player == (Player)null))
		{
			progress = Mathf.Clamp01(progress);
			int num = Mathf.RoundToInt(progress * 100f);
			string arg = BuildProgressBar(progress);
			string message = "<size=29><b><color=#4b4b4b93>『</color></b></size> <size=21><b><color=#dcdcff>" + title + "</color></b></size> <size=29><b><color=#4b4b4b93>』</color></b></size>\n<size=18><b><color=#ffffff99>" + _config.LoadingText + "</color></b></size>\n" + $"<size=18><b><mark=#00000060>{arg}</mark> <color=#8a72ff>{num}%</color></b></size>";
			player.ShowMeowHint(Mathf.Max(0.35f, _config.LoadingUpdateInterval + 0.18f), message, (HintVerticalAlign)0, _config.LoadingY, _config.LoadingX, (HintAlignment)2);
		}
	}

	private string BuildProgressBar(float progress)
	{
		int num = Mathf.Clamp(_config.LoadingBarSegments, 6, 40);
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(progress) * (float)num);
		if (num2 > num)
		{
			num2 = num;
		}
		string text = new string('█', num2);
		string text2 = new string('█', num - num2);
		return "<color=#8a72ff>" + text + "</color><color=#ffffff24>" + text2 + "</color>";
	}

	private void ShowSmallHint(Player player, string text)
	{
		if (!(player == (Player)null))
		{
			player.ShowMeowHint(4f, "<size=29><b><color=#4b4b4b93>|</color></b></size> <size=19><b>" + text + "</b></size> <size=29><b><color=#4b4b4b93>|</color></b></size>", (HintVerticalAlign)0, _config.NotifyY, _config.NotifyX, (HintAlignment)2);
		}
	}

	private void CloseMenu(Player player)
	{
		if (!(player == (Player)null))
		{
			if (_menuCoroutines.TryGetValue(player, out var value))
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
				_menuCoroutines.Remove(player);
			}
			_selectedOption.Remove(player);
		}
	}

	private string GetPlayerKey(Player player)
	{
		if (player == (Player)null)
		{
			return string.Empty;
		}
		if (!string.IsNullOrWhiteSpace(player.UserId))
		{
			return player.UserId;
		}
		return player.Id.ToString();
	}
}
