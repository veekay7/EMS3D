using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyVarAttribute))]
public class ReadOnlyVarPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}


	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(rect, prop, label, true);
		GUI.enabled = true;
	}
}
