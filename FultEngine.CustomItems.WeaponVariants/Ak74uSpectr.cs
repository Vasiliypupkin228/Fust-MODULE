using System;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class Ak74uSpectr : VariantWeaponBase
{
	public override uint Id { get; set; } = 407u;


	public override string Name { get; set; } = "<b><color=#9fe8ff>AK-74U \"Спектр\"</color></b>";


	public override string Description { get; set; } = "Компактный калаш для быстрой агрессии.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.05f;


	public override float Damage { get; set; } = 27f;


	public override byte ClipSize { get; set; } = 35;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Компактная версия калаша для ближней дистанции.";

	protected override string FeatureDescription => "После попадания даёт стрелку краткий MovementBoost.";

	protected override string AttachmentsDescription => "No Rifle Stock, Holo Sight, Flashlight, Extended Mag JHP, Laser";

	protected override string AccentColor => "#9fe8ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		ev.Attacker.EnableEffect((EffectType)23, (byte)15, 1.25f, false);
	}

	public Ak74uSpectr()
	{
		AttachmentName[] array = new AttachmentName[5];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
