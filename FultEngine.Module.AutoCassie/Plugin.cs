using System;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.API.Libraries.Audio;
using FultEngine.API.Libraries.Cassie;
using FultEngine.LoaderModule;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace FultEngine.Module.AutoCassie;

public class Plugin : IFultEngineModule
{
	private Config _config;

	public string Name { get; } = "AutoCassie";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(0, 0, 5);


	public void OnEnabled()
	{
		Server.RespawnedTeam += (CustomEventHandler<RespawnedTeamEventArgs>)OnRespawnedTeam;
		Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
		Warhead.DeadmanSwitchInitiating += (CustomEventHandler<DeadmanSwitchInitiatingEventArgs>)OnDeadmanSwitchInitiating;
	}

	public void OnDisabled()
	{
		Server.RespawnedTeam -= (CustomEventHandler<RespawnedTeamEventArgs>)OnRespawnedTeam;
		Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
		Warhead.DeadmanSwitchInitiating -= (CustomEventHandler<DeadmanSwitchInitiatingEventArgs>)OnDeadmanSwitchInitiating;
	}

	private void OnRespawnedTeam(RespawnedTeamEventArgs ev)
	{
		if (((ev != null) ? ev.Players : null) == null || !ev.Players.Any())
		{
			return;
		}
		Cassie.Clear();
		Player val = ev.Players.FirstOrDefault();
		if (val == (Player)null)
		{
			return;
		}
		Team team = val.Role.Team;
		if ((int)team == 1)
		{
			CassieRussianHelper.MessageRu("<b><size=21><color=#8B8B05>ВНИМАНИЕ</color> | Всему выжившему персоналу:</size></b>\n<split><b><size=20><color=#00008B>МОГ Эпсилон-11 «Девятихвостая лисица» вошла в комплекс.</color></size></b>\n<split><b><size=20>Начата операция по восстановлению условий содержания <color=#9A1010>SCP-объектов</color>.</size></b>", "bell_start . pitch_0.98 attention to all personnel . mtfunit epsilon 11 designated ninetailedfox hasentered for starting operation by recontainment scpsubjects . bell_end");
			return;
		}
		ChangeLightsColor(255, 111, 111);
		AudioManager.CreateGlobalAudio("alert");
		Timing.CallDelayed(9f, (Action)delegate
		{
			AudioManager.DestroyForGlobal("alert");
			CassieRussianHelper.MessageRu("<b><size=21><color=#8B0000>ТРЕВОГА</color> | Всему персоналу:</size></b>\n<split><b><size=20>Зона атакована неизвестным вооружённым формированием.</size></b>\n<split><b><size=20>Код <color=#6ccc23>«Белый»</color> активирован, гермозатворы заблокированы.</size></b>\n<split><b><size=20>Немедленно пройдите в ближайшее безопасное место и ожидайте зачистки комплекса.</size></b>", "attention to all personnel . . zone was attacked by unknown military unit . code white has been activated yd_1 and gates was lockdownd . . personnel are advised to report in the nearest safe location and wait until the facility have been secured from all possible threats . pitch_0.8 .g5 .g5 pitch_0.3 .g5");
			Timing.CallDelayed(15f, (Action)Map.ResetLightsColor);
		});
	}

	public static void ChangeLightsColor(int r, int g, int b)
	{
		Color networkOverrideColor = default(Color);
		((Color)(ref networkOverrideColor))._002Ector((float)r / 255f, (float)g / 255f, (float)b / 255f);
		foreach (RoomLightController instance in RoomLightController.Instances)
		{
			instance.NetworkOverrideColor = networkOverrideColor;
		}
	}

	private void OnRoundStarted()
	{
		CassieRussianHelper.MessageRu("<b><size=26><color=yellow>Сообщение для всего персонала</color></size></b>\n<split><b><size=21>Все <color=#FF4500>системы</color> работают в <color=green>штатном режиме</color>.</size></b>\n<split><b><size=21>Все ваши <color=yellow>задачи</color> вы получите от ваших <color=red>руководителей</color>.</size></b>\n<split><b><size=21><color=#006400>Удачи и хорошего дня!</color></size></b>", "pitch_2 .g1 .g1 pitch_0.98 message from personnel yield_1 all system do it in normal mode .g1 yield_1 all your task you will get from your managers yield_1 good luck and nice day .g1 .g3");
	}

	private void OnDeadmanSwitchInitiating(DeadmanSwitchInitiatingEventArgs ev)
	{
		ev.IsAllowed = false;
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
		_config = (Config)config;
	}
}
