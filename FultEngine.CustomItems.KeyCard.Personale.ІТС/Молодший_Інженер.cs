using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems.KeyCard.Personale.ІТС;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Молодший_Інженер : CustomKeycard
{
	public override uint Id { get; set; } = 1012u;


	public override string Name { get; set; } = "<color=#FF8C00><b>Карта Младшего Инженера</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Младший Инженер";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32(byte.MaxValue, (byte)140, (byte)0, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32((byte)218, (byte)165, (byte)32, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32((byte)244, (byte)164, (byte)96, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)209;

}
