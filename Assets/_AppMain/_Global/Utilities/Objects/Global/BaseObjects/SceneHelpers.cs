using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHelpers 
{

    private static Dictionary<Type, string> _sceneNames = null;
    protected static Dictionary<Type, string> SceneNames
    {
        get
        {
            if (_sceneNames == null)
            {
                _sceneNames = new Dictionary<Type, string>();
                _sceneNames.Add(typeof(LauncherScene), "LauncherScene");
                _sceneNames.Add(typeof(MainScene), "MainScene");
                _sceneNames.Add(typeof(CatalogScene), "CatalogScene");
                _sceneNames.Add(typeof(GameManager), "GameScene");
                _sceneNames.Add(typeof(NetworkScene), "NetworkLobby");
                _sceneNames.Add(typeof(DeckEditorScene), "DeckEditorScene");
            }
            return _sceneNames;
        }
    }
    public static string SceneName(Type sceneType)
    {
        if (SceneNames.ContainsKey(sceneType))
        {
            return SceneNames[sceneType];
        }
        App.DisplayError($"The script {sceneType.Name} does not exist in Scene Names. Manually add it to the Scene Names Dictionary.");
        return "";
    }
  




}
