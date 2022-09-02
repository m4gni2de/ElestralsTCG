using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class LoadingBar : MonoBehaviour
{
    public static LoadingBar Instance { get { return AppManager.Instance.loadingBar; } }
    
    #region Properties
    public event Action OnLoadComplete;
    private bool _LoadComplete = false;
    public bool LoadComplete { get { return _LoadComplete; } }
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
    public float LoadPercent { get { return (slider.value / slider.maxValue) * 100f; } }

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
        Hide();
        
    }

    public void AddJob(DownloadJob job)
    {
        LoadQueue.Add(job);
        if (LoadQueue.Count == 1)
        {
            StartJob(job);
        }
    }

    protected async void StartJob(DownloadJob job)
    {
        Display(job.message, job.startVal, job.maxVal, job.minVal);
        job.job.DynamicInvoke();
        job.OnJobUpdate += MoveSlider;

        do
        {
            await Task.Delay(1);


        } while (true && !LoadComplete);

        job.CompleteJob();
        job.OnJobUpdate -= MoveSlider;

        LoadQueue.Remove(job);
        if (LoadQueue.Count > 0)
        {
            StartJob(LoadQueue[0]);
        }
        else
        {
            Hide();
        }
    }

    
  
    public void Display(string display, float start, float max, float min = 0f)
    {
        gameObject.SetActive(true);
        _LoadComplete = false;
        slider.maxValue = max;
        slider.minValue = min;
        //countText.text = $"{slider.minValue} / {slider.maxValue} - {LoadPercent}%";
        countText.text = $"{LoadPercent}%";
        if (start >= max)
        {
            start = max;
        }
        slider.value = start;
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
        
        //OnLoadComplete?.Invoke();
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
