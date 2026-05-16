using System;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class Ak103Vulkan : VariantWeaponBase
{
	public override uint Id { get; set; } = 406u;


	public override string Name { get; set; } = "<b><color=#ff7a59>AK-103 \"Вулкан\"</color></b>";


	public override string Description { get; set; } = "Агрессивный калаш с зажигательной подачей.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.27f;


	public override float Damage { get; set; } = 33f;


	public override byte ClipSize { get; set; } = 30;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Штурмовой калаш с огневым давлением.";

	protected override string FeatureDescription => "18% шанс наложить Burned на 3 секунды.";

	protected override string AttachmentsDescription => "Sound Suppressor, Laser, Standard Mag AP, Recoil Reducing Stock";

	protected override string AccentColor => "#ff7a59";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.18f)
		{
			ev.Player.EnableEffect((EffectType)6, (byte)1, 3f, false);
		}
	}

	public Ak103Vulkan()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
