using System;
using System.Runtime.CompilerServices;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;

namespace FultEngine.Module.DestroyCamer;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private static class __O
	{
		public static CustomEventHandler<ShootingEventArgs> _0__OnShooting;

		public static CustomEventHandler<ChangingCameraEventArgs> _1__OnScp079ChangingCamera;

		public static CustomEventHandler _2__OnWaitingForPlayers;
	}

	public string Name { get; } = "DestroyCamer";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 4, 1);


	private Config Config { get; set; }

	public void OnEnabled()
	{
		Player.Shooting += (CustomEventHandler<ShootingEventArgs>)EventHandlers.OnShooting;
		Scp079.ChangingCamera += (CustomEventHandler<ChangingCameraEventArgs>)EventHandlers.OnScp079ChangingCamera;
		Event waitingForPlayers = Server.WaitingForPlayers;
		object obj = __O._2__OnWaitingForPlayers;
		if (obj == null)
		{
			CustomEventHandler val = EventHandlers.OnWaitingForPlayers;
			__O._2__OnWaitingForPlayers = val;
			obj = (object)val;
		}
		Server.WaitingForPlayers = waitingForPlayers + (CustomEventHandler)obj;
		DestroyCamerAPI.Initialize(this);
	}

	public void OnDisabled()
	{
		Player.Shooting -= (CustomEventHandler<ShootingEventArgs>)EventHandlers.OnShooting;
		Scp079.ChangingCamera -= (CustomEventHandler<ChangingCameraEventArgs>)EventHandlers.OnScp079ChangingCamera;
		Event waitingForPlayers = Server.WaitingForPlayers;
		object obj = __O._2__OnWaitingForPlayers;
		if (obj == null)
		{
			CustomEventHandler val = EventHandlers.OnWaitingForPlayers;
			__O._2__OnWaitingForPlayers = val;
			obj = (object)val;
		}
		Server.WaitingForPlayers = waitingForPlayers - (CustomEventHandler)obj;
		CameraManager.Clear();
		DestroyCamerAPI.Deinitialize();
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config
		{
			BreakChance = 0.3f,
			BreakRadius = 0.7f,
			ParticleDuration = 1f
		};
	}

	public void SetConfig(object config)
	{
		if (config is Config config2)
		{
			Config = config2;
		}
	}

	internal Config GetConfig()
	{
		return Config;
	}
}
