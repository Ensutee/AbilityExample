using System;
using UnityEngine;


[Serializable]
public class AbilityStepSettings
{
	[SerializeField]
	[SerializeReference]
	private AbilityEffectSettings[] effects;
		
		
	public AbilityStep CreateAbilityStep()
	{
		AbilityEffect[] abilityEffects = new AbilityEffect[effects.Length];
		for (int i = 0; i < effects.Length; i++)
		{
			abilityEffects[i] = effects[i].CreateAbilityEffect();
		}
			
		AbilityStep abilityStep = new AbilityStep(abilityEffects);
		return abilityStep;
	}
		
		
#if UNITY_EDITOR
	public void SetEffects(AbilityEffectSettings[] effects)
	{
		this.effects = effects;
	}
#endif
}