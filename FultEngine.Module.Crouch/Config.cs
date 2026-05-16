using UnityEngine;

namespace FultEngine.Module.Crouch;

public sealed class Config
{
	public int KeybindId { get; set; } = 9421;


	public string KeybindLabel { get; set; } = "Присесть / Встать";


	public string KeybindDescription { get; set; } = "Уменьшает размер игрока плавно и замедляет движение.";


	public KeyCode DefaultKey { get; set; } = (KeyCode)99;


	public Vector3 CrouchScale { get; set; } = new Vector3(1f, 0.72f, 1f);


	public float AnimationDuration { get; set; } = 0.18f;


	public float AnimationStep { get; set; } = 0.03f;


	public int SlownessIntensity { get; set; } = 18;


	public bool AllowScps { get; set; } = false;


	public bool ShowToggleHint { get; set; } = true;


	public float ToggleHintDuration { get; set; } = 1.2f;


	public string CrouchEnabledHint { get; set; } = "<b><color=#9fd3ff>ПРИСЕД</color></b>\n<size=20>Ты двигаешься медленнее и можешь пролезать ниже.</size>";


	public string CrouchDisabledHint { get; set; } = "<b><color=#ffd39f>СТОЙКА</color></b>\n<size=20>Ты вернулся к обычному размеру.</size>";

}
