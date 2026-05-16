using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.CustomItems;
using FultEngine.Module.DoorTeleport;
using FultEngine.Module.DoorTeleport.Features;
using Hints;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FultEngine.CItemModule.Toolgun;

public class Item
{
	[CustomItem(/*Could not decode attribute arguments.*/)]
	public class Toolgun : CustomItem
	{
		private readonly EventHandlers _eventHandlers;

		private readonly HintDisplay _hintDisplay = new HintDisplay();

		public override uint Id { get; set; } = 8u;


		public override string Name { get; set; } = "Toolgun";


		public override string Description { get; set; } = "Пистолет для создания телепортов между схематиками";


		public override float Weight { get; set; } = 1f;


		public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


		public Toolgun()
		{
			_eventHandlers = new EventHandlers(this, _hintDisplay);
		}

		protected override void SubscribeEvents()
		{
			AudioClipStorage.LoadClip("/home/EXILED/EXILED/Configs/AudioPluginCore/select.ogg", "select");
			AudioClipStorage.LoadClip("/home/EXILED/EXILED/Configs/AudioPluginCore/error.ogg", "error");
			_eventHandlers.SubscribeEvents();
			((CustomItem)this).SubscribeEvents();
		}

		protected override void UnsubscribeEvents()
		{
			_eventHandlers.UnsubscribeEvents();
			((CustomItem)this).UnsubscribeEvents();
		}
	}

	public class EventHandlers
	{
		private readonly HintDisplay _hintDisplay;

		private readonly Dictionary<Player, string> _pendingDoor = new Dictionary<Player, string>();

		public EventHandlers(Toolgun toolgun, HintDisplay hintDisplay)
		{
			_hintDisplay = hintDisplay;
		}

