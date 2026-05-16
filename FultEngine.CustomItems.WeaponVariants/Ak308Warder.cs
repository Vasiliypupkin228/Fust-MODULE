using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class Ak308Warder : VariantWeaponBase
{
	public override uint Id { get; set; } = 410u;


	public override string Name { get; set; } = "<b><color=#d6d6ff>AK-308 \"Вардер\"</color></b>";


	public override string Description { get; set; } = "Марксманский калаш против крепких целей.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.38f;


	public override float Damage { get; set; } = 35f;


	public override byte ClipSize { get; set; } = 20;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Марксманская версия калаша для жёстких попаданий.";

	protected override string FeatureDescription => "Наносит +6 урона по SCP; по остальным +3 урона на 20+ метров.";

	protected override string AttachmentsDescription => "Scope Sight, Heavy Stock, Sound Suppressor, Standard Mag AP";

	protected override string AccentColor => "#d6d6ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (ev.Player.IsScp)
		{
			ev.Amount += 6f;
		}
		else if (VariantWeaponBase.Distance(ev.Attacker, ev.Player) >= 20f)
		{
			ev.Amount += 3f;
		}
	}

	public Ak308Warder()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
