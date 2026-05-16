using System;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace FultEngine.CustomItems.WeaponVariants;

[CustomItem(/*Could not decode attribute arguments.*/)]
public sealed class A7Mirage : VariantWeaponBase
{
	public override uint Id { get; set; } = 459u;


	public override string Name { get; set; } = "<b><color=#93e4ff>A7 \"Мираж\"</color></b>";


	public override string Description { get; set; } = "Тихая винтовка разведывательного типа.";


	public override ItemType Type { get; set; } = (ItemType)53;


	public override float Weight { get; set; } = 1.08f;


	public override float Damage { get; set; } = 28f;


	public override byte ClipSize { get; set; } = 32;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Тихая винтовка для мобильной разведки и доразведки.";

	protected override string FeatureDescription => "20% шанс наложить Stained и краткий Slowness.";

	protected override string AttachmentsDescription => "Holo Sight, Sound Suppressor, Lightweight Stock, Extended Mag FMJ";

	protected override string AccentColor => "#93e4ff";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.2f)
		{
			ev.Player.EnableEffect((EffectType)26, (byte)1, 4f, false);
			ev.Player.EnableEffect((EffectType)43, (byte)10, 1.2f, false);
		}
	}

	public A7Mirage()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
