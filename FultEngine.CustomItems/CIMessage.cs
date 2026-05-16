using Exiled.API.Features;
using FultEngine.API.Libraries.DisplayHint;
using HintServiceMeow.Core.Enum;

namespace FultEngine.CustomItems;

public static class CIMessage
{
	public static void SendMessage(Player player, string message, float duration = 5f)
	{
		if (!(player == (Player)null) && !string.IsNullOrEmpty(message))
		{
			player.ShowMeowHint(duration, "</b><size=95><color=#61616193>|</color></size>", (HintVerticalAlign)0, 828, -215, (HintAlignment)0);
			player.ShowMeowHint(duration, "</b><size=77><color=#61616193>\ud83d\udce6</size></color>", (HintVerticalAlign)0, 839, -319, (HintAlignment)0);
			player.ShowMeowHint(duration, message, (HintVerticalAlign)0, 850, -191, (HintAlignment)0);
		}
	}
}
