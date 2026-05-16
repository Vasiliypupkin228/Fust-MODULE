namespace FultEngine.Module.LockDown;

public sealed class LockDownConfig
{
	public float LockDuration { get; set; } = 60f;


	public float HintRefreshInterval { get; set; } = 1f;


	public float EffectRefreshInterval { get; set; } = 0.25f;


	public bool ApplyBlindness { get; set; } = true;


	public bool DisableMovement { get; set; } = true;


	public float FlickerDuration { get; set; } = 5f;


	public bool PlayStartCassie { get; set; } = true;


	public string StartCassieTranslationRu { get; set; } = "<b><i>||</i></b> <color=#DC143C>Внимание!</color>\n<b><i>||</i></b> Зафиксировано массовое нарушение особых условий содержания. <color=orange>Протоколы</color> <color=#ea9999>P-L-1</color> и <color=#ea9999>P-S-2</color> <color=green>активированы</color>.\n<b><size=25>Система <color=#4DFFB8>C.A.S.S.I.E.</color> входит в режим экстренных случаев. Всему персоналу оставаться на своих местах, пока <color=#003ECA>МОГ</color> не прибудет в комплекс. <color=#5B6370>Служба безопасности</color> обязана помочь персоналу и немедленно сообщить о <color=red>НОУС</color> через интерком.</size></b>";


	public string StartCassieWords { get; set; } = "pitch_0.1 .g4 .g4 .g4 pitch_0.95 Attention . . containment breach jam_040_3 detected . . . protocol P L 1 and P S 2 .g3 .g3 .g3 ACTIVATED All remaining personnel are advised to standby your current location until MTFunit has entered the facility . . Security personnel must help other personnel and report about containment breach in the intercom . . bell_end";


	public bool PlayUnlockCassie { get; set; } = true;


	public string UnlockCassieTranslationRu { get; set; } = "<b><color=#9ad0ff>Камеры SCP открыты.</color></b> Особые условия содержания завершены.";


	public string UnlockCassieWords { get; set; } = "attention . containment chambers unlocked";

}
