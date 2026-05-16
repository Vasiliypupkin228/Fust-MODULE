using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;

namespace FultEngineMvp;

public sealed class FultEngineMvpPlugin : IFultEngineModule
{
	private Config _config;

	public string Name => "MVP";

	public string Author => "FUST";

	public Version Version => new Version(1, 4, 0);

	public static FultEngineMvpPlugin Instance { get; private set; }

	public Config Config => _config ?? (_config = new Config());

	public MvpMusicService MusicService { get; private set; }

	public MvpStatsService StatsService { get; private set; }

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

	public void OnEnabled()
	{
		if (!Config.IsEnabled)
		{
			Log.Info("[FULT-ENGINE.MVP] Модуль отключён в конфиге.");
			return;
		}
		Instance = this;
		StatsService = new MvpStatsService(this);
		MusicService = new MvpMusicService(this);
		StatsService.RegisterEvents();
		StatsService.ResetRound();
		MusicService.LoadTracks();
		MusicService.RegisterServerSpecific();
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
		Log.Info("[FULT-ENGINE.MVP] Модуль MVP Music включён.");
	}

	public void OnDisabled()
	{
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		StatsService?.UnregisterEvents();
		MusicService?.UnregisterServerSpecific();
		MusicService?.StopCurrentTrack();
		MusicService = null;
		StatsService = null;
		Instance = null;
		Log.Info("[FULT-ENGINE.MVP] Модуль MVP Music выключен.");
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		StatsService?.HandleRoundEnded();
		MusicService?.HandleRoundEnded();
	}

	private void OnRestartingRound()
	{
		StatsService?.ResetRound();
		MusicService?.HandleRestartingRound();
	}
}
