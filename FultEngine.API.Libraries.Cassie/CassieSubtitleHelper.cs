using Exiled.API.Features;

namespace FultEngine.API.Libraries.Cassie;

public static class CassieSubtitleHelper
{
	public static void Message(string subtitleRichText, string cassieText, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
	{
		Cassie.MessageTranslated(cassieText ?? string.Empty, subtitleRichText ?? string.Empty, isHeld, isNoisy, isSubtitles);
	}

	public static string Build(string subtitleRichText, string cassieText)
	{
		if (subtitleRichText == null)
		{
			subtitleRichText = string.Empty;
		}
		if (cassieText == null)
		{
			cassieText = string.Empty;
		}
		return "Cassie: " + cassieText + "\nSubtitle: " + subtitleRichText;
	}
}
