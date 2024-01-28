using System;

public class Sentinel : Enemy
{
	private const int goblinHealth = 20;
	private const int goblinDamage = 5;

	public Sentinel()
	{
		Health = goblinHealth;
		Damage = goblinDamage;
	}
}
