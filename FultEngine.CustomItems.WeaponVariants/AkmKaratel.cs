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
public sealed class AkmKaratel : VariantWeaponBase
{
	public override uint Id { get; set; } = 401u;


	public override string Name { get; set; } = "<b><color=#d46a2e>AKM \"Каратель\"</color></b>";


	public override string Description { get; set; } = "Тяжёлый калаш 7.62 — высокий урон и кровотечение.";


	public override ItemType Type { get; set; } = (ItemType)40;


	public override float Weight { get; set; } = 1.35f;


	public override float Damage { get; set; } = 34f;


	public override byte ClipSize { get; set; } = 30;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override AttachmentName[] Attachments { get; set; }

	protected override string WeaponSubtitle => "Тяжёлый штурмовой автомат 7.62×39 мм.";

	protected override string FeatureDescription => "22% шанс наложить Bleeding и короткий Concussed.";

	protected override string AttachmentsDescription => "Standard Stock, Muzzle Brake, Standard Mag FMJ, Dot Sight";

	protected override string AccentColor => "#d46a2e";

	protected override void OnVariantHurting(HurtingEventArgs ev)
	{
		if (Random.value <= 0.22f)
		{
			ev.Player.EnableEffect((EffectType)4, (byte)2, 6f, false);
			ev.Player.EnableEffect((EffectType)7, (byte)1, 1.25f, false);
		}
	}

	public AkmKaratel()
	{
		AttachmentName[] array = new AttachmentName[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Attachments = (AttachmentName[])(object)array;
		base._002Ector();
	}
}
