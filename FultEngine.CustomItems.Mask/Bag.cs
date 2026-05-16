using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace FultEngine.CustomItems.Mask;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class Bag : CustomItem
{
	private readonly EventHandlers _eventHandlers;

	public override uint Id { get; set; } = 6u;


	public override string Name { get; set; } = "<b><color=#ff00e563>Мешок Scp-096</color></b>";


	public override string Description { get; set; } = "";


	public override float Weight { get; set; } = 1f;


	public override Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public Bag()
	{
		_eventHandlers = new EventHandlers(this);
	}

	protected override void SubscribeEvents()
	{
		_eventHandlers.SubscribeEvents();
		((CustomItem)this).SubscribeEvents();
	}

	protected override void UnsubscribeEvents()
	{
		_eventHandlers.UnsubscribeEvents();
		((CustomItem)this).UnsubscribeEvents();
	}
}
