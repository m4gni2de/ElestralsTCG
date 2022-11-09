using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatalogScene : MonoBehaviour , iSceneScript
{ 
    public CardCatalog Catalog;
    
    public static string SceneName
    {
        get
        {
            return SceneHelpers.SceneName(typeof(CatalogScene));
        }
    }
    public static void LoadScene()
    {
        App.ChangeScene(SceneName);
    }

    #region Interface
    public void StartScene()
    {
        WorldCanvas.FindCamera();
        DisplayManager.ClearButton();
        DisplayManager.ToggleVisible(true);
        DisplayManager.SetDefault(() => App.TryChangeScene("MainScene"));
    }
    #endregion

    private void Awake()
    {
       
    }
    private void OnDestroy()
    {
        
    }
    void Start()
    {
        if (Catalog != null)
        {
            Catalog.Open();
        }

        StartScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
