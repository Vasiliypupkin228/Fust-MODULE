using HintServiceMeow.Core.Enum;

namespace FultEngine.Module.Hud;

public class HudConfig
{
	public float UpdateInterval { get; set; } = 1f;


	public HintVerticalAlign HintVerticalAlign { get; set; } = (HintVerticalAlign)0;


	public HintAlignment HintAlignment { get; set; } = (HintAlignment)0;

}
