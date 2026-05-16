using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp106;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.EventArgs.Scp3114;
using Exiled.Events.EventArgs.Scp330;
using Exiled.Events.EventArgs.Scp939;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using Interactables.Interobjects;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using UnityEngine;

namespace FultEngine.Module.BetterSCP;

public class Plugin : IFultEngineModule
{
	[CompilerGenerated]
	private sealed class UpdateEnergy_d_44 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public Plugin __4__this;

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
		public UpdateEnergy_d_44(int __1__state)
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
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if ((Role)(object)__4__this._scp079 != (Role)null)
			{
				__4__this._scp079.Energy = 200f;
			}
			__2__current = Timing.WaitForSeconds(0.1f);
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

	private Scp079Role _scp079;

	private Scp106Role _scp106;

	private CoroutineHandle _energyCoroutine;

	public string Name => "BetterSCP";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 1);

	public static Plugin Instance { get; private set; }

	public void OnEnabled()
	{
		Instance = this;
		RegisterEvents();
		_energyCoroutine = Timing.RunCoroutine(UpdateEnergy());
	}

	public void OnDisabled()
	{
		UnregisterEvents();
		if (((CoroutineHandle)(ref _energyCoroutine)).IsRunning)
		{
			Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { _energyCoroutine });
		}
		Instance = null;
	}

	private void RegisterEvents()
	{
		Server.WaitingForPlayers += new CustomEventHandler(OnWaitingForPlayers);
		Scp330.EatenScp330 += (CustomEventHandler<EatenScp330EventArgs>)OnEatenScp330;
		Server.RespawningTeam += (CustomEventHandler<RespawningTeamEventArgs>)OnRespawningTeam;
		Map.AnnouncingScpTermination += (CustomEventHandler<AnnouncingScpTerminationEventArgs>)OnAnnouncingScpTermination;
		Player.ReceivingEffect += (CustomEventHandler<ReceivingEffectEventArgs>)OnReceivingEffect;
		Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Jumping += (CustomEventHandler<JumpingEventArgs>)OnJumping;
		Player.Hurting += (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.VoiceChatting += (CustomEventHandler<VoiceChattingEventArgs>)OnUsingMicrophone;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.InteractingLocker += (CustomEventHandler<InteractingLockerEventArgs>)OnInteractingLocker;
		Player.InteractingElevator += (CustomEventHandler<InteractingElevatorEventArgs>)OnInteractingElevator;
		Player.EscapingPocketDimension += (CustomEventHandler<EscapingPocketDimensionEventArgs>)OnEscapingPocketDimension;
		Player.FailingEscapePocketDimension += (CustomEventHandler<FailingEscapePocketDimensionEventArgs>)OnFailingEscapePocketDimension;
		Player.TriggeringTesla += (CustomEventHandler<TriggeringTeslaEventArgs>)OnTriggeringTesla;
		Scp049.Attacking += (CustomEventHandler<AttackingEventArgs>)OnAttacking049;
		Scp079.Pinging += (CustomEventHandler<PingingEventArgs>)OnPinging;
		Scp096.Enraging += (CustomEventHandler<EnragingEventArgs>)OnEnraging;
		Scp096.CalmingDown += (CustomEventHandler<CalmingDownEventArgs>)OnCalmingDown;
		Scp106.Attacking += (CustomEventHandler<AttackingEventArgs>)OnAttacking106;
		Scp173.PlacingTantrum += (CustomEventHandler<PlacingTantrumEventArgs>)OnPlacingTantrum;
		Scp173.UsingBreakneckSpeeds += (CustomEventHandler<UsingBreakneckSpeedsEventArgs>)OnUsingBreakneckSpeeds;
		Scp939.PlacingAmnesticCloud += (CustomEventHandler<PlacingAmnesticCloudEventArgs>)OnPlacingAmnesticCloud;
		Scp3114.Disguising += (CustomEventHandler<DisguisingEventArgs>)OnDisguising;
	}

	private void UnregisterEvents()
	{
		Server.WaitingForPlayers -= new CustomEventHandler(OnWaitingForPlayers);
		Scp330.EatenScp330 -= (CustomEventHandler<EatenScp330EventArgs>)OnEatenScp330;
		Server.RespawningTeam -= (CustomEventHandler<RespawningTeamEventArgs>)OnRespawningTeam;
		Map.AnnouncingScpTermination -= (CustomEventHandler<AnnouncingScpTerminationEventArgs>)OnAnnouncingScpTermination;
		Player.ReceivingEffect -= (CustomEventHandler<ReceivingEffectEventArgs>)OnReceivingEffect;
		Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
		Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
		Player.Jumping -= (CustomEventHandler<JumpingEventArgs>)OnJumping;
		Player.Hurting -= (CustomEventHandler<HurtingEventArgs>)OnHurting;
		Player.VoiceChatting -= (CustomEventHandler<VoiceChattingEventArgs>)OnUsingMicrophone;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
		Player.InteractingLocker -= (CustomEventHandler<InteractingLockerEventArgs>)OnInteractingLocker;
		Player.InteractingElevator -= (CustomEventHandler<InteractingElevatorEventArgs>)OnInteractingElevator;
		Player.EscapingPocketDimension -= (CustomEventHandler<EscapingPocketDimensionEventArgs>)OnEscapingPocketDimension;
		Player.FailingEscapePocketDimension -= (CustomEventHandler<FailingEscapePocketDimensionEventArgs>)OnFailingEscapePocketDimension;
		Player.TriggeringTesla -= (CustomEventHandler<TriggeringTeslaEventArgs>)OnTriggeringTesla;
		Scp049.Attacking -= (CustomEventHandler<AttackingEventArgs>)OnAttacking049;
		Scp079.Pinging -= (CustomEventHandler<PingingEventArgs>)OnPinging;
		Scp096.Enraging -= (CustomEventHandler<EnragingEventArgs>)OnEnraging;
		Scp096.CalmingDown -= (CustomEventHandler<CalmingDownEventArgs>)OnCalmingDown;
		Scp106.Attacking -= (CustomEventHandler<AttackingEventArgs>)OnAttacking106;
		Scp173.PlacingTantrum -= (CustomEventHandler<PlacingTantrumEventArgs>)OnPlacingTantrum;
		Scp173.UsingBreakneckSpeeds -= (CustomEventHandler<UsingBreakneckSpeedsEventArgs>)OnUsingBreakneckSpeeds;
		Scp939.PlacingAmnesticCloud -= (CustomEventHandler<PlacingAmnesticCloudEventArgs>)OnPlacingAmnesticCloud;
		Scp3114.Disguising -= (CustomEventHandler<DisguisingEventArgs>)OnDisguising;
	}

	private static void DisableInvisibility(Player player)
	{
		if (player != null)
		{
			player.DisableEffect((EffectType)20);
		}
	}

	private void OnWaitingForPlayers()
	{
		_scp079 = null;
		_scp106 = null;
		IzmCommand.ClearStaticState();
	}

	private void OnPlacingTantrum(PlacingTantrumEventArgs ev)
	{
		ev.IsAllowed = false;
	}

	private void OnDisguising(DisguisingEventArgs ev)
	{
		ev.Scp3114.DisguiseDuration = 5000000f;
	}

	private void OnPlacingAmnesticCloud(PlacingAmnesticCloudEventArgs ev)
	{
		ev.IsAllowed = false;
	}

	private void OnUsingBreakneckSpeeds(UsingBreakneckSpeedsEventArgs ev)
	{
		ev.IsAllowed = false;
	}

	private void OnJumping(JumpingEventArgs ev)
	{
		DisableInvisibility(ev.Player);
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		DisableInvisibility(ev.Player);
	}

	private void OnUsingMicrophone(VoiceChattingEventArgs ev)
	{
		DisableInvisibility(ev.Player);
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		DisableInvisibility(ev.Player);
	}

	private void OnInteractingLocker(InteractingLockerEventArgs ev)
	{
		DisableInvisibility(ev.Player);
	}

	private void OnInteractingElevator(InteractingElevatorEventArgs ev)
	{
		ElevatorChamber elevator = ev.Elevator;
		Type typeFromHandle = typeof(ElevatorChamber);
		SetField(elevator, typeFromHandle, "_doorOpenTime", 7f);
		SetField(elevator, typeFromHandle, "_doorCloseTime", 7f);
	}

	private static void SetField(object obj, Type type, string fieldName, float value)
	{
		type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(obj, value);
	}

	private void OnAttacking106(AttackingEventArgs ev)
	{
		if (ev.IsAllowed)
		{
			float y = ev.Target.Position.y;
			if (y >= -300f && y <= -290f)
			{
				ev.Target.Kill((DamageType)27, "");
				return;
			}
			ev.IsAllowed = false;
			ev.Target.EnableEffect((EffectType)37, 0f, false);
		}
	}

	private void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
	{
		Player player = ev.Player;
		if (player != null && (int)player.Role.Type == 3)
		{
			ev.IsAllowed = false;
			Room val = Room.List.OrderBy((Room _) => Random.value).FirstOrDefault();
			if (val != null)
			{
				ev.Player.Position = val.Position;
			}
		}
	}

	private void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
	{
		Player player = ev.Player;
		if (player != null && (int)player.Role.Type == 3)
		{
			ev.IsAllowed = false;
			Room val = Room.List.OrderBy((Room _) => Random.value).FirstOrDefault();
			if (val != null)
			{
				ev.Player.Position = val.Position;
			}
		}
	}

	private void OnDied(DiedEventArgs ev)
	{
		Player player = ev.Player;
		if (player != null && (int)player.Role.Type == 3)
		{
			_scp106 = null;
		}
		Player player2 = ev.Player;
		if (player2 != null && (int)player2.Role.Type == 7)
		{
			_scp079 = null;
		}
		Player attacker = ev.Attacker;
		if (attacker == null || (int)attacker.Role.Type != 9)
		{
			return;
		}
		Timing.CallDelayed(0.1f, (Action)delegate
		{
			Ragdoll val = ((IEnumerable<Ragdoll>)Ragdoll.List).FirstOrDefault((Func<Ragdoll, bool>)((Ragdoll r) => r.Owner == ev.Player));
			if (val != null)
			{
				val.Destroy();
			}
		});
	}

	private void OnReceivingEffect(ReceivingEffectEventArgs ev)
	{
		if ((int)ev.Player.Role.Type == 3 && (int)EffectTypeExtension.GetEffectType(ev.Effect) == 14)
		{
			Scp106Role val = ((TypeCastObject<Role>)(object)ev.Player.Role).As<Scp106Role>();
			if ((Role)(object)val != (Role)null)
			{
				val.IsSubmerged = true;
			}
		}
		else if ((int)ev.Player.Role.Type == 9 && (int)EffectTypeExtension.GetEffectType(ev.Effect) != 12 && (int)EffectTypeExtension.GetEffectType(ev.Effect) != 23)
		{
			ev.IsAllowed = false;
		}
	}

	private void OnRespawningTeam(RespawningTeamEventArgs ev)
	{
	}

	private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
	{
		ev.IsAllowed = false;
	}

	private void OnEatenScp330(EatenScp330EventArgs ev)
	{
		if (!(ev.Player == (Player)null))
		{
			ev.Player.DisableAllEffects();
			ev.Player.ArtificialHealth = 0f;
		}
	}

	private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
	{
		if ((int)ev.Player.Role.Type != 3)
		{
			return;
		}
		Scp106Role val = ((TypeCastObject<Role>)(object)ev.Player.Role).As<Scp106Role>();
		if (!((Role)(object)val != (Role)null))
		{
			return;
		}
		val.IsSubmerged = true;
		Room randomRoom = Room.List.OrderBy((Room _) => Random.value).FirstOrDefault();
		if (randomRoom != null)
		{
			Timing.CallDelayed(0.5f, (Action)delegate
			{
				ev.Player.Position = randomRoom.Position;
			});
		}
	}

	private void OnEnraging(EnragingEventArgs ev)
	{
		if (ev.IsAllowed)
		{
			ev.Player.EnableEffect((EffectType)12, 2f, false);
			ev.Player.EnableEffect((EffectType)23, (byte)175, 0f, false);
		}
	}

	private void OnCalmingDown(CalmingDownEventArgs ev)
	{
		if (ev.IsAllowed)
		{
			ev.Player.DisableEffect((EffectType)23);
		}
	}

	private void OnAttacking049(AttackingEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && !(ev.Target == (Player)null))
		{
			ev.Target.Kill((DamageType)22, "");
		}
	}

	private void OnSpawned(SpawnedEventArgs ev)
	{
		if (ev.Player == (Player)null)
		{
			return;
		}
		RoleTypeId type = ev.Player.Role.Type;
		RoleTypeId val = type;
		if ((int)val <= 9)
		{
			if ((int)val != 0)
			{
				switch (val - 3)
				{
				case 2:
					ev.Player.EnableEffect((EffectType)23, (byte)8, 0f, false);
					ev.Player.MaxHealth = 500f;
					ev.Player.Health = 500f;
					ev.Player.MaxHumeShield = 0f;
					ev.Player.HumeShield = 0f;
					break;
				case 6:
					ev.Player.MaxHealth = 50000f;
					ev.Player.Health = 50000f;
					ev.Player.MaxHumeShield = 0f;
					ev.Player.HumeShield = 0f;
					break;
				case 0:
					ev.Player.MaxHealth = 50000f;
					ev.Player.Health = 50000f;
					ev.Player.MaxHumeShield = 0f;
					ev.Player.HumeShield = 0f;
					ev.Player.EnableEffect((EffectType)37, 0f, false);
					_scp106 = ((TypeCastObject<Role>)(object)ev.Player.Role).As<Scp106Role>();
					Timing.CallDelayed(1f, (Action)delegate
					{
						ev.Player.EnableEffect((EffectType)41, 0f, false);
						ev.Player.DisableEffect((EffectType)37);
					});
					break;
				case 4:
				{
					_scp079 = ((TypeCastObject<Role>)(object)ev.Player.Role).As<Scp079Role>();
					Scp079Role scp = _scp079;
					if (scp != null)
					{
						scp.AddExperience(430, (Scp079HudTranslation)45);
					}
					break;
				}
				case 1:
				case 3:
				case 5:
					break;
				}
			}
			else
			{
				ev.Player.MaxHealth = 5000f;
				ev.Player.Health = 5000f;
				ev.Player.MaxHumeShield = 0f;
				ev.Player.HumeShield = 0f;
				ev.Player.EnableEffect((EffectType)23, (byte)95, 0f, false);
				ev.Player.Position = new Vector3(67.004f, 112.428f, 137.996f);
			}
		}
		else if ((int)val != 16)
		{
			if ((int)val == 23)
			{
				ev.Player.MaxHealth = 2000f;
				ev.Player.Health = 2000f;
				ev.Player.MaxHumeShield = 0f;
				ev.Player.HumeShield = 0f;
			}
		}
		else
		{
			ev.Player.MaxHealth = 4500f;
			ev.Player.Health = 4500f;
			ev.Player.MaxHumeShield = 0f;
			ev.Player.HumeShield = 0f;
		}
	}

	private void OnPinging(PingingEventArgs ev)
	{
		ev.IsAllowed = false;
	}

	[IteratorStateMachine(typeof(UpdateEnergy_d_44))]
	private IEnumerator<float> UpdateEnergy()
	{
		return new UpdateEnergy_d_44(0)
		{
			__4__this = this
		};
	}

	public Type GetConfigType()
	{
		return typeof(SubClassConfig);
	}

	public void SetConfig(object config)
	{
	}

	public object GetDefaultConfig()
	{
		return new { };
	}
}
