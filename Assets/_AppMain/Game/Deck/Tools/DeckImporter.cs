using System;
using System.Collections;
using System.Collections.Generic;
using Databases;
using Decks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckImporter : MonoBehaviour, iShowHide, iFreeze
{
    #region Interface
    public event Action<bool> OnDisplayChanged;

    public void Show()
    {
        this.Freeze();
        gameObject.SetActive(true);
        OnDisplayChanged?.Invoke(true);
    }

    public void Hide()
    {
        this.ThawOnRelease();
        gameObject.SetActive(false);
        OnDisplayChanged?.Invoke(false);
    }
    #endregion
    #region Properties
    [SerializeField] private TMP_InputField txtInput;
    [SerializeField] private Button importButton;
    public UploadedDeckDTO ImportedDeck { get; private set; }
    #endregion


    #region InputField Events
    private UploadedDeckDTO FromImport(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) { return null; }
        UploadedDeckDTO deck = new UploadedDeckDTO();
        int count = App.Account.DeckLists.Count;

        string key = UniqueString.CreateId(7, "dk");
        deck.deckKey = key;
        deck.title = $"Imported Deck {key}";
        DataList d = JsonConvert.DeserializeObject<DataList>(json);
        deck.deck = d.items;
        return deck;
    }
    public void OnTextInputChanged()
    {
        importButton.interactable = !txtInput.text.IsEmpty();
    }
    #endregion

    #region Life Cycle
    private void Refresh()
    {
        ClearButton();
        ImportedDeck = null;
    }
    #endregion

    #region Buttons
    public void PasteButton()
    {
        txtInput.text = GUIUtility.systemCopyBuffer;
        txtInput.caretPosition = 0;
    }
    public void ClearButton()
    {
        txtInput.text = "";
        txtInput.caretPosition = 0;
    }
    public void ImportButton()
    {
        UploadedDeckDTO dto = FromImport(txtInput.text.Trim());
        if (dto == null)
        {
            App.ShowMessage($"Import Error. Either text is formatting incorrectly, or empty.");
            return;
        }
        ImportedDeck = dto;

        Decklist deck = ImportedDeck;

        deck.AddAndSave();
        App.Account.DeckLists.Add(deck);
        DeckEditor.EditDeck(deck);
        Hide();
        Refresh();
    }

    
    #endregion



}
