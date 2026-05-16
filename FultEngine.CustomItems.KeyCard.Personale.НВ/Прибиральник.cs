using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems.KeyCard.Personale.НВ;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Прибиральник : CustomKeycard
{
	public override uint Id { get; set; } = 1016u;


	public override string Name { get; set; } = "<color=#ff8c00><b>Карта Уборщика</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Уборщик";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32((byte)169, (byte)59, (byte)237, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32((byte)59, (byte)102, (byte)212, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)0;

}
