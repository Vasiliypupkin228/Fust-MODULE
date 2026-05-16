using System;
using FultEngine.LoaderModule;

namespace FultEngine.Module.ScpVoiceChat;

public class Plugin : IFultEngineModule
{
	public static Plugin? Singleton;

	public ScpVoiceSystem? VoiceSystem;

	public string Name => "ScpVoiceChat";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 0);

	public void OnEnabled()
	{
		VoiceSystem = new ScpVoiceSystem();
		VoiceSystem.SubscribeEvents();
	}

	public void OnDisabled()
	{
		VoiceSystem?.Dispose();
		VoiceSystem = null;
	}

	public Type GetConfigType()
	{
		return null;
	}

	public object GetDefaultConfig()
	{
		return null;
	}

	public void SetConfig(object config)
	{
	}
}
