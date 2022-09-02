using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packs;

public class MainScene : MonoBehaviour
{
    public GameObject menuButtons;

    // Start is called before the first frame update
    void Start()
    {
        //CardCatalog.Open();
        App.StartApp();
        AssetPipeline.DoRedownloadAllCards();
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }



    #region Buttons
    public void CardCatalogLoad()
    {
        App.ChangeScene("CatalogScene");
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
        App.ChangeScene("GameScene");
    }
    #endregion
}
