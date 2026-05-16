using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using Exiled.Permissions.Extensions;
using FultEngine.LoaderModule;

namespace FultEngine.ModuleCommand.Admin.TeslaGate;

public class EventHandler : IFultEngineModule
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class TeslaGateCommand : ICommand
	{
		public string Command => "tesla";

		public string[] Aliases => Array.Empty<string>();

		public string Description => "Включает или выключает тесла-ворота (on/off)";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			Player val = Player.Get(sender);
			if (val == (Player)null || !Permissions.CheckPermission(sender, "RedEngine.tesla"))
			{
				response = "<b><color=#ff3b00ce>❗ У вас нет прав для использования этой команды. <color=#00ff84ae>`RedEngine.tesla`</color> <color=#ff3b00ce>❗</color></b>";
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "Использование: tesla [on/off]";
				return false;
			}
			string text = CollectionExtensions.At<string>(arguments, 0).ToLower();
			if (text == "on")
			{
				IsTeslaEnabled = true;
				response = "<b><color=#00ff2ace>\ud83d\udd18</color> Тесла-ворота включены</b>";
			}
			else
			{
				if (!(text == "off"))
				{
					response = "Неверный аргумент. Используйте on или off.";
					return false;
				}
				IsTeslaEnabled = false;
				response = "<b><color=#3b3b3bd7>\ud83d\udd18</color> Тесла-ворота выключены</b>";
			}
			return true;
		}
	}

	private Config _config;

	public string Name => "TeslaGate";

	public string Author => "FUST";

	public Version Version => new Version(0, 1);

	public static bool IsTeslaEnabled { get; set; } = true;


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
		Player.TriggeringTesla += (CustomEventHandler<TriggeringTeslaEventArgs>)OnTriggeringTesla;
	}

	private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
	{
		if (!IsTeslaEnabled)
		{
			ev.IsAllowed = false;
		}
	}

	public void OnDisabled()
	{
		Player.TriggeringTesla -= (CustomEventHandler<TriggeringTeslaEventArgs>)OnTriggeringTesla;
	}
}
