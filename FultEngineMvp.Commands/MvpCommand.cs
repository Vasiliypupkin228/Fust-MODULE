using System;
using CommandSystem;
using Exiled.API.Features;

namespace FultEngineMvp.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
public sealed class MvpCommand : ICommand
{
	public string Command => "mvp";

	public string[] Aliases => new string[1] { "mvpmusic" };

	public string Description => "Выбор и запуск MVP музыки после окончания раунда.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player val = Player.Get(sender);
		if (val == (Player)null)
		{
			response = "Эту команду может использовать только игрок.";
			return false;
		}
		MvpMusicService mvpMusicService = FultEngineMvpPlugin.Instance?.MusicService;
		if (mvpMusicService == null)
		{
			response = "MVP Music Service не запущен.";
			return false;
		}
		if (arguments.Count == 0)
		{
			return mvpMusicService.TryPlaySelected(val, out response);
		}
		string arg = GetArg(arguments, 0);
		if (arg.Equals("list", StringComparison.OrdinalIgnoreCase) || arg.Equals("список", StringComparison.OrdinalIgnoreCase))
		{
			int page = 1;
			if (arguments.Count >= 2 && int.TryParse(GetArg(arguments, 1), out var result))
			{
				page = result;
			}
			response = mvpMusicService.BuildTrackListPage(page);
			return true;
		}
		if (int.TryParse(arg, out var result2))
		{
			return mvpMusicService.TryPlayByNumber(val, result2, out response);
		}
		string name = string.Join(" ", GetArgs(arguments));
		return mvpMusicService.TryPlayByName(val, name, out response);
	}

	private static string GetArg(ArraySegment<string> args, int index)
	{
		return args.Array[args.Offset + index];
	}

	private static string[] GetArgs(ArraySegment<string> args)
	{
		string[] array = new string[args.Count];
		for (int i = 0; i < args.Count; i++)
		{
			array[i] = GetArg(args, i);
		}
		return array;
	}
}
