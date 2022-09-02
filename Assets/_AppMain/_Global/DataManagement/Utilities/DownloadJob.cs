using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DownloadJob 
{
    public Delegate job { get; set; }
    public string message { get; set; }
    public float minVal { get; set; }
    public float maxVal { get; set; }
    public float startVal { get; set; }
    public float Value { get; set; }
    public event Action<float> OnJobUpdate;
    public bool IsComplete { get; set; }
   


    public DownloadJob(string msg, float min, float max, float start, Delegate ac)
    {
        IsComplete = false;
        Value = start;
        job = ac;
        message = msg;
        minVal = min;
        maxVal = max;
        startVal = start;
    }

    public static DownloadJob Create(string msg, float min, float max, float start, Action ac)
    {
        return new DownloadJob(msg, min, max, start, ac);
    }
    public static DownloadJob Create<T>(string msg, float min, float max, float start, Action<T> ac)
    {
        return new DownloadJob(msg, min, max, start, ac);
    }
    

    public void WorkEvent(float value)
    {
        OnJobUpdate?.Invoke(value);

    }
    public void CompleteJob()
    {
        IsComplete = true;
    }

    
}
