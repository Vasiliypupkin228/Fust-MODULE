using Exiled.API.Features;

namespace FultEngine.API.Libraries.Cassie;

public static class CassieRussianHelper
{
	public static void Message(string russianSubtitle, string cassieText, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
	{
		Cassie.MessageTranslated(cassieText ?? string.Empty, russianSubtitle ?? string.Empty, isHeld, isNoisy, isSubtitles);
	}

	public static void MessageRu(string russianSubtitle, string cassieText, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
	{
		Message(russianSubtitle, cassieText, isHeld, isNoisy, isSubtitles);
	}

	public static string Build(string russianSubtitle, string cassieText)
	{
		if (russianSubtitle == null)
		{
			russianSubtitle = string.Empty;
		}
		if (cassieText == null)
		{
			cassieText = string.Empty;
		}
		return "Cassie: " + cassieText + "\nSubtitle: " + russianSubtitle;
	}

	public static string BuildRu(string russianSubtitle, string cassieText)
	{
		return Build(russianSubtitle, cassieText);
	}
}
