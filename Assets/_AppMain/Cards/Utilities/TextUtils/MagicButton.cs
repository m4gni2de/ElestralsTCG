using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MagicButton : MonoBehaviour
{
    #region Properties
    [SerializeField] private Button button;

    [SerializeField]
    protected UnityEvent _onClickEvent = null;
    public UnityEvent OnClickEvent { get { _onClickEvent ??= new UnityEvent(); return _onClickEvent; } }

    private bool _isListening = false;

    [SerializeField] private SpriteDisplay bgSprite;
    [SerializeField] private SpriteDisplay faceSprite;
    #endregion


    #region Button Properties/Functions
    private bool _canClick;
    public bool CanClick
    {
        get
        {
            _canClick = button.interactable;
            return _canClick;
        }
        set
        {
            if (button.interactable == value) { return; }
            button.interactable = value;
            InteractableChanged();
            if (value == false)
            {
                bgSprite.SetAlpha(.45f);
                faceSprite.SetAlpha(.45f);
            }
            else
            {
                bgSprite.SetAlpha(1f);
                faceSprite.SetAlpha(1f);
            }

        }
    }

    private void DoInteractable(bool canClick)
    {
        if (canClick)
        {
            bgSprite.SetAlpha(1f);
            faceSprite.SetAlpha(1f);

        }
        else
        {
            bgSprite.SetAlpha(.45f);
            faceSprite.SetAlpha(.45f);
        }
    }
    #endregion

    #region Events
    private UnityEvent _OnInteractableChanged = null;
    public UnityEvent OnInteractableChanged
    {
        get
        {
            _OnInteractableChanged ??= new UnityEvent();
            return _OnInteractableChanged;
        }
    }
    protected void InteractableChanged()
    {
        OnInteractableChanged?.Invoke();
    }
    #endregion


    #region Life Cycle
    private void Awake()
    {
        if (!_isListening)
        {
            button.onClick.AddListener(() => OnClick());
            _isListening = true;
        }
        DoInteractable(CanClick);
       
    }
    private void OnDestroy()
    {
        if (_isListening)
        {
            button.onClick.RemoveListener(() => OnClick());
            _isListening = false;
        }
    }
    #endregion

    #region On Click
    private void OnClick()
    {
        OnClickEvent?.Invoke();
    }
    
    #endregion
}
