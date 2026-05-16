using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Yakor : CustomItem
{
	[CompilerGenerated]
	private sealed class Process_d_30 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public Yakor __4__this;

		private Vector3 startPos;

		private float startTime;

		private float endTime;

		private Vector3 origin;

		private float distance;

		private Vector3 yakorPosition;

		private float progress;

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
		public Process_d_30(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
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
				startPos = player.Position;
				startTime = Time.time;
				endTime = startTime + 5f;
				origin = player.CameraTransform.position;
				distance = Vector3.Distance(origin, startPos);
				if (distance > 3f)
				{
					__4__this.ShowMeowHint(player, 2f, "Вы слишком далеко, чтобы установить якорь (макс. 3 м)!");
					return false;
				}
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if (Time.time < endTime)
			{
				if (!player.IsAlive || player.CurrentItem == null || !((CustomItem)__4__this).Check(player.CurrentItem))
				{
					__4__this.Cancel(player, "Процесс прерван (предмет потерян).");
					return false;
				}
				if (Vector3.Distance(startPos, player.Position) > 0.2f)
				{
					__4__this.Cancel(player, "Процесс прерван (вы сдвинулись).");
					return false;
				}
				progress = (Time.time - startTime) / 5f;
				__4__this.ShowProgress(player, progress);
				__2__current = Timing.WaitForSeconds(0.1f);
				__1__state = 1;
				return true;
			}
			yakorPosition = player.Position + new Vector3(0f, -0.9f, 0f);
			YakorZoneManager.ActiveYakors.Add(new YakorZoneManager.YakorZone
			{
				Position = yakorPosition
			});
			ObjectSpawner.SpawnSchematic("Yakor", yakorPosition, (Quaternion?)Quaternion.identity, (Vector3?)Vector3.one, (SchematicObjectDataList)null);
			player.RemoveItem(player.CurrentItem, true);
			__4__this._activeCoroutines.Remove(player);
			__4__this.ShowMeowHint(player, 3f, "Якорь реальности установлен. Юмы в радиусе 15 м подавлены.");
			return false;
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

	private readonly Dictionary<Player, CoroutineHandle> _activeCoroutines = new Dictionary<Player, CoroutineHandle>();

	private const float Duration = 5f;

	public override uint Id { get; set; } = 16u;


	public override string Name { get; set; } = "<b><color=#28a0a8e4>Якорь</color></b>";


	public override string Description { get; set; } = "Удерживай, чтобы активировать якорь реальности.";


	public override float Weight { get; set; } = 1f;


	public override SpawnProperties SpawnProperties { get; set; }

	protected override void SubscribeEvents()
	{
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDrop;
		Server.RoundEnded += (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDrop;
		Server.RoundEnded -= (CustomEventHandler<RoundEndedEventArgs>)OnRoundEnded;
		((CustomItem)this).UnsubscribeEvents();
	}

	private void OnRoundEnded(RoundEndedEventArgs ev)
	{
		YakorZoneManager.ActiveYakors.Clear();
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
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
		CIMessage.SendMessage(player, "Якорь реальности");
	}

	private void OnDrop(DroppingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
			StopProcess(ev.Player);
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item))
		{
			ev.IsAllowed = false;
			StopProcess(ev.Player);
			_activeCoroutines[ev.Player] = Timing.RunCoroutine(Process(ev.Player));
		}
	}

	[IteratorStateMachine(typeof(Process_d_30))]
	private IEnumerator<float> Process(Player player)
	{
		return new Process_d_30(0)
		{
			__4__this = this,
			player = player
		};
	}

	private void ShowProgress(Player player, float progress)
	{
		int num = 18;
		int num2 = Mathf.RoundToInt(progress * (float)num);
		int count = num - num2;
		int num3 = Mathf.RoundToInt(progress * 100f);
		string text = "<color=#00ff00>" + new string('▒', num2) + "</color><color=#616161>" + new string('▒', count) + "</color>";
		string message = string.Format("<size=25><b><color=#61616193>『</color></size> <size=21>{0}</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{1}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>Прогресс: {2}%</size> <size=29><b><color=#61616193>|</color></size></b>\n<size=15><color=#ff0000>{3}</color></size>", "УСТАНОВКА ЯКОРЯ", text, num3, "Не убирайте предмет из рук и оставайтесь в радиусе установки!");
		player.ShowMeowHint(0.2f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}

	private void Cancel(Player player, string reason)
	{
		ShowMeowHint(player, 2f, reason ?? "");
		StopProcess(player);
	}

	private void ShowMeowHint(Player player, float time, string message)
	{
		player.ShowMeowHint(time, "<size=29><b><color=#61616193>|</color></size> <size=19>" + message + "</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
	}

	private void StopProcess(Player player)
	{
		if (_activeCoroutines.TryGetValue(player, out var value))
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { value });
			_activeCoroutines.Remove(player);
		}
	}
}
