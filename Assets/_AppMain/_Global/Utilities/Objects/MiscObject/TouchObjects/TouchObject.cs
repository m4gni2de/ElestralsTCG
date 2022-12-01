using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using TouchControls;


public class TouchObject : ValidationObject, iFreeze
{
   

    #region Static Properties
    protected static List<int> _LayerFilters = null;
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

    
    private static List<TouchObject> _currentObjects = null;
    public static List<TouchObject> CurrentObjects
    {
        get
        {
            if (_currentObjects == null)
            {
                _currentObjects = new List<TouchObject>();
            }
            return _currentObjects;
        }
    }

    protected List<TouchObject> GetTappedObjects()
    {
        List<TouchObject> buttons = new List<TouchObject>();

        for (int i = 0; i < CurrentObjects.Count; i++)
        {
            TouchObject obj = CurrentObjects[i];
            if (obj.IsPointerOverMe())
            {
                buttons.Add(obj);
            }

        }
        return buttons;
    }
    #endregion
    #region Properties

    protected Transform Source;

    #region Events
    [SerializeField]
    protected UnityEvent _OnClickEvent;
    public UnityEvent OnClickEvent { get { _OnClickEvent ??= new UnityEvent(); return _OnClickEvent; } }

    protected UnityEvent _OverrideClickEvent;
    public UnityEvent OverrideClickEvent { get { _OverrideClickEvent ??= new UnityEvent(); return _OverrideClickEvent; } }

    [SerializeField]
    protected UnityEvent _OnHoldEvent;
    public UnityEvent OnHoldEvent { get { _OnHoldEvent ??= new UnityEvent(); return _OnHoldEvent; } }


    public event Action<TouchObject> OnThisClicked;
    protected void ClickObject()
    {
        OnThisClicked?.Invoke(this);
    }
    public event Action<TouchObject> OnThisHeld;
    protected void HoldObject()
    {
        OnThisHeld?.Invoke(this);
    }
    #endregion

    #region Customization

    [Tooltip("Use this to allow the object to recieve click/touch events at all.")]
    public bool Interactable = true;   
    [Header("Customize Clickability")]
    [Tooltip("If true, this object's touches are blocked by if it is within a UI object.")]
    public bool IsMaskable;
    [Tooltip("If true, this object's touches are still live even when the app is in Frozen Mode.")]
    public bool bypassFreeze;
    [Tooltip("If true, this object's touches ignore all Validation aside from if the object was clicked on. Override Listeners will be used.")]
    public bool ForceClickOverride = false;
    [Tooltip("If true, button will only register clicks if it's within a UI Object, like a Button")]
    public bool UIMode = false;
    [Tooltip("If true, any TouchObjects behind this object will not have touches registered.")]
    public bool BlocksTouchesBehind = false;
    #endregion

    #region Group Management
    protected string GroupId;
    protected bool HasGroup
    {
        get
        {
            if (string.IsNullOrEmpty(GroupId)) { return false; }
            return true;
        }
    }
    public void RemoveFromGroup()
    {
        if (!HasGroup) { return; }
        TouchGroup group = ObjectPool.FindByKey<TouchGroup>(GroupId);
        group.Remove(this);
    }
    #endregion

    #region Interaction Checks
    protected bool _isClicked = false;
    public bool IsClicked { get { return _isClicked; } }

    protected bool IsDoubleTap { get; set; }
    protected bool _isHeld = false;
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

