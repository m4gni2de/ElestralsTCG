using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class SlotSelector
{
    #region Properties
    public int ExpectedCount = 0;
    private List<CardSlot> _Selections = null;
    public List<CardSlot> SelectedCards
    {
        get
        {
            return _Selections;
        }
    }
    public List<CardSlot> TargetSlots = null;

    #endregion
    public event Action<bool, List<CardSlot>> OnSelectionHandled;
    protected void DoSelection(bool isSuccess)
    {
        OnSelectionHandled?.Invoke(isSuccess, SelectedCards);
    }

    public SlotSelector(int countToSelect, CardSlot[] targets)
    {
        ExpectedCount = countToSelect;
        TargetSlots = new List<CardSlot>();
        _Selections = new List<CardSlot>();

        for (int i = 0; i < targets.Length; i++)
        {
            TargetSlots.Add(targets[i]);
        }
    }


    public void SelectSlot(CardSlot slot)
    {
        if (!SelectedCards.Contains(slot))
        {
            SelectedCards.Add(slot);
        }
    }
    
    public static SlotSelector AttackSlots(Player defender, int attacks, bool canDirectAttack = false)
    {
        List<CardSlot> targets = new List<CardSlot>();
        for (int i = 0; i < defender.gameField.ElestralSlots.Count; i++)
        {
            CardSlot slot = defender.gameField.ElestralSlots[i];
            if (slot.MainCard != null) { targets.Add(slot); }
        }
        if (!canDirectAttack) { canDirectAttack = targets.Count > 0; }
        if (canDirectAttack)
        {
            targets.Add(defender.gameField.SpiritDeckSlot);
        }
        return new SlotSelector(attacks, targets.ToArray());
    }
}
