using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class E11Ranger : VariantWeaponBase
{
	public override uint Id { get; set; } = 454u;


	public override string Name { get; set; } = "<b><color=#c9d6ff>E-11 SR \"Рейнджер\"</color></b>";


	public override string Description { get; set; } = "Анти-SCP винтовка сопровождения.";


	public override ItemType Type { get; set; } = (ItemType)20;


	public override float Weight { get; set; } = 1.2f;


	public override float Damage { get; set; } = 31f;


	public override byte ClipSize { get; set; } = 30;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Винтовка сопровождения с упором в пробитие.";

	protected override string FeatureDescription => "Наносит +6 урона по SCP.";

	protected override string AttachmentsDescription => "Scope Sight, Muzzle Brake, Standard Mag AP, Extended Stock";

	protected override string AccentColor => "#c9d6ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (ev.Player.IsScp)
		{
			ev.Amount += 6f;
		}
	}

	public E11Ranger()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
