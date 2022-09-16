using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class TouchObject : MonoBehaviour, iFreeze
{


    #region Static Properties
    private static List<int> _LayerFilters = null;
    public static List<int> LayerFilters
    {
        get
        {
            _LayerFilters ??= new List<int>();
            return _LayerFilters;
        }
    }

    public static void AddFilter(int layer)
    {
        if (!LayerFilters.Contains(layer))
        {
            LayerFilters.Add(layer);
        }
    }
    #endregion
    #region Properties
    private RectTransform Source;
    [SerializeField]
    private UnityEvent _OnClickEvent;
    public UnityEvent OnClickEvent { get { _OnClickEvent ??= new UnityEvent(); return _OnClickEvent; } }

    private UnityEvent _OverrideClickEvent;
    public UnityEvent OverrideClickEvent { get { _OverrideClickEvent ??= new UnityEvent(); return _OverrideClickEvent; } }

    [SerializeField]
    private UnityEvent _OnHoldEvent;
    public UnityEvent OnHoldEvent { get { _OnHoldEvent ??= new UnityEvent(); return _OnHoldEvent; } }


    #region Customization

    [Tooltip("Use this to allow the object to recieve click/touch events at all.")]
    public bool Interactable = true;
    [Tooltip("If true, this object's touches can be blocked by UI objects.")]
    [Header("Customize Clickability")]
    public bool IsMaskable;
    [Tooltip("If true, this object's touches are still live even when the app is in Frozen Mode.")]
    public bool bypassFreeze;
    [Tooltip("If true, this object's touches ignore all Validation aside from if the object was clicked on. Override Listeners will be used.")]
    public bool ForceClickOverride = false;
    #endregion


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

    #region Event Management
    private List<UnityAction> _ClickListeners = null;
    protected List<UnityAction> ClickListeners
    {
        get
        {
            _ClickListeners ??= new List<UnityAction>();
            return _ClickListeners;
        }
    }
    public void AddClickListener(UnityAction ac)
    {
        if (!ClickListeners.Contains(ac))
        {
            ClickListeners.Add(ac);
            OnClickEvent.AddListener(ac);
        }
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;

    }
    public void RemoveClickListener(UnityAction ac)
    {
        if (ClickListeners.Contains(ac))
        {
            ClickListeners.Remove(ac);
            OnClickEvent.RemoveListener(ac);
        }
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;

    }

    private List<UnityAction> _HoldListeners = null;
    protected List<UnityAction> HoldListeners
    {
        get
        {
            _HoldListeners ??= new List<UnityAction>();
            return _HoldListeners;
        }
    }
    public void AddHoldListener(UnityAction ac)
    {
        if (!HoldListeners.Contains(ac))
        {
            HoldListeners.Add(ac);
            OnHoldEvent.AddListener(ac);
        }
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;

    }
    public void RemoveHoldListener(UnityAction ac)
    {
        if (HoldListeners.Contains(ac))
        {
            HoldListeners.Remove(ac);
            OnHoldEvent.RemoveListener(ac);
        }
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;

    }

    public void OverrideClick(UnityAction ac)
    {
        ForceClickOverride = true;
        OverrideClickEvent.RemoveAllListeners();
        OverrideClickEvent.AddListener(ac);
    }
    public void RemoveOverrideClick()
    {
        ForceClickOverride = false;
        OverrideClickEvent.RemoveAllListeners();
    }
    public void FreezeClick()
    {
        Interactable = false;
    }
    public void CheckFreeze()
    {
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;
    }
    #endregion

    #region Tap Properties
    private float _holdTime = 0f;
    public float HoldTime { get { return _holdTime; } }

    //private int _LastInput = -1;

    [Tooltip("Defines the minimum time a tap needs to be held down before it's considered Held.")]
    public float HoldThreshold = 1f;

    [Tooltip("Defines the maximum time between two taps to make it double tap.")]
    [SerializeField] private float tapThreshold = 0.10f;
    private float tapTimer = 0.0f;


    private List<float> _TapTimes = null;
    public List<float> TapTimes { get { _TapTimes ??= new List<float>(); return _TapTimes; } }

    private List<string> _ErrorList = null;
    protected List<string> ErrorList { get { _ErrorList ??= new List<string>(); return _ErrorList; } }
    protected bool Validate()
    {
        

        if (!Source.IsPointerOverMe()) { return false; }
        if (ForceClickOverride) { return true; }
        if (!Interactable) { return false; }

        ErrorList.Clear();

        if (IsMaskable && UIHelpers.IsPointerOverUIObject())
        {
            AddError("This button is Maskable and is behind a UI Item.");
        }

        if (LayerFilters.Count > 0)
        {
            if (!LayerFilters.Contains(gameObject.layer))
            {
                AddError("This button belongs to a layer that is not in the Layer Filter.");
            }
        }

        if (AppManager.IsFrozen)
        {
            if (!bypassFreeze)
            {
                AddError("App is in Freeze Mode and this Button is not set to Bypass Freeze Mode.");
            }
        }

            return ErrorList.Count == 0;
    }

    protected void AddError(string msg)
    {
        ErrorList.Add(msg);
    }
    #endregion

    
    private void Awake()
    {
        Source = GetComponent<RectTransform>();
    }

    #region Click Listeners
    public void ClearClick()
    {
        OnClickEvent.RemoveAllListeners();
        ClickListeners.Clear();
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;
    }
    public void ClearHold()
    {
        HoldListeners.Clear();
        OnHoldEvent.RemoveAllListeners();
        Interactable = ClickListeners.Count + HoldListeners.Count > 0;
    }
    public void ClearAll()
    {
        ClearClick();
        ClearHold();
    }
   
    #endregion

    
    private void Update()
    {

       if (Interactable)
        {
            CheckTouch();   
        }
        


    }
    protected virtual void CheckTouch()
    {
        if (!IsClicked)
        {
            if (Input.GetMouseButtonDown(0) && Validate())
            {
                StartClick();
               
            }
           
        }
        else
        {
            if (!Interactable)
            {
                Cancel();
                
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
            
        }

        //if (Input.GetMouseButton(0)) { _LastInput = 0; } else if (!Input.GetMouseButton(0)) { _LastInput = -1; }
    }

    

    protected void StartClick()
    {
        _isClicked = true;
        _holdTime = 0f;
        tapTimer = Time.time;
        DoFreeze();



    }
    protected void EndClick()
    {
        DoThaw();
        _isClicked = false;
        if (!_isHeld) { DoClick(); }
        IsHeld = false;
        


    }

    protected void Cancel()
    {
        DoThaw();
        _isClicked = false;
        _isHeld = false;
        _holdTime = 0f;
        
    }


    protected void DoClick()
    {
        if (!ForceClickOverride)
        {
            OnClickEvent?.Invoke();
        }
        else
        {
            OverrideClickEvent?.Invoke();
        }
        

    }
    protected void DoHold()
    {
        OnHoldEvent?.Invoke();
    }
    protected void DoDoubleClick()
    {
        Debug.Log("here");
    }


    #region Game Freezing
    protected void DoFreeze()
    {
        this.Freeze();
    }

    protected void DoThaw()
    {
        this.Thaw();
    }
    #endregion
}
