using System;
using Exiled.API.Features;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;
using UnityEngine;

namespace FultEngine.Module;

public class HackingUI
{
	public void ShowMessage(Player player, string message, float duration = 3f)
	{
		if (player == (Player)null)
		{
			return;
		}
		try
		{
			player.ShowMeowHint(duration, "<size=29><b><color=#61616193>|</color></size> <size=19>" + message + "</size> <size=29><color=#61616193>|</color></b></size>", (HintVerticalAlign)0, 139, 0, (HintAlignment)2);
		}
		catch (Exception ex)
		{
			Log.Error("ShowMessage: Ошибка при показе подсказки игроку " + player.Nickname + ": " + ex.Message);
		}
	}

	public void UpdateHackingUI(Player player, int currentSegment, int totalSegments)
	{
		float num = (float)(currentSegment + 1) / (float)totalSegments;
		int num2 = 18;
		int num3 = Mathf.RoundToInt(num * (float)num2);
		int count = num2 - num3;
		int num4 = Mathf.RoundToInt(num * 100f);
		string arg = "<color=#ff0000>" + new string('▒', num3) + "</color><color=#730b0b>" + new string('▒', count) + "</color>";
		string message = $"<size=25><b><color=#61616193>『</color></size> <size=21>Влом ЖОПЫ/ЖОРЫ комплекса</size> <size=25><b><color=#61616193>』</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{arg}</size> <size=29><b><color=#61616193>|</color></size>\n<size=29><b><color=#61616193>|</color></size> <size=19>{num4}%</size> <size=29><b><color=#61616193>|</color></size></b>";
		player.ShowMeowHint(1f, message, (HintVerticalAlign)1, 755, 0, (HintAlignment)2);
	}
}
