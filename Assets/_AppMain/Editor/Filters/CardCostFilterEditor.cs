using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CardsUI.Filtering;

[CustomEditor(typeof(CardCostFilterGroup))]
[CanEditMultipleObjects]
public class CardCostFilterEditor : Editor
{
    private SerializedProperty m_NoneIsAll;


    private void OnEnable()
    {
        m_NoneIsAll = serializedObject.FindProperty("NoneIsAll");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_NoneIsAll, new GUIContent("None Is All", "Is checked, every Toggle in the group will become checked if is no Toggle is checked."));
        serializedObject.ApplyModifiedProperties();
    }
}
