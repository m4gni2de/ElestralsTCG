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
                GameObject go = AssetPipeline.WorldObjectClone(AssetName);
                _Instance = go.GetComponent<NumberInput>();
            }
            return _Instance;
        }
    }
    private static string AssetName { get { return RemoteAssetHelpers.GetAssetName<NumberInput>(); } }

    #endregion
    #region Properties

    [SerializeField]
    private TMP_InputField _input;
    [SerializeField]
    private TMP_Text placeHolder;
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
    [SerializeField]
    private TMP_Text titleText;

    private int _minVal;
    public int minVal { get { return _minVal; } set { _minVal = value; } }
    private int _maxVal;
    public int maxVal { get { return _maxVal; } set { _maxVal = value; } }

    protected void SetValueInput(int newVal)
    {
        Value = newVal;
    }
    #endregion
    public static NumberInput Load(Transform parent, string title, int startVal, int min = -1, int max = -1)
    {
        Instance.Open(title, startVal, min, max);
        Instance.transform.SetParent(parent);
        Instance.transform.localPosition = Vector2.zero;
        return Instance;
    }


    #region Initialization
    private void Awake()
    {

    }
    
    public void Open(string title, int startVal, int min, int max)
    {
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
    }
    public override void Cancel()
    {
        base.Cancel();
    }
    #endregion
}
