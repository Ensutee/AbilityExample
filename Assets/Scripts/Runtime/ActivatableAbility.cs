using System;


public class ActivatableAbility
{
    public int CurrentWarmup { get; internal set; } = 0;
    public int CurrentCooldown { get; internal set; } = 0;
    
    public int Cost = 0;
    public AbilityTargetFilter[] TargetFilters;
    public AbilityStep[] Steps;
    public int MaxWarmup;
    public int MaxCooldown;
	

    internal ActivatableAbility()
    {
        TargetFilters = Array.Empty<AbilityTargetFilter>();
        Steps = Array.Empty<AbilityStep>();
    }
	
    public ActivatableAbility(AbilityTargetFilter[] targetFilters, params AbilityStep[] steps)
    {
        if (steps.Length == 0)
        {
            throw new ArgumentException("ActivatableAbility must have at least one step");
        }
		
        TargetFilters = targetFilters;
        Steps = steps;
    }


    public void Reset()
    {
        CurrentWarmup = 0;
        CurrentCooldown = 0;
    }
}


public class AbilityStep
{
    public AbilityEffect[] Effects { get; private set; }


    public AbilityStep(params AbilityEffect[] effects)
    {
        Effects = effects;
    }
}