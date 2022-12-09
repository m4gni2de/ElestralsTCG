using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            _currentValue = value;
            ValueChanged();
        }
    }

    
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
        gameObject.SetActive(false);
    }
    void Refresh()
    {
        

    }
    public void Load(string title, float startVal, float changeVal, float min = float.MinValue, float max = float.MaxValue)
    {
        
        this.titleText.SetText(title);
        this.maxValue = max;
        this.minValue = min;
        this.changeValue = changeVal;
        _currentValue = startVal;
        qtyText.SetText(_currentValue.ToString());
        Show();
    }

    
    #endregion

    #region Value Events
    public event Action<float> OnValueChanged;

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
       
    }
    #endregion
}
