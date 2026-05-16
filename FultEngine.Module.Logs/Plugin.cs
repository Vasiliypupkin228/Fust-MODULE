using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using FultEngine.Module.Logs.Event;

namespace FultEngine.Module.Logs;

public class Plugin : IFultEngineModule
{
	public enum LogType
	{
		Player,
		Admin,
		Server,
		Ban
	}

	[CompilerGenerated]
	private static class __O
	{
		public static CustomEventHandler _0__OnWaitingForPlayers;
	}

	private static string _playerLogFilePath;

	private static string _adminLogFilePath;

	private static string _serverLogFilePath;

	private static string _banLogFilePath;

	private static bool _isWaitingForPlayers;

	private static readonly HttpClient _httpClient = new HttpClient();

	private static LogsConfig _config = new LogsConfig();

	private static readonly Dictionary<string, string> PlayerEventEmojis = new Dictionary<string, string>
	{
		{ "был верифицирован", "✅" },
		{ "подключается к серверу", "\ud83d\udce5" },
		{ "покинул сервер", "\ud83d\udce4" },
		{ "получил урон", "\ud83e\ude79" },
		{ "умер", "\ud83d\udc80" },
		{ "меняет роль", "\ud83c\udfad" },
		{ "сбежал", "\ud83c\udfc3" },
		{ "надевает наручники", "⛓\ufe0f" },
		{ "снимает наручники", "\ud83d\udd13" },
		{ "подобрал предмет", "\ud83d\udee0\ufe0f" },
		{ "выбрасывает предмет", "\ud83d\uddd1\ufe0f" },
		{ "выбросил предмет", "\ud83d\uddd1\ufe0f" },
		{ "использует предмет", "\ud83e\uddf0" },
		{ "взаимодействует с дверью", "\ud83d\udeaa" },
		{ "взаимодействует с шкафчиком", "\ud83d\uddc4\ufe0f" },
		{ "взаимодействует с лифтом", "\ud83d\uded7" },
		{ "активирует тесла-ворота", "⚡\ufe0f" },
		{ "вошёл в карманное измерение", "\ud83c\udf00" },
		{ "пытается сбежать из карманного измерения", "\ud83c\udfc3\u200d♂\ufe0f" },
		{ "активирует панель ядерной боеголовки", "☢\ufe0f" },
		{ "разблокирует генератор", "\ud83d\udd27" },
		{ "активирует генератор", "⚙\ufe0f" },
		{ "останавливает генератор", "\ud83d\uded1" },
		{ "говорит по интеркому", "\ud83d\udce3" },
		{ "бросил гранату", "\ud83d\udca3" },
		{ "выбрасывает патроны", "\ud83d\udd2b" },
		{ "подбрасывает монету", "\ud83e\ude99" },
		{ "использует энергию MicroHID", "\ud83d\udd0b" },
		{ "активирует рабочую станцию", "\ud83d\udcbb" },
		{ "деактивирует рабочую станцию", "\ud83d\udda5\ufe0f" },
		{ "перезаряжает оружие", "\ud83d\udd04" },
		{ "сделал холостой выстрел", "\ud83d\udd2b" },
		{ "разряжает оружие", "\ud83d\udd27" },
		{ "меняет предмет", "\ud83d\udd04" },
		{ "получает эффект", "✨" },
		{ "переключает режим NoClip", "\ud83d\udc7b" },
		{ "повреждает окно", "\ud83e\ude9f" },
		{ "меняет ник", "\ud83d\udcdd" },
		{ "вылечился", "\ud83e\ude7a" },
		{ "меняет эмоцию", "\ud83d\ude0a" },
		{ "стреляет", "\ud83d\udd2b" },
		{ "выстрелил", "\ud83d\udd2b" }
	};

	private static readonly Dictionary<string, string> AdminEventEmojis = new Dictionary<string, string>
	{
		{ "кикнул", "\ud83d\udeb7" },
		{ "замутил", "\ud83d\udd07" },
		{ "размутил", "\ud83d\udd0a" }
	};

