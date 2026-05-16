using System;
using System.Runtime.CompilerServices;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class A7Bastet : VariantWeaponBase
{
	public override uint Id { get; set; } = 460u;


	public override string Name { get; set; } = "<b><color=#ffd56e>A7 \"Бастет\"</color></b>";


	public override string Description { get; set; } = "Винтовка поддержки с элементом самоотхила.";


	public override ItemType Type { get; set; } = (ItemType)53;


	public override float Weight { get; set; } = 1.19f;


	public override float Damage { get; set; } = 31f;


	public override byte ClipSize { get; set; } = 36;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Универсальная винтовка поддержки для затяжного боя.";

	protected override string FeatureDescription => "Каждое попадание лечит стрелка на 4 HP.";

	protected override string AttachmentsDescription => "Scope Sight, Muzzle Booster, Extended Mag JHP, Extended Stock";

	protected override string AccentColor => "#ffd56e";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		ev.Attacker.Heal(4f, false);
	}

	public A7Bastet()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
