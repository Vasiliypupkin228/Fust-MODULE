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
public sealed class E11Viper : VariantWeaponBase
{
	public override uint Id { get; set; } = 453u;


	public override string Name { get; set; } = "<b><color=#77e68c>E-11 SR \"Вайпер\"</color></b>";


	public override string Description { get; set; } = "Тактическая винтовка с токсичным уроном.";


	public override ItemType Type { get; set; } = (ItemType)20;


	public override float Weight { get; set; } = 1.14f;


	public override float Damage { get; set; } = 29f;


	public override byte ClipSize { get; set; } = 40;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Манёвренная тактическая винтовка с мягкой отдачей.";

	protected override string FeatureDescription => "18% шанс наложить Poisoned на 4 секунды.";

	protected override string AttachmentsDescription => "Dot Sight, Laser, Extended Mag JHP, Sound Suppressor";

	protected override string AccentColor => "#77e68c";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.18f)
		{
			ev.Player.EnableEffect((EffectType)18, (byte)2, 4f, false);
		}
	}

	public E11Viper()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
