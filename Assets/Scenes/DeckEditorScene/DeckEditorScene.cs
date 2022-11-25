using System.Collections;
using System.Collections.Generic;
using Decks;
using nsSettings;
using PopupBox;
using UnityEngine;
using Users;

public class DeckEditorScene : MonoBehaviour, iSceneScript
{
    #region Interface
    public void StartScene()
    {
        WorldCanvas.FindCamera();
        DisplayManager.ClearButton();
        DisplayManager.ToggleVisible(true);
        DisplayManager.SetDefault(() => SetDefaultLeaveScene());
    }

    private void SetDefaultLeaveScene()
    {
        if (!DeckEditor.HasChanges)
        {
            DisplayManager.SetDefault(() => App.TryChangeScene("MainScene"));
        }
        else
        {
            DisplayManager.SetDefault(() => TryQuitEditor());
        }
    }

    private void TryQuitEditor()
    {
        string msg = $"There are unsaved changes to the deck. Do you wish to Save them before quitting?";
        App.AskYesNoCancel(msg, TryLeaveScene);
    }
    private void TryLeaveScene(PopupResposne response)
    {

        switch (response)
        {
            case PopupResposne.Cancel:
                break;
            case PopupResposne.Yes:
                DeckEditor.Instance.SaveActiveDeck();
                App.ChangeScene("MainScene");
                break;
            case PopupResposne.No:
                App.ChangeScene("MainScene");
                break;
        }
    }

    #endregion

    #region Properties
    private List<Decklist> _deckList = null;
    public List<Decklist> Decklist
    {
        get
        {
            if (_deckList == null)
            {
                _deckList = App.Account.DeckLists;
            }
            return _deckList;
        }
    }
    #endregion

    public DeckEditor deckEditor;

    public static string SceneName
    {
        get
        {
            return SceneHelpers.SceneName(typeof(DeckEditorScene));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartScene();
        deckEditor.LoadDecks(App.Account.DeckLists);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
