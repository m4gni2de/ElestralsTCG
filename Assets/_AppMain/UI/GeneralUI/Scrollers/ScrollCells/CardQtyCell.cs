using System.Collections;
using System.Collections.Generic;
using CardsUI.Filtering;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CardQtyCell : MonoBehaviour, iGridCell
{
    #region Interface
    public GameObject GetGameObject() { return gameObject; }
    public int Index { get { return _index; } }
    public void LoadData(object data, int index)
    {
        CardStack stack = data as CardStack;
        SetCard(stack, index);
    }
    public void Clear()
    {
        if (_connectedCard != null)
        {
            _connectedCard.QuantityText.RemoveTextChangeListener(() => UpdateQuantity());
        }
        nameText.Blank();
        qtyText.Blank();
        _connectedCard = null;
        touch.ClearAll();
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

    private void UpdateQuantity()
    {
        qtyText.SetText($"x {_connectedCard.quantity}");
    }
    #endregion

    #region Properties
    private RectTransform rect;
    private int _index;
    
    private CardStack _connectedCard = null;
    public CardStack ConnectedCard { get { return _connectedCard; } }

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

    private void SetCard(CardStack stack, int index)
    {
        _connectedCard = stack;
        nameText.SetText(_connectedCard.DisplayName);
        qtyText.SetText($"x {_connectedCard.quantity}");
        this._index = index;
        Show();

        _connectedCard.QuantityText.AddTextChangeListener(() => UpdateQuantity());
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
        if (_connectedCard != null)
        {
            _connectedCard.QuantityText.RemoveTextChangeListener(() => UpdateQuantity());
        }
        
    }
}
