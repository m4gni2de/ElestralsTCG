using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

public class Spirit : Card
{
    #region Properties
    private CardData _data = null;
    public CardData Data { get { return _data; } }
    #endregion

    #region Overrides
    protected override iCardData GetCardData() { return Data; }
    protected override CardType GetCardType() { return CardType.Spirit; }
    #endregion
    public Spirit(CardData data)
    {
        _data = data;
    }
}