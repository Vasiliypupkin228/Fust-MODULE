using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class E11Sentinel : VariantWeaponBase
{
	public override uint Id { get; set; } = 451u;


	public override string Name { get; set; } = "<b><color=#58c4ff>E-11 SR \"Сентинел\"</color></b>";


	public override string Description { get; set; } = "Точная штурмовая винтовка для дальних линий.";


	public override ItemType Type { get; set; } = (ItemType)20;


	public override float Weight { get; set; } = 1.18f;


	public override float Damage { get; set; } = 30f;


	public override byte ClipSize { get; set; } = 35;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Точная штурмовая винтовка для дисциплинированной стрельбы.";

	protected override string FeatureDescription => "На дистанции 22+ метров наносит +5 урона.";

	protected override string AttachmentsDescription => "Scope Sight, Sound Suppressor, Lightweight Stock, Standard Mag FMJ";

	protected override string AccentColor => "#58c4ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (VariantWeaponBase.Distance(ev.Attacker, ev.Player) >= 22f)
		{
			ev.Amount += 5f;
		}
	}

	public E11Sentinel()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
