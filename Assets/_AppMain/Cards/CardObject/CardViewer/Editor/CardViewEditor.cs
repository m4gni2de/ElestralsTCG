using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteDisplay;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(CardView))]
[CanEditMultipleObjects]
public class CardViewEditor : Editor
{
    private class Contents
    {
        public static readonly GUIContent SortLayerLabel = new GUIContent("Sort Layer", "SpriteRenderer Sorting Layer Name for the sprites of the card.");
        public static readonly GUIContent SortOrderLabel = new GUIContent("Sort Order", "SpriteRenderer Sorting Order for the Background sprites of the card. All other Sprites use this as a reference for their own Sort Order.");
        public static readonly GUIContent SpriteLabel = new GUIContent("Sprite Renderer", "The Sprite Renderer for this Sprite Display. If left blank, an SpriteRenderer Component will be added automatically.");
        public static readonly GUIContent ImageLabel = new GUIContent("Image", "The Image for this Sprite Display. If left blank, an Image Component will be added automatically.");


        public static readonly GUIContent UpdateSortButton = new GUIContent("Update Sort Layer", "Updates the Sorting Layer of all of the Card's sprites.");
        public static readonly GUIContent UpdateSortOrderButton = new GUIContent("Update Sorting", "Updates Sort Order of the card's Background Sprites, then updates all other sprites to stay relative to the Background Sprite's Order.");
        public static readonly GUIContent PingCardButton = new GUIContent("Ping Card", "Ping the In Scene Location of this card; useful during Run-Time.");

        public Contents()
        {

        }
    }

    


    private Contents k_Contents;


    #region Properties

    private CardView activeCard;

    
    private int cardSortLayer;
    private List<string> _sortLayers = null;
    public List<string> SortLayers
    {
        get
        {
            if (_sortLayers == null)
            {
                _sortLayers = new List<string>();
                for (int i = 0; i < SortingLayer.layers.Length; i++)
                {
                    _sortLayers.Add(SortingLayer.layers[i].name);
                }
            }
            return _sortLayers;
        }
    }
    private int cardSortOrder;

    private bool isSortFoldout = false;
    #endregion

    private SerializedProperty TargetProperty(string propName)
    {
        return serializedObject.FindProperty(propName);
    }

    private void OnEnable()
    {
        k_Contents = new Contents();
        activeCard = (CardView)serializedObject.targetObject;
        //sortLayerName = TargetProperty("_sortLayer");
        if (activeCard != null)
        {
            for (int i = 0; i < SortLayers.Count; i++)
            {
                if (SortLayers[i].ToLower() == activeCard.CurrentConfig.BaseSortLayer.ToLower())
                {
                    cardSortLayer = i;
                }
            }

            cardSortOrder = activeCard.CurrentConfig.BaseSortOrder;
        }
        




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
        EditorGUILayout.LabelField("EDITOR USE ONLY", GUILayout.ExpandWidth(true));
        isSortFoldout = EditorGUILayout.Foldout(isSortFoldout, "Card Sprite Sort Layer");
        
        if (isSortFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            string currentLayer = activeCard.CurrentConfig.BaseSortLayer;

            int layerNum = cardSortLayer;
            cardSortLayer = EditorGUILayout.Popup(Contents.SortLayerLabel, layerNum, SortLayers.ToArray());
            if (currentLayer != SortLayers[cardSortLayer])
            {
                    if (GUILayout.Button(Contents.UpdateSortButton))
                    {
                   
                        activeCard.DefaultConfig.ChangeSortLayer(SortLayers[cardSortLayer], true);
                        activeCard.FullArtConfig.ChangeSortLayer(SortLayers[cardSortLayer], true);

                    }
                
            }


            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            int currentSortOrder = activeCard.CurrentConfig.BaseSortOrder;
            EditorGUILayout.LabelField(Contents.SortOrderLabel);
            cardSortOrder = EditorGUILayout.IntField(cardSortOrder);

            if (GUILayout.Button(Contents.UpdateSortOrderButton))
            {

                activeCard.DefaultConfig.ChangeSortOrder(cardSortOrder, true);
                activeCard.FullArtConfig.ChangeSortOrder(cardSortOrder, true);

            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5f);
            if (GUILayout.Button(Contents.PingCardButton))
            {
                EditorGUIUtility.PingObject(target);
            }
        }

       
        

        

       



    }
}
