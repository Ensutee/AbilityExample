using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


[CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
public partial class AbilitySO : ScriptableObject
{
    [Header("Naming and text information")]
    [SerializeField]
    private string narrativeName;
    [SerializeField]
    private string effectDescription;

    [Header("Graphics")]
    public Sprite icon;

    [SerializeField]
    private int cost = 2;
    
    [SerializeField]
    private int warmup = 0;
    
    [SerializeField]
    public int cooldown;
    
    
    [SerializeField]
    [SerializeReference]
    private AbilityFilterSettings[] targetGroups;
    
    [SerializeField]
    private AbilityStepSettings[] steps;


    public ActivatableAbility Create()
    {
        using PooledObject<List<AbilityStep>> _ = ListPool<AbilityStep>.Get(out List<AbilityStep> stepsCreationBuffer);
        
        stepsCreationBuffer.Clear();
        for (int i = 0; i < steps.Length; i++)
        {
            AbilityStepSettings step = steps[i];
            
            if (step == null) continue;

            try
            {
                stepsCreationBuffer.Add(step.CreateAbilityStep());
            }
            catch (Exception e)
            {
                Debug.LogWarning($"\"{name}\" Could not create ability step {i} - skipping\nException: {e}");
            }
        }

        AbilityStep[] abilitySteps = stepsCreationBuffer.ToArray();
        
        
        AbilityTargetFilter[] targetFilters = new AbilityTargetFilter[targetGroups.Length];
        for (int i = 0; i < targetGroups.Length; i++)
        {
            targetFilters[i] = targetGroups[i].CreateAbilityTargetFilter();
        }
                
        return new ActivatableAbility(targetFilters, abilitySteps)
        {
            Cost = cost,
            MaxWarmup = warmup,
            MaxCooldown = cooldown
        };
    }
}