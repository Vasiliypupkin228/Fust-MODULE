using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Pickups;
using MEC;
using UnityEngine;

namespace FultEngine.ModuleCommand;

[CommandHandler(typeof(ClientCommandHandler))]
public class Take : ICommand
{
	public string Command { get; } = "take";


	public string Description { get; } = "Позволяет 049 подобрать предмет.";


	public string[] Aliases { get; } = Array.Empty<string>();


	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player pl = Player.Get(sender);
		if ((int)pl.Role.Type != 5)
		{
			response = "Вы не SCP-049.";
			return true;
		}
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(pl.CameraTransform.position, pl.CameraTransform.forward, ref val, 6f);
		ItemPickupBase componentInParent = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponentInParent<ItemPickupBase>();
		if ((Object)(object)componentInParent == (Object)null)
		{
			response = "Предмет не найден.";
			return false;
		}
		Pickup pickup = Pickup.Get(((Component)componentInParent).gameObject);
		if (pickup == null)
		{
			response = "Предмет не найден.";
			return false;
		}
		if (pl.CurrentItem != null || pl.Items.Count >= 8)
		{
			response = "Вы уже держите что-то в руках. Выбросить - T (бинд по-умолчанию на кидание предметов)";
			return false;
		}
		pl.AddItem(pickup.Type);
		Timing.CallDelayed(0.1f, (Action)delegate
		{
			foreach (Item item in pl.Items)
			{
				pl.CurrentItem = item;
			}
			pickup.Destroy();
		});
		response = "Успешно!";
		return true;
	}
}
