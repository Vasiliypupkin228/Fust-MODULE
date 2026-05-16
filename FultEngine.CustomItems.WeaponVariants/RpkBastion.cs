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
public sealed class RpkBastion : VariantWeaponBase
{
	public override uint Id { get; set; } = 405u;


	public override string Name { get; set; } = "<b><color=#f0c75e>RPK-74 \"Бастион\"</color></b>";


	public override string Description { get; set; } = "Тяжёлая поддержка на базе калаша.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.45f;


	public override float Damage { get; set; } = 30f;


	public override byte ClipSize { get; set; } = 60;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Подавляющий калаш для удержания коридоров.";

	protected override string FeatureDescription => "18% шанс наложить Deafened на 4 секунды.";

	protected override string AttachmentsDescription => "Heavy Stock, Muzzle Brake, Drum Mag FMJ, Flashlight";

	protected override string AccentColor => "#f0c75e";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.18f)
		{
			ev.Player.EnableEffect((EffectType)9, 4f, false);
		}
	}

	public RpkBastion()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
