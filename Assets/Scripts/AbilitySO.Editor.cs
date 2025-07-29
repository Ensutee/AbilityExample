#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


public partial class AbilitySO
{
    private static Dictionary<string, Type> typesCache;


    public struct SheetData
    {
        public string name;
        public string description;
        public string iconEngineId;
        public string cost;
        public string warmup;
        public string cooldown;

        public string[] filters;
        public string[] steps;
    }


    private static readonly List<AbilityFilterSettings> filtersBuffer = new();
    private static readonly List<AbilityStepSettings> stepsBuffer = new();
    public void ParseSheetData(SheetData data)
    {
        Undo.RecordObject(this, "Parsing gSheet data");
        Debug.Log($"\"{name}\" Parsing sheet data", this);


        narrativeName = data.name;
        effectDescription = data.description;

        
        string iconsDirectory = "Assets/Visuals/Sprites/Ability Icons";
        Sprite iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{iconsDirectory}/{data.iconEngineId}.png");
        if (iconSprite != null)
        {
            icon = iconSprite;
        }


        if (!int.TryParse(data.cost, out cost))
        {
            Debug.LogError($"Could not parse cast {data.cost} to int, defaulting to {cost}");
        }
        if (!int.TryParse(data.warmup, out warmup))
        {
            Debug.LogError($"Could not parse cast {data.warmup} to int, defaulting to {warmup}");
        }
        if (!int.TryParse(data.cooldown, out cooldown))
        {
            Debug.LogError($"Could not parse cast {data.cooldown} to int, defaulting to {cooldown}");
        }
        
        
        if (typesCache == null || typesCache.Count == 0)
        {
            CacheAvailableTypes();
        }
        
        
        filtersBuffer.Clear();
        foreach (string filterString in data.filters)
        {
            AbilityFilterSettings[] filters = ParseElements<AbilityFilterSettings>(filterString);
            filtersBuffer.AddRange(filters);
        }
        targetGroups = filtersBuffer.ToArray();
        
        
        stepsBuffer.Clear();
        foreach (string effectsString in data.steps)
        {
            AbilityStepSettings step = ParseStep(effectsString);
            if (step == null) continue;
            
            stepsBuffer.Add(step);
        }
        steps = stepsBuffer.ToArray();


        EditorUtility.SetDirty(this);
    }
    
    
    private AbilityStepSettings ParseStep(string effectsString)
    {
        AbilityEffectSettings[] effects = ParseElements<AbilityEffectSettings>(effectsString);
        if (effects.Length == 0) return null;

        AbilityStepSettings stepSettings = new AbilityStepSettings();
        stepSettings.SetEffects(effects);
        return stepSettings;
    }


    #region === CLASS STRING PARSING ===
    private static void CacheAvailableTypes()
    {
        typesCache = new Dictionary<string, Type>();

        TypeCache.TypeCollection types = TypeCache.GetTypesWithAttribute<ClassLabelAttribute>();
        foreach (Type type in types)
        {
            object[] attributes = type.GetCustomAttributes(typeof(ClassLabelAttribute), true);
            if (attributes.Length > 1)
            {
                Debug.Log($"Multiple {nameof(ClassLabelAttribute)} attributes found on type \"{type.Name}\" - skipping type");
                continue;
            }

            ClassLabelAttribute classLabel = (ClassLabelAttribute)attributes[0];
            typesCache.Add(classLabel.Label, type);
        }
    }

    
    private static readonly Regex elementRegex = new(@"([a-zA-Z]+)(?:\(([a-zA-Z0-9 ,_-]+)\))?");
    
    private T[] ParseElements<T>(string elementsString)
    {
        using var _ = ListPool<T>.Get(out List<T> elementsBuffer);

        foreach (Match match in elementRegex.Matches(elementsString))
        {
            string elementName = match.Groups[1].Value;
            string parametersString = match.Groups[2].Value;
        
        
            // Find the correct effect setting type.
            if (!typesCache.TryGetValue(elementName, out Type type))
            {
                Debug.LogError($"\"{elementsString}\" Element type \"{elementName}\" not found in cache - skipping", this);
                continue;
            }
            if (!typeof(T).IsAssignableFrom(type))
            {
                Debug.LogError($"\"{elementsString}\" Element type \"{type.Name}\" does not derive from {nameof(T)} - skipping", this);
                continue;
            }

            // Create an instance of the type with the parsed parameters.
            T element;
            object[] parameters = ParseParameters(parametersString);
            try
            {
                element = (T)Activator.CreateInstance(type, parameters);
            }
            catch (MissingMethodException)
            {
                string parametersOutput = parameters.Length > 0 ? string.Join(", ", parameters) : "none";
                Debug.LogError(
                    $"Could not find a valid constructor for {type.Name} - skipping\nParameters:\n\"{parametersOutput}\"",
                    this);
                continue;
            }
            catch (Exception e)
            {
                Debug.LogError($"Creating element of type {type.Name} failed with exception:\n\n{e}", this);
                continue;
            }
            
            elementsBuffer.Add(element);
        }

        return elementsBuffer.ToArray();
    }

    
    private static readonly Regex parameterRegex = new(" ?([a-zA-Z0-9 _-]+)");
    
    private object[] ParseParameters(string parametersString)
    {
        List<object> parameters = ListPool<object>.Get();


        foreach (Match parameterMatch in parameterRegex.Matches(parametersString))
        {
            string parameter = parameterMatch.Groups[1].Value;

            if (int.TryParse(parameter, out int intValue))
            {
                parameters.Add(intValue);
            }
            else
            {
                parameters.Add(parameter);
            }
        }


        object[] parameterArray = parameters.ToArray();
        ListPool<object>.Release(parameters);
        return parameterArray;
    }
    #endregion
}

#endif