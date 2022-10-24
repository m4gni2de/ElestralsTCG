using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatalogScene : MonoBehaviour 
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
