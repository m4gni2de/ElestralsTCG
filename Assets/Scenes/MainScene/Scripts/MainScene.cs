using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packs;

public class MainScene : MonoBehaviour
{
    #region Scene Loading
    public static string SceneName
    {
        get
        {
            return SceneHelpers.SceneName(typeof(MainScene));
        }
    }
    public static void LoadScene()
    {
        App.ChangeScene(SceneName);
    }
    #endregion


    public GameObject menuButtons;

    // Start is called before the first frame update
    void Start()
    {
        //CardCatalog.Open();
        menuButtons.SetActive(true);
        //App.StartApp();
        //AssetPipeline.DoRedownloadAllCards();
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }



    #region Buttons
    public void CardCatalogLoad()
    {
        App.ChangeScene(CatalogScene.SceneName);
        menuButtons.SetActive(false);
    }

    public void PackGeneratorLoad()
    {
        menuButtons.SetActive(false);
        PackGenerator.LoadGenerator();
    }
    public void PlayGameButton()
    {
        menuButtons.SetActive(false);
        //App.ChangeScene("GameScene");
        // App.ChangeScene(OnlineGameManager.SceneName);
        GameManager.StartLocalGame();
    }
    public void PvPButton()
    {
        menuButtons.SetActive(false);
        App.ChangeScene(NetworkScene.SceneName);
    }
    #endregion
}
