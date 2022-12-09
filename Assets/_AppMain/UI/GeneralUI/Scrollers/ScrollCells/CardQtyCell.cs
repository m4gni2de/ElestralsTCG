using System.Collections;
using System.Collections.Generic;
using Cards;
using CardsUI.Filtering;
using Databases;
using Decks;
using GlobalUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Decks.Decklist;

public class CardQtyCell : MonoBehaviour, iGridCell
{
    #region Interface
    public GameObject GetGameObject() { return gameObject; }
    public int Index { get { return _index; } }
    public void LoadData(object data, int index)
    {
        DeckCard stack = data as DeckCard;
        SetCard(stack, index);
    }
    public void Clear()
    {
       
        nameText.Blank();
        qtyText.Blank();
        touch.ClearAll();
        _connectedCard = null;
        _index = -1;
        _activeCard = null;
        Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Remove()
    {
        Destroy(gameObject);
    }

    public void SetInsideView(bool isInside)
    {
        touch.Interactable = isInside;
    }
    #endregion

    #region Properties
    private RectTransform rect;
    private int _index;
    
    private DeckCard _connectedCard = null;
    public DeckCard ConnectedCard { get { return _connectedCard; } }

    private Card _activeCard = null;
    public Card ActiveCard { get { return _activeCard; } }

    [SerializeField] private MagicTextBox nameText;
    [SerializeField] private MagicTextBox qtyText;
    #endregion

    #region Touch
    private TouchObject _touch = null;
    public TouchObject touch
    {
        get
        {
            _touch ??= GetComponent<TouchObject>();
            return _touch;
        }
    }
    #endregion

    #region Initialize
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void SetCard(DeckCard card, int index)
    {
        qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", card.key);
        Card c = dto;
        _connectedCard = card;
        _activeCard = c;
        nameText.SetText(c.DisplayName);
        qtyText.SetText($"x {_connectedCard.copy}");
        this._index = index;
        Show();
    }
    public void SetClickListener(UnityAction ac)
    {
        touch.AddClickListener(ac);
    }
    public void SetHoldListener(UnityAction ac)
    {
        touch.AddHoldListener(ac);
    }

    
    #endregion

    private void OnDestroy()
    {
        touch.ClearAll();
        
    }
}
