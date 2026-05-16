using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;

namespace FultEngine.CustomItems.Tizer;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Taizer : CustomWeapon
{
	private readonly EventHandlers _eventHandlers;

	public override uint Id { get; set; } = 17u;


	public override string Name { get; set; } = "<color=#5447de96><b>Тайзер</b></color>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 1f;


	public override byte ClipSize { get; set; } = 1;


	public override float Damage { get; set; } = 1f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public Taizer()
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
