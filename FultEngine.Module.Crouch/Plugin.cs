using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using UnityEngine;

namespace FultEngine.Module.Crouch;

public sealed class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class AnimateScaleCoroutine_d_20 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Vector3 targetScale;

		public Plugin __4__this;

		private Vector3 startScale;

		private float elapsed;

		private float duration;

		private float t;

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
		public AnimateScaleCoroutine_d_20(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
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
				if (player == (Player)null)
				{
					return false;
				}
				startScale = player.Scale;
				elapsed = 0f;
				duration = Mathf.Max(0.01f, __4__this._config.AnimationDuration);
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (elapsed < duration)
			{
				if (player == (Player)null || !player.IsConnected)
				{
					return false;
				}
				elapsed += __4__this._config.AnimationStep;
				t = Mathf.Clamp01(elapsed / duration);
				player.Scale = Vector3.Lerp(startScale, targetScale, t);
				__2__current = Timing.WaitForSeconds(__4__this._config.AnimationStep);
				__1__state = 1;
				return true;
			}
			if (player != (Player)null && player.IsConnected)
			{
				player.Scale = targetScale;
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

	private Config _config = new Config();

	private KeybindSetting _crouchKeybind;

	private readonly Dictionary<string, bool> _isCrouching = new Dictionary<string, bool>();

	private readonly Dictionary<string, CoroutineHandle> _animationCoroutines = new Dictionary<string, CoroutineHandle>();

	public string Name => "Crouch";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 1);

	public void OnEnabled()
	{
		RegisterKeybind();
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
	}

	public void OnDisabled()
	{
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		foreach (Player item in Player.List)
		{
			ResetPlayer(item, restoreScale: false);
		}
		foreach (CoroutineHandle value in _animationCoroutines.Values)
		{
			CoroutineHandle current2 = value;
			if (((CoroutineHandle)(ref current2)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { current2 });
			}
		}
		_animationCoroutines.Clear();
		_isCrouching.Clear();
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
		_config = (config as Config) ?? new Config();
	}

	private void RegisterKeybind()
	{
		try
		{
			_crouchKeybind = new KeybindSetting(_config.KeybindId, _config.KeybindLabel, _config.DefaultKey, true, _config.KeybindDescription, (HeaderSetting)null, (Action<Player, SettingBase>)OnKeyPressed);
			SettingBase.Register((IEnumerable<SettingBase>)(object)new SettingBase[1] { (SettingBase)_crouchKeybind }, (Func<Player, bool>)null);
		}
		catch (Exception arg)
		{
			Log.Error($"[Crouch] Ошибка регистрации бинда: {arg}");
		}
	}

	private void OnKeyPressed(Player player, SettingBase setting)
	{
		if (!(player == (Player)null) && setting != null && player.IsAlive && CanUseCrouch(player))
		{
			ToggleCrouch(player);
		}
	}

	private bool CanUseCrouch(Player player)
	{
		if (player == (Player)null || !player.IsAlive)
		{
			return false;
		}
		if (!_config.AllowScps && player.IsScp)
		{
			return false;
		}
		return true;
	}

	private void ToggleCrouch(Player player)
	{
		string key = player.UserId ?? player.Id.ToString();
		bool value;
		bool flag = !_isCrouching.TryGetValue(key, out value) || !value;
		_isCrouching[key] = flag;
		AnimateScale(player, flag ? _config.CrouchScale : Vector3.one);
		if (flag)
		{
			if (_config.SlownessIntensity > 0)
			{
				player.EnableEffect((EffectType)43, (byte)_config.SlownessIntensity, 0f, false);
			}
			if (_config.ShowToggleHint)
			{
				player.ShowHint(_config.CrouchEnabledHint, _config.ToggleHintDuration);
			}
		}
		else
		{
			player.DisableEffect((EffectType)43);
			if (_config.ShowToggleHint)
			{
				player.ShowHint(_config.CrouchDisabledHint, _config.ToggleHintDuration);
			}
		}
	}

	private void AnimateScale(Player player, Vector3 targetScale)
	{
		if (!(player == (Player)null))
		{
			string key = player.UserId ?? player.Id.ToString();
			if (_animationCoroutines.TryGetValue(key, out var value) && ((CoroutineHandle)(ref value)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			}
			_animationCoroutines[key] = Timing.RunCoroutine(AnimateScaleCoroutine(player, targetScale));
		}
	}

	[IteratorStateMachine(typeof(AnimateScaleCoroutine_d_20))]
	private IEnumerator<float> AnimateScaleCoroutine(Player player, Vector3 targetScale)
	{
		return new AnimateScaleCoroutine_d_20(0)
		{
			__4__this = this,
			player = player,
			targetScale = targetScale
		};
	}

	private void ResetPlayer(Player player, bool restoreScale = true)
	{
		if (!(player == (Player)null))
		{
			string key = player.UserId ?? player.Id.ToString();
			if (_animationCoroutines.TryGetValue(key, out var value) && ((CoroutineHandle)(ref value)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			}
			_animationCoroutines.Remove(key);
			_isCrouching.Remove(key);
			player.DisableEffect((EffectType)43);
			if (restoreScale && player.IsConnected)
			{
				player.Scale = Vector3.one;
			}
		}
	}

	private void OnSpawned(SpawnedEventArgs ev)
	{
		ResetPlayer(ev.Player);
	}

	private void OnDied(DiedEventArgs ev)
	{
		ResetPlayer(ev.Player);
	}

	private void OnLeft(LeftEventArgs ev)
	{
		ResetPlayer(((JoinedEventArgs)ev).Player, restoreScale: false);
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		ResetPlayer(ev.Player);
	}
}
