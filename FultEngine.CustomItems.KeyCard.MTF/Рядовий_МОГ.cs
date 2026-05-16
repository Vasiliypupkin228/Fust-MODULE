using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems.KeyCard.MTF;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Рядовий_МОГ : CustomKeycard
{
	public override uint Id { get; set; } = 1019u;


	public override string Name { get; set; } = "<color=#00FFFF><b>Карта Рядового МОГ</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Рядовой МОГ";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32((byte)100, (byte)100, (byte)100, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32((byte)0, (byte)0, byte.MaxValue, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32((byte)0, byte.MaxValue, byte.MaxValue, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)292;

}
