using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class Ak15Titan : VariantWeaponBase
{
	public override uint Id { get; set; } = 408u;


	public override string Name { get; set; } = "<b><color=#f26f91>AK-15 \"Титан\"</color></b>";


	public override string Description { get; set; } = "Тяжёлый калаш под мощный добивающий огонь.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.4f;


	public override float Damage { get; set; } = 36f;


	public override byte ClipSize { get; set; } = 25;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Тяжёлый автомат под точечные добивания.";

	protected override string FeatureDescription => "Наносит +8 урона по целям ниже 50% HP.";

	protected override string AttachmentsDescription => "Heavy Stock, Scope Sight, Muzzle Brake, Standard Mag AP";

	protected override string AccentColor => "#f26f91";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (ev.Player.Health <= ev.Player.MaxHealth * 0.5f)
		{
			ev.Amount += 8f;
		}
	}

	public Ak15Titan()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
