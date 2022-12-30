using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MagicInputText : MagicTextBox, iValidate
{
    #region Enums
    public enum InputMode
    {
        DisplayMode = 0,
        EditMode = 1,
    }
    private bool IsDisplayMode { get => Mode == InputMode.DisplayMode; }
    #endregion
    #region Interface
    private List<string> _errorList = null;
    public List<string> ErrorList { get { _errorList ??= new List<string>(); return _errorList; } }
    public void AddError(string msg)
    {
        ErrorList.Add(msg);
    }

    public bool Validate()
    {
        return ValidateInput();
    }
    #endregion

    #region Properties
    [SerializeField] private TMP_InputField InputText;
    [SerializeField] private Button editButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button displayButton;

    [SerializeField] private int minLength = 0;
    [SerializeField] private int maxLength = 30;
    [SerializeField] private InputMode _mode;
    public InputMode Mode
    {
        get { return _mode; }
        private set
        {
            InputMode current = _mode;
            _mode = value;
            if (current != value)
            {
                ModeChanged(value);
            }
        }
    }
    
    public string Input
    {
        get
        {
            return InputText.text.Trim();
        }
        set
        {
            InputText.text = value;
        }
    }
    
    private MagicTextBox _placeHolder = null;
    private MagicTextBox placeHolder
    {
        get
        {
            _placeHolder ??= InputText.placeholder.gameObject.GetOrAddComponent<MagicTextBox>();
            return _placeHolder;
        }
    }
    #endregion

    #region Settings
    public void SetLengthContraints(int min = 0, int max = 0)
    {
        if (min > 0)
        {
            minLength = min;
        }
        if (max > 0)
        {
            maxLength = max;
        }

        if (maxLength <= minLength)
        {
            maxLength = minLength + 1;
        }
    }
    #endregion

    #region Customization
    //private bool 
    #endregion

    #region Overrides
    protected override void Load()
    {
        base.Load();
        SaveInitialText = true;
        InputText.characterLimit = maxLength;
    }

    public override void Refresh(bool clearListeners = true)
    {
        base.Refresh();
        SaveInitialText = true;
        _mode = InputMode.DisplayMode;
    }
    #endregion

    #region Events
    public void OnInputChanged(string txt)
    {
        OnTextChanged?.Invoke();
    }
    #endregion

    #region Initialize
    private void Awake()
    {
        IsDirty = false;
        if (_mode == InputMode.DisplayMode)
        {
            TextBox.gameObject.SetActive(true);
            Input = "";
            InputText.gameObject.SetActive(false);
        }
        else if (_mode == InputMode.EditMode)
        {
            InputText.gameObject.SetActive(true);
            Input = Content;
            TextBox.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Mode Toggling
    /// <summary>
    /// Called after the Mode property changes to a new value. Any verifcation is done prior to this.
    /// </summary>
    /// <param name="newMode"></param>
    private void ModeChanged(InputMode newMode)
    {
        if (newMode == InputMode.DisplayMode)
        {
            ToDisplayMode();
        }
        else if (newMode == InputMode.EditMode)
        {
            ToEditMode();
        }
    }

    public void SetToDefault(int mode, bool clearAll, bool saveFirstText)
    {
        IsDirty = false;
        SaveInitialText = saveFirstText;
        if ((InputMode)mode == InputMode.DisplayMode)
        {
            TextBox.gameObject.SetActive(true);
            if (clearAll)
            {
                InputText.SetTextWithoutNotify("");

            }
            else
            {
                InputText.SetTextWithoutNotify(Content);
            }
            InputText.gameObject.SetActive(false);
        }
        else if (_mode == InputMode.EditMode)
        {
            InputText.gameObject.SetActive(true);
            if (clearAll)
            {
                InputText.SetTextWithoutNotify("");
            }
            else
            {
                InputText.SetTextWithoutNotify(Content);
            }

            TextBox.gameObject.SetActive(false);
        }
    }

    public void ToggleInputEnabled(bool enabled)
    {
        editButton.interactable = enabled;
        InputText.interactable = enabled;
    }
    #endregion

    #region Button Listeners
    public void EditButtonClick()
    {
        Mode = InputMode.EditMode;
    }
    public void DisplayButtonClick()
    {
        if (!ValidateInput())
        {
            if (ErrorList.Count > 1)
            {
                App.DisplayError($"There are multiple errors with your input.");
                return;
            }
            else if (ErrorList.Count == 1)
            {
                App.DisplayError($"{ErrorList[0]}");
                return;
            }
        }
        else
        {
            if (Input != Content) { SetDirty(true); }
            Mode = InputMode.DisplayMode;

        }
    }
    

    public void CancelButtonClick()
    {
        Rollback();
        Mode = InputMode.DisplayMode;
    }

    public void CancelButtonClickSilent()
    {
        Rollback();
        Mode = InputMode.EditMode;
    }
    public void ClearInput()
    {
        bool hasInput = !Input.IsEmpty();
        Content = "";
        Input = "";
        Mode = InputMode.EditMode;

        if (hasInput)
        {
            OnTextChanged?.Invoke();
        }
        
    }

    private void ToEditMode()
    {
        InputText.gameObject.SetActive(true);
        Input = Content;
        InputText.MoveTextEnd(true);
        TextBox.gameObject.SetActive(false);

    }
    private void ToDisplayMode()
    {
        if (IsDirty)
        {
            SetText(Input);
        }
        
        TextBox.gameObject.SetActive(true);
        Input = "";
        InputText.gameObject.SetActive(false);
    }

    public void AddDisplayListener(UnityAction ac, bool removeOthers = true)
    {
        if (removeOthers)
        {
            displayButton.onClick.RemoveAllListeners();
        }
        displayButton.onClick.AddListener(ac);
    }
    public void AddEditListener(UnityAction ac, bool removeOthers = true)
    {
        if (removeOthers)
        {
            editButton.onClick.RemoveAllListeners();
        }
        editButton.onClick.AddListener(ac);
    }
    #endregion
    //public void TrySetNewMode(int mode)
    //{
    //    if (IsDisplayMode)
    //    {
    //        Mode = (InputMode)mode;
    //    }
    //    else
    //    {
    //        if (HasChanges())
    //        {
    //            App.AskYesNoCancel($"There are un-confirmed changes with this text. Do you want to Save the un-confirmed changes?", ConfirmChanges);
    //        }
    //        else
    //        {
    //            Mode = (InputMode)mode;
    //        }
    //    }
    //}

    //private void ConfirmChanges(PopupBox.PopupResponse response)
    //{
    //    switch (response)
    //    {
    //        case PopupBox.PopupResponse.Cancel:
    //            break;
    //        case PopupBox.PopupResponse.Yes:
    //            SetDirty(true);
    //            Mode = InputMode.DisplayMode;
    //            break;
    //        case PopupBox.PopupResponse.No:
    //            Rollback();
    //            Mode = InputMode.DisplayMode;
    //            break;
    //    }
    //}







    #region Validate Changes
    private bool ValidateInput()
    {
        ErrorList.Clear();
        if (Mode != InputMode.EditMode) { return true; }
        if (Input.Length < minLength) { AddError($"Input text must be at least {minLength} Characters!"); }
        return ErrorList.Count <= 0;
    }
    private bool HasChanges()
    {
        if (Input != Content) { return true; }
        return false;

    }



    private void Rollback()
    {
        InputText.SetTextWithoutNotify(Content);
        InputText.MoveTextEnd(true);
    }

    

    #endregion







}