	private static readonly Dictionary<string, string> ServerEventEmojis = new Dictionary<string, string>
	{
		{ "Запустился раунд", "▶\ufe0f" },
		{ "Раунд завершен", "\ud83c\udfc1" },
		{ "Перезапустился раунд", "\ud83d\udd04" },
		{ "Сервер начал ожидание игроков", "⏳" },
		{ "Плагин логов включен", "\ud83d\udcdc" }
	};

	public string Name { get; } = "Logs";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 0, 0);


	private static string PlayerWebhookUrl => _config?.PlayerWebhookUrl ?? string.Empty;

	private static string AdminWebhookUrl => _config?.AdminWebhookUrl ?? string.Empty;

	private static string ServerWebhookUrl => _config?.ServerWebhookUrl ?? string.Empty;

	private static string BanWebhookUrl => _config?.BanWebhookUrl ?? string.Empty;

	public void OnEnabled()
	{
		string path = Path.Combine(Paths.Plugins, "FULT-ENGINE", "Log");
		string text = Path.Combine(path, "Player");
		string text2 = Path.Combine(path, "Admin");
		string text3 = Path.Combine(path, "Server");
		string text4 = Path.Combine(path, "Ban");
		Directory.CreateDirectory(text);
		Directory.CreateDirectory(text2);
		Directory.CreateDirectory(text3);
		Directory.CreateDirectory(text4);
		_isWaitingForPlayers = true;
		string text5 = DateTime.UtcNow.AddHours(3.0).ToString("yyyy-MM-dd_HH-mm-ss");
		_playerLogFilePath = Path.Combine(text, text5 + ".txt");
		_adminLogFilePath = Path.Combine(text2, text5 + ".txt");
		_serverLogFilePath = Path.Combine(text3, text5 + ".txt");
		_banLogFilePath = Path.Combine(text4, text5 + ".txt");
		Player.Enabled();
		Admin.Enabled();
		Server.Enabled();
		Event waitingForPlayers = Server.WaitingForPlayers;
		object obj = __O._0__OnWaitingForPlayers;
		if (obj == null)
		{
			CustomEventHandler val = OnWaitingForPlayers;
			__O._0__OnWaitingForPlayers = val;
			obj = (object)val;
		}
		Server.WaitingForPlayers = waitingForPlayers + (CustomEventHandler)obj;
		Log.Info("Logs plugin enabled and subscribed to WaitingForPlayers event.");
		LogToFile("Плагин логов включен.", LogType.Server);
	}

	public void OnDisabled()
	{
		Event waitingForPlayers = Server.WaitingForPlayers;
		object obj = __O._0__OnWaitingForPlayers;
		if (obj == null)
		{
			CustomEventHandler val = OnWaitingForPlayers;
			__O._0__OnWaitingForPlayers = val;
			obj = (object)val;
		}
		Server.WaitingForPlayers = waitingForPlayers - (CustomEventHandler)obj;
		Log.Info("Logs plugin disabled.");
	}

	public Type GetConfigType()
	{
		return typeof(LogsConfig);
	}

	public object GetDefaultConfig()
	{
		return new LogsConfig();
	}

	public void SetConfig(object config)
	{
		_config = (config as LogsConfig) ?? new LogsConfig();
	}

	public static async void LogToFile(string message, LogType logType)
	{
		if (_isWaitingForPlayers)
		{
			string timestamp = DateTime.UtcNow.AddHours(3.0).ToString("yyyy-MM-dd HH:mm:ss");
			if (1 == 0)
			{
			}
			string text = logType switch
			{
				LogType.Player => _playerLogFilePath, 
				LogType.Admin => _adminLogFilePath, 
				LogType.Server => _serverLogFilePath, 
				LogType.Ban => _banLogFilePath, 
				_ => throw new ArgumentException("Invalid log type"), 
			};
			if (1 == 0)
			{
			}
			string logFilePath = text;
			try
			{
				File.AppendAllText(logFilePath, "[" + timestamp + "] " + message + "\n");
			}
			catch (Exception e)
			{
				Log.Warn("[Logs] Ошибка записи в файл " + logFilePath + ": " + e.Message);
			}
			await SendWebhook(message, logType, timestamp, Path.GetFileName(logFilePath));
		}
	}

	private static async Task SendWebhook(string message, LogType logType, string timestamp, string logFileName)
	{
		if (1 == 0)
		{
		}
		string text = logType switch
		{
			LogType.Player => PlayerWebhookUrl, 
			LogType.Admin => AdminWebhookUrl, 
			LogType.Server => ServerWebhookUrl, 
			LogType.Ban => BanWebhookUrl, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		string webhookUrl = text;
		if (string.IsNullOrEmpty(webhookUrl))
		{
			return;
		}
		message = EscapeJson(message);
		string emoji = GetEmojiForEvent(message, logType);
		if (1 == 0)
		{
		}
		int num = logType switch
		{
			LogType.Player => 65280, 
			LogType.Admin => 16753920, 
			LogType.Server => 255, 
			LogType.Ban => 9109504, 
			_ => 3092790, 
		};
		if (1 == 0)
		{
		}
		int color = num;
		string titleTime;
		try
		{
			DateTime parsedTimestamp = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", null);
			titleTime = $"{emoji} Время: {parsedTimestamp:HH:mm} | Дата: {parsedTimestamp:yyyy-MM-dd}";
		}
		catch
		{
			titleTime = $"{emoji} {logType}: Событие";
		}
		string jsonPayload = string.Format("\n            {{\n                \"embeds\": [\n                    {{\n                        \"title\": \"{0}\",\n                        \"description\": \"```{1}```\",\n                        \"color\": {2},\n                        \"timestamp\": \"{3}\",\n                        \"footer\": {{\n                            \"text\": \"Сервер: 7778 | Файл: {4}\"\n                        }}\n                    }}\n                ]\n            }}", EscapeJson(titleTime), message, color, DateTime.UtcNow.ToString("o"), logFileName);
		try
		{
			StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
			await _httpClient.PostAsync(webhookUrl, (HttpContent)(object)content);
		}
		catch (Exception ex)
		{
			Log.Warn("[Logs] Ошибка webhook: " + ex.Message);
		}
	}

	private static string GetEmojiForEvent(string message, LogType logType)
	{
		string eventKey = message.ToLower().Split(new char[1] { ' ' }).Skip(1)
			.FirstOrDefault()?.Trim() ?? message.ToLower();
		if (1 == 0)
		{
		}
		string result = logType switch
		{
			LogType.Player => PlayerEventEmojis.FirstOrDefault((KeyValuePair<string, string> x) => eventKey.Contains(x.Key.ToLower())).Value ?? "\ud83d\udc64", 
			LogType.Admin => AdminEventEmojis.FirstOrDefault((KeyValuePair<string, string> x) => eventKey.Contains(x.Key.ToLower())).Value ?? "\ud83d\udee1\ufe0f", 
			LogType.Server => ServerEventEmojis.FirstOrDefault((KeyValuePair<string, string> x) => eventKey.Contains(x.Key.ToLower())).Value ?? "\ud83d\udda5\ufe0f", 
			LogType.Ban => "\ud83d\udd28", 
			_ => "\ud83d\udcdd", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static string EscapeJson(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n")
			.Replace("\r", "\\r")
			.Replace("\t", "\\t");
	}

	private static void OnWaitingForPlayers()
	{
		string text = DateTime.UtcNow.AddHours(3.0).ToString("yyyy-MM-dd_HH-mm-ss");
		string path = Path.Combine(Paths.Plugins, "FULT-ENGINE", "Log");
		_playerLogFilePath = Path.Combine(path, "Player", text + ".txt");
		_adminLogFilePath = Path.Combine(path, "Admin", text + ".txt");
		_serverLogFilePath = Path.Combine(path, "Server", text + ".txt");
		_banLogFilePath = Path.Combine(path, "Ban", text + ".txt");
		_isWaitingForPlayers = true;
		LogToFile("Сервер начал ожидание игроков.", LogType.Server);
	}
}
