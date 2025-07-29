using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class SheetDemo : EditorWindow
{
    [MenuItem("CodeExample/Show Window")]
    public static void OpenWindow()
    {        
        if (!HasOpenInstances<SheetDemo>())
        {
            SheetDemo window = GetWindow<SheetDemo>();
            window.ShowUtility();
            window.minSize = new Vector2(1400, 250);
        }
    }
 
    
    private GUIStyle textStyle;
    private readonly AbilitySO.SheetData[] elements = new AbilitySO.SheetData[5];
    private readonly string[] elementIDs = new string[5];


    private HashSet<AbilitySO> existingAbilities = new();
    

    private void OnEnable()
    {
        elements[0] = new AbilitySO.SheetData()
        {
            name = "All out",
            description = "Take 5 damage to deal str * 2 to the target.",
            iconEngineId = "Attack",
            cost = "2",
            cooldown = "5",
            warmup = "1",
            filters = new [] { "Direct(5)" },
            steps = new [] { "Damage(0, 5, Melee).Damage(1, Strength, MultipliedBy, 2, Ranged)" }
        };
        for (int i = 1; i < elements.Length; i++)
        {
            elements[i] = new AbilitySO.SheetData { filters = new string[1], steps = new string[1] };    
        }

        
        elementIDs[0] = "TestAbility";
        for (int i = 1; i < elementIDs.Length; i++)
        {
            elementIDs[i] = "";    
        }
    }


    public void OnGUI()
    {
        if (textStyle == null)
        {
            textStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
        }
        
        
        GUILayout.Label("Welcome to my little ability demo. This demo contains functionality from the turn based combat system we developed for the 3 last demos i worked on at PP Game Productions (The Long Night, Njord's Embrace and Laguna. Should you lose this window it can be reopened in the menu point \"CodeExample/ShowWindow\"", textStyle);

        
        GUILayout.Space(10);
        
        
        GUILayout.Label("Below you can simulate a simplified example of a spreadsheet and 'import' the data into the project.", textStyle);
        
        
        GUILayout.Space(5);

        
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Space(35);
            GUILayout.Label("ID", EditorStyles.boldLabel, GUILayout.Width(75));
            GUILayout.Label("Name", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.Label("Description", EditorStyles.boldLabel, GUILayout.Width(300));
            GUILayout.Label("IconId", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.Label("Cost", EditorStyles.boldLabel, GUILayout.Width(75));
            GUILayout.Label("Warmup", EditorStyles.boldLabel, GUILayout.Width(75));
            GUILayout.Label("Cooldown", EditorStyles.boldLabel, GUILayout.Width(75));
            GUILayout.Label("Target filter", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.Label("Effects", EditorStyles.boldLabel, GUILayout.Width(400));    
        }
        
        for (int i = 0; i < elements.Length; i++)
        {
            DrawElement(ref elementIDs[i], ref elements[i]);
        }

        
        GUILayout.Space(10);
        

        if (GUILayout.Button("Import", GUILayout.Width(200), GUILayout.Height(30)))
        {
            ImportElements();
        }
    }


    private void DrawElement(ref string id, ref AbilitySO.SheetData element)
    {
        using GUILayout.HorizontalScope horizontalScope = new();

        if (GUILayout.Button(new GUIContent(EditorGUIUtility.FindTexture("d_Grid.EraserTool")), GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            id = "";
            element = new AbilitySO.SheetData { filters = new string[1], steps = new string[1] }; 
            return;
        }
        
        id = GUILayout.TextField(id, GUILayout.Width(75));
        element.name = GUILayout.TextField(element.name, GUILayout.Width(100));
        element.description = GUILayout.TextField(element.description, GUILayout.Width(300));
        element.iconEngineId = GUILayout.TextField(element.iconEngineId, GUILayout.Width(100));
        element.cost = GUILayout.TextField(element.cost, GUILayout.Width(75));
        element.warmup = GUILayout.TextField(element.warmup, GUILayout.Width(75));
        element.cooldown = GUILayout.TextField(element.cooldown, GUILayout.Width(75));
        element.filters[0] = GUILayout.TextField(element.filters[0], GUILayout.Width(100));
        element.steps[0] = GUILayout.TextField(element.steps[0], GUILayout.Width(400));
    }


    private void ImportElements()
    {
        existingAbilities.Clear();
        
        foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(AbilitySO)}", new [] { "Assets/Abilities" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AbilitySO ability = AssetDatabase.LoadAssetAtPath<AbilitySO>(path);
            existingAbilities.Add(ability);
        }

        for (int i = 0; i < elements.Length; i++)
        {
            if (string.IsNullOrEmpty(elementIDs[i])) continue;
            
            AbilitySO existingSO = existingAbilities.FirstOrDefault(x => x.name == elementIDs[i]);
            if (existingSO != null)
            {
                existingAbilities.Remove(existingSO);
                existingSO.ParseSheetData(elements[i]);
            }
            else
            {
                AbilitySO newInstance = CreateInstance<AbilitySO>();
                newInstance.name = elementIDs[i];
                AssetDatabase.CreateAsset(newInstance, $"Assets/Abilities/{newInstance.name}.asset");
                newInstance.ParseSheetData(elements[i]);
            }
        }

        foreach (AbilitySO so in existingAbilities)
        {
            string path = AssetDatabase.GetAssetPath(so);
            DestroyImmediate(so, true);
            AssetDatabase.DeleteAsset(path);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}


