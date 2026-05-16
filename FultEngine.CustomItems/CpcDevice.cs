using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using PlayerRoles;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class CpcDevice : CustomItem
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class CpcCommand : ICommand
	{
		public string Command => "cpc";

		public string Description => "Отправить сообщение через КПК.";

		public string[] Aliases { get; } = new string[0];


		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			Player val = Player.Get(sender);
			if (val == (Player)null || !CustomItem.Get(9u).Check(val.CurrentItem))
			{
				response = "Нужен КПК в руке.";
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "Использование: .cpc <ID/All/PERS/NTF> [текст]";
				return false;
			}
			string text = CollectionExtensions.At<string>(arguments, 0).ToLower();
			string msg = string.Join(" ", arguments.Skip(1));
			string displayNickname = val.DisplayNickname;
			if (int.TryParse(text, out var result))
			{
				Player val2 = Player.Get(result);
				CustomItem val5 = default(CustomItem);
				if (val2 != (Player)null && val2.Items.Any((Item i) => CustomItem.TryGet(i, ref val5) && val5.Id == 9))
				{
					Send(val2, "личный", msg, displayNickname);
					response = "Отправлено.";
					return true;
				}
				response = "Игрок не найден или без КПК.";
				return false;
			}
			if (text == "all")
			{
				int num = 0;
				CustomItem val4 = default(CustomItem);
				foreach (Player item in Player.List)
				{
					if (item.Items.Any((Item i) => CustomItem.TryGet(i, ref val4) && val4.Id == 9))
					{
						Send(item, "общий", msg, displayNickname);
						num++;
					}
				}
				response = ((num > 0) ? $"Отправлено всем ({num})." : "Нет получателей.");
				return true;
			}
			if (1 == 0)
			{
			}
			string text2 = ((text == "pers") ? "ПЕРСОНАЛ" : ((!(text == "ntf")) ? null : "НТФ"));
			if (1 == 0)
			{
			}
			string text3 = text2;
			if (text3 == null)
			{
				response = "Неверная цель.";
				return false;
			}
			int num2 = 0;
			CustomItem val3 = default(CustomItem);
			foreach (Player item2 in Player.List)
			{
				if (item2.Items.Any((Item i) => CustomItem.TryGet(i, ref val3) && val3.Id == 9))
				{
					if (1 == 0)
					{
					}
					bool flag = ((text == "pers") ? (item2.Role != (RoleTypeId)6 || (int)item2.Role.Team == 1) : (text == "ntf" && (int)item2.Role.Team == 1 && item2.Role != (RoleTypeId)15));
					if (1 == 0)
					{
					}
					if (flag)
					{
						Send(item2, text3, msg, displayNickname);
						num2++;
					}
				}
			}
			response = ((num2 > 0) ? $"Отправлено {text3} ({num2})." : "Нет получателей.");
			return true;
		}

		private static void Send(Player p, string type, string msg, string from)
		{
			string text = ((type == "общий") ? "yellow" : "cyan");
			string text2 = ((type == "общий") ? "#FFFF00" : "#00FF00");
			string text3 = "[" + type + "][от " + from + "] КПК: " + msg;
			string text4 = "<color=" + text2 + ">[" + type + "][от " + from + "] КПК: " + msg + "</color>";
			p.SendConsoleMessage(text3, text);
			p.ShowMeowHint(7f, "<size=29><b><color=#61616193>|</color></size> <size=19>На ваш КПК пришло новое уведомление. (откройте консоль ~)</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 93, 0, (HintAlignment)2);
			AudioManager.CreateForPlayer(p, "Cpc", 0.7f, 5f);
		}
	}

	public override uint Id { get; set; } = 9u;


	public override string Name { get; set; } = "<b><color=#48d914e2>КПК</b></color>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 0.3f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
	{
		Limit = 1u,
		DynamicSpawnPoints = new List<DynamicSpawnPoint>()
	};


	protected override void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
	}

	protected override void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Pickup != null && ((CustomItem)this).Check(ev.Pickup))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			ShowItemHint(ev.Player);
		}
	}

	private void ShowItemHint(Player player)
	{
		CIMessage.SendMessage(player, "КПК\n<size=19>карманный персональный компьютер\r\n</size>");
	}

	private void OnRoundStarted()
	{
	}
}
