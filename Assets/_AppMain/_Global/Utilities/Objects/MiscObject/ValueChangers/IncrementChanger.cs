using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IncrementChanger : MonoBehaviour
{
    #region Properties
    [SerializeField] private MagicTextBox titleText;
    [SerializeField] private MagicTextBox qtyText;
    [SerializeField] private Button upBtn, downBtn;


    [SerializeField] private float changeValue = 1f;
    [SerializeField] private float maxValue;
    [SerializeField] private float minValue;
    [SerializeField] private SpriteDisplay bg;

    private float _currentValue = 0;
    public float CurrentValue
    {
        get { return _currentValue; }
        private set
        {
            bool didChange = value != _currentValue;
            _currentValue = value;
            if (didChange) { ValueChanged(); }
        }
    }

    #region Functions
    public float UpValue { get { return changeValue; } }
    public float DownValue { get { return -changeValue; } }
    #endregion

    #endregion

    #region Initialization
    private void Awake()
    {

    }
    private void Start()
    {
    }
    void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        OnValueChanged.RemoveAllListeners();
        gameObject.SetActive(false);
    }
    void Refresh()
    {
        OnValueChanged.RemoveAllListeners();

    }
    public void Load(string title, float startVal, float changeVal, float min = float.MinValue, float max = float.MaxValue)
    {
        Refresh();

        this.titleText.SetText(title);
        this.maxValue = max;
        this.minValue = min;
        this.changeValue = changeVal;
        _currentValue = startVal;
        Show();
        ValueChanged();
    }

    #endregion

    #region Value Events
    private UnityEvent<float> _onValueChanged = null;
    public UnityEvent<float> OnValueChanged
    {
        get
        {
            _onValueChanged ??= new UnityEvent<float>();
            return _onValueChanged;
        }
    }
    private void ValueChanged()
    {
        qtyText.SetText(CurrentValue.ToString());
        OnValueChanged?.Invoke(CurrentValue);
    }
    public void SetValue(float newVal)
    {
        if (newVal >= maxValue) { newVal = maxValue; }
        if (newVal <= minValue) { newVal = minValue; }
        CurrentValue = newVal;
    }
    public void AddValueListener(Action<float> ac)
    {
        OnValueChanged.RemoveAllListeners();
        UnityAction<float> uac = new UnityAction<float>(ac);
        OnValueChanged.AddListener(uac);
    }
    #endregion

    #region Buttons
    public void ButtonClick(bool isUp)
    {
        float newVal = CurrentValue + changeValue;
        if (!isUp) { newVal = CurrentValue - changeValue; }
        SetValue(newVal);
    }
    public void ToggleUpButton(bool isInteractable)
    {
        upBtn.interactable = isInteractable;
    }
    public void ToggleDownButton(bool isInteractable)
    {
        downBtn.interactable = isInteractable;
    }
    #endregion


    #region UI
    private List<Renderer> _renderers = null;
    protected List<Renderer> Renderers
    {
        get
        {
            if (_renderers == null)
            {
                _renderers = new List<Renderer>();
                _renderers.Add(titleText.textRenderer);
                _renderers.Add(qtyText.textRenderer);
            }
            return _renderers;
        }
    }
    public void SetSortingLayer(string sortLayer)
    {
        for (int i = 0; i < Renderers.Count; i++)
        {
            Renderers[i].sortingLayerName = sortLayer;
        }
        bg.SetSortLayer(sortLayer);

    }

    public void SetSortingOrder(int order)
    {

        for (int i = 0; i < Renderers.Count; i++)
        {
            Renderers[i].sortingOrder = order + 1;
        }
        bg.SetSortOrder(order);
    }
    #endregion
    #region Clean Up
    private void OnDestroy()
    {
        if (_onValueChanged != null)
        {
            OnValueChanged.RemoveAllListeners();
            _onValueChanged = null;
        }
    }
    #endregion
}
