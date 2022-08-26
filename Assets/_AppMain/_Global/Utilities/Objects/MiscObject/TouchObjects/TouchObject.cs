using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class TouchObject : MonoBehaviour
{
    #region Properties
    private RectTransform Source;
    [SerializeField]
    private UnityEvent _OnClickEvent;
    public UnityEvent OnClickEvent { get { _OnClickEvent ??= new UnityEvent(); return _OnClickEvent; } }

    private UnityEvent _OnHoldEvent;
    public UnityEvent OnHoldEvent { get { _OnHoldEvent ??= new UnityEvent(); return _OnHoldEvent; } }

    [Tooltip("If true, this object's touches can be blocked by UI objects.")]
    public bool IsMaskable;
    #region Interaction Checks
    private bool _isClicked = false;
    public bool IsClicked { get { return _isClicked; } }

    private bool IsDoubleTap { get; set; }
    private bool _isHeld = false;
    public bool IsHeld
    {
        get
        {
            return _isHeld;
        }
        set
        {
            if (value != _isHeld)
            {
                if (value == true)
                {

                    DoHold();
                }
            }
            _isHeld = value;
        }
    }
    #endregion

    #endregion

    #region Tap Properties
    private float _holdTime = 0f;
    public float HoldTime { get { return _holdTime; } }

    private int _LastInput = -1;

    [Tooltip("Defines the minimum time a tap needs to be held down before it's considered Held.")]
    public float HoldThreshold = 1f;

    [Tooltip("Defines the maximum time between two taps to make it double tap.")]
    [SerializeField] private float tapThreshold = 0.10f;
    private float tapTimer = 0.0f;


    private List<float> _TapTimes = null;
    public List<float> TapTimes { get { _TapTimes ??= new List<float>(); return _TapTimes; } }
    #endregion

    private void Awake()
    {
        Source = GetComponent<RectTransform>();
    }

    #region Click Listeners
    public void ClearClick()
    {
        OnClickEvent.RemoveAllListeners();
    }
    public void ClearHold()
    {
        OnHoldEvent.RemoveAllListeners();
    }
    public void ClearAll()
    {
        ClearClick();
        ClearHold();
    }
    #endregion

    
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (Source.IsPointerOverMe())
        //    {
        //        if (Time.time <= tapTimer + tapThreshold)
        //        {
        //            DoDoubleClick();
        //            Cancel();
        //            return;
        //        }
        //        StartClick();
        //    }
        //}

        //if (IsClicked)
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        _holdTime += Time.deltaTime;
        //        IsHeld = HoldTime >= HoldThreshold;
        //    }
        //    else
        //    {
        //        EndClick();
        //    }

        //}
        //if (IsClicked)
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        _holdTime += Time.deltaTime;
        //        IsHeld = HoldTime >= HoldThreshold;
        //    }
        //    else
        //    {
        //        if (Time.time > tapTimer + tapThreshold)
        //        {
        //            EndClick();
        //        }
        //        else
        //        {
        //            Cancel();
        //        }

        //    }

        //}

        CheckTouch();
        
    }
    protected virtual void CheckTouch()
    {
        if (!IsClicked)
        {
            if (!IsMaskable)
            {
                if (Input.GetMouseButtonDown(0) && Source.IsPointerOverMe()) { StartClick(); }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && Source.IsPointerOverMe() && !UIHelpers.IsPointerOverUIObject()) { StartClick(); }
            }
            

        }
        else
        {
            _holdTime += Time.deltaTime;
            IsHeld = HoldTime >= HoldThreshold;
            if (!Input.GetMouseButton(0))
            {
                EndClick();
            }
        }

        if (Input.GetMouseButton(0)) { _LastInput = 0; } else if (!Input.GetMouseButton(0)) { _LastInput = -1; }
    }

    

    protected void StartClick()
    {
        _isClicked = true;
        _holdTime = 0f;
        tapTimer = Time.time;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Freeze(true);
        }
        
       
    }
    protected void EndClick()
    {
        
        _isClicked = false;
        if (!_isHeld) { DoClick(); }
        IsHeld = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Freeze(false);
        }
        

    }

    protected void Cancel()
    {
        _isClicked = false;
        _isHeld = false;
        _holdTime = 0f;
        GameManager.Instance.Freeze(false);
    }


    protected void DoClick()
    {
        OnClickEvent.Invoke();
    }
    protected void DoHold()
    {
        OnHoldEvent.Invoke();
    }
    protected void DoDoubleClick()
    {
        Debug.Log("here");
    }
    
}
