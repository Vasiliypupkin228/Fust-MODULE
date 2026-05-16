using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems.KeyCard.Personale.ІТС;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Керівник : CustomKeycard
{
	public override uint Id { get; set; } = 1011u;


	public override string Name { get; set; } = "<color=#FF8C00><b>Карта Руководителя И.Т.С</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Руководитель И.Т.С";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32(byte.MaxValue, (byte)140, (byte)0, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32((byte)184, (byte)134, (byte)11, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32(byte.MaxValue, (byte)222, (byte)173, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)209;

}
