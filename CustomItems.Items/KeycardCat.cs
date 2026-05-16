using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace CustomItems.Items;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class KeycardCat : CustomKeycard
{
	public override uint Id { get; set; } = 1000u;


	public override string Name { get; set; } = "<color=##fff><b>__________KEYCARD__________</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "__________КАРТЫ ДОСТУПА__________";


	public override string KeycardName { get; set; } = "__________КАРТЫ ДОСТУПА__________";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)4;

}
