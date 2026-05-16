using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;

namespace FultEngine.CustomItems.Tranq;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Tranquilizer : CustomWeapon
{
	private readonly EventHandlers _eventHandlers;

	public override uint Id { get; set; } = 12u;


	public override string Name { get; set; } = "<b><color=#2f51cce2>Транквилизатор</b></color>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 1f;


	public override byte ClipSize { get; set; } = 1;


	public override float Damage { get; set; } = 0f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public Tranquilizer()
	{
		_eventHandlers = new EventHandlers(this);
	}

	protected override void SubscribeEvents()
	{
		_eventHandlers.SubscribeEvents();
		((CustomWeapon)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		_eventHandlers.UnsubscribeEvents();
		((CustomWeapon)this).UnsubscribeEvents();
	}
}
