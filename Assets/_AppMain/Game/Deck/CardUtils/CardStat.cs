using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStat<T>
{
    public string StatName;
    public T BaseValue;
    public T Value;
    


    private List<T> _history = null;
    public List<T> History
    {
        get
        {
            _history ??= new List<T>();
            return _history;
        }
    }

    public CardStat(string statName, T baseValue)
    {
        StatName = statName;
        BaseValue = baseValue;
        ChangeValue(baseValue);
    }

    public static CardStat<T> Default(string statName)
    {
        return new CardStat<T>(statName, (T)default);
    }

    public void ChangeValue(T newVal)
    {
        History.Add(newVal);
        Value = newVal;
    }
}
