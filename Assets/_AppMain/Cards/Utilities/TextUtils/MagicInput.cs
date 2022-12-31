using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicInput : MonoBehaviour, iValidate
{
    #region Interfaces
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
    [SerializeField] protected TMP_InputField InputText;
    [SerializeField] protected int minLength = 0;
    [SerializeField] protected int maxLength = 30;
    //[SerializeField] protected Button cancelButton;

    

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
    #endregion

    #region Functions
    public bool CanInput
    {
        get
        {
            return InputText.interactable;
        }
    }
    #endregion

    #region Life Cycle
    private void Awake()
    {
        InputText.onValueChanged.AddListener(OnTextInputChanged);
        InputText.onSelect.AddListener(OnSelected);
        InputText.onSubmit.AddListener(OnSubmit);
    }
    private void OnDestroy()
    {
        InputText.onValueChanged.RemoveListener(OnTextInputChanged);
        InputText.onSelect.RemoveListener(OnSelected);
        InputText.onSubmit.RemoveListener(OnSubmit);
    }
    #endregion

    #region Listeners
    private void OnTextInputChanged(string txt)
    {

    }
    private void OnSelected(string txt)
    {

    }

    public event Action<MagicInput> OnSubmitInput;
    private void OnSubmit(string txt)
    {
        OnSubmitInput?.Invoke(this);
    }
    #endregion


    #region Validation
    protected bool ValidateInput()
    {
        ErrorList.Clear();
        if (Input.Length < minLength) { AddError($"Input text must be at least {minLength} Characters!"); }
        return ErrorList.Count <= 0;
    }


    #endregion

    #region Input Management
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
    public void ToggleInputEnabled(bool enabled)
    {
        InputText.interactable = enabled;
    }
    public void SetText(string text)
    {
        Input = text;
        InputText.MoveTextEnd(true);
    }
    public void ClearInput()
    {
        SetText("");
        InputText.MoveTextStart(true);
    }
    #endregion
}
