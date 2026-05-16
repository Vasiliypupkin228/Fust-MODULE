using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using MEC;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace FultEngine.API.Libraries.SSBinds;

public static class KeybindManager
{
	[CompilerGenerated]
	private sealed class HintCoroutine_d_18 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		private IEnumerator<Player> __s__1;

		private Player player;

		private bool needsHints;

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
		public HintCoroutine_d_18(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__s__1 = null;
			player = null;
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
					if (!(player == (Player)null) && player.IsConnected && (!GetPlayerHintSetting(player, out needsHints) || needsHints))
					{
						player.ShowHint("\n\n\n\n\n\n\n\n<size=33><b><color=#a390905d>⌜</color><color=#820909a9>⚠</color> Просим вас обратить внимание! <color=#820909a9>⚠</color><color=#a390905d>⌝</color></b>\n<size=33><b><color=#a390905d>▏</color> Установите бинды для использования реанимации и т. д. <color=#a390905d>▕</color></b></size>\n<size=29><b><color=#a390905d>▏</color> Зайдите в Настройки → Server Specific <color=#a390905d>▕</color></b></size>\n<size=29><b><color=#a390905d>▏</color> Начните настраивать кнопки <color=#a390905d>▕</color></b></size>\n<b><size=33><color=#a390905d>▏</color><color=#820909a9>⚠ </color>Нажмите \"Да\" в бинде \"Убрать подсказку?\" <color=#820909a9>⚠</color><color=#a390905d>▕</color></b>", 1f);
						player = null;
					}
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
			__2__current = Timing.WaitForSeconds(1f);
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

	private static bool _isInitialized;

	private static HeaderSetting _header;

	private static KeybindSetting _keybind;

	private static TwoButtonsSetting _bindConfirmation;

	private static CoroutineHandle _hintCoroutine;

	private static readonly string ConfigPath = Path.Combine(Paths.Plugins, "FULT-ENGINE", "SSBinds", "keybind_settings.txt");

	private const string HintMessage = "\n\n\n\n\n\n\n\n<size=33><b><color=#a390905d>⌜</color><color=#820909a9>⚠</color> Просим вас обратить внимание! <color=#820909a9>⚠</color><color=#a390905d>⌝</color></b>\n<size=33><b><color=#a390905d>▏</color> Установите бинды для использования реанимации и т. д. <color=#a390905d>▕</color></b></size>\n<size=29><b><color=#a390905d>▏</color> Зайдите в Настройки → Server Specific <color=#a390905d>▕</color></b></size>\n<size=29><b><color=#a390905d>▏</color> Начните настраивать кнопки <color=#a390905d>▕</color></b></size>\n<b><size=33><color=#a390905d>▏</color><color=#820909a9>⚠ </color>Нажмите \"Да\" в бинде \"Убрать подсказку?\" <color=#820909a9>⚠</color><color=#a390905d>▕</color></b>";

	public static event Action<ReferenceHub, ServerSpecificSettingBase> OnObjectInteraction;

	public static void Initialize()
	{
		if (!_isInitialized)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
			RegisterKeybind();
			Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnVerified;
			StartHintCoroutine();
			_isInitialized = true;
		}
	}

	public static void Uninitialize()
	{
		if (_isInitialized)
		{
			Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnVerified;
			StopHintCoroutine();
			_isInitialized = false;
		}
	}

	public static void AddCustomKeybind(int id, string label, KeyCode defaultKey, bool preventInteractionOnGUI, string hintDescription)
	{
		try
		{
			KeybindSetting val = new KeybindSetting(id, label, defaultKey, preventInteractionOnGUI, hintDescription, _header, (Action<Player, SettingBase>)OnKeybindActivated);
			SettingBase.Register((IEnumerable<SettingBase>)(object)new SettingBase[1] { (SettingBase)val }, (Func<Player, bool>)null);
		}
		catch (Exception)
		{
		}
	}

	private static void RegisterKeybind()
	{
		try
		{
			_bindConfirmation = new TwoButtonsSetting(2, "╔ <color=#2E8B57>\ud83c\udf40</color> Убрать подсказку", "Нi", "Так", false, "", (HeaderSetting)null, (Action<Player, SettingBase>)delegate(Player player, SettingBase setting)
			{
				try
				{
					TwoButtonsSetting val = (TwoButtonsSetting)(object)((setting is TwoButtonsSetting) ? setting : null);
					if (val != null)
					{
						UpdatePlayerHintSetting(player, val.IsSecond);
					}
				}
				catch (Exception)
				{
				}
			});
			SettingBase[] array = (SettingBase[])(object)new SettingBase[3]
			{
				(SettingBase)_bindConfirmation,
				(SettingBase)_header,
				(SettingBase)_keybind
			};
			SettingBase.Register((IEnumerable<SettingBase>)array, (Func<Player, bool>)null);
		}
		catch (Exception)
		{
		}
	}

	private static void OnVerified(VerifiedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Player.IsAlive && !GetPlayerHintSetting(ev.Player, out var _))
		{
			UpdatePlayerHintSetting(ev.Player, isSecond: false);
		}
	}

	private static void OnKeybindActivated(Player player, SettingBase setting)
	{
		KeybindSetting val = (KeybindSetting)(object)((setting is KeybindSetting) ? setting : null);
		if (val == null)
		{
			return;
		}
		SSKeybindSetting @base = val.Base;
		if (@base != null)
		{
			try
			{
				KeybindManager.OnObjectInteraction?.Invoke(player.ReferenceHub, (ServerSpecificSettingBase)(object)@base);
			}
			catch (Exception)
			{
			}
		}
	}

	private static void StartHintCoroutine()
	{
		StopHintCoroutine();
		_hintCoroutine = Timing.RunCoroutine(HintCoroutine());
	}

	private static void StopHintCoroutine()
	{
		if (((CoroutineHandle)(ref _hintCoroutine)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _hintCoroutine });
		}
	}

	[IteratorStateMachine(typeof(HintCoroutine_d_18))]
	private static IEnumerator<float> HintCoroutine()
	{
		return new HintCoroutine_d_18(0);
	}

	private static bool GetPlayerHintSetting(Player player, out bool needsHints)
	{
		needsHints = false;
		try
		{
			if (!File.Exists(ConfigPath))
			{
				return false;
			}
			string[] array = File.ReadAllLines(ConfigPath);
			string text = player.UserId.Split(new char[1] { '@' })[0];
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string[] array3 = text2.Split(new char[1] { ':' });
				if (array3.Length == 2 && array3[0] == text && bool.TryParse(array3[1], out var result))
				{
					needsHints = !result;
					return true;
				}
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private static void UpdatePlayerHintSetting(Player player, bool isSecond)
	{
		try
		{
			string key = player.UserId.Split(new char[1] { '@' })[0];
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			if (File.Exists(ConfigPath))
			{
				string[] array = File.ReadAllLines(ConfigPath);
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split(new char[1] { ':' });
					if (array3.Length == 2 && bool.TryParse(array3[1], out var result))
					{
						dictionary[array3[0]] = result;
					}
				}
			}
			dictionary[key] = isSecond;
			IEnumerable<string> contents = dictionary.Select((KeyValuePair<string, bool> kvp) => $"{kvp.Key}:{kvp.Value}");
			File.WriteAllLines(ConfigPath, contents);
		}
		catch (Exception)
		{
		}
	}
}
