using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInput : InputBox, iRemoteAsset
{
    public static string AssetName { get { return RemoteAssetHelpers.GetAssetName<TextInput>(); } }
    private static readonly int DefaultMaxLength = 25;
    private static readonly int DefaultMinLength = 2;

    #region Instance
    private static TextInput _Instance = null;
    public static TextInput Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject go = AssetPipeline.GameObjectClone(AssetName, WorldCanvas.Instance.transform);
                _Instance = go.GetComponent<TextInput>();
            }
            return _Instance;
        }
    }
   
    #endregion

    #region Properties
    public string Value
    {
        get
        {
            return _input.text;
        }
        set
        {
            _input.text = value.Trim();
        }
    }

   
    #endregion


    #region Initalization
    public static TextInput Load(string title, Action<string> returnedStr, int min = -1, int max = -1)
    {
        Instance.Open(title, returnedStr, min, max);
        Instance.transform.localPosition = Vector2.zero;

        return Instance;
    }

    public static TextInput Reload(string title, Action<string> returnedStr, int min = -1, int max = -1)
    {
        Instance.Open(title, returnedStr, min, max);
        Instance.transform.localPosition = Vector2.zero;

        return Instance;
    }

    public void Open(string title, Action<string> returnedStr, int minCharacters = -1, int maxCharacters = -1)
    {
        IsHandled = false;
        gameObject.SetActive(true);
        placeHolder.text = title;
        Value = "";
        titleText.text = title;
        if (minCharacters >= 0)
        {
            minVal = minCharacters;
        }
        else
        {
            minVal = DefaultMinLength;
        }

        if (maxCharacters >= 0)
        {
            maxVal = maxCharacters;
        }
        else
        {
            maxVal = DefaultMaxLength;
        }

        Input(returnedStr);
    }

    public void Reload(Action<string> returnedStr)
    {
        IsHandled = false;
        gameObject.SetActive(true);
        Input(returnedStr);
    }
    #endregion

    #region Functionality
    protected void Input(Action<string> returnedStr)
    {

        StartCoroutine(AwaitInput(callback =>
        {
            if (callback)
            {
                string val = Value;
                returnedStr(val);
                Close();
            }
            else
            {
                Close();
            }
            
        }));
    }

    protected IEnumerator AwaitInput(Action<bool> callback)
    {

        do
        {
            yield return new WaitForEndOfFrame();
        } while (true && !IsHandled);

        if (Result == InputResult.Confirm)
        {
            callback(true);
        }
        if (Result == InputResult.Cancel)
        {
            callback(false);
        }
    }
    #endregion

    protected void SetText(string text)
    {
        Value = text;
    }

   
    public override void Close()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
    }


    #region Commands
    protected bool Validate(string text)
    {
        if (text.Length > maxVal) { return App.DisplayError($"Username must not exceed {maxVal} characters!"); }
        if (text.Length < minVal) { return App.DisplayError($"Username must be at least {minVal} characters!"); }

        return true;
    }
    public override void Confirm()
    {
        
        if (Validate(_input.text))
        {
            SetText(Value);
            base.Confirm();
        }
        
    }
    public override void Cancel()
    {
        base.Cancel();
    }
    #endregion
}
