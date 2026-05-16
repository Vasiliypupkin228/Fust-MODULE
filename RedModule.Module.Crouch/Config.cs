using UnityEngine;

namespace RedModule.Module.Crouch;

public class Config
{
	public bool IsEnabled { get; set; } = true;


	public bool HumansOnly { get; set; } = true;


	public int KeybindId { get; set; } = 91;


	public KeyCode DefaultKey { get; set; } = (KeyCode)99;


	public string KeybindLabel { get; set; } = "╔ <color=#89d6ff>\ud83e\uddb4</color> Присесть / Встать";


	public string KeybindHintDescription { get; set; } = "Плавно уменьшает размер игрока";


	public Vector3 CrouchScale { get; set; } = new Vector3(1f, 0.68f, 1f);


	public float AnimationDuration { get; set; } = 0.25f;


	public int SlownessIntensity { get; set; } = 20;


	public bool ShowStateHint { get; set; } = true;


	public float StateHintDuration { get; set; } = 2.2f;

}
