using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class FrmgKraken : VariantWeaponBase
{
	public override uint Id { get; set; } = 457u;


	public override string Name { get; set; } = "<b><color=#bf9cff>FR-MG-0 \"Кракен\"</color></b>";


	public override string Description { get; set; } = "Антиброневой вариант платформы FR-MG-0.";


	public override ItemType Type { get; set; } = (ItemType)52;


	public override float Weight { get; set; } = 1.36f;


	public override float Damage { get; set; } = 32f;


	public override byte ClipSize { get; set; } = 35;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Силовой вариант для целей с бронёй и AHP.";

	protected override string FeatureDescription => "Наносит +7 урона по целям с бронёй или AHP.";

	protected override string AttachmentsDescription => "Scope Sight, Sound Suppressor, Standard Mag AP, Heavy Stock";

	protected override string AccentColor => "#bf9cff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (VariantWeaponBase.IsTargetArmored(ev.Player))
		{
			ev.Amount += 7f;
		}
	}

	public FrmgKraken()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
