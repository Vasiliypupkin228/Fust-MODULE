using System;
using System.Reflection;
using Exiled.API.Features;
using FultEngine.LoaderModule;
using HarmonyLib;

namespace FultEngine.Patch;

public class Plugin : IFultEngineModule
{
	private Harmony _harmony;

	public string Name { get; } = "RegisterPatch";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 2);


	public void OnEnabled()
	{
		try
		{
			ApplyPatch();
		}
		catch (Exception arg)
		{
			Log.Error($"Failed to enable RegisterPatch: {arg}");
		}
	}

	public void OnDisabled()
	{
		try
		{
			RemovePatch();
		}
		catch (Exception arg)
		{
			Log.Error($"Failed to disable RegisterPatch: {arg}");
		}
	}

	public void ApplyPatch()
	{
		try
		{
			if (_harmony == null)
			{
				_harmony = new Harmony("com.fultengine.patch.protected");
			}
			_harmony.PatchAll(Assembly.GetExecutingAssembly());
			Log.Warn("Патчи успешно загружены.");
		}
		catch (Exception arg)
		{
			Log.Error($"Failed to apply protected patches: {arg}");
		}
	}

	public void RemovePatch()
	{
		try
		{
			if (_harmony == null)
			{
				Log.Warn("No Harmony instance to remove.");
				return;
			}
			_harmony.UnpatchAll("com.fultengine.patch.protected");
			_harmony = null;
			Log.Warn("Патчи успешно выгружены!");
		}
		catch (Exception arg)
		{
			Log.Error($"Failed to remove patches: {arg}");
		}
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
