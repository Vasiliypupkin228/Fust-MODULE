using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;

namespace FultEngine.Module.DonorRank;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class CheckAndAssignGroup_d_14 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public string steamId;

		public Plugin __4__this;

		private PlayerInfo info;

		private string tierGroup;

		private string url;

		private HttpRequestMessage request;

		private HttpResponseMessage response;

		private string body;

		private Exception ex;

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
		public CheckAndAssignGroup_d_14(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			info = null;
			tierGroup = null;
			url = null;
			request = null;
			response = null;
			body = null;
			ex = null;
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
				__2__current = Timing.WaitForSeconds(1.5f);
				__1__state = 1;
				return true;
			case 1:
				__1__state = -1;
				if (player == (Player)null || !player.IsConnected)
				{
					return false;
				}
				info = null;
				try
				{
					url = __4__this._config.ApiUrl.TrimEnd(new char[1] { '/' }) + "/api/player/" + steamId;
					request = new HttpRequestMessage(HttpMethod.Get, url);
					try
					{
						((HttpHeaders)request.Headers).Add("X-Plugin-Key", __4__this._config.ApiKey);
						response = Http.SendAsync(request).GetAwaiter().GetResult();
						if (!response.IsSuccessStatusCode)
						{
							Log.Warn($"[DonorRank] API вернул {(int)response.StatusCode} для {steamId}");
							return false;
						}
						body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
						info = ParseJson(body);
						response = null;
						body = null;
					}
					finally
					{
						if (request != null)
						{
							((IDisposable)request).Dispose();
						}
					}
					request = null;
					url = null;
				}
				catch (Exception ex)
				{
					ex = ex;
					Log.Error("[DonorRank] Ошибка запроса к API для " + steamId + ": " + ex.Message);
					return false;
				}
				if (info == null || !info.Found)
				{
					return false;
				}
				if (info.DonorTier != null && __4__this._config.Groups.TryGetValue(info.DonorTier, out tierGroup))
				{
					__4__this.AssignGroup(player, tierGroup, steamId, info.DonorTier);
				}
				return false;
			}
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

	private Config _config;

	private static readonly HttpClient Http = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(5.0)
	};

	public string Name { get; } = "DonorRank";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 0, 0);


	public void OnEnabled()
	{
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Log.Info("[DonorRank] Загружен. API: " + (_config?.ApiUrl ?? "не задан"));
	}

	public void OnDisabled()
	{
		Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		Log.Info("[DonorRank] Выгружен.");
	}

	private void OnVerified(VerifiedEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			string input = ev.Player.RawUserId ?? string.Empty;
			Match match = Regex.Match(input, "(\\d{17})@steam");
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				Player player = ev.Player;
				Timing.RunCoroutine(CheckAndAssignGroup(player, value));
			}
		}
	}

	[IteratorStateMachine(typeof(CheckAndAssignGroup_d_14))]
	private IEnumerator<float> CheckAndAssignGroup(Player player, string steamId)
	{
		return new CheckAndAssignGroup_d_14(0)
		{
			__4__this = this,
			player = player,
			steamId = steamId
		};
	}

	private void AssignGroup(Player player, string groupName, string steamId, string tierName)
	{
		if (string.IsNullOrWhiteSpace(groupName))
		{
			return;
		}
		UserGroup group = ServerStatic.PermissionsHandler.GetGroup(groupName);
		if (group == null)
		{
			Log.Warn("[DonorRank] Группа '" + groupName + "' не найдена в config_remoteadmin.txt! Steam: " + steamId);
			return;
		}
		player.Group = group;
		Log.Info("[DonorRank] Выдана группа '" + groupName + "' (" + tierName + ") игроку " + player.Nickname + " [" + steamId + "]");
		if (_config.SendWelcomeHint)
		{
			string text = _config.WelcomeMessage.Replace("{TIER}", tierName).Replace("{GROUP}", groupName).Replace("{NAME}", player.Nickname);
			player.ShowHint(text, 6f);
		}
	}

	private static PlayerInfo ParseJson(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return null;
		}
		try
		{
			return new PlayerInfo
			{
				Found = GetBool(json, "found"),
				DonorTier = GetString(json, "donorTier"),
				Balance = GetInt(json, "balance"),
				Role = GetString(json, "role"),
				IsVyshka = GetBool(json, "isVyshka")
			};
		}
		catch
		{
			return null;
		}
	}

	private static string GetString(string json, string key)
	{
		Match match = Regex.Match(json, "\"" + key + "\"\\s*:\\s*\"([^\"]*)\"");
		return match.Success ? match.Groups[1].Value : null;
	}

	private static bool GetBool(string json, string key)
	{
		Match match = Regex.Match(json, "\"" + key + "\"\\s*:\\s*(true|false)");
		return match.Success && match.Groups[1].Value == "true";
	}

	private static int GetInt(string json, string key)
	{
		Match match = Regex.Match(json, "\"" + key + "\"\\s*:\\s*(\\d+)");
		return match.Success ? int.Parse(match.Groups[1].Value) : 0;
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config
		{
			ApiUrl = "http://localhost:8090",
			ApiKey = "nezerhill-plugin-secret-2026",
			Groups = new Dictionary<string, string>
			{
				{ "O5_COUNCIL", "o5council" },
				{ "RESEARCHER", "researcher" },
				{ "D_CLASS", "dclass" }
			},
			SendWelcomeHint = true,
			WelcomeMessage = "<color=#e62020><b>NEZERHILL</b></color>\nДобро пожаловать, <b>{NAME}</b>!\nВаш статус: <color=#ffd700>{TIER}</color>"
		};
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}
}