		public void SubscribeEvents()
		{
			Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
			Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
			Player.Shot += (CustomEventHandler<ShotEventArgs>)OnShot;
			Player.Left += (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		}

		public void UnsubscribeEvents()
		{
			Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
			Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
			Player.Shot -= (CustomEventHandler<ShotEventArgs>)OnShot;
			Player.Left -= (CustomEventHandler<LeftEventArgs>)OnPlayerLeft;
		}

		private void OnPlayerLeft(LeftEventArgs ev)
		{
			_pendingDoor.Remove(((JoinedEventArgs)ev).Player);
		}

		private void OnChangingItem(ChangingItemEventArgs ev)
		{
			if (ev.Item != null && CustomItem.Get(8u).Check(ev.Item))
			{
				CIMessage.SendMessage(ev.Player, "Пистолет для создания телепортов между схематиками");
			}
		}

		private void OnItemAdded(ItemAddedEventArgs ev)
		{
			if (CustomItem.Get(8u).Check(ev.Item))
			{
				CIMessage.SendMessage(ev.Player, "Пистолет для создания телепортов между схематиками");
			}
		}

		private void OnShot(ShotEventArgs ev)
		{
			Player player = ev.Player;
			if (((player != null) ? player.CurrentItem : null) == null || !CustomItem.Get(8u).Check(ev.Player.CurrentItem))
			{
				return;
			}
			ev.CanHurt = false;
			Vector3 val = ev.Player.CameraTransform.position + ev.Player.CameraTransform.forward;
			Vector3 forward = ev.Player.CameraTransform.forward;
			Vector3 normalized = ((Vector3)(ref forward)).normalized;
			SchematicObject laser = ObjectSpawner.SpawnSchematic("LaserToolGun", val, (Quaternion?)Quaternion.LookRotation(normalized), (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			Timing.CallDelayed(0.9f, (Action)delegate
			{
				if ((Object)(object)laser != (Object)null)
				{
					((MapEditorObject)laser).Destroy();
				}
			});
			if (!TryGetSchematicInSight(ev.Player, out var schematic))
			{
				AudioManager.CreateForGameObject(ev.Player.GameObject, "error", 3f, 5f);
				CIMessage.SendMessage(ev.Player, "<color=red>Схематика не найдена в зоне видимости.</color>", 3f);
				return;
			}
			if (Plugin.Instance == null)
			{
				AudioManager.CreateForGameObject(ev.Player.GameObject, "error", 3f, 5f);
				CIMessage.SendMessage(ev.Player, "<color=red>Плагин DoorTeleport не инициализирован.</color>", 3f);
				return;
			}
			string schematicName = schematic.Name;
			try
			{
				if (_pendingDoor.TryGetValue(ev.Player, out var firstDoorName))
				{
					if (firstDoorName == schematicName)
					{
						AudioManager.CreateForGameObject(ev.Player.GameObject, "error", 3f, 5f);
						CIMessage.SendMessage(ev.Player, "<color=red>Нельзя связать дверь саму с собой.</color>", 3f);
						return;
					}
					List<DoorPairConfig> source = Frequency.LoadAllDoorPairs();
					if (source.Any((DoorPairConfig p) => p.SchematicName1 == firstDoorName || p.SchematicName2 == firstDoorName || p.SchematicName1 == schematicName || p.SchematicName2 == schematicName))
					{
						AudioManager.CreateForGameObject(ev.Player.GameObject, "error", 3f, 5f);
						CIMessage.SendMessage(ev.Player, "<color=red>Одна из дверей уже связана.</color>", 3f);
						return;
					}
					DoorPairConfig pair = new DoorPairConfig
					{
						SchematicName1 = firstDoorName,
						SchematicName2 = schematicName
					};
					SaveDoorPair(pair);
					AudioManager.CreateForGameObject(ev.Player.GameObject, "select", 3f, 5f);
					CIMessage.SendMessage(ev.Player, "Связаны двери: <color=green>" + firstDoorName + " ↔ " + schematicName + "</color>", 3f);
					_pendingDoor.Remove(ev.Player);
				}
				else
				{
					_pendingDoor[ev.Player] = schematicName;
					AudioManager.CreateForGameObject(ev.Player.GameObject, "select", 3f, 5f);
					CIMessage.SendMessage(ev.Player, "Выбрана первая дверь: <color=green>" + schematicName + "</color>. Выберите вторую.", 3f);
				}
			}
			catch
			{
				AudioManager.CreateForGameObject(ev.Player.GameObject, "error", 3f, 5f);
				CIMessage.SendMessage(ev.Player, "<color=red>Ошибка при создании телепорта.</color>", 3f);
			}
		}

		private bool TryGetSchematicInSight(Player player, out SchematicObject schematic)
		{
			schematic = null;
			float num = 5f;
			Ray val = default(Ray);
			((Ray)(ref val))._002Ector(player.CameraTransform.position, player.CameraTransform.forward);
			RaycastHit val2 = default(RaycastHit);
			if (!Physics.Raycast(val, ref val2, num, LayerMask.GetMask(new string[2] { "Default", "Interactable" })))
			{
				return false;
			}
			schematic = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<SchematicObject>();
			return (Object)(object)schematic != (Object)null;
		}

		private void SaveDoorPair(DoorPairConfig pair)
		{
			if (!Directory.Exists(Frequency.ConfigPath))
			{
				Directory.CreateDirectory(Frequency.ConfigPath);
			}
			string[] files = Directory.GetFiles(Frequency.ConfigPath, "door_*.yml");
			int num = files.Length + 1;
			string path = Path.Combine(Frequency.ConfigPath, $"door_{num}.yml");
			string contents = ((BuilderSkeleton<SerializerBuilder>)new SerializerBuilder()).WithNamingConvention(CamelCaseNamingConvention.Instance).WithIndentedSequences().ConfigureDefaultValuesHandling((DefaultValuesHandling)0)
				.Build()
				.Serialize((object)pair);
			File.WriteAllText(path, contents);
		}
	}
}
