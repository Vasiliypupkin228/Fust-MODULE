namespace FultEngine.Module.RealDemage;

public class Injury
{
	public string BodyPart { get; set; }

	public string Type { get; set; }

	public float DamagePerSecond { get; set; }

	public float Timestamp { get; set; }

	public bool IsBleeding => Type.Contains("Кровотечение");

	public float BleedIntensity
	{
		get
		{
			string type = Type;
			if (1 == 0)
			{
			}
			float result = ((type == "Сильное кровотечение") ? 2f : ((!(type == "Кровотечение")) ? 0f : 1f));
			if (1 == 0)
			{
			}
			return result;
		}
	}
}
