using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Decks;
using UnityEngine;
using UnityEngine.UI;

public class DeckDownloader : MonoBehaviour, iShowHide
{
    #region Interfaces
    public event Action<bool> OnDisplayChanged;
    #endregion

    #region Properties
    [SerializeField] private MagicInputText searchTxt;
    private List<Decklist> _browseResults = null;
    public List<Decklist> BrowseResults { get { _browseResults ??= new List<Decklist>(); return _browseResults; } }

    private Decklist SearchResult = null;

    [SerializeField] private Button searchButton;
    #endregion

    #region Events
    public event Action<Decklist> OnSearchComplete;
    private void SearchComplete()
    {
        if (SearchResult != null)
        {
            OnSearchComplete?.Invoke(SearchResult);
        }
        
    }
    #endregion

    #region Life Cycle
    private void Awake()
    {
        searchTxt.SetToDefault(1, true, false);
        searchTxt.SetLengthContraints(4, 16);
    }

    public void Toggle(bool isOn)
    {
        if (isOn) { Refresh(); Show(); } else { Hide(); }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        OnDisplayChanged?.Invoke(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        OnDisplayChanged?.Invoke(false);
    }
    public void Refresh()
    {
        BrowseResults.Clear();
        searchTxt.SetToDefault(1, true, false);
        searchTxt.EditButtonClick();
        ToggleButton(searchButton, true);
    }
    #endregion


    #region Searching
    public async void SearchButton()
    {
        ToggleButton(searchButton, false);
        string code = searchTxt.Content;
        bool validCode = await SearchDeckCode(code);

        if (!validCode)
        {
            ShowError($"There is no deck with that code. Please check your code and try again.");
            ToggleButton(searchButton, true);
        }
        else
        {
            SearchComplete();
            Hide();
        }
    }

    private async Task<bool> SearchDeckCode(string code)
    {
        BrowseResults.Clear();
        SearchResult = null;
        DownloadedDeckDTO deck = await RemoteData.SearchDeck(code);
        if (deck != null)
        {
            if (deck.owner.ToLower() != App.Account.Id.ToLower())
            {
                Decklist decklist = deck;
                BrowseResults.Add(decklist);
                SearchResult = decklist;
                return true;
            }
            App.DisplayError($"You are the owner of this deck.");
            return false;
        }
        return false;

    }
    #endregion


    #region Display Messages
    private void ShowError(string msg)
    {
        App.DisplayError(msg);
    }
    #endregion


    #region Object Management
    private void ToggleButton(Button b, bool isInteractable)
    {
        b.interactable = isInteractable;
    }
    
    #endregion
}
