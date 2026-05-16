using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.CustomItems;

namespace CustomItems.Item;

[CustomItem(/*Could not decode attribute arguments.*/)]
public class SmokeGrenade : CustomGrenade
{
	public override uint Id { get; set; } = 8u;


	public override string Name { get; set; } = "<b><color=#787878e2>Дымовая завеса</color></b>";


	public override string Description { get; set; } = "Создаёт плотную дымовую завесу, которая держится 19 секунд.";


	public override float Weight { get; set; } = 1.15f;


	public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();


	public override bool ExplodeOnCollision { get; set; } = false;


	public override float FuseTime { get; set; } = 5f;


	public int MaxDiameter { get; set; } = 3;


	public int TotalSmokingTime { get; set; } = 19;


	public float FadeTime { get; set; } = 3f;


	protected override void SubscribeEvents()
	{
		((CustomGrenade)this).SubscribeEvents();
		Player.ItemAdded += (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem += (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
	}

	protected override void UnsubscribeEvents()
	{
		((CustomGrenade)this).UnsubscribeEvents();
		Player.ItemAdded -= (CustomEventHandler<ItemAddedEventArgs>)OnItemAdded;
		Player.ChangingItem -= (CustomEventHandler<ChangingItemEventArgs>)OnChangingItem;
	}

	private void OnItemAdded(ItemAddedEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Дымовая завеса");
		}
	}

	private void OnChangingItem(ChangingItemEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Item != null && ((CustomItem)this).Check(ev.Item))
		{
			CIMessage.SendMessage(ev.Player, "Дымовая завеса");
		}
	}
}
