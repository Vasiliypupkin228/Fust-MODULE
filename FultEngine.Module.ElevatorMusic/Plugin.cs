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
using FultEngine.LoaderModule;
using Interactables.Interobjects;
using MEC;
using UnityEngine;

namespace FultEngine.Module.ElevatorMusic;

public class Plugin : IFultEngineModule
{
	private sealed class ElevatorRuntime
	{
		public int Id;

		public ElevatorChamber Elevator;

		public double LastMovingTime;

		public float DebugTimer;

		public string CurrentClipName;
	}

	private sealed class ActivePlayerMusic
	{
		public string PlayerKey;

		public int ElevatorId;

		public string ClipName;
	}

	[CompilerGenerated]
	private sealed class ElevatorLoop_d_20 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

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
		public ElevatorLoop_d_20(int __1__state)
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
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				break;
			case 2:
				__1__state = -1;
				break;
			}
			try
			{
				__4__this.Tick();
			}
			catch (Exception ex)
			{
				ex = ex;
				Log.Error("[FULT-ENGINE.ElevatorMusic] Tick error: " + ex);
			}
			__2__current = Timing.WaitForSeconds(Math.Max(0.05f, __4__this._config.PollInterval));
			__1__state = 2;
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

	[CompilerGenerated]
	private sealed class GetPlayersInsideElevator_d_23 : IEnumerable<Player>, IEnumerable, IEnumerator<Player>, IDisposable, IEnumerator
	{
		private int __1__state;

		private Player __2__current;

		private int __l__initialThreadId;

		private ElevatorChamber elevator;

		public ElevatorChamber __3__elevator;

		public Plugin __4__this;

		private Bounds? bounds;

		private Bounds expanded;

		private bool useBounds;

		private Vector3 center;

		private IEnumerator<Player> __s__5;

		private Player player;

		private Vector3 pos;

		private Vector3 flatA;

		private Vector3 flatB;

		private float horizontal;

		private float vertical;

		Player IEnumerator<Player>.Current
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
		public GetPlayersInsideElevator_d_23(int __1__state)
		{
			this.__1__state = __1__state;
			__l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = __1__state;
			if (num == -3 || (uint)(num - 1) <= 1u)
			{
				try
				{
				}
				finally
				{
					__m__Finally1();
				}
			}
			__s__5 = null;
			player = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (__1__state)
				{
				default:
					return false;
				case 0:
					__1__state = -1;
					bounds = __4__this.TryGetElevatorBounds(elevator);
					expanded = default(Bounds);
					useBounds = bounds.HasValue;
					if (useBounds)
					{
						expanded = bounds.Value;
						((Bounds)(ref expanded)).Expand(Math.Max(0f, __4__this._config.BoundsExpand));
					}
					center = ((Component)elevator).transform.position;
					__s__5 = Player.List.GetEnumerator();
					__1__state = -3;
					goto IL_0254;
				case 1:
					__1__state = -3;
					goto IL_0254;
				case 2:
					{
						__1__state = -3;
						goto IL_024c;
					}
					IL_024c:
					player = null;
					goto IL_0254;
					IL_0254:
					while (true)
					{
						if (__s__5.MoveNext())
						{
							player = __s__5.Current;
							if (__4__this.IsValidPlayer(player))
							{
								pos = player.Position;
								if (!useBounds)
								{
									break;
								}
								if (((Bounds)(ref expanded)).Contains(pos))
								{
									__2__current = player;
									__1__state = 1;
									return true;
								}
							}
							continue;
						}
						__m__Finally1();
						__s__5 = null;
						return false;
					}
					flatA = new Vector3(pos.x, 0f, pos.z);
					flatB = new Vector3(center.x, 0f, center.z);
					horizontal = Vector3.Distance(flatA, flatB);
					vertical = Mathf.Abs(pos.y - center.y);
					if (horizontal <= __4__this._config.DetectionRadius && vertical <= __4__this._config.HeightTolerance)
					{
						__2__current = player;
						__1__state = 2;
						return true;
					}
					goto IL_024c;
				}
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
			if (__s__5 != null)
			{
				__s__5.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Player> IEnumerable<Player>.GetEnumerator()
		{
			GetPlayersInsideElevator_d_23 GetPlayersInsideElevatord__;
			if (__1__state == -2 && __l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				__1__state = 0;
				GetPlayersInsideElevatord__ = this;
			}
			else
			{
				GetPlayersInsideElevatord__ = new GetPlayersInsideElevator_d_23(0)
				{
					__4__this = __4__this
				};
			}
			GetPlayersInsideElevatord__.elevator = __3__elevator;
			return GetPlayersInsideElevatord__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Player>)this).GetEnumerator();
		}
	}

	private Config _config;

	private CoroutineHandle _loop;

	private readonly Dictionary<int, ElevatorRuntime> _elevators = new Dictionary<int, ElevatorRuntime>();

	private readonly Dictionary<string, ActivePlayerMusic> _activePlayers = new Dictionary<string, ActivePlayerMusic>(StringComparer.OrdinalIgnoreCase);

	private readonly Random _random = new Random();

	private double _nextRefresh;

	public string Name => "ElevatorMusic";

	public string Author => "FUST";

	public Version Version => new Version(1, 1, 0);

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config();
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}

	public void OnEnabled()
	{
		if (_config == null)
		{
			_config = new Config();
		}
		if (_config.IsEnabled)
		{
			Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
			Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
			Server.WaitingForPlayers += new CustomEventHandler(OnWaitingForPlayers);
			_loop = Timing.RunCoroutine(ElevatorLoop(), (Segment)0);
			Log.Info("[FULT-ENGINE.ElevatorMusic] Модуль включён. DefaultClip=" + _config.DefaultClipName + ", Tracks=" + GetTrackPoolInfo() + ", Local2D=" + _config.UseLocal2DForPlayers);
		}
	}

	public void OnDisabled()
	{
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		Server.WaitingForPlayers -= new CustomEventHandler(OnWaitingForPlayers);
		if (((CoroutineHandle)(ref _loop)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _loop });
		}
		StopAllPlayers();
		_elevators.Clear();
		Log.Info("[FULT-ENGINE.ElevatorMusic] Модуль выключен.");
	}

	private void OnWaitingForPlayers()
	{
		RefreshElevators(force: true);
	}

	private void OnRestartingRound()
	{
		StopAllPlayers();
		_elevators.Clear();
		_nextRefresh = 0.0;
	}

	private void OnPlayerLeft(LeftEventArgs ev)
	{
		if (!(((ev != null) ? ((JoinedEventArgs)ev).Player : null) == (Player)null))
		{
			StopPlayer(((JoinedEventArgs)ev).Player, "left");
		}
	}

	[IteratorStateMachine(typeof(ElevatorLoop_d_20))]
	private IEnumerator<float> ElevatorLoop()
	{
		return new ElevatorLoop_d_20(0)
		{
			__4__this = this
		};
	}

	private void Tick()
	{
		double num = Time.realtimeSinceStartup;
		if (num >= _nextRefresh || _elevators.Count == 0)
		{
			RefreshElevators(force: false);
		}
		if (_elevators.Count == 0)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		ElevatorRuntime[] array = _elevators.Values.ToArray();
		foreach (ElevatorRuntime elevatorRuntime in array)
		{
			ElevatorChamber elevator = elevatorRuntime.Elevator;
			if ((Object)(object)elevator == (Object)null || (Object)(object)((Component)elevator).gameObject == (Object)null)
			{
				continue;
			}
			string sequenceName;
			bool? isReady;
			bool flag = IsElevatorMoving(elevator, out sequenceName, out isReady);
			if (flag)
			{
				elevatorRuntime.LastMovingTime = num;
			}
			bool flag2 = flag || num - elevatorRuntime.LastMovingTime <= (double)Math.Max(0f, _config.StopGraceSeconds);
			if (_config.Debug)
			{
				elevatorRuntime.DebugTimer -= Time.deltaTime;
				if (elevatorRuntime.DebugTimer <= 0f)
				{
					elevatorRuntime.DebugTimer = 3f;
					Log.Debug("[FULT-ENGINE.ElevatorMusic] " + GetElevatorLabel(elevator) + " sequence=" + sequenceName + ", ready=" + (isReady.HasValue ? isReady.Value.ToString() : "?") + ", play=" + flag2);
				}
			}
			if (!flag2)
			{
				elevatorRuntime.CurrentClipName = null;
				continue;
			}
			string clipName = ResolveClipForElevator(elevatorRuntime);
			foreach (Player item in GetPlayersInsideElevator(elevator))
			{
				string playerKey = GetPlayerKey(item);
				if (!string.IsNullOrWhiteSpace(playerKey))
				{
					hashSet.Add(playerKey);
					EnsurePlayerMusic(item, elevatorRuntime.Id, clipName);
				}
			}
		}
		ActivePlayerMusic[] array2 = _activePlayers.Values.ToArray();
		foreach (ActivePlayerMusic activePlayerMusic in array2)
		{
			if (!hashSet.Contains(activePlayerMusic.PlayerKey))
			{
				StopPlayerByKey(activePlayerMusic.PlayerKey, activePlayerMusic.ClipName, "not_in_elevator");
			}
		}
	}

	private void RefreshElevators(bool force)
	{
		double num = Time.realtimeSinceStartup;
		_nextRefresh = num + (double)Math.Max(1f, _config.ElevatorRefreshInterval);
		ElevatorChamber[] array;
		try
		{
			array = Object.FindObjectsOfType<ElevatorChamber>();
		}
		catch (Exception ex)
		{
			Log.Error("[FULT-ENGINE.ElevatorMusic] Не удалось найти ElevatorChamber: " + ex.Message);
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		ElevatorChamber[] array2 = array;
		foreach (ElevatorChamber val in array2)
		{
			if ((Object)(object)val == (Object)null || (Object)(object)((Component)val).gameObject == (Object)null)
			{
				continue;
			}
			int instanceID = ((Object)val).GetInstanceID();
			hashSet.Add(instanceID);
			if (!_elevators.ContainsKey(instanceID))
			{
				_elevators[instanceID] = new ElevatorRuntime
				{
					Id = instanceID,
					Elevator = val,
					LastMovingTime = -9999.0,
					DebugTimer = 0f
				};
				if (_config.Debug || force)
				{
					Log.Info("[FULT-ENGINE.ElevatorMusic] Найден лифт: " + GetElevatorLabel(val) + " id=" + instanceID);
				}
			}
			if (_config.TryMuteNativeElevatorAudioSources)
			{
				TryMuteNativeAudio(val);
			}
		}
		int[] array3 = _elevators.Keys.ToArray();
		foreach (int num2 in array3)
		{
			if (!hashSet.Contains(num2))
			{
				_elevators.Remove(num2);
			}
		}
		if (_config.Debug || force)
		{
			Log.Info("[FULT-ENGINE.ElevatorMusic] Лифтов в кеше: " + _elevators.Count);
		}
	}

	[IteratorStateMachine(typeof(GetPlayersInsideElevator_d_23))]
	private IEnumerable<Player> GetPlayersInsideElevator(ElevatorChamber elevator)
	{
		return new GetPlayersInsideElevator_d_23(-2)
		{
			__4__this = this,
			__3__elevator = elevator
		};
	}

	private bool IsValidPlayer(Player player)
	{
		if (player == (Player)null || player.ReferenceHub == (ReferenceHub)null || (Object)(object)player.GameObject == (Object)null)
		{
			return false;
		}
		if (string.IsNullOrWhiteSpace(player.UserId))
		{
			return false;
		}
		if (player.UserId.IndexOf("Dummy", StringComparison.OrdinalIgnoreCase) >= 0 || player.UserId.IndexOf("ID_Dummy", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return false;
		}
		try
		{
			if (!player.IsAlive)
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	private void EnsurePlayerMusic(Player player, int elevatorId, string clipName)
	{
		string key = GetPlayerKey(player);
		if (string.IsNullOrWhiteSpace(key))
		{
			return;
		}
		if (_activePlayers.TryGetValue(key, out var value))
		{
			if (value.ElevatorId == elevatorId && string.Equals(value.ClipName, clipName, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			StopPlayerByKey(key, value.ClipName, "switch_elevator_or_clip");
		}
		bool flag;
		if (_config.UseLocal2DForPlayers)
		{
			flag = AudioManager.CreateLocal2DForPlayer(player, clipName, _config.Volume, _config.Loop, !_config.Loop);
			if (!flag && _config.UseGlobalFallbackIfLocalFails)
			{
				if (!_activePlayers.Values.Any((ActivePlayerMusic a) => string.Equals(a.ClipName, clipName, StringComparison.OrdinalIgnoreCase) && a.ElevatorId == elevatorId && a.PlayerKey != key))
				{
					AudioManager.CreateGlobalAudio(clipName, _config.Volume, _config.Loop, !_config.Loop);
				}
				flag = true;
			}
		}
		else
		{
			if (!_activePlayers.Values.Any((ActivePlayerMusic a) => string.Equals(a.ClipName, clipName, StringComparison.OrdinalIgnoreCase) && a.ElevatorId == elevatorId && a.PlayerKey != key))
			{
				AudioManager.CreateGlobalAudio(clipName, _config.Volume, _config.Loop, !_config.Loop);
			}
			flag = true;
		}
		if (!flag)
		{
			Log.Warn("[FULT-ENGINE.ElevatorMusic] Не удалось запустить клип " + clipName + " игроку " + player.Nickname);
			return;
		}
		_activePlayers[key] = new ActivePlayerMusic
		{
			PlayerKey = key,
			ElevatorId = elevatorId,
			ClipName = clipName
		};
		if (_config.Debug)
		{
			Log.Debug("[FULT-ENGINE.ElevatorMusic] START " + clipName + " -> " + player.Nickname + " elevator=" + elevatorId);
		}
	}

	private void StopPlayer(Player player, string reason)
	{
		if (player == (Player)null)
		{
			return;
		}
		string playerKey = GetPlayerKey(player);
		if (!string.IsNullOrWhiteSpace(playerKey) && _activePlayers.TryGetValue(playerKey, out var value))
		{
			try
			{
				AudioManager.DestroyClipForPlayer(player, value.ClipName);
			}
			catch (Exception ex)
			{
				Log.Warn("[FULT-ENGINE.ElevatorMusic] StopPlayer error: " + ex.Message);
			}
			_activePlayers.Remove(playerKey);
			if (_config.Debug)
			{
				Log.Debug("[FULT-ENGINE.ElevatorMusic] STOP " + value.ClipName + " -> " + player.Nickname + " reason=" + reason);
			}
		}
	}

	private void StopPlayerByKey(string playerKey, string clipName, string reason)
	{
		Player val = ((IEnumerable<Player>)Player.List).FirstOrDefault((Func<Player, bool>)((Player p) => string.Equals(GetPlayerKey(p), playerKey, StringComparison.OrdinalIgnoreCase)));
		if (val != (Player)null)
		{
			StopPlayer(val, reason);
			return;
		}
		_activePlayers.Remove(playerKey);
		try
		{
			if (!string.IsNullOrWhiteSpace(clipName))
			{
				AudioManager.DestroyForGlobal(clipName);
			}
		}
		catch
		{
		}
	}

	private void StopAllPlayers()
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		ActivePlayerMusic[] array = _activePlayers.Values.ToArray();
		foreach (ActivePlayerMusic active in array)
		{
			Player val = ((IEnumerable<Player>)Player.List).FirstOrDefault((Func<Player, bool>)((Player p) => string.Equals(GetPlayerKey(p), active.PlayerKey, StringComparison.OrdinalIgnoreCase)));
			if (val != (Player)null)
			{
				try
				{
					AudioManager.DestroyClipForPlayer(val, active.ClipName);
				}
				catch
				{
				}
			}
			if (!string.IsNullOrWhiteSpace(active.ClipName))
			{
				hashSet.Add(active.ClipName);
			}
		}
		_activePlayers.Clear();
		if (!string.IsNullOrWhiteSpace(_config?.DefaultClipName))
		{
			hashSet.Add(_config.DefaultClipName);
		}
		if (_config?.TrackPool != null)
		{
			foreach (ElevatorTrackChance item in _config.TrackPool)
			{
				if (item != null && !string.IsNullOrWhiteSpace(item.ClipName))
				{
					hashSet.Add(item.ClipName.Trim());
				}
			}
		}
		foreach (string item2 in hashSet)
		{
			try
			{
				AudioManager.DestroyForGlobal(item2);
			}
			catch
			{
			}
		}
	}

	private string ResolveClipForElevator(ElevatorRuntime runtime)
	{
		if (runtime == null)
		{
			return GetFallbackClip();
		}
		if (!string.IsNullOrWhiteSpace(runtime.CurrentClipName))
		{
			return runtime.CurrentClipName;
		}
		ElevatorChamber elevator = runtime.Elevator;
		string text = ResolveForcedClipForElevator(elevator);
		if (!string.IsNullOrWhiteSpace(text))
		{
			runtime.CurrentClipName = text.Trim();
			return runtime.CurrentClipName;
		}
		runtime.CurrentClipName = RollTrackFromPool();
		if (_config.Debug)
		{
			Log.Debug("[FULT-ENGINE.ElevatorMusic] Выбран трек лифта: " + runtime.CurrentClipName + " elevator=" + runtime.Id);
		}
		return runtime.CurrentClipName;
	}

	private string ResolveForcedClipForElevator(ElevatorChamber elevator)
	{
		string elevatorLabel = GetElevatorLabel(elevator);
		if (_config.ClipByElevatorName != null)
		{
			foreach (KeyValuePair<string, string> item in _config.ClipByElevatorName)
			{
				if (string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Value) || elevatorLabel.IndexOf(item.Key, StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}
				return item.Value.Trim();
			}
		}
		return null;
	}

	private string RollTrackFromPool()
	{
		if (_config.TrackPool == null || _config.TrackPool.Count == 0)
		{
			return GetFallbackClip();
		}
		double num = 0.0;
		foreach (ElevatorTrackChance item in _config.TrackPool)
		{
			if (item != null && !string.IsNullOrWhiteSpace(item.ClipName) && !(item.Chance <= 0.0))
			{
				num += item.Chance;
			}
		}
		if (num <= 0.0)
		{
			return GetFallbackClip();
		}
		double num2;
		lock (_random)
		{
			num2 = _random.NextDouble() * num;
		}
		double num3 = 0.0;
		foreach (ElevatorTrackChance item2 in _config.TrackPool)
		{
			if (item2 != null && !string.IsNullOrWhiteSpace(item2.ClipName) && !(item2.Chance <= 0.0))
			{
				num3 += item2.Chance;
				if (num2 <= num3)
				{
					return item2.ClipName.Trim();
				}
			}
		}
		return GetFallbackClip();
	}

	private string GetFallbackClip()
	{
		return string.IsNullOrWhiteSpace(_config.DefaultClipName) ? "ElevatorMusic" : _config.DefaultClipName.Trim();
	}

	private string GetTrackPoolInfo()
	{
		if (_config == null || _config.TrackPool == null || _config.TrackPool.Count == 0)
		{
			return GetFallbackClip();
		}
		return string.Join(", ", from t in _config.TrackPool
			where t != null && !string.IsNullOrWhiteSpace(t.ClipName)
			select t.ClipName + ":" + t.Chance);
	}

	private bool IsElevatorMoving(ElevatorChamber elevator, out string sequenceName, out bool? isReady)
	{
		sequenceName = GetSequenceName(elevator);
		isReady = TryGetBoolMember(elevator, "IsReady") ?? TryGetBoolMember(elevator, "Ready");
		string text = ((sequenceName == null) ? string.Empty : sequenceName.ToLowerInvariant());
		if (_config.MovingSequenceKeywords != null)
		{
			foreach (string movingSequenceKeyword in _config.MovingSequenceKeywords)
			{
				if (string.IsNullOrWhiteSpace(movingSequenceKeyword) || text.IndexOf(movingSequenceKeyword.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}
				return true;
			}
		}
		if (isReady.HasValue && !isReady.Value && text.IndexOf("idle", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("ready", StringComparison.OrdinalIgnoreCase) < 0)
		{
			return true;
		}
		return false;
	}

	private string GetSequenceName(ElevatorChamber elevator)
	{
		object obj = GetMemberValue(elevator, "CurSequence") ?? GetMemberValue(elevator, "CurrentSequence") ?? GetMemberValue(elevator, "Sequence") ?? GetMemberValue(elevator, "_curSequence") ?? GetMemberValue(elevator, "curSequence") ?? GetMemberValue(elevator, "Network_curSequence");
		return (obj == null) ? "unknown" : obj.ToString();
	}

	private Bounds? TryGetElevatorBounds(ElevatorChamber elevator)
	{
		object obj = GetMemberValue(elevator, "WorldSpaceBounds") ?? GetMemberValue(elevator, "WorldspaceBounds") ?? GetMemberValue(elevator, "worldSpaceBounds") ?? GetMemberValue(elevator, "elevatorBounds") ?? GetMemberValue(elevator, "ElevatorBounds") ?? GetMemberValue(elevator, "Bounds");
		if (obj is Bounds value)
		{
			return value;
		}
		if (obj != null)
		{
			try
			{
				object obj2 = GetMemberValue(obj, "center") ?? GetMemberValue(obj, "Center");
				object obj3 = GetMemberValue(obj, "size") ?? GetMemberValue(obj, "Size");
				if (obj2 is Vector3 val && obj3 is Vector3 val2)
				{
					return new Bounds(val, val2);
				}
			}
			catch
			{
			}
		}
		return null;
	}

	private object GetMemberValue(object target, string name)
	{
		if (target == null || string.IsNullOrWhiteSpace(name))
		{
			return null;
		}
		Type type = target.GetType();
		try
		{
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.GetIndexParameters().Length == 0)
			{
				return property.GetValue(target, null);
			}
		}
		catch
		{
		}
		try
		{
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return field.GetValue(target);
			}
		}
		catch
		{
		}
		return null;
	}

	private bool? TryGetBoolMember(object target, string name)
	{
		if (GetMemberValue(target, name) is bool value)
		{
			return value;
		}
		return null;
	}

	private string GetElevatorLabel(ElevatorChamber elevator)
	{
		if ((Object)(object)elevator == (Object)null)
		{
			return "null";
		}
		string text = "";
		try
		{
			object obj = GetMemberValue(elevator, "Group") ?? GetMemberValue(elevator, "AssignedGroup") ?? GetMemberValue(elevator, "_assignedGroup");
			if (obj != null)
			{
				text = obj.ToString();
			}
		}
		catch
		{
		}
		string text2 = (((Object)(object)((Component)elevator).gameObject == (Object)null) ? ((Object)elevator).name : ((Object)((Component)elevator).gameObject).name);
		return string.IsNullOrWhiteSpace(text) ? text2 : (text2 + " / " + text);
	}

	private void TryMuteNativeAudio(ElevatorChamber elevator)
	{
		if ((Object)(object)elevator == (Object)null || (Object)(object)((Component)elevator).gameObject == (Object)null)
		{
			return;
		}
		try
		{
			AudioSource[] componentsInChildren = ((Component)elevator).GetComponentsInChildren<AudioSource>(true);
			AudioSource[] array = componentsInChildren;
			foreach (AudioSource val in array)
			{
				if (!((Object)(object)val == (Object)null) && (_config.AggressiveNativeMute || LooksLikeElevatorMusicSource(val)))
				{
					val.Stop();
					val.mute = true;
					val.volume = 0f;
					((Behaviour)val).enabled = false;
				}
			}
		}
		catch (Exception ex)
		{
			if (_config.Debug)
			{
				Log.Debug("[FULT-ENGINE.ElevatorMusic] Native mute failed: " + ex.Message);
			}
		}
	}

	private bool LooksLikeElevatorMusicSource(AudioSource source)
	{
		string text = ((Object)source).name ?? string.Empty;
		string text2 = (((Object)(object)((Component)source).gameObject == (Object)null) ? string.Empty : ((Object)((Component)source).gameObject).name);
		string text3 = (((Object)(object)source.clip == (Object)null) ? string.Empty : ((Object)source.clip).name);
		string text4 = (text + " " + text2 + " " + text3).ToLowerInvariant();
		return text4.IndexOf("music", StringComparison.OrdinalIgnoreCase) >= 0 || text4.IndexOf("elevator", StringComparison.OrdinalIgnoreCase) >= 0 || text4.IndexOf("lift", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private string GetPlayerKey(Player player)
	{
		if (player == (Player)null)
		{
			return null;
		}
		if (!string.IsNullOrWhiteSpace(player.UserId))
		{
			return player.UserId;
		}
		return player.Id.ToString();
	}
}
