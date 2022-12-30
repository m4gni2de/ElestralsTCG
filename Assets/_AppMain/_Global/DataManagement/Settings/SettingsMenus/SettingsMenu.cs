using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using UnityEngine;
using nsSettings;
using Databases;

public abstract class SettingsMenu : MonoBehaviour, iShowHide
{
    #region Properties
    [SerializeField] protected GameObject background;
    protected CanvasGroup bgCanvas { get; private set; }

    #endregion
    #region Interface
    public event Action<bool> OnDisplayChanged;

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnDisplayChanged?.Invoke(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnDisplayChanged?.Invoke(true);
    }
    public abstract void LoadSettings();
    public abstract void Reload();
    public virtual bool HasChanges()
    {
        return false;
    }
    #endregion


    #region Initialization
    private void Awake()
    {
        if (bgCanvas == null)
        {
            bgCanvas = background.GetComponent<CanvasGroup>();
        }
    }

    
    #endregion



}

