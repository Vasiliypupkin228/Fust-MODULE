using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace CustomItems.Items;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Капрал_ВБ : CustomKeycard
{
	public override uint Id { get; set; } = 1006u;


	public override string Name { get; set; } = "<color=#292929><b>Капрал ВБ</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Капрал Отдела Безопасности";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32((byte)41, (byte)41, (byte)41, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32((byte)41, (byte)41, (byte)41, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32((byte)0, (byte)0, (byte)0, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)145;

}
