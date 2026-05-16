namespace FultEngine.Module.AdvancedInfo;

public class Config
{
	public bool IsEnabled { get; set; } = true;


	public string DefaultInfoFormat { get; set; } = "<size=15><b><color=#FFFFFF>\"{Role}\"</color></b></size>\n<size=11><color=#FFFFFF>「 ID: {PlayerId} | Псевдоним: {Nickname}  」 </color></size>\n<size=15><color=#FFFFFF>――――――――――――――――――――</color></size>\n";


	public string AdminInfoFormat { get; set; } = "<size=15><b><color=#FFFFFF>\"{Role}\"</color></b></size>\n<size=11><color=#FFFFFF>「 ID: {PlayerId} | Псевдоним: {Nickname}  」 </color></size>\n<size=9><color=#FFFFFF>「 {CustomInfo} 」 </color></size>\n<size=15><color=#FFFFFF>――――――――――――――――――――</color></size>\n";


	public string AdminRoleFormat { get; set; } = "<size=15><b><color=#FFFFFF>\"{Role}\"</color></b></size>\n<size=11><color=#FFFFFF>「 ID: {PlayerId} | Псевдоним: {Nickname}  」 </color></size>\n<size=9><color=#FFFFFF>「 {CustomInfo} 」 </color></size>\n<size=15><color=#FFFFFF>――――――――――――――――――――</color></size>\n";


	public float ShowHintDuration { get; set; } = 5f;


	public bool EnableCommands { get; set; } = true;

}
