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
public sealed class A7Helix : VariantWeaponBase
{
	public override uint Id { get; set; } = 458u;


	public override string Name { get; set; } = "<b><color=#ff8d73>A7 \"Хеликс\"</color></b>";


	public override string Description { get; set; } = "Универсальная винтовка с поджогом цели.";


	public override ItemType Type { get; set; } = (ItemType)53;


	public override float Weight { get; set; } = 1.16f;


	public override float Damage { get; set; } = 30f;


	public override byte ClipSize { get; set; } = 35;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Лёгкая винтовка для быстрой агрессии.";

	protected override string FeatureDescription => "17% шанс наложить Burned на 3 секунды.";

	protected override string AttachmentsDescription => "Dot Sight, Sound Suppressor, Standard Mag JHP, Laser";

	protected override string AccentColor => "#ff8d73";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.17f)
		{
			ev.Player.EnableEffect((EffectType)6, (byte)1, 3f, false);
		}
	}

	public A7Helix()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
