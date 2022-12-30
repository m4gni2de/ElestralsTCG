using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packs;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using nsSettings;

public class MainScene : MonoBehaviour, iSceneScript
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

    #region Interface
    public void StartScene()
    {
       

        WorldCanvas.FindCamera();
        ToggleButtons(true);
        menuButtons.SetActive(true);
        StartCoroutine(DoAwaitAssets());


        DisplayManager.ClearButton();
        DisplayManager.ToggleVisible(false);
        DisplayManager.SetDefault(() => AppManager.Instance.TryQuit());
    }
    #endregion

    #region Properties
    public List<Button> mainButtons = new List<Button>();
    [SerializeField] private AccountSettingsMenu accountMenu;

    #endregion

    #region Button Management
    private void ToggleButtons(bool isOn)
    {
        for (int i = 0; i < mainButtons.Count; i++)
        {
            mainButtons[i].interactable = isOn;
        }
    }
    #endregion


    public GameObject menuButtons;

    // Start is called before the first frame update
    void Start()
    {
        StartScene();
        
    }


    private IEnumerator DoAwaitAssets()
    {
        yield return new WaitForEndOfFrame();
        do
        {
            yield return new WaitForEndOfFrame();
        } while (true && !CardFactory.AssetsLoaded);

        ToggleButtons(true);
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
        //DisplayManager.SetAction(() => ClosePackGenerator());
        DisplayManager.AddAction(ClosePackGenerator);
        DisplayManager.ToggleVisible(true);
        menuButtons.SetActive(false);
        PackGenerator.LoadGenerator();
    }
    private void ClosePackGenerator()
    {
        //DisplayManager.RemoveAction(() => ClosePackGenerator());
        DisplayManager.RemoveAction(ClosePackGenerator);
        DisplayManager.ToggleVisible(false);
        PackGenerator.CloseGenerator();
        menuButtons.SetActive(true);
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

    public void DeckEditorButton()
    {
        menuButtons.SetActive(false);
        App.ChangeScene(DeckEditorScene.SceneName);
    }

    public void AccountButton(bool doOpen)
    {
        if (doOpen)
        {
            accountMenu.OnDisplayChanged += OnAccountMenuDislayChanged;
            menuButtons.SetActive(false);
            accountMenu.LoadSettings();
        }
        else
        {
            menuButtons.SetActive(true);
        }
        
    }

    private void OnAccountMenuDislayChanged(bool isShowing)
    {
        if (!isShowing)
        {
            accountMenu.OnDisplayChanged -= OnAccountMenuDislayChanged;
            menuButtons.SetActive(true);
        }
    }
    #endregion
}
