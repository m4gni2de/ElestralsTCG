using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;



[CustomEditor(typeof(SpriteDisplay))]
[CanEditMultipleObjects]
public class SpriteDisplayEditor : Editor
{

    private class Contents
    {
        public static readonly GUIContent RenderTypeLabel = new GUIContent("RenderType", "Does this SpriteDisplay use a SpriteRenderer or Image?");
        public static readonly GUIContent SpriteLabel = new GUIContent("Sprite Renderer", "The Sprite Renderer for this Sprite Display. If left blank, an SpriteRenderer Component will be added automatically.");
        public static readonly GUIContent ImageLabel = new GUIContent("Image", "The Image for this Sprite Display. If left blank, an Image Component will be added automatically.");
        
        public Contents()
        {

        }
    }

    private SpriteDisplay activeDisplay;
    private SerializedProperty m_RendType;
    private SerializedProperty m_Sp;
    private SerializedProperty m_Image;

    private SpriteDisplay.RenderType renderType;
    
    private void OnEnable()
    {
        activeDisplay = (SpriteDisplay)target;
        m_RendType = serializedObject.FindProperty("_rendType");
        m_Sp = serializedObject.FindProperty("_sp");
        m_Image = serializedObject.FindProperty("_image");
        renderType = activeDisplay.RendType;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        SetInspector();


        serializedObject.ApplyModifiedProperties();
    }

    void SetInspector()
    {
        
        EditorGUI.BeginChangeCheck();
        renderType = (SpriteDisplay.RenderType)EditorGUILayout.EnumPopup(Contents.RenderTypeLabel, renderType, GUILayout.MinWidth(200f));
        if (EditorGUI.EndChangeCheck())
        {
            m_RendType.SetEnumValue<SpriteDisplay.RenderType>(renderType);
        }

        if (renderType == SpriteDisplay.RenderType.Sprite)
        {
            EditorGUILayout.PropertyField(m_Sp, Contents.SpriteLabel, GUILayout.MinWidth(200f));
        }
        else if (renderType == SpriteDisplay.RenderType.Image)
        {
            EditorGUILayout.PropertyField(m_Image, Contents.ImageLabel, GUILayout.MinWidth(200f));
        }
        
    }

 }
