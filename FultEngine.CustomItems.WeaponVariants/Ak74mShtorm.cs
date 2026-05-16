using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class Ak74mShtorm : VariantWeaponBase
{
	public override uint Id { get; set; } = 402u;


	public override string Name { get; set; } = "<b><color=#51a8ff>AK-74M \"Шторм\"</color></b>";


	public override string Description { get; set; } = "Стабильный калаш под дальнюю стрельбу.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.15f;


	public override float Damage { get; set; } = 29f;


	public override byte ClipSize { get; set; } = 30;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Сбалансированный автомат для средней и дальней дистанции.";

	protected override string FeatureDescription => "При стрельбе дальше 18 метров наносит +4 урона.";

	protected override string AttachmentsDescription => "Dot Sight, Flashlight, Lightweight Stock, Standard Mag FMJ";

	protected override string AccentColor => "#51a8ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (VariantWeaponBase.Distance(ev.Attacker, ev.Player) >= 18f)
		{
			ev.Amount += 4f;
		}
	}

	public Ak74mShtorm()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
