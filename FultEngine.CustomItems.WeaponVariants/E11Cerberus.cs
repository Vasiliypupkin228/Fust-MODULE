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
public sealed class E11Cerberus : VariantWeaponBase
{
	public override uint Id { get; set; } = 452u;


	public override string Name { get; set; } = "<b><color=#ffb347>E-11 SR \"Цербер\"</color></b>";


	public override string Description { get; set; } = "Штурмовая винтовка подавляющего типа.";


	public override ItemType Type { get; set; } = (ItemType)20;


	public override float Weight { get; set; } = 1.22f;


	public override float Damage { get; set; } = 31f;


	public override byte ClipSize { get; set; } = 35;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Штурмовая винтовка для удержания направления.";

	protected override string FeatureDescription => "16% шанс наложить Slowness и Concussed.";

	protected override string AttachmentsDescription => "Holo Sight, Muzzle Brake, Recoil Reducing Stock, Extended Mag FMJ";

	protected override string AccentColor => "#ffb347";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.16f)
		{
			ev.Player.EnableEffect((EffectType)43, (byte)15, 1.6f, false);
			ev.Player.EnableEffect((EffectType)7, (byte)1, 1.1f, false);
		}
	}

	public E11Cerberus()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
