using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[InitializeOnLoad]
public class Launcher
{
    static Launcher()
    {
        //EditorApplication.playModeStateChanged += ModeChanged;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private static void ModeChanged(PlayModeStateChange newState)
    {
        if (newState == PlayModeStateChange.EnteredPlayMode || newState == PlayModeStateChange.ExitingPlayMode)
        {
            if (newState == PlayModeStateChange.ExitingPlayMode)
            {
                App.Close();
            }
            ConnectionManager.Disconnect();
        }

    }

    private static void SceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        
        //start the app manager somewhere here
        if (scene.buildIndex != 0)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }


    }

    [UnityEditor.Callbacks.DidReloadScripts]
    static void ScriptsReloaded()
    {
        ConnectionManager.Disconnect();
    }
}
#endif