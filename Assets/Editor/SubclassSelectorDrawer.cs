using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            EditorGUI.LabelField(position, label.text, "Use [SerializeReference] with this.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // Draw the label
        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // Calculate dropdown rect
        var dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        // Get the current type name
        string typeName = property.managedReferenceFullTypename.Split(' ').LastOrDefault();
        if (string.IsNullOrEmpty(typeName)) typeName = "None (null)";
        else typeName = typeName.Split('.').Last();

        if (GUI.Button(dropdownRect, typeName, EditorStyles.popup))
        {
            ShowTypeMenu(property);
        }

        // Draw the children (the public variables inside your state)
        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        var menu = new GenericMenu();
    
        // Option to clear the slot
        menu.AddItem(new GUIContent("None"), string.IsNullOrEmpty(property.managedReferenceFullTypename), () => {
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        });

        // fieldInfo.FieldType is the type of the variable in your Controller 
        // (e.g., State<EntityController>)
        var fieldType = fieldInfo.FieldType;

        // TypeCache is ultra-fast but needs a specific base type.
        // This will find everything that inherits from your specific State<T>
        var types = TypeCache.GetTypesDerivedFrom(fieldType)
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var type in types)
        {
            // We use type.FullName or type.Name for the menu display
            menu.AddItem(new GUIContent(type.Name), false, () => {
                // Create instance using the parameterless constructor 
                // (Which is why we removed the constructors earlier!)
                object instance = Activator.CreateInstance(type);
            
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedProperties();
            });
        }
    
        menu.ShowAsContext();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}