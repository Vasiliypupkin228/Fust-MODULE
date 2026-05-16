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
public sealed class Ak12Operator : VariantWeaponBase
{
	public override uint Id { get; set; } = 403u;


	public override string Name { get; set; } = "<b><color=#7dffb3>AK-12 \"Оператор\"</color></b>";


	public override string Description { get; set; } = "Тактический калаш для контроля боя.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.2f;


	public override float Damage { get; set; } = 31f;


	public override byte ClipSize { get; set; } = 35;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Тактическая платформа для штурма и точечной работы.";

	protected override string FeatureDescription => "15% шанс кратко наложить Ensnared на цель.";

	protected override string AttachmentsDescription => "Holo Sight, Laser, Recoil Reducing Stock, Extended Mag JHP, Flashlight";

	protected override string AccentColor => "#7dffb3";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.15f)
		{
			ev.Player.EnableEffect((EffectType)12, (byte)1, 1.2f, false);
		}
	}

	public Ak12Operator()
	{
		AttachmentName[] array = new AttachmentName[5];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
