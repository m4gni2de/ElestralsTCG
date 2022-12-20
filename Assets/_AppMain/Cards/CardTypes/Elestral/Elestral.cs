using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

public class Elestral : Card
{
   
    public enum SubClass
    {
        None = 0,
        Aquatic = 1,
        Archaic = 2,
        Avian = 3,
        Behemoth = 4,
        Brute = 5,
        Dragon = 6,
        Dryad = 7,
        Eldritch = 8,
        Ethereal = 9,
        Golem = 10,
        Insectoid = 11,
        Oceanic = 12
    }

    #region Properties
    private ElestralData _data = null;
    public ElestralData Data { get { return _data; } }
    #endregion

    #region Overrides
    protected override iCardData GetCardData() { return Data; }
    protected override CardType GetCardType() { return CardType.Elestral; }

    #endregion

    public Elestral(ElestralData data)
    {
        _data = data;

    }
}
