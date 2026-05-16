using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class AkAlphaPhantom : VariantWeaponBase
{
	public override uint Id { get; set; } = 404u;


	public override string Name { get; set; } = "<b><color=#b98cff>AK Alpha \"Фантом\"</color></b>";


	public override string Description { get; set; } = "Элитный антиброневой калаш.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.28f;


	public override float Damage { get; set; } = 32f;


	public override byte ClipSize { get; set; } = 45;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Элитная штурмовая версия с упором в пробитие.";

	protected override string FeatureDescription => "Наносит +7 урона целям с бронёй или AHP.";

	protected override string AttachmentsDescription => "Scope Sight, Sound Suppressor, Laser, Drum Mag AP, Heavy Stock";

	protected override string AccentColor => "#b98cff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (VariantWeaponBase.IsTargetArmored(ev.Player))
		{
			ev.Amount += 7f;
		}
	}

	public AkAlphaPhantom()
	{
		AttachmentName[] array = new AttachmentName[5];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
