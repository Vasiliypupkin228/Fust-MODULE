using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace CustomItems.Items;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Зам_Директора_Комплекса : CustomKeycard
{
	public override uint Id { get; set; } = 1026u;


	public override string Name { get; set; } = "<color=#520d0d><b>Зам. Директора Комплекса</b></color>";


	public override float Weight { get; set; } = 0f;


	public override string Description { get; set; } = "";


	public override SpawnProperties SpawnProperties { get; set; }

	public override string KeycardLabel { get; set; } = "Зам. Директора Комплекса";


	public override string KeycardName { get; set; } = "";


	public override Color32? KeycardPermissionsColor { get; set; } = new Color32((byte)82, (byte)13, (byte)13, byte.MaxValue);


	public override Color32? TintColor { get; set; } = new Color32((byte)178, (byte)34, (byte)34, byte.MaxValue);


	public override Color32? KeycardLabelColor { get; set; } = new Color32(byte.MaxValue, (byte)160, (byte)122, byte.MaxValue);


	public override KeycardPermissions Permissions { get; set; } = (KeycardPermissions)248;

}
