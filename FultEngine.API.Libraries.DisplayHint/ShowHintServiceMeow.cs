using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using MEC;

namespace FultEngine.API.Libraries.DisplayHint;

public static class ShowHintServiceMeow
{
	private class HintInfo
	{
		public DateTime CreationTime { get; set; }

		public float DisplayDuration { get; set; }

		public HintInfo(DateTime creationTime, float displayDuration)
		{
			CreationTime = creationTime;
			DisplayDuration = displayDuration;
		}

		public bool HasExpired()
		{
			return (DateTime.UtcNow - CreationTime).TotalSeconds >= (double)DisplayDuration;
		}
	}

	private static readonly Dictionary<Hint, HintInfo> _hintCreationTimes = new Dictionary<Hint, HintInfo>();

	private const int Y_OFFSET = 23;

	public static void ShowMeowHint(this Player ply, float time, string message, HintVerticalAlign Verticalalign = 0, int ycoordinate = 725, int xcoordinate = 0, HintAlignment aligmenthint = 2, int fontsize = 27, bool priority = false)
	{
		if (ply == (Player)null)
		{
			return;
		}
		PlayerDisplay playerDisplay = PlayerDisplay.Get(ply);
		foreach (KeyValuePair<Hint, HintInfo> item in _hintCreationTimes.ToList())
		{
			Hint hint = item.Key;
			HintInfo value = item.Value;
			if (value.HasExpired() || !playerDisplay.GetHints().Any((AbstractHint h) => h.Guid == ((AbstractHint)hint).Guid))
			{
				_hintCreationTimes.Remove(hint);
			}
		}
		Hint newHint = new Hint
		{
			Text = message,
			YCoordinateAlign = Verticalalign,
			YCoordinate = ycoordinate,
			XCoordinate = xcoordinate,
			Alignment = aligmenthint,
			FontSize = fontsize
		};
		Hint val = playerDisplay.GetHints().OfType<Hint>().FirstOrDefault((Func<Hint, bool>)((Hint h) => h.YCoordinateAlign == newHint.YCoordinateAlign && h.YCoordinate == newHint.YCoordinate && h.XCoordinate == newHint.XCoordinate && h.Alignment == newHint.Alignment));
		if (val != null)
		{
			if (priority)
			{
				Hint obj = newHint;
				obj.YCoordinate += 23f;
			}
			else
			{
				playerDisplay.RemoveHint((AbstractHint)(object)val);
				_hintCreationTimes.Remove(val);
			}
		}
		playerDisplay.AddHint((AbstractHint)(object)newHint);
		Timing.CallDelayed(time, (Action)delegate
		{
			playerDisplay.RemoveHint((AbstractHint)(object)newHint);
			_hintCreationTimes.Remove(newHint);
		});
		_hintCreationTimes[newHint] = new HintInfo(DateTime.UtcNow, time);
	}
}
