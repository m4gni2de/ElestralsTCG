using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    #region Properies
    public event Action OnSingleTap;
    public event Action OnDoubleTap;
    [Tooltip("Defines the maximum time between two taps to make it double tap")]
    [SerializeField] private float tapThreshold = 0.25f;
    private Action updateDelegate;
    [SerializeField]
    private float tapTimer = 0.0f;
    private bool tap = false;
    #endregion


    #region Static Properties
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

    public static void AddObject(TouchObject obj)
    {
        if (!CurrentObjects.Contains(obj))
        {
            CurrentObjects.Add(obj);
            Debug.Log(obj.name);
        }
    }

    private List<TouchObject> _tappedObjects = null;
    public List<TouchObject> TappedObjects
    {
        get
        {
            _tappedObjects ??= new List<TouchObject>();
            return _tappedObjects;
        }
        set
        {
            _tappedObjects = value;
        }
    }
    #endregion

    private void Awake()
    {
        
#if UNITY_EDITOR || UNITY_STANDALONE
        //updateDelegate = UpdateEditor;
        updateDelegate = TouchUpdate;
#elif UNITY_IOS || UNITY_ANDROID
        updateDelegate = UpdateMobile;
#endif
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        if (updateDelegate != null) { updateDelegate(); }
    }
    private void OnDestroy()
    {
        OnSingleTap = null;
        OnDoubleTap = null;
    }

    protected void TouchUpdate()
    {

        if (!this.tap)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.tap = true;
                this.tapTimer = Time.time;

                TappedObjects = GetTappedObjects();
            }
        }
        else
        {
            List<TouchObject> toRemove = new List<TouchObject>();

            for (int i = 0; i < TappedObjects.Count; i++)
            {
                TouchObject obj = TappedObjects[i];
                if (!obj.Interactable)
                {
                    obj.Cancel();
                }
                else
                {
                    float holdTime = Time.time - this.tapTimer;
                    obj.IsHeld = holdTime >= obj.holdThreshold;
                }

                if (!obj.IsClicked) { toRemove.Add(obj); }  
            }


            for (int i = 0; i < toRemove.Count; i++)
            {
                TappedObjects.Remove(toRemove[i]);
            }


            if (!Input.GetMouseButton(0))
            {
                for (int i = 0; i < TappedObjects.Count; i++)
                {
                    TappedObjects[i].EndClick();
                }

                TappedObjects.Clear();
                this.tap = false;
            }

            
            
        }

    }
#if UNITY_EDITOR || UNITY_STANDALONE
    private void UpdateEditor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < this.tapTimer + this.tapThreshold)
            {
                if (OnDoubleTap != null) { OnDoubleTap(); }
                this.tap = false;
                return;
            }
            this.tap = true;
            this.tapTimer = Time.time;

           
        }
        if (this.tap == true && Time.time > this.tapTimer + this.tapThreshold)
        {
            this.tap = false;
            if (OnSingleTap != null) { OnSingleTap(); }
        }
    }
#elif UNITY_IOS || UNITY_ANDROID
    private void UpdateMobile ()
    {
        for(int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                if(Input.GetTouch(i).tapCount == 2)
                {
                    if(OnDoubleTap != null){ OnDoubleTap();}
                }
                if(Input.GetTouch(i).tapCount == 1)
                {
                    if(OnSingleTap != null) { OnSingleTap(); }
                }
            }
        }
    }
#endif


    protected void ValidateTouch()
    {
        List<TouchObject> touches = GetTappedObjects();

        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].Validate() && !TappedObjects.Contains(touches[i]))
            {
                TappedObjects.Add(touches[i]);
                touches[i].StartClick();

            }
        }
    }

   

    protected void DoSingleTap()
    {
       
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
}

