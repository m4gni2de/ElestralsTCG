using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

public class Rune : Card
{
    public enum RuneType
    {
        none = 0,
        Invoke = 1,
        Counter = 2,
        Artifact = 3,
        Stadium = 4,
        Divine = 5
    }

    #region Properties
    private RuneData _data = null;
    public RuneData Data { get { return _data; } }

    public RuneType GetRuneType
    {
        get
        {
            return (RuneType)Data.runeType;
        }
    }
    #endregion

    #region Overrides
    protected override iCardData GetCardData() { return Data; }
    protected override CardType GetCardType() { return CardType.Rune; }
    #endregion

    public Rune(RuneData data)
    {
        _data = data;
    }
}
