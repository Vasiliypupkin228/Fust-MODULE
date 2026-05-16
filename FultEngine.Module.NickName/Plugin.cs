using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using PlayerRoles;

namespace FultEngine.Module.NickName;

public class Plugin : IFultEngineModule
{
	private Config _config;

	private Dictionary<Player, int> _playerOffsets = new Dictionary<Player, int>();

	private static readonly List<string> UsedCallsigns = new List<string>();

	private static readonly List<string> UsedFirstNames = new List<string>();

	private static readonly List<string> UsedLastNames = new List<string>();

	public string Name { get; } = "NickName";


	public string Author => "FUST";

	public Version Version { get; } = new Version(1, 0, 0);


	public void OnEnabled()
	{
		Player.ChangingNickname += (CustomEventHandler<ChangingNicknameEventArgs>)OnChangingNickname;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Verified += (CustomEventHandler<VerifiedEventArgs>)OnVerified;
	}

	public void OnDisabled()
	{
		Player.ChangingNickname -= (CustomEventHandler<ChangingNicknameEventArgs>)OnChangingNickname;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Verified -= (CustomEventHandler<VerifiedEventArgs>)OnVerified;
		_playerOffsets.Clear();
		UsedCallsigns.Clear();
		UsedFirstNames.Clear();
		UsedLastNames.Clear();
	}

	private void OnChangingNickname(ChangingNicknameEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && !string.IsNullOrWhiteSpace(ev.NewName) && !Regex.IsMatch(ev.NewName, "^\\[\\d+\\]"))
		{
			Timing.CallDelayed(0.5f, (Action)delegate
			{
				ev.Player.DisplayNickname = $"[{ev.Player.Id}] {ev.NewName}";
			});
		}
	}

	private void OnVerified(VerifiedEventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			Timing.CallDelayed(0.5f, (Action)delegate
			{
				ev.Player.DisplayNickname = $"[{ev.Player.Id}] {ev.Player.Nickname}";
			});
		}
	}

	public void OnSpawned(SpawnedEventArgs ev)
	{
		if (ev.Player == (Player)null)
		{
			return;
		}
		if (!_config.EnableRPMode || (int)ev.Player.Role.Team == 5 || (int)ev.Player.Role.Team == 0)
		{
			ev.Player.DisplayNickname = $"[{ev.Player.Id}] {ev.Player.Nickname}";
			return;
		}
		string text = "";
		object obj;
		if (!(ev.Player.Role == (RoleTypeId)14))
		{
			Team team = ev.Player.Role.Team;
			obj = ((object)(Team)(ref team)).ToString();
		}
		else
		{
			obj = "Tutorial";
		}
		string key = (string)obj;
		if (_config.NameFormats.TryGetValue(key, out var value))
		{
			Random random = new Random();
			text = value.Replace("{PlayerId}", ev.Player.Id.ToString()).Replace("{Nickname}", ev.Player.Nickname).Replace("{RandomNumber}", random.Next(_config.ClassDNumberRangeMin, _config.ClassDNumberRangeMax + 1).ToString())
				.Replace("{RandomCallSign}", GetUniqueCallsign())
				.Replace("{RandomLastName}", GetUniqueLastName())
				.Replace("{RandomFirstName}", GetUniqueFirstName());
		}
		else
		{
			text = $"[{ev.Player.Id}] {ev.Player.Nickname}";
		}
		ev.Player.DisplayNickname = text;
	}

	private string GetUniqueCallsign()
	{
		List<string> list = new List<string>(_config.Callsigns);
		list.RemoveAll((string x) => UsedCallsigns.Contains(x));
		if (list.Count == 0)
		{
			UsedCallsigns.Clear();
			list = new List<string>(_config.Callsigns);
		}
		string text = list[new Random().Next(list.Count)];
		UsedCallsigns.Add(text);
		return text;
	}

	private string GetUniqueFirstName()
	{
		List<string> list = new List<string>(_config.FirstNames);
		list.RemoveAll((string x) => UsedFirstNames.Contains(x));
		if (list.Count == 0)
		{
			UsedFirstNames.Clear();
			list = new List<string>(_config.FirstNames);
		}
		string text = list[new Random().Next(list.Count)];
		UsedFirstNames.Add(text);
		return text;
	}

	private string GetUniqueLastName()
	{
		List<string> list = new List<string>(_config.LastNames);
		list.RemoveAll((string x) => UsedLastNames.Contains(x));
		if (list.Count == 0)
		{
			UsedLastNames.Clear();
			list = new List<string>(_config.LastNames);
		}
		string text = list[new Random().Next(list.Count)];
		UsedLastNames.Add(text);
		return text;
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config
		{
			EnableRPMode = true,
			NameFormats = new Dictionary<string, string>
			{
				{ "ClassD", "[{PlayerId}] D-{RandomNumber}" },
				{ "FoundationForces", "[{PlayerId}] {RandomCallSign}" },
				{ "ChaosInsurgency", "[{PlayerId}] {RandomCallSign}" },
				{ "Scientists", "[{PlayerId}] {RandomLastName} {RandomFirstName}" },
				{ "Dead", "[{PlayerId}] {Nickname}" },
				{ "Spectator", "[{PlayerId}] {Nickname}" },
				{ "Tutorial", "[{PlayerId}] {Nickname}" }
			},
			ClassDNumberRangeMin = 4000,
			ClassDNumberRangeMax = 5000,
			Callsigns = new List<string>
			{
				"Барс", "Беркут", "Вепрь", "Вихрь", "Гром", "Жнец", "Клык", "Коготь", "Молот", "Оскал",
				"Рысь", "Сокол", "Сталкер", "Тень", "Ураган", "Шакал", "Ястреб", "Боец", "Ворон", "Гроза",
				"Дым", "Зверь", "Кречет", "Охотник", "Пламя", "Рубеж", "Скала", "Смерч", "Туман", "Шторм",
				"Лев", "Тигр", "Медведь", "Волк", "Орёл"
			},
			FirstNames = new List<string>
			{
				"Алексей", "Андрей", "Артём", "Борис", "Вадим", "Виктор", "Владимир", "Глеб", "Даниил", "Денис",
				"Дмитрий", "Егор", "Иван", "Игорь", "Илья", "Кирилл", "Константин", "Лев", "Максим", "Марк",
				"Михаил", "Никита", "Николай", "Олег", "Павел", "Роман", "Сергей", "Станислав", "Тимур", "Юрий",
				"Антон", "Вячеслав", "Евгений", "Фёдор", "Ярослав"
			},
			LastNames = new List<string>
			{
				"Белов", "Волков", "Гордеев", "Григорьев", "Данилов", "Егоров", "Жуков", "Зайцев", "Иванов", "Ковалёв",
				"Козлов", "Крылов", "Кузнецов", "Лебедев", "Медведев", "Мельников", "Морозов", "Новиков", "Орлов", "Павлов",
				"Петров", "Попов", "Романов", "Сидоров", "Смирнов", "Соколов", "Титов", "Фёдоров", "Шевцов", "Яковлев",
				"Богданов", "Васильев", "Калинин", "Макаров", "Никитин"
			}
		};
	}

	public void SetConfig(object config)
	{
		_config = (Config)config;
	}
}
