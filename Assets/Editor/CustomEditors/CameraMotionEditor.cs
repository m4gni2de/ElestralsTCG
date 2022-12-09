using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;



[CustomEditor(typeof(CameraMotion))]
[CanEditMultipleObjects]

public class CameraMotionEditor : Editor
{
    private CameraMotion camera;


    private void OnEnable()
    {
        camera = (CameraMotion)serializedObject.targetObject;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        SetInspector();
        serializedObject.ApplyModifiedProperties();



    }

    void SetInspector()
    {
        EditorGUILayout.Space(20f);
        EditorGUILayout.Separator();

        if (GUILayout.Button("Re-Size Camera"))
        {

            //camera.ReScale();

        }
    }
}
