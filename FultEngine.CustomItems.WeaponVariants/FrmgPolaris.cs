using System;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class FrmgPolaris : VariantWeaponBase
{
	public override uint Id { get; set; } = 455u;


	public override string Name { get; set; } = "<b><color=#7fd1ff>FR-MG-0 \"Полярис\"</color></b>";


	public override string Description { get; set; } = "Точная тяжёлая винтовка с хорошим контролем.";


	public override ItemType Type { get; set; } = (ItemType)52;


	public override float Weight { get; set; } = 1.32f;


	public override float Damage { get; set; } = 33f;


	public override byte ClipSize { get; set; } = 30;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Тяжёлая точная винтовка для удержания дальней линии.";

	protected override string FeatureDescription => "На дистанции 20+ метров наносит +4 урона и кратко помечает цель.";

	protected override string AttachmentsDescription => "Scope Sight, Recoil Reducing Stock, Standard Mag FMJ, Flashlight";

	protected override string AccentColor => "#7fd1ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (VariantWeaponBase.Distance(ev.Attacker, ev.Player) >= 20f)
		{
			ev.Amount += 4f;
			ev.Player.EnableEffect((EffectType)26, (byte)1, 2f, false);
		}
	}

	public FrmgPolaris()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
