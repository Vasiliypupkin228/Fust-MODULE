using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using FultEngine.LoaderModule;
using HintServiceMeow.Core.Enum;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.Module.Hud;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class UpdateHud_d_23 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

		private IEnumerator<Player> __s__1;

		private Player player;

		private string displayName;

		private Color roleColor;

		private string subclassName;

		private SubClassData subclass;

		private Player spectatedPlayer;

		private string colorHex;

		private string nameType;

		private string hudText;

		private int adminCount;

		private string elapsed;

		private SubClassData spectatedSubclass;

		private Color solidColor;

		private Team __s__15;

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
		public UpdateHud_d_23(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__1 = null;
			player = null;
			displayName = null;
			subclassName = null;
			subclass = null;
			spectatedPlayer = null;
			colorHex = null;
			nameType = null;
			hudText = null;
			elapsed = null;
			spectatedSubclass = null;
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
				break;
			}
			__s__1 = Player.List.GetEnumerator();
			try
			{
				while (__s__1.MoveNext())
				{
					player = __s__1.Current;
					displayName = player.DisplayNickname ?? player.Nickname;
					if (displayName.Contains("|"))
					{
						displayName = displayName.Split(new char[1] { '|' })[1].Trim();
					}
					roleColor = player.Role.Color;
					subclassName = (FultEngine.Module.Plugin.PlayerSubclasses.TryGetValue(player, out subclass) ? subclass.Id : string.Empty);
					if ((int)player.Role.Team == 5 && __4__this._spectatedPlayers.TryGetValue(player, out spectatedPlayer))
					{
						Player obj = spectatedPlayer;
						object obj2 = ((obj != null) ? obj.DisplayNickname : null);
						if (obj2 == null)
						{
							Player obj3 = spectatedPlayer;
							obj2 = ((obj3 != null) ? obj3.Nickname : null) ?? displayName;
						}
						displayName = (string)obj2;
						if (displayName.Contains("|"))
						{
							displayName = displayName.Split(new char[1] { '|' })[1].Trim();
						}
						roleColor = ((spectatedPlayer != (Player)null) ? spectatedPlayer.Role.Color : roleColor);
						subclassName = ((spectatedPlayer != (Player)null && FultEngine.Module.Plugin.PlayerSubclasses.TryGetValue(spectatedPlayer, out spectatedSubclass)) ? spectatedSubclass.Id : string.Empty);
						spectatedSubclass = null;
					}
					try
					{
						solidColor = new Color(roleColor.r, roleColor.g, roleColor.b);
						colorHex = "#" + ColorUtility.ToHtmlStringRGB(solidColor);
					}
					catch
					{
						colorHex = "#FFFFFF";
					}
					Team team = player.Role.Team;
					__s__15 = team;
					Team val = __s__15;
					switch ((int)val)
					{
					case 1:
					case 2:
						nameType = "Позывной";
						break;
					case 3:
						nameType = "Имя";
						break;
					case 0:
						nameType = "Объект";
						break;
					case 4:
						nameType = "Номер";
						break;
					default:
						nameType = "Имя";
						break;
					}
					hudText = "<size=17><b><color=white>" + nameType + ":</color></b></size>\n<size=17><b><color=" + colorHex + "><size=23>" + displayName + "</color></b></size>\n<size=17><b><color=" + colorHex + ">→ " + subclassName + "</color></b></size>\n" + $"<size=17><b><color=white><size=11>[ Ваш псевдоним: `{player.Nickname}` | Ваш ID: `{player.Id}` ]</color></b></size>";
					adminCount = Player.List.Count((Player p) => p.RemoteAdminAccess);
					elapsed = Round.ElapsedTime.ToString("hh\\:mm\\:ss");
					Server.PlayerListName = "<size=29><b>NezerHill  | <color=#FF7F50>Medium RP</color>\n" + $"<size=25>Раунд идёт: {elapsed} | Админов: {adminCount}</size>\n\n" + "<size=19><color=#ffffff6f>Есть ошибки в плагине? Напишите мне в DISCORD: `Fulty`</color></size></b>";
					player.ShowMeowHint(__4__this._config.UpdateInterval, "<size=83><color=#61616193><b>|</b></color></size>", (HintVerticalAlign)0, 127, -319, (HintAlignment)0);
					player.ShowMeowHint(__4__this._config.UpdateInterval, "<size=28><color=#ffffff8f><b>" + GetHudIcon(player) + "</b></color></size>", (HintVerticalAlign)0, 151, -302, (HintAlignment)0);
					player.ShowMeowHint(__4__this._config.UpdateInterval, hudText, (HintVerticalAlign)0, 166, -300, (HintAlignment)0);
					displayName = null;
					subclassName = null;
					subclass = null;
					spectatedPlayer = null;
					colorHex = null;
					nameType = null;
					hudText = null;
					elapsed = null;
					player = null;
				}
			}
			finally
			{
				if (__s__1 != null)
				{
					__s__1.Dispose();
				}
			}
			__s__1 = null;
			__2__current = Timing.WaitForSeconds(__4__this._config.UpdateInterval);
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

	private readonly Dictionary<Player, Player> _spectatedPlayers = new Dictionary<Player, Player>();

	private HudConfig _config;

	private HudTextConfig _textConfig;

	private CoroutineHandle _hudCoroutine;

	public string Name { get; } = "Hud";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 1);


	public void OnEnabled()
	{
		_config = (HudConfig)GetDefaultConfig();
		Player.ChangingSpectatedPlayer += (CustomEventHandler<ChangingSpectatedPlayerEventArgs>)OnChangingSpectatedPlayer;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
		Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
	}

	public void OnDisabled()
	{
		Player.ChangingSpectatedPlayer -= (CustomEventHandler<ChangingSpectatedPlayerEventArgs>)OnChangingSpectatedPlayer;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnPlayerSpawned;
		Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
		if (((CoroutineHandle)(ref _hudCoroutine)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _hudCoroutine });
		}
		_spectatedPlayers.Clear();
	}

	public Type GetConfigType()
	{
		return typeof(HudConfig);
	}

	public Type GetTextConfigType()
	{
		return typeof(HudTextConfig);
	}

	public object GetDefaultConfig()
	{
		return new HudConfig
		{
			UpdateInterval = 1f,
			HintVerticalAlign = (HintVerticalAlign)0,
			HintAlignment = (HintAlignment)0
		};
	}

	public void SetConfig(object config)
	{
		_config = (HudConfig)config;
	}

	public void SetTextConfig(object textConfig)
	{
		_textConfig = (HudTextConfig)textConfig;
	}

	public void OnRoundStarted()
	{
		if (((CoroutineHandle)(ref _hudCoroutine)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _hudCoroutine });
		}
		_hudCoroutine = Timing.RunCoroutine(UpdateHud());
	}

	private void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
	{
		Player player = ev.Player;
		Player newTarget = ev.NewTarget;
		if (_spectatedPlayers.ContainsKey(player))
		{
			_spectatedPlayers[player] = newTarget;
		}
		else
		{
			_spectatedPlayers.Add(player, newTarget);
		}
	}

	private void OnPlayerSpawned(SpawnedEventArgs ev)
	{
		if (_spectatedPlayers.ContainsKey(ev.Player))
		{
			_spectatedPlayers.Remove(ev.Player);
		}
	}

	[IteratorStateMachine(typeof(UpdateHud_d_23))]
	private IEnumerator<float> UpdateHud()
	{
		return new UpdateHud_d_23(0)
		{
			__4__this = this
		};
	}

	private static string GetHudIcon(Player player)
	{
		if (player == (Player)null)
		{
			return "▤";
		}
		Room currentRoom = player.CurrentRoom;
		string text = ((currentRoom != null) ? currentRoom.Name : null) ?? string.Empty;
		if ((int)player.Role.Type == 1)
		{
			if (text.AsSpan().Contains("ClassD".AsSpan(), StringComparison.OrdinalIgnoreCase) || text.AsSpan().Contains("Cells".AsSpan(), StringComparison.OrdinalIgnoreCase) || text.AsSpan().Contains("Cell".AsSpan(), StringComparison.OrdinalIgnoreCase))
			{
				return "⌗";
			}
			return "⛓";
		}
		if ((int)player.Role.Type == 6)
		{
			return "⚗";
		}
		if (player.IsScp)
		{
			return "☣";
		}
		if ((int)player.Role.Team == 1)
		{
			return "✦";
		}
		if ((int)player.Role.Team == 2)
		{
			return "◆";
		}
		ZoneType zone = player.Zone;
		ZoneType val = zone;
		switch (val - 1)
		{
		default:
			if ((int)val != 8)
			{
				break;
			}
			return "◎";
		case 3:
			return "◈";
		case 1:
			return "▣";
		case 0:
			return "◉";
		case 2:
			break;
		}
		return "▤";
	}
}
