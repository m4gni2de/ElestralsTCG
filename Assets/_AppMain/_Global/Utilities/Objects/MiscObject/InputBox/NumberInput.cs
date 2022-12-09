using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NumberInput : InputBox, iRemoteAsset
{
    #region Instance
    private static NumberInput _Instance = null;
    public static NumberInput Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject go = AssetPipeline.GameObjectClone(AssetName, WorldCanvas.Instance.transform);
                _Instance = go.GetComponent<NumberInput>();
            }
            return _Instance;
        }
    }
    private static string AssetName { get { return RemoteAssetHelpers.GetAssetName<NumberInput>(); } }

    #endregion
    #region Properties

    public int Value
    {
        get
        {
            return int.Parse(_input.text);
        }
        set
        {
            _input.text = value.ToString();
        }
    }

   
    protected void SetValueInput(int newVal)
    {
        Value = newVal;
    }
    #endregion
    public static NumberInput Load(Transform parent, string title, int startVal, int min = -1, int max = -1)
    {
        Instance.Open(title, startVal, min, max);
        Instance.transform.SetParent(parent, true);
        Instance.transform.localPosition = Vector2.zero;
        Instance.transform.localEulerAngles = Vector3.zero;
        
        return Instance;
    }


    #region Initialization
    private void Awake()
    {

    }

    public void Open(string title, int startVal, int min, int max)
    {
        DoFreeze();
        IsHandled = false;
        gameObject.SetActive(true);
        placeHolder.text = title;
        Value = startVal;
        titleText.text = title;
        minVal = min;
        maxVal = max;
    }
    public override void Close()
    {
        DoThaw();
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
      
    }
    #endregion


    #region Commands
    
    public override void Confirm()
    {
        SetValueInput(Value);
        base.Confirm();
        Close();
        
    }
    public override void Cancel()
    {
        base.Cancel();
        Close();
    }
    #endregion
}
