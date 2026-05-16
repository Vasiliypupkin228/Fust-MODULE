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
public sealed class Ak19Pulse : VariantWeaponBase
{
	public override uint Id { get; set; } = 409u;


	public override string Name { get; set; } = "<b><color=#7ee081>AK-19 \"Пульс\"</color></b>";


	public override string Description { get; set; } = "Современный калаш для точного давления.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.18f;


	public override float Damage { get; set; } = 30f;


	public override byte ClipSize { get; set; } = 40;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Современная версия калаша с хорошим контролем.";

	protected override string FeatureDescription => "20% шанс пометить цель эффектом Stained на 4 секунды.";

	protected override string AttachmentsDescription => "Dot Sight, Flashlight, Extended Mag FMJ, Sound Suppressor";

	protected override string AccentColor => "#7ee081";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.2f)
		{
			ev.Player.EnableEffect((EffectType)26, (byte)1, 4f, false);
		}
	}

	public Ak19Pulse()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
