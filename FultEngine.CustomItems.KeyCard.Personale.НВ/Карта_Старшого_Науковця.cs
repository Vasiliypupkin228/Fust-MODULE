using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems.KeyCard.Personale.НВ;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Карта_Старшого_Науковця : CustomKeycard
{
	public override uint Id { get; set; } = 1009u;


	public override string Name { get; set; } = "<color=#ff8c00><b>Карта Старшего Учёного </b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Старший Учёный";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32(byte.MaxValue, (byte)140, (byte)0, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32(byte.MaxValue, (byte)165, (byte)0, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32((byte)219, (byte)201, (byte)39, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)68;

}
