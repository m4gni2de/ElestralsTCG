using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class LoadingBar : MonoBehaviour
{
    
    #region Properties
    private bool _LoadComplete = false;
    public bool LoadComplete { get { return _LoadComplete; } }

    public bool IsRunning = false;

    #endregion
    private Slider _slider = null;
    public Slider slider { get { _slider ??= GetComponentInChildren<Slider>(); return _slider; } }
    public bool IsActive = false;

    public float Value
    {
        get { return slider.value; }
        set
        {
            if (value > slider.maxValue)
            {
                value = slider.maxValue;
            }
            slider.value = value;
            countText.text = $"{LoadPercent}%";
            _LoadComplete = value >= slider.maxValue;
        }
    }

    private int roundedDigits = -1;
    public double LoadPercent
    {
        get
        {
            double val = (slider.value / slider.maxValue) * 100f;
            if (roundedDigits > -1)
            {
                return Math.Round(val, roundedDigits);
            }
            return val;
        }
    }
    
    [SerializeField]
    private TMP_Text displayText;
    [SerializeField]
    private TMP_Text countText;

    #region Queue
    private List<DownloadJob> _LoadQueue = null;
    public List<DownloadJob> LoadQueue { get { _LoadQueue ??= new List<DownloadJob>(); return _LoadQueue; } }
    #endregion


    private void Awake()
    {
        //Hide();
        
    }

    
    public void Display(string display, float start, float max, float min = 0f, int percentDecimals = -1)
    {
        gameObject.SetActive(true);
        _LoadComplete = false;
        IsRunning = true;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = start;
        roundedDigits = percentDecimals;
        if (start >= max)
        {
            start = max;
        }
        
        SetText(display);
        

        if (start == max) { CompleteLoad(); }
    }
    public void Hide()
    {
        _LoadComplete = false;
        countText.text = "";
        displayText.text = "";
        roundedDigits = -1;
        gameObject.SetActive(false);
    }

    public void SetText(string txt)
    {
        displayText.text = txt;
    }

    public void MoveSlider(float amount)
    {
        float newVal = Value + amount;
        Value = newVal;
    }
    public void SetSlider(float amount)
    {
        Value = amount;
    }

    public void CompleteLoad()
    {
        Value = slider.maxValue;
        _LoadComplete = true;
        IsRunning = false;
        gameObject.SetActive(false);
    }


    public IEnumerator AwaitCompletion(DownloadJob job, Action<bool> callBack)
    {
        WaitForSeconds waiter = new WaitForSeconds(.5f);
        do
        {
            yield return waiter;

        } while (true && LoadQueue.Contains(job));

        callBack(true);
    }

    
}
