using System;


[Serializable]
public abstract class AbilityFilterSettings
{
	public abstract AbilityTargetFilter CreateAbilityTargetFilter();
}