using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoadingBar : MonoBehaviour
{
    public static LoadingBar Instance { get { return AppManager.Instance.loadingBar; } }
    #region Properties
    public event Action OnLoadComplete;
    private bool _LoadComplete = false;
    public bool LoadComplete { get { return _LoadComplete; } }
    #endregion
    private Slider _slider = null;
    private Slider slider { get { _slider ??= GetComponentInChildren<Slider>(); return _slider; } }

    public float Value
    {
        get { return slider.value; }
        set
        {
            slider.value = value;
            countText.text = $"{slider.value} / {slider.maxValue} - {LoadPercent}%";
            _LoadComplete = value == slider.maxValue;
        }
    }
    public float LoadPercent { get { return (slider.value / slider.maxValue) * 100f; } }

    [SerializeField]
    private TMP_Text displayText;
    [SerializeField]
    private TMP_Text countText;

    private void Awake()
    {
        Hide();
        
    }
    public void Display(string display, float start, float max, float min = 0f)
    {
        gameObject.SetActive(true);
        _LoadComplete = false;
        slider.maxValue = max;
        slider.minValue = min;
        countText.text = $"{slider.minValue} / {slider.maxValue} - {LoadPercent}%";
        MoveSlider(start);
        SetText(display);

        if (start == max) { CompleteLoad(); }
    }
    public void Hide()
    {
        _LoadComplete = false;
        countText.text = "";
        displayText.text = "";
        gameObject.SetActive(false);
    }

    public void SetText(string txt)
    {
        displayText.text = txt;
    }

    public void MoveSlider(float amount)
    {
        Value += amount;
    }

    public void CompleteLoad()
    {
        Value = slider.maxValue;
        OnLoadComplete.Invoke();
    }

    
}
