using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.CustomItems;
using FultEngine.LoaderModule;

namespace FultEngine.Module.AHint;

public class Plugin : IFultEngineModule
{
	private Config _config;

	private static readonly Dictionary<ItemType, string> VanillaHints = new Dictionary<ItemType, string>
	{
		{
			(ItemType)14,
			"</b>Advanced First Aid Kit"
		},
		{
			(ItemType)20,
			"</b>HK416A5\n<size=19>Немецкая штурмовая винтовка калибра 5.56x45mm</size>"
		},
		{
			(ItemType)40,
			"</b>AKM\n<size=19>Советская штурмовая винтовка калибра 7.62x39mm</size>"
		},
		{
			(ItemType)52,
			"</b>HK G36\n<size=19>Немецкая штурмовая винтовка калибра 5.56x45mm</size>"
		},
		{
			(ItemType)21,
			"</b>KRISS Vector\n<size=19>Американский пистолет-пулемёт под патрон 9x19mm</size>"
		},
		{
			(ItemType)23,
			"</b>HK MP7A1\n<size=19>Немецкий пистолет-пулемёт под патрон 9x19mm</size>"
		},
		{
			(ItemType)13,
			"</b>Ruger SR9c\n<size=19>Американский самозарядный пистолет под патрон 9x19mm</size>"
		},
		{
			(ItemType)30,
			"</b>HK45\n<size=19>Немецкий самозарядный пистолет под патрон 9x19mm</size>"
		},
		{
			(ItemType)39,
			"</b>Taurus Judge\n<size=19>Револьвер барабанного типа калибра .45 Colt</size>"
		},
		{
			(ItemType)41,
			"</b>DP-12\n<size=19>Двуствольное помповое ружьё 12-го калибра</size>"
		},
		{
			(ItemType)24,
			"</b>HK MG5\n<size=19>Немецкий пулемёт под патрон 7.62x51mm</size>"
		},
		{
			(ItemType)53,
			"</b>Образец A7\n<size=19>Оружие, созданное при помощи SCP-914. Использует патрон 7.62x39mm</size>"
		},
		{
			(ItemType)48,
			"</b>Образец COM-45\n<size=19>Оружие, созданное при помощи SCP-914. Использует патрон 9x19mm</size>"
		},
		{
			(ItemType)16,
			"</b>Micro H.I.D\n<size=19>Экспериментальная противодействующая система высокой интенсивности заряда</size>"
		},
		{
			(ItemType)25,
			"</b>РГД-5\n<size=19>Советская ручная граната</size>"
		},
		{
			(ItemType)26,
			"</b>Model 7290\n<size=19>Американская светозвуковая граната</size>"
		}
	};

	private static readonly List<(uint Id, string Message)> CustomHints = new List<(uint, string)>
	{
		(1488u, "</b>M18\n<size=19>Американская дымовая ручная граната</size>"),
		(401u, "</b>АК-101\n<size=19>Современная версия семейства Калашникова под 5.56x45mm</size>"),
		(402u, "</b>АК-102\n<size=19>Укороченная версия АК-101 для ближнего боя</size>"),
		(403u, "</b>АК-103\n<size=19>Мощная штурмовая винтовка под 7.62x39mm</size>"),
		(404u, "</b>АК-104\n<size=19>Компактная версия АК-103 для штурма помещений</size>"),
		(405u, "</b>АК-105\n<size=19>Укороченный автомат под 5.45x39mm</size>"),
		(406u, "</b>АКС-74У\n<size=19>Компактный автомат для ближних дистанций</size>"),
		(407u, "</b>АК-12\n<size=19>Современный российский автомат нового поколения</size>"),
		(408u, "</b>АК-15\n<size=19>Версия АК-12 под патрон 7.62x39mm</size>"),
		(409u, "</b>РПК-16\n<size=19>Ручной пулемёт платформы Калашникова</size>"),
		(410u, "</b>АК Alpha\n<size=19>Тактическая модификация автомата Калашникова</size>"),
		(451u, "</b>M4A1\n<size=19>Американская модульная штурмовая винтовка 5.56x45mm</size>"),
		(452u, "</b>HK416\n<size=19>Немецкая штурмовая винтовка повышенной надёжности</size>"),
		(453u, "</b>FN SCAR-L\n<size=19>Лёгкая модульная винтовка под 5.56x45mm</size>"),
		(454u, "</b>FN SCAR-H\n<size=19>Боевая винтовка под патрон 7.62x51mm</size>"),
		(455u, "</b>G36C\n<size=19>Компактная винтовка для ближнего боя</size>"),
		(456u, "</b>L85A2\n<size=19>Британская штурмовая винтовка bullpup-компоновки</size>"),
		(457u, "</b>FAMAS\n<size=19>Французская автоматическая винтовка высокой скорострельности</size>"),
		(458u, "</b>AUG A3\n<size=19>Австрийская модульная винтовка bullpup-компоновки</size>"),
		(459u, "</b>SIG MCX\n<size=19>Современная платформа для штурмовых операций</size>"),
		(460u, "</b>QBZ-191\n<size=19>Современная китайская штурмовая винтовка</size>")
	};

	public string Name => "AHint";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 2);

	public void OnEnabled()
	{
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Handcuffing += (CustomEventHandler<HandcuffingEventArgs>)OnHandcuffing;
	}

	public void OnDisabled()
	{
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.Handcuffing -= (CustomEventHandler<HandcuffingEventArgs>)OnHandcuffing;
	}

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
		_config = (config as Config) ?? new Config();
	}

	private void OnHandcuffing(HandcuffingEventArgs ev)
	{
		if (_config.DisableHandcuffing)
		{
			ev.IsAllowed = false;
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (_config.IsEnabled && !(ev.Player == (Player)null) && ev.Item != null && !TryShowCustomHint(ev.Player, ev.Item) && VanillaHints.TryGetValue(ev.Item.Type, out var value))
		{
			CIMessage.SendMessage(ev.Player, value, _config.Duration);
		}
	}

	private bool TryShowCustomHint(Player player, Item item)
	{
		for (int i = 0; i < CustomHints.Count; i++)
		{
			(uint, string) tuple = CustomHints[i];
			CustomItem val = CustomItem.Get(tuple.Item1);
			if (val != null && val.Check(item))
			{
				CIMessage.SendMessage(player, tuple.Item2, _config.Duration);
				return true;
			}
		}
		return false;
	}
}