                    TryHold();
                }
            }
            _isHeld = value;
        }
    }
    #endregion

    #endregion
    #region Event Management

    #region OnClick
    protected List<UnityAction> _ClickListeners = null;
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
        Interactable = GetInteractable(true, false);

    }
    public void RemoveClickListener(UnityAction ac)
    {
        if (ClickListeners.Contains(ac))
        {
            ClickListeners.Remove(ac);
            OnClickEvent.RemoveListener(ac);
        }
        Interactable = GetInteractable(true, false);

    }
    #endregion

    #region OnHold
    protected List<UnityAction> _HoldListeners = null;
    protected List<UnityAction> HoldListeners
    {
        get
        {
            _HoldListeners ??= new List<UnityAction>();
            return _HoldListeners;
        }
    }
    public void AddHoldListener(UnityAction ac, float holdTimeThreshold = 0f)
    {
        if (!HoldListeners.Contains(ac))
        {
            HoldListeners.Add(ac);
            OnHoldEvent.AddListener(ac);
        }

        if (holdTimeThreshold > 0f)
        {
            holdThreshold = holdTimeThreshold;
        }
        else
        {
            holdThreshold = m_holdThresholdDefault;
        }
        Interactable = GetInteractable(true, false);

    }
    public void RemoveHoldListener(UnityAction ac)
    {
        if (HoldListeners.Contains(ac))
        {
            HoldListeners.Remove(ac);
            OnHoldEvent.RemoveListener(ac);
        }
        Interactable = GetInteractable(true, false);

    }
    #endregion

    #region OverrideClick
    protected List<UnityAction> _OverClickListeners = null;
    protected List<UnityAction> OverClickListeners
    {
        get
        {
            _OverClickListeners ??= new List<UnityAction>();
            return _OverClickListeners;
        }
    }
    public void AddOverrideClickListener(UnityAction ac)
    {
        if (!OverClickListeners.Contains(ac))
        {
            OverClickListeners.Add(ac);
            OverrideClickEvent.AddListener(ac);
        }
    }
    public void RemoveOverrideClickListener(UnityAction ac)
    {
        if (OverClickListeners.Contains(ac))
        {
            OverClickListeners.Remove(ac);
            OverrideClickEvent.RemoveListener(ac);
        }

    }
    public void SetOverrideClick(UnityAction ac)
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
    #endregion


    public void FreezeClick()
    {
        Interactable = false;
    }
    public void CheckFreeze()
    {
        Interactable = GetInteractable(true, false);
    }

    private int PersistentListenerCount(bool includeHold = false, bool includeOverride = false)
    {
        int count = OnClickEvent.GetPersistentEventCount();
        if (includeHold) { count += OnHoldEvent.GetPersistentEventCount(); }
        if (includeOverride) { count += OverrideClickEvent.GetPersistentEventCount(); }
        return count;
        
    }
    private int AddedListenerCount(bool includeHold = false, bool includeOverride = false)
    {
        int count = ClickListeners.Count;
        if (includeHold) { count += HoldListeners.Count; }
        if (includeOverride) { count += OverClickListeners.Count; }
        return count;

    }
    private bool GetInteractable(bool includeHold = false, bool includeOverride = false)
    {
        bool isInteractable = false;
        isInteractable = (AddedListenerCount(includeHold, includeOverride) > 0 || PersistentListenerCount(includeHold, includeOverride) > 0);
        return isInteractable;
    }
    #endregion

    #region Click Listener Management
    public void ClearClick()
    {
        OnClickEvent.RemoveAllListeners();
        ClickListeners.Clear();
        Interactable = GetInteractable(true, false);
    }
    public void ClearHold()
    {
        HoldListeners.Clear();
        OnHoldEvent.RemoveAllListeners();
        Interactable = GetInteractable(true, false);
    }
    public void ClearAll()
    {
        ClearClick();
        ClearHold();
    }

    #endregion

    #region Tap Properties
    protected float m_holdThresholdDefault = 1f;
    protected float _holdTime = 0f;
    public float HoldTime { get { return _holdTime; } }

    //protected int _LastInput = -1;

    [Tooltip("Defines the minimum time a tap needs to be held down before it's considered Held.")]
    public float holdThreshold = 1f;

    [Tooltip("Defines the maximum time between two taps to make it double tap.")]
    [SerializeField] protected float tapThreshold = 0.10f;
    protected float tapTimer = 0.0f;


    protected List<float> _TapTimes = null;
    public List<float> TapTimes { get { _TapTimes ??= new List<float>(); return _TapTimes; } }
    #endregion

    #region Validation/Functions
    public virtual bool IsPointerOverMe()
    {
        RectTransform rect = (RectTransform)Source;
        return rect.IsPointerOverMe();
    }
    public virtual bool IsBlocked()
    {
        for (int i = 0; i < CurrentObjects.Count; i++)
        {
            TouchObject obj = CurrentObjects[i];
            if (obj != this) { continue;}
            if (obj.BlocksTouchesBehind && obj.GetSortValue() > GetSortValue())
            {
                return true;
            }
        }
        return false;
    }



    public override bool Validate()
    {

        if (!IsPointerOverMe()) { return false; }
        if (IsBlocked())
        {
            AddError("This is blocked by another TouchObject overlapping it");
        }
        if (ForceClickOverride) { return true; }
        if (!Interactable) { return false; }

        ErrorList.Clear();

        if (IsMaskable && UIHelpers.IsPointerOverUIObject())
        {
            AddError("This button is Maskable and is behind a UI Item.");
        }
        if (UIMode && !UIHelpers.IsPointerOverUIObject())
        {
            AddError("This button is in UI mode and needs to be behind a UI item.");
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

    
    #endregion


    protected void Awake()
    {
        SetBounds();
        if (!CurrentObjects.Contains(this))
        {
            CurrentObjects.Add(this);
        }
        
    }

    private void OnDestroy()
    {
        if (CurrentObjects.Contains(this))
        {
            CurrentObjects.Remove(this);
        }
    }

    protected void Start()
    {
       
    }


    protected virtual void SetBounds()
    {
        Source = GetComponent<RectTransform>();

    }
    public void AddToGroup(TouchGroup group)
    {
        GroupId = group.groupName;
    }

    
    protected void Update()
    {
        CheckTouch();  

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
                IsHeld = HoldTime >= holdThreshold;
                if (!Input.GetMouseButton(0))
                {
                    EndClick();
                }
            }

        }

       
    }

   

    public virtual void StartClick()
    {
        _isClicked = true;
        _holdTime = 0f;
        tapTimer = Time.time;
        DoFreeze();

    }
    public virtual void EndClick()
    {
        DoThaw();
        _isClicked = false;
        if (!_isHeld) { TryClick(); }
        IsHeld = false;
        _holdTime = 0f;

    }

    public virtual void Cancel()
    {
        DoThaw();
        _isClicked = false;
        _isHeld = false;
        _holdTime = 0f;

    }


    protected void TryClick()
    {
        
       
        if (!HasGroup)
        {
            DoClick();
        }
        else
        {
            ClickObject();
        }
    }
    public void DoClick()
    {
        UnityEvent clickEvent = OnClickEvent;
        if (ForceClickOverride)
        {
            clickEvent = OverrideClickEvent;
        }
        clickEvent?.Invoke();

    }
    protected void TryHold()
    {
        if (!HasGroup)
        {
            DoHold();
        }
        else
        {
            HoldObject();
        }
       
    }
    public void DoHold()
    {
        OnHoldEvent?.Invoke();
    }
    protected void DoDoubleClick()
    {
        //Debug.Log("here");
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



    #region Comparing
    public virtual float GetSortValue()
    {
        return Source.localPosition.z;
       
    }
    #endregion
}
