using System;
using UnityEngine;


[Serializable]
public abstract class AbilityEffectSettings
{
	[SerializeField, TargetGroupSelector]
	protected int targetGroupIndex = 0;
		
		
	public AbilityEffectSettings() { }
		
	public AbilityEffectSettings(int targetGroupIndex)
	{
		this.targetGroupIndex = targetGroupIndex;
	}
		
		
	public abstract AbilityEffect CreateAbilityEffect();
}