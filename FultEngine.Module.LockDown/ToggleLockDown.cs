using System;
using CommandSystem;

namespace FultEngine.Module.LockDown;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class ToggleLockDown : ICommand
{
	public string Command => "containment";

	public string[] Aliases => new string[2] { "ct", "lockdown" };

	public string Description => "Вкл/выкл локдаун SCP-камер.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (Plugin.Instance == null)
		{
			response = "LockDown plugin не найден.";
			return false;
		}
		if (Plugin.Instance.IsLocked)
		{
			Plugin.Instance.ForceUnlock();
			response = "Локдаун отключён.";
		}
		else
		{
			Plugin.Instance.StartLockdown();
			response = "Локдаун включён.";
		}
		return true;
	}
}
