using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using UnityEngine.UI;

public class MagicInputText : MagicTextBox
{
    #region Enums
    public enum InputMode
    {
        DisplayMode = 0,
        EditMode = 1,
    }
    private bool IsDisplayMode { get => Mode == InputMode.DisplayMode; }
    #endregion



    #region Properties
    [SerializeField] private TMP_InputField InputText;
    [SerializeField] private Button editButton;
    [SerializeField] private Button displayButton;
    private InputMode _mode;
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

    #region Overrides
    protected override void Load()
    {
        base.Load();
        SaveInitialText = true;

    }

    public override void Refresh()
    {
        base.Refresh();
        SaveInitialText = true;
        _mode = InputMode.DisplayMode;
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


    /// <summary>
    /// Call this from a button or something else to initialize changing the modes.
    /// </summary>
    /// <param name="mode"></param>
    public void TrySetNewMode(int mode)
    {
        if (IsDisplayMode)
        {
            Mode = (InputMode)mode;
        }
        else
        {
            if (HasChanges())
            {
                App.AskYesNoCancel($"There are un-confirmed changes with this text. Do you want to Save the un-confirmed changes?", ConfirmChanges);
            }
            else
            {
                Mode = (InputMode)mode;
            }
        }
    }
    
    private void ConfirmChanges(PopupBox.PopupResponse response)
    {
        switch (response)
        {
            case PopupBox.PopupResponse.Cancel:
                break;
            case PopupBox.PopupResponse.Yes:
                SetDirty(true);
                Mode = InputMode.DisplayMode;
                break;
            case PopupBox.PopupResponse.No:
                Rollback();
                Mode = InputMode.DisplayMode;
                break;
        }
    }


    private void ToEditMode()
    {
        InputText.gameObject.SetActive(true);
        InputText.text = Content;
        InputText.caretPosition = InputText.text.Length;
        TextBox.gameObject.SetActive(false);
 
    }
    private void ToDisplayMode()
    {
        string textVal = InputText.text.Trim();
        if (textVal != Content.Trim())
        {
            SetText(textVal);
        }
        TextBox.gameObject.SetActive(true);
        InputText.text = "";
        InputText.gameObject.SetActive(false);
    }


    #endregion


    #region Validate Changes
    private bool HasChanges()
    {
        if (InputText.text.Trim() != Content.Trim()) { return true; }
        return false;

    }

    private void Rollback()
    {
        InputText.text = Content;
    }

    #endregion



    
 

   
}
