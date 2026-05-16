using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using UnityEngine;

namespace FultEngine.Module.JumpLimiter;

public sealed class Plugin : IFultEngineModule
{
	private sealed class JumpState
	{
		public int ConsecutiveJumps;

		public float LastJumpTime;

		public CoroutineHandle RegenHandle;
	}

	[CompilerGenerated]
	private sealed class TemporaryStaminaPenalty_d_21 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public FpcRole fpcRole;

		public Plugin __4__this;

		private float originalUsage;

		private float originalRegen;

		private FpcRole currentRole;

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
		public TemporaryStaminaPenalty_d_21(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			currentRole = null;
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
				originalUsage = fpcRole.StaminaUsageMultiplier;
				originalRegen = fpcRole.StaminaRegenMultiplier;
				fpcRole.StaminaUsageMultiplier = __4__this._config.StaminaUsageMultiplierWhilePunished;
				fpcRole.StaminaRegenMultiplier = __4__this._config.StaminaRegenMultiplierWhilePunished;
				__2__current = Timing.WaitForSeconds(__4__this._config.PenaltySeconds);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				if (player != (Player)null && player.IsAlive)
				{
					Role role = player.Role;
					currentRole = (FpcRole)(object)((role is FpcRole) ? role : null);
					if (currentRole != null)
					{
						currentRole.StaminaUsageMultiplier = originalUsage;
						currentRole.StaminaRegenMultiplier = originalRegen;
					}
				}
				return false;
			}
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

	private readonly Dictionary<Player, JumpState> _states = new Dictionary<Player, JumpState>();

	private Config _config;

	public string Name { get; } = "JumpLimiter";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 0, 0);


	public void OnEnabled()
	{
		_config = (Config)GetDefaultConfig();
		Player.Jumping += (CustomEventHandler<JumpingEventArgs>)OnJumping;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
	}

	public void OnDisabled()
	{
		Player.Jumping -= (CustomEventHandler<JumpingEventArgs>)OnJumping;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
		Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		foreach (JumpState value in _states.Values)
		{
			if (((CoroutineHandle)(ref value.RegenHandle)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value.RegenHandle });
			}
		}
		_states.Clear();
	}

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

	private void OnSpawned(SpawnedEventArgs ev)
	{
		ResetPlayer(ev.Player);
	}

	private void OnDying(DyingEventArgs ev)
	{
		ResetPlayer(ev.Player);
	}

	private void OnLeft(LeftEventArgs ev)
	{
		ResetPlayer(((JoinedEventArgs)ev).Player);
	}

	private void OnRestartingRound()
	{
		foreach (Player item in Player.List)
		{
			ResetPlayer(item);
		}
		_states.Clear();
	}

	private void OnJumping(JumpingEventArgs ev)
	{
		Player player = ev.Player;
		if (player == (Player)null || !player.IsAlive || (_config.OnlyHumans && player.IsScp))
		{
			return;
		}
		Role role = player.Role;
		FpcRole val = (FpcRole)(object)((role is FpcRole) ? role : null);
		if (val == null)
		{
			return;
		}
		JumpState state = GetState(player);
		float time = Time.time;
		if (time - state.LastJumpTime > _config.ComboResetSeconds)
		{
			state.ConsecutiveJumps = 0;
		}
		state.LastJumpTime = time;
		state.ConsecutiveJumps++;
		if (state.ConsecutiveJumps > _config.AllowedJumps)
		{
			state.ConsecutiveJumps = 0;
			if (_config.Damage > 0f)
			{
				player.Hurt(_config.Damage, _config.DamageReason, "");
			}
			val.ResetStamina(true);
			if (((CoroutineHandle)(ref state.RegenHandle)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { state.RegenHandle });
			}
			state.RegenHandle = Timing.RunCoroutine(TemporaryStaminaPenalty(player, val));
			if (_config.ShowHint)
			{
				player.ShowHint(_config.HintText.Replace("{damage}", _config.Damage.ToString("0")), _config.HintDuration);
			}
		}
	}

	[IteratorStateMachine(typeof(TemporaryStaminaPenalty_d_21))]
	private IEnumerator<float> TemporaryStaminaPenalty(Player player, FpcRole fpcRole)
	{
		return new TemporaryStaminaPenalty_d_21(0)
		{
			__4__this = this,
			player = player,
			fpcRole = fpcRole
		};
	}

	private JumpState GetState(Player player)
	{
		if (_states.TryGetValue(player, out var value))
		{
			return value;
		}
		value = new JumpState();
		_states[player] = value;
		return value;
	}

	private void ResetPlayer(Player player)
	{
		if (player == (Player)null)
		{
			return;
		}
		if (_states.TryGetValue(player, out var value))
		{
			if (((CoroutineHandle)(ref value.RegenHandle)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value.RegenHandle });
			}
			_states.Remove(player);
		}
		Role role = player.Role;
		FpcRole val = (FpcRole)(object)((role is FpcRole) ? role : null);
		if (val != null)
		{
			val.StaminaUsageMultiplier = 1f;
			val.StaminaRegenMultiplier = 1f;
		}
	}
}
