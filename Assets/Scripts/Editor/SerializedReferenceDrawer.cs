using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


public abstract class SerializedReferenceDrawer<RefType> : PropertyDrawer
{
	private static Dictionary<string, Type> typeMap;
	private static Dictionary<Type, int> indexMap;
	private static string[] options = Array.Empty<string>();
	
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int count = 0;
		float height = 0;

		// Title.
		height += EditorGUIUtility.singleLineHeight;
		if (!property.isExpanded) return height;
		height += EditorGUIUtility.standardVerticalSpacing;
		
		// Fields.
		using SerializedProperty iterator = property.Copy();
		while (iterator.NextVisible(true) && iterator.depth > property.depth)
		{
			if (iterator.depth > property.depth + 1) continue;
			
			count++;
			height += EditorGUI.GetPropertyHeight(iterator);
		}
		
		return height + (count - 1) * EditorGUIUtility.standardVerticalSpacing;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (typeMap == null) CacheTypeInfo();
		
		
		// Draw title.
		position.height = EditorGUIUtility.singleLineHeight;
		
			// Draw foldout if we have a value.
		if (property.managedReferenceValue != null)
		{
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);	
		}

			// Draw popup.
		int currentSelection = GetCurrentOptionIndex(property);
		int newSelection = EditorGUI.Popup(position, currentSelection, options);
		if (newSelection != currentSelection)
		{
			if (newSelection == 0)
			{
				property.managedReferenceValue = null;
				property.isExpanded = false;
			}
			else
			{
				Type newType = typeMap[options[newSelection]];
				object newInstance = Activator.CreateInstance(newType);

				property.managedReferenceValue = newInstance;
				property.isExpanded = true;
			}
		}
		position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		
		// If we have no value or are not folded out, we're done.
		if (property.managedReferenceValue == null || !property.isExpanded) return;


		using EditorGUI.IndentLevelScope indent = new();
		
		
		// Draw fields.
		using SerializedProperty iterator = property.Copy();
		while (iterator.NextVisible(true) && iterator.depth > property.depth)
		{
			if (iterator.depth > property.depth + 1) continue;

			position.height = EditorGUI.GetPropertyHeight(iterator);
			EditorGUI.PropertyField(position, iterator);
			position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;
		}
	}


	private void CacheTypeInfo()
	{
		typeMap = new Dictionary<string, Type>();
		indexMap = new Dictionary<Type, int>();
		List<string> optionsBuffer = ListPool<string>.Get();
		
		typeMap.Add("None", null);
		optionsBuffer.Add("None");

		foreach (Type type in TypeCache.GetTypesDerivedFrom(typeof(RefType)))
		{
			object[] attributes = type.GetCustomAttributes(typeof(ClassLabelAttribute), true);
			if (attributes.Length == 0) continue;
			ClassLabelAttribute labelAttribute = (ClassLabelAttribute)attributes[0];
			string label = labelAttribute.Category + labelAttribute.Label;
			
			indexMap.Add(type, typeMap.Count);
			typeMap.Add(label, type);
			optionsBuffer.Add(label);
		}

		options = optionsBuffer.ToArray();
		ListPool<string>.Release(optionsBuffer);
	}
	
	
	private int GetCurrentOptionIndex(SerializedProperty property)
	{
		if (property.managedReferenceValue == null) return 0;
		
		if (indexMap.TryGetValue(property.managedReferenceValue.GetType(), out int index))
		{
			return index;
		}

		return 0;
	}
}