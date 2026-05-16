using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.SSBinds;
using MEC;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FultEngine.LoaderModule;

public class FultEngine : Plugin<Config>
{
	private class PluginInfo
	{
		public string Name { get; set; }

		public string Version { get; set; }

		public string Status { get; set; }
	}

	private static readonly ISerializer Serializer = new SerializerBuilder()
		.WithNamingConvention(CamelCaseNamingConvention.Instance)
		.WithIndentedSequences()
		.ConfigureDefaultValuesHandling(DefaultValuesHandling.Include)
		.Build();

	private static readonly IDeserializer Deserializer = new DeserializerBuilder()
		.WithNamingConvention(CamelCaseNamingConvention.Instance)
		.IgnoreUnmatchedProperties()
		.Build();

	private readonly List<IFultEngineModule> _loadedPlugins = new List<IFultEngineModule>();

	private int _errorCount;

	private readonly List<PluginInfo> _activePlugins = new List<PluginInfo>();

	public static FultEngine Instance { get; private set; }

	public override string Name => "FULT-ENGINE";

	public override string Prefix => "FULT-ENGINE";

	public override string Author => "FUST";

	public override Version Version => new Version(1, 1, 3);

	public override Version RequiredExiledVersion => new Version(9, 5, 1);

	private void EnsureConfigDirectoryExists()
	{
		string path = Path.Combine(Paths.Plugins, "FULT-ENGINE", "ModuleConfig");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			Log.Info("Создана директория: " + path);
		}
	}

	private void LoadPluginConfig(IFultEngineModule plugin)
	{
		Type configType = plugin.GetConfigType();
		if (configType == null)
		{
			return;
		}
		string pluginConfigPath = GetPluginConfigPath(plugin);
		try
		{
			if (!File.Exists(pluginConfigPath))
			{
				object defaultConfig = plugin.GetDefaultConfig();
				string contents = Serializer.Serialize(defaultConfig);
				File.WriteAllText(pluginConfigPath, contents);
				plugin.SetConfig(defaultConfig);
			}
			else
			{
				string yaml = File.ReadAllText(pluginConfigPath);
				object config = Deserializer.Deserialize(yaml, configType);
				plugin.SetConfig(config);
			}
		}
		catch (Exception ex)
		{
			_errorCount++;
			Log.Error($"Ошибка при загрузке конфигурации для {plugin.Name}: {ex}");
		}
	}

	private static string GetPluginConfigPath(IFultEngineModule plugin)
	{
		return Path.Combine(Paths.Plugins, "FULT-ENGINE", "ModuleConfig", plugin.Name + ".yml");
	}

	private void RegisterAllEvents()
	{
		foreach (IFultEngineModule loadedPlugin in _loadedPlugins)
		{
			try
			{
				loadedPlugin.OnEnabled();
			}
			catch (Exception ex)
			{
				_errorCount++;
				Log.Error($"Ошибка при регистрации событий для {loadedPlugin.Name}: {ex}");
			}
		}
	}

	private void UnregisterAllEvents()
	{
		foreach (IFultEngineModule loadedPlugin in _loadedPlugins)
		{
			try
			{
				loadedPlugin.OnDisabled();
			}
			catch (Exception ex)
			{
				_errorCount++;
				Log.Error($"Ошибка при отмене регистрации событий для {loadedPlugin.Name}: {ex}");
			}
		}
	}

	public override void OnEnabled()
	{
		try
		{
			Instance = this;
			CustomRole.RegisterRoles(false, null);
			Log.Warn(AsciiArt.FultEngineBanner);
			EnsureConfigDirectoryExists();
			InitializePlugins();
		}
		catch (Exception ex)
		{
			Log.Error("Ошибка инициализации: " + ex.Message + "\n" + ex.StackTrace);
			OnDisabled();
		}
	}

	public override void OnDisabled()
	{
		CustomItem.UnregisterItems();
		UnregisterAllEvents();
		Player.Verified -= OnVerified;
		Server.WaitingForPlayers -= OnWaitingForPlayers;
		_loadedPlugins.Clear();
		_activePlugins.Clear();
		Instance = null;
		Log.Warn($"{Name} v{Version} успешно выгружен!");
	}

	private static bool IsValidPluginType(Type type)
	{
		return type != null
			&& type.IsClass
			&& !type.IsAbstract
			&& typeof(IFultEngineModule).IsAssignableFrom(type)
			&& type.GetConstructor(Type.EmptyTypes) != null;
	}

	private void InitializePlugins()
	{
		CustomItem.RegisterItems(false, null);
		KeybindManager.Initialize();
		LoadAllPlugins();
		RegisterAllEvents();
		Server.WaitingForPlayers += OnWaitingForPlayers;
		Player.Verified += OnVerified;
	}

	private void OnVerified(VerifiedEventArgs ev)
	{
		string info = $"|---| Модулей: {_loadedPlugins.Count} |---|";
		string agreement = "|---| Пользовательское соглашение FULT-ENGINE: https://docs.google.com/document/d/1ThkopSAjMjHlXhWd2qH-aA5b_tGo_3h3TOBDIoN1DmM/edit?usp=sharing |---|\n";
		string text = AsciiArt.VerifiedBanner
			+ "\n|---| Создатель модуля: " + Author + " |---|\n"
			+ info + "\n" + agreement;
		ev.Player.SendConsoleMessage(text, "yellow");
	}

	private void OnWaitingForPlayers()
	{
		string text = AsciiArt.WaitingForPlayersBanner
			+ $"\n|------| Загружено плагинов: {(byte)_loadedPlugins.Count} | Ошибок: {_errorCount} | Создатель модуля: {Author} |------|";
		Log.Info(text);
		Log.Warn("[FULT-ENGINE] Пользовательское соглашение FULT-ENGINE: https://docs.google.com/document/d/1ThkopSAjMjHlXhWd2qH-aA5b_tGo_3h3TOBDIoN1DmM/edit?usp=sharing");
		try
		{
			AudioManager.RegisterAudio();
			Log.Info($"|------| Все звуки прогружены. Клипов в storage: {AudioClipStorage.AudioClips.Count} |------|");
		}
		catch (Exception ex)
		{
			Log.Error($"[FULT-ENGINE.Audio] Ошибка при RegisterAudio: {ex}");
		}
	}

	private void LoadAllPlugins()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		foreach (Type type in executingAssembly.GetTypes().Where(IsValidPluginType))
		{
			try
			{
				if (Activator.CreateInstance(type) is IFultEngineModule module)
				{
					LoadPluginConfig(module);
					_loadedPlugins.Add(module);
					Log.Warn($"Загружен плагин: {module.Name} v{module.Version}");
				}
			}
			catch (Exception ex)
			{
				_errorCount++;
				Log.Error("Ошибка загрузки плагина " + type.Name + ": " + ex.Message + "\n" + ex.StackTrace);
			}
		}
	}
}
