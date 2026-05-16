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
public sealed class FrmgAtlas : VariantWeaponBase
{
	public override uint Id { get; set; } = 456u;


	public override string Name { get; set; } = "<b><color=#f4c26b>FR-MG-0 \"Атлас\"</color></b>";


	public override string Description { get; set; } = "Тяжёлая подавляющая винтовка.";


	public override ItemType Type { get; set; } = (ItemType)52;


	public override float Weight { get; set; } = 1.4f;


	public override float Damage { get; set; } = 34f;


	public override byte ClipSize { get; set; } = 40;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Подавляющая винтовка для медленного продавливания.";

	protected override string FeatureDescription => "20% шанс наложить Deafened и Concussed.";

	protected override string AttachmentsDescription => "Holo Sight, Heavy Stock, Drum Mag FMJ, Muzzle Brake";

	protected override string AccentColor => "#f4c26b";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.2f)
		{
			ev.Player.EnableEffect((EffectType)9, 4f, false);
			ev.Player.EnableEffect((EffectType)7, (byte)1, 1.2f, false);
		}
	}

	public FrmgAtlas()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
