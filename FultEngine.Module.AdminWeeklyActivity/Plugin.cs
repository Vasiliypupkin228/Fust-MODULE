using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;

namespace FultEngine.Module.AdminWeeklyActivity;

public sealed class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class AutosaveLoop_d_29 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

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
		public AutosaveLoop_d_29(int __1__state)
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
				break;
			case 1:
				__1__state = -1;
				__4__this.Service?.CommitAndSave();
				break;
			}
			__2__current = Timing.WaitForSeconds((float)Math.Max(15, __4__this._config.AutosaveSeconds));
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

	private Config _config = new Config();

	private CoroutineHandle _autosaveCoroutine;

	public static Plugin Instance { get; private set; }

	public string Name { get; } = "AdminWeeklyActivity";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 0, 1);


	public Config ModuleConfig => _config;

	public WeeklyActivityService Service { get; private set; }

	public string ModuleDirectoryPath => Path.Combine(Paths.Plugins, "FULT-ENGINE", "ModuleData", "AdminWeeklyActivity");

	public void OnEnabled()
	{
		Instance = this;
		Directory.CreateDirectory(ModuleDirectoryPath);
		Service = new WeeklyActivityService(this);
		Service.Initialize();
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		Server.WaitingForPlayers += new CustomEventHandler(OnWaitingForPlayers);
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
		StartAutosave();
		Log.Info("[AdminWeeklyActivity] Модуль учёта недельной активности администраторов включён.");
	}

	public void OnDisabled()
	{
		Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
		Server.WaitingForPlayers -= new CustomEventHandler(OnWaitingForPlayers);
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		if (((CoroutineHandle)(ref _autosaveCoroutine)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _autosaveCoroutine });
		}
		Service?.CommitAndSave();
		Service = null;
		Instance = null;
		Log.Info("[AdminWeeklyActivity] Модуль учёта недельной активности администраторов выключен.");
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

	private void StartAutosave()
	{
		if (_config.IsEnabled)
		{
			if (((CoroutineHandle)(ref _autosaveCoroutine)).IsRunning)
			{
				Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _autosaveCoroutine });
			}
			_autosaveCoroutine = Timing.RunCoroutine(AutosaveLoop());
		}
	}

	[IteratorStateMachine(typeof(AutosaveLoop_d_29))]
	private IEnumerator<float> AutosaveLoop()
	{
		return new AutosaveLoop_d_29(0)
		{
			__4__this = this
		};
	}

	private void OnVerified(VerifiedEventArgs ev)
	{
		if (_config.IsEnabled && ev != null && !(ev.Player == (Player)null))
		{
			Service.StartSession(ev.Player);
		}
	}

	private void OnLeft(LeftEventArgs ev)
	{
		if (_config.IsEnabled && ev != null && !(((JoinedEventArgs)ev).Player == (Player)null))
		{
			Service.StopSession(((JoinedEventArgs)ev).Player);
			Service.Save();
		}
	}

	private void OnWaitingForPlayers()
	{
		if (_config.IsEnabled)
		{
			Service.CommitAndSave();
		}
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		if (_config.IsEnabled)
		{
			Service.CommitAndSave();
		}
	}

	private void OnRestartingRound()
	{
		if (_config.IsEnabled)
		{
			Service.CommitAndSave();
		}
	}
}
