using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.Cassie;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using MEC;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using VoiceChat;

namespace FultEngine.CustomItems;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class MASK : CustomItem
{
	[CompilerGenerated]
	private sealed class AttractPlayers_d_83 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Pickup pickup;

		public MASK __4__this;

		private IEnumerator<Player> __s__1;

		private Player player;

		private float distance;

		private Vector3 direction;

		private Vector3 playerVelocity;

		private float playerSpeed;

		private Vector3 playerMoveDirection;

		private float resistanceFactor;

		private float effectiveSpeed;

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
		public AttractPlayers_d_83(int __1__state)
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
				__s__1 = Player.List.GetEnumerator();
				try
				{
					while (__s__1.MoveNext())
					{
						player = __s__1.Current;
						if (!(player == (Player)null) && !ChangedPlayers.Contains(player) && (int)player.Role.Side != 0)
						{
							distance = Vector3.Distance(player.Position, pickup.Position);
							if (distance <= __4__this.AttractionRadius && distance > __4__this.TransformationRadius)
							{
								Vector3 val = pickup.Position - player.Position;
								direction = ((Vector3)(ref val)).normalized;
								playerVelocity = player.GameObject.GetComponent<CharacterController>().velocity;
								playerSpeed = ((Vector3)(ref playerVelocity)).magnitude;
								playerMoveDirection = ((Vector3)(ref playerVelocity)).normalized;
								resistanceFactor = Mathf.Max(0f, 1f - Vector3.Dot(direction, playerMoveDirection) * -1f);
								effectiveSpeed = __4__this.AttractionSpeed * resistanceFactor;
								Player obj = player;
								obj.Position += direction * effectiveSpeed * Time.deltaTime;
							}
							else if (distance <= __4__this.TransformationRadius)
							{
								__4__this.TransformPlayer(player, pickup);
								pickup.Destroy();
								return false;
							}
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
				break;
			}
			if (pickup != null)
			{
				pickup = Pickup.Get(pickup.Serial);
				if (pickup == null)
				{
					return false;
				}
				__2__current = Timing.WaitForSeconds(0.05f);
				__1__state = 1;
				return true;
			}
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

	[CompilerGenerated]
	private sealed class Corrosion_d_92 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Player player;

		public MASK __4__this;

		private float currentDamage;

		private float timeElapsed;

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
		public Corrosion_d_92(int __1__state)
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
				currentDamage = __4__this.DamagePerTick;
				timeElapsed = 0f;
				break;
			case 1:
				__1__state = -1;
				player.Hurt((DamageHandlerBase)new UniversalDamageHandler(currentDamage, DeathTranslations.Poisoned, (CassieAnnouncement)null));
				timeElapsed += 1f;
				if (timeElapsed >= __4__this.BlurredEffectTime)
				{
					player.EnableEffect((EffectType)47, __4__this.LifeDuration - timeElapsed, false);
				}
				if (timeElapsed % __4__this.CorrosionIncreaseInterval == 0f)
				{
					currentDamage += __4__this.CorrosionIncreaseAmount;
				}
				break;
			}
			if (timeElapsed < __4__this.LifeDuration)
			{
				__2__current = Timing.WaitForSeconds(1f);
				__1__state = 1;
				return true;
			}
			if (player.IsAlive)
			{
				player.Hurt((DamageHandlerBase)new UniversalDamageHandler(player.Health, DeathTranslations.Poisoned, (CassieAnnouncement)null));
			}
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

	private readonly HashSet<uint> _processedPickups = new HashSet<uint>();

	private readonly Dictionary<Pickup, GameObject> _maskSchematics = new Dictionary<Pickup, GameObject>();

	private readonly Dictionary<Player, GameObject> _playerSchematics = new Dictionary<Player, GameObject>();

	private readonly Dictionary<Pickup, (Player, Vector3, GameObject)> _placedMasks = new Dictionary<Pickup, (Player, Vector3, GameObject)>();

	private static readonly List<Player> ChangedPlayers = new List<Player>();

	private static readonly List<Player> StopRagdollsList = new List<Player>();

	private bool cassie;

	public override uint Id { get; set; } = 10u;


	public override string Name { get; set; } = "<color=#2e7f4fd6><b>SCP-035</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "Театральная маска, излучающая зловещую ауру. Притягивает к себе, завладевая разумом и телом.";


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
	{
		Limit = 1u,
		DynamicSpawnPoints = new List<DynamicSpawnPoint>()
	};


	[Description("Радиус притяжения маски SCP-035.")]
	public float AttractionRadius { get; set; } = 5f;


	[Description("Радиус, в котором происходит трансформация в SCP-035.")]
	public float TransformationRadius { get; set; } = 0.7f;


	[Description("Скорость притяжения к маске (единиц/с).")]
	public float AttractionSpeed { get; set; } = 1f;


	[Description("Максимальное здоровье SCP-035.")]
	public int MaxHealth { get; set; } = 999;


	[Description("Начальный урон в секунду от коррозии.")]
	public float DamagePerTick { get; set; } = 1.5f;


	[Description("Интервал (в секундах) для увеличения урона коррозии.")]
	public float CorrosionIncreaseInterval { get; set; } = 30f;


	[Description("Величина увеличения урона коррозии.")]
	public float CorrosionIncreaseAmount { get; set; } = 0.5f;


	[Description("Время жизни SCP-035 (в секундах).")]
	public float LifeDuration { get; set; } = 540f;


	[Description("Время (в секундах) для активации эффекта Blurred.")]
	public float BlurredEffectTime { get; set; } = 490f;


	[Description("Предметы, которые лечат SCP-035, и объем восстанавливаемого здоровья.")]
	public Dictionary<ItemType, float> HealingItems { get; set; } = new Dictionary<ItemType, float>
	{
		{
			(ItemType)14,
			50f
		},
		{
			(ItemType)17,
			100f
		}
	};


	[Description("Воспроизводить ли сообщение CASSIE при смерти SCP-035.")]
	public bool CassieMessageOnDied { get; set; } = true;


	[Description("Пользовательское сообщение CASSIE при смерти SCP-035. Оставьте пустым для стандартного.")]
	public string CustomCassieMessageOnDied { get; set; } = "";


	protected override void SubscribeEvents()
	{
		Player.PickingUpItem += (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.ChangedItem += (CustomEventHandler<ChangedItemEventArgs>)OnChangedItem;
		Player.Dying += (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.UsingItem += (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.DroppingItem += (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.Destroying += (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Server.EndingRound += (CustomEventHandler<EndingRoundEventArgs>)OnEndingRound;
		Player.SpawningRagdoll += (CustomEventHandler<SpawningRagdollEventArgs>)OnSpawningRagdoll;
		Map.PickupAdded += (CustomEventHandler<PickupAddedEventArgs>)OnMapPickupAdded;
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		Player.PickingUpItem -= (CustomEventHandler<PickingUpItemEventArgs>)OnPickingUpItem;
		Player.ChangedItem -= (CustomEventHandler<ChangedItemEventArgs>)OnChangedItem;
		Player.DroppingItem -= (CustomEventHandler<DroppingItemEventArgs>)OnDroppingItem;
		Player.Dying -= (CustomEventHandler<DyingEventArgs>)OnDying;
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.UsingItem -= (CustomEventHandler<UsingItemEventArgs>)OnUsingItem;
		Player.Destroying -= (CustomEventHandler<DestroyingEventArgs>)OnDestroying;
		Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
		Server.EndingRound -= (CustomEventHandler<EndingRoundEventArgs>)OnEndingRound;
		Player.SpawningRagdoll -= (CustomEventHandler<SpawningRagdollEventArgs>)OnSpawningRagdoll;
		Map.PickupAdded -= (CustomEventHandler<PickupAddedEventArgs>)OnMapPickupAdded;
		((CustomItem)this).UnsubscribeEvents();
	}

	private void OnMapPickupAdded(PickupAddedEventArgs ev)
	{
		if (ev.Pickup == null || (Object)(object)ev.Pickup.Transform == (Object)null)
		{
			return;
		}
		CustomItem val = CustomItem.Get(10u);
		if (val == null || !val.Check(ev.Pickup))
		{
			return;
		}
		Timing.RunCoroutine(AttractPlayers(ev.Pickup));
		Rigidbody component = ev.Pickup.GameObject.GetComponent<Rigidbody>();
		if ((Object)(object)component != (Object)null)
		{
			component.freezeRotation = true;
		}
		SchematicObject val2 = ObjectSpawner.SpawnSchematic("Pickup035", ev.Pickup.Position, (Quaternion?)Quaternion.identity, (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		if (!((Object)(object)val2 == (Object)null))
		{
			GameObject gameObject = ((Component)val2).gameObject;
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider val3 in componentsInChildren)
			{
				val3.enabled = false;
			}
			gameObject.transform.parent = ev.Pickup.Transform;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}

	protected override void OnWaitingForPlayers()
	{
		_maskSchematics.Clear();
		_placedMasks.Clear();
		_processedPickups.Clear();
		_playerSchematics.Clear();
		ChangedPlayers.Clear();
		StopRagdollsList.Clear();
		((CustomItem)this).OnWaitingForPlayers();
	}

	private void OnPickingUpItem(PickingUpItemEventArgs ev)
	{
		if (!((CustomItem)this).Check(ev.Pickup) || ChangedPlayers.Contains(ev.Player))
		{
			if (ChangedPlayers.Contains(ev.Player))
			{
				ev.IsAllowed = false;
			}
			return;
		}
		if (!cassie)
		{
			cassie = true;
			CassieRussianHelper.Message("<b><size=21><color=#8B0000>ВНИМАНИЕ</color> | <color=#78020c>[Класс: Кетер]</color> SCP-035</size></b>\n<split><b><size=20>Обнаружен прорыв содержания объекта <color=#ba1818>SCP-035</color>.</size></b>\n<split><b><size=20>Всему персоналу с <color=orange>3 уровнем доступа</color> немедленно проследовать в <color=green>безопасную зону</color>.</size></b>\n<split><b><size=20>Избегайте шума и не привлекайте внимание объекта.</size></b>", "pitch_0.34 .g4 .g4 .g3 . pitch_0.66 .g1 .g1 pitch_0.9 detected containment breach keter class scp 0 3 5 yield_2 all personnel with level 3 access proceed to safe location and avoid attracting attention yield_1 pitch_0.76 .g6 .g6 pitch_0.53 .g2", isHeld: false, isNoisy: false);
		}
		if (!_processedPickups.Contains(ev.Pickup.Serial))
		{
			_processedPickups.Add(ev.Pickup.Serial);
			SchematicObject val = ObjectSpawner.SpawnSchematic("Mask035", ev.Pickup.Position, (Quaternion?)Quaternion.Euler(0f, 0f, 0f), (Vector3?)new Vector3(0f, 0f, 0f), (SchematicObjectDataList)null);
			GameObject gameObject = ((Component)val).gameObject;
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider val2 in componentsInChildren)
			{
				val2.enabled = false;
			}
			gameObject.transform.SetParent(ev.Player.Transform);
			_placedMasks[ev.Pickup] = (ev.Player, ev.Pickup.Position, ((Object)(object)val != (Object)null) ? ((Component)val).gameObject : null);
			AudioPlayerFactory.CreateForPlayer(ev.Player, "Whisper035", 1f, 7f);
			Timing.RunCoroutine(AttractPlayers(ev.Pickup));
		}
		ShowHint(ev.Player, "Маска шепчет твоё имя... Ты не в силах сопротивляться!");
		TransformPlayer(ev.Player, ev.Pickup);
	}

	private void OnChangedItem(ChangedItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item) && !ChangedPlayers.Contains(ev.Player))
		{
			if (!cassie)
			{
				cassie = true;
				CassieRussianHelper.Message("<b><size=21><color=#8B0000>ВНИМАНИЕ</color> | <color=#78020c>[Класс: Кетер]</color> SCP-035</size></b>\n<split><b><size=20>Обнаружен прорыв содержания объекта <color=#ba1818>SCP-035</color>.</size></b>\n<split><b><size=20>Всему персоналу с <color=orange>3 уровнем доступа</color> немедленно проследовать в <color=green>безопасную зону</color>.</size></b>\n<split><b><size=20>Избегайте шума и не привлекайте внимание объекта.</size></b>", "pitch_0.34 .g4 .g4 .g3 . pitch_0.66 .g1 .g1 pitch_0.9 detected containment breach keter class scp 0 3 5 yield_2 all personnel with level 3 access proceed to safe location and avoid attracting attention yield_1 pitch_0.76 .g6 .g6 pitch_0.53 .g2", isHeld: false, isNoisy: false);
			}
			ShowHint(ev.Player, "Маска шепчет твоё имя... Ты не в силах сопротивляться!");
			TransformPlayer(ev.Player);
		}
	}

	private void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (ChangedPlayers.Contains(ev.Player))
		{
			CleanupPlayer(ev.Player);
		}
	}

	private void OnDroppingItem(DroppingItemEventArgs ev)
	{
		if (((CustomItem)this).Check(ev.Item) && !(ev.Player == (Player)null) && ev.Item != null && ChangedPlayers.Contains(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	[IteratorStateMachine(typeof(AttractPlayers_d_83))]
	private IEnumerator<float> AttractPlayers(Pickup pickup)
	{
		return new AttractPlayers_d_83(0)
		{
			__4__this = this,
			pickup = pickup
		};
	}

	private void TransformPlayer(Player player, Pickup pickup = null)
	{
		if (ChangedPlayers.Contains(player))
		{
			return;
		}
		ChangedPlayers.Add(player);
		foreach (Item item in player.Items.ToList())
		{
			if (((CustomItem)this).Check(item))
			{
				player.RemoveItem(item, true);
			}
		}
		SchematicObject val = ObjectSpawner.SpawnSchematic("Take035", player.Position, (Quaternion?)Quaternion.identity, (Vector3?)Vector3.one, (SchematicObjectDataList)null);
		if ((Object)(object)val != (Object)null)
		{
			CullingExtensions.DestroySchematic(player, val);
			GameObject gameObject = ((Component)val).gameObject;
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider val2 in componentsInChildren)
			{
				val2.enabled = false;
			}
			gameObject.transform.SetParent(player.Transform);
			gameObject.transform.localPosition = new Vector3(0.03f, 0f, 0.135f);
			gameObject.transform.localRotation = Quaternion.Euler(0f, 203f, 0f);
			_playerSchematics[player] = ((Component)val).gameObject;
		}
		player.Health = MaxHealth;
		player.MaxHealth = MaxHealth;
		player.AddAhp(35f, 35f, 0f, 0.7f, 0f, false);
		player.Scale = ((CustomItem)this).Scale;
		player.VoiceChannel = (VoiceChatChannel)3;
		player.IsGodModeEnabled = false;
		player.CustomInfo = "<color=#C50000>" + player.Nickname + "\nSCP-035</color>";
		NicknameSync nicknameSync = player.ReferenceHub.nicknameSync;
		nicknameSync.ShownPlayerInfo = (PlayerInfoArea)(nicknameSync.ShownPlayerInfo & -58);
		CustomItem val3 = default(CustomItem);
		foreach (Item item2 in player.Items.ToList())
		{
			if (CustomItem.TryGet(item2, ref val3))
			{
				val3.Spawn(player.Position, item2, (Player)null);
				player.RemoveItem(item2, true);
			}
		}
		player.EnableEffect((EffectType)6, (byte)244, LifeDuration, false);
		ShowHint(player, "Ты — SCP-035, и твоя оболочка начинает разрушаться");
		AudioPlayerFactory.CreateForPlayer(player, "Whisper035", 1f, 7f);
		Timing.RunCoroutine(Corrosion(player), player.UserId + "-corrosion");
		if (_placedMasks.ContainsKey(pickup))
		{
			if (_maskSchematics.TryGetValue(pickup, out var value))
			{
				Object.Destroy((Object)(object)value);
			}
			if ((Object)(object)_placedMasks[pickup].Item3 != (Object)null)
			{
				Object.Destroy((Object)(object)_placedMasks[pickup].Item3);
			}
			_maskSchematics.Remove(pickup);
			_placedMasks.Remove(pickup);
		}
		pickup.Destroy();
	}

	private void OnDying(DyingEventArgs ev)
	{
		if (ev.Attacker != (Player)null && ChangedPlayers.Contains(ev.Attacker))
		{
			ev.Attacker.Heal(155f, false);
		}
		if (!(ev.Player != (Player)null) || ChangedPlayers.Contains(ev.Player))
		{
			StopRagdollsList.Add(ev.Player);
			CleanupPlayer(ev.Player);
		}
	}

	private void OnDestroying(DestroyingEventArgs ev)
	{
		if (ChangedPlayers.Contains(ev.Player))
		{
			CleanupPlayer(ev.Player);
		}
	}

	private void CleanupPlayer(Player player)
	{
		if (_playerSchematics.TryGetValue(player, out var value))
		{
			Object.Destroy((Object)(object)value);
			_playerSchematics.Remove(player);
		}
		Timing.KillCoroutines(player.UserId + "-appearance");
		Timing.KillCoroutines(player.UserId + "-corrosion");
		ChangedPlayers.Remove(player);
		StopRagdollsList.Remove(player);
		player.Scale = Vector3.one;
		player.DisableAllEffects();
		player.CustomInfo = string.Empty;
		NicknameSync nicknameSync = player.ReferenceHub.nicknameSync;
		nicknameSync.ShownPlayerInfo = (PlayerInfoArea)(nicknameSync.ShownPlayerInfo | 0x39);
		player.VoiceChannel = (VoiceChatChannel)1;
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		if (ev.Attacker != (Player)null && ChangedPlayers.Contains(ev.Attacker) && (int)ev.Player.Role.Side == 0)
		{
			ev.IsAllowed = Server.FriendlyFire || ev.Attacker.IsFriendlyFireEnabled;
		}
	}

	private void OnUsingItem(UsingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null)
		{
			if (((CustomItem)this).Check(ev.Item))
			{
				ev.IsAllowed = false;
			}
			if (ChangedPlayers.Contains(ev.Player) && HealingItems.ContainsKey(ev.Item.Type))
			{
				ev.IsAllowed = false;
				float num = HealingItems[ev.Item.Type];
				ev.Player.Heal(num, false);
				ev.Player.RemoveItem(ev.Item, true);
			}
		}
	}

	private void OnEndingRound(EndingRoundEventArgs ev)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (Player item in Player.List)
		{
			if (!(item == (Player)null))
			{
				if (ChangedPlayers.Contains(item) || (int)item.Role.Side == 0)
				{
					flag2 = true;
				}
				else if ((int)item.Role.Side == 1 || item.Role == (RoleTypeId)1)
				{
					flag = true;
				}
				if (flag2 && flag)
				{
					break;
				}
			}
		}
		ev.IsAllowed = !(flag && flag2);
		foreach (Player item2 in ChangedPlayers.ToList())
		{
			CleanupPlayer(item2);
		}
	}

	private void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
	{
		if (StopRagdollsList.Contains(ev.Player))
		{
			ev.IsAllowed = false;
		}
	}

	[IteratorStateMachine(typeof(Corrosion_d_92))]
	private IEnumerator<float> Corrosion(Player player)
	{
		return new Corrosion_d_92(0)
		{
			__4__this = this,
			player = player
		};
	}

	private void ShowHint(Player player, string message)
	{
		player.ShowMeowHint(9f, "<size=29><b><color=#61616193>|</color></size> <size=19>" + message + "</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
	}
}
