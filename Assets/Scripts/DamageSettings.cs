using System;
using UnityEngine;


[ClassLabel("Effects/Damage")]
public class DamageSettings : AbilityEffectSettings
{
	[SerializeField]
	private StatName stat;
	[SerializeField]
	private Damage.Operations operation = Damage.Operations.None;
	[SerializeField]
	private int amount;
	[SerializeField]
	private Damage.Types type = Damage.Types.Melee;

		
	public DamageSettings() { }

	public DamageSettings(int filterIndex, int amount, string type) : base(filterIndex)
	{
		this.amount = amount;
			
		if (!Enum.TryParse(type, out this.type))
		{
			Debug.LogWarning($"Could not parse damage type \"{type}\" to a valid type, defaulting to {this.type}");
		}
	}
		
	public DamageSettings(int filterIndex, string stat, string operation, int amount, string type) : base(filterIndex)
	{
		if (!Enum.TryParse(stat, out this.stat))
		{
			Debug.LogWarning($"Could not parse stat \"{stat}\" to a valid stat name, defaulting to {this.stat}");
		}
			
		if (!Enum.TryParse(operation, out this.operation))
		{
			Debug.LogWarning($"Could not parse operation \"{operation}\" to a valid operation, defaulting to {this.operation}");
		}
			
		this.amount = amount;
			
		if (!Enum.TryParse(type, out this.type))
		{
			Debug.LogWarning($"Could not parse damage type \"{type}\" to a valid type, defaulting to {this.type}");
		}
	}
		
		
	public override AbilityEffect CreateAbilityEffect()
	{
		return new Damage(targetGroupIndex, amount, type, stat, operation);
	}
}