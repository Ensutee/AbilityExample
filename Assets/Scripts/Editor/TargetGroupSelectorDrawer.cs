using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(TargetGroupSelectorAttribute))]
public class TargetGroupSelectorDrawer : PropertyDrawer
{
	private static readonly List<GUIContent> optionsBuffer = new();
		
		
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUIContent[] options = GetOptions(property);
		property.intValue = EditorGUI.Popup(position, label, property.intValue, options);
	}


	private GUIContent[] GetOptions(SerializedProperty property)
	{
		optionsBuffer.Clear();

		optionsBuffer.Add(new GUIContent("Self"));
			
		using SerializedProperty targetGroups = property.serializedObject.FindProperty("targetGroups");
		if (targetGroups != null)
		{
			for (int i = 0; i < targetGroups.arraySize; i++)
			{
				using SerializedProperty targetGroup = targetGroups.GetArrayElementAtIndex(i);

				string label = "None";
				
				if (targetGroup.managedReferenceValue != null)
				{
					ClassLabelAttribute labelAttribute = (ClassLabelAttribute)targetGroup.managedReferenceValue.GetType().GetCustomAttribute(typeof(ClassLabelAttribute));
					label = labelAttribute.Label;
				}
				
				optionsBuffer.Add(new GUIContent($"{label} ({i})"));
			}	
		}
		return optionsBuffer.ToArray();
	}
}