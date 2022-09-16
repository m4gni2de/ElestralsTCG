using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Newtonsoft.Json.Bson;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class SlotSelector
{
    
    #region Properties
    public int ExpectedCount = 0;
    private List<CardSlot> _Selections = null;
    public List<CardSlot> SelectedSlots
    {
        get
        {
            return _Selections;
        }
    }
    public List<CardSlot> TargetSlots = null;

    string confirmString = "";
    private List<string> _selectionStrings = null;
    public List<string> SelectionStrings
    {
        get
        {
            _selectionStrings ??= new List<string>();
            return _selectionStrings;
        }
    }
    public bool ConfirmEach = false;
    #endregion
    public event Action<bool, SlotSelector> OnSelectionHandled;
    public event Action<bool, CardSlot> OnSelectionChanged;
    protected void DoSelection(bool isSuccess)
    {
        OnSelectionHandled?.Invoke(isSuccess, this);
    }
    protected void ChangeSelection(bool selectionChanged, CardSlot selection)
    {
        OnSelectionChanged?.Invoke(selectionChanged, selection);
        if (selectionChanged)
        {
            GameManager.Instance.messageControl.ShowMessage(SelectionStrings[SelectedSlots.Count]);
        }
        
    }

   
    SlotSelector(List<string> msgs, string confirm, int countToSelect, CardSlot[] targets, bool confirmEach)
    {
        ConfirmEach = confirmEach;
        ExpectedCount = countToSelect;
        confirmString = confirm;

        TargetSlots = new List<CardSlot>();
        _Selections = new List<CardSlot>();
        for (int i = 0; i < msgs.Count; i++)
        {
            SelectionStrings.Add(msgs[i]);
        }
        for (int i = 0; i < targets.Length; i++)
        {
            TargetSlots.Add(targets[i]);
        }

        GameManager.Instance.messageControl.ShowMessage(SelectionStrings[0]);
    }

   
    public void SelectSlot(CardSlot slot)
    {
        if (!SelectedSlots.Contains(slot))
        {
            
            SelectedSlots.Add(slot);
            if (SelectedSlots.Count == ExpectedCount)
            {
                App.AskYesNo($"Confirm {TargetString()}?", ConfirmSelector);
            }
            else if (ConfirmEach)
            {
                App.AskYesNo($"Confirm {TargetString()}?", ConfirmSelection);
            }
            else
            {
                ChangeSelection(true, slot);
                
            }
        }
        else
        {
            slot.MainCard.SelectCard(false);
            SelectedSlots.Remove(slot);
            ChangeSelection(true, slot);
        }

        
    }

    public void UndoSelect()
    {
        if (SelectedSlots.Count > 0)
        {
            ConfirmSelector(false);
        }
        else
        {
            OnSelectionHandled(false, this);
        }
    }

    protected void ConfirmSelection(bool confirm)
    {
        if (!confirm)
        {
            
            SelectedSlots.RemoveAt(SelectedSlots.Count - 1);
            
        }
        else
        {
            SelectedSlots[SelectedSlots.Count - 1].MainCard.SelectCard(true);
        }
        GameManager.Instance.messageControl.ShowMessage(SelectionStrings[SelectedSlots.Count]);

    }

    protected void ConfirmSelector(bool confirm)
    {
        if (confirm)
        {
            DoSelection(true);
        }
        else
        {
            SelectedSlots.RemoveAt(SelectedSlots.Count -1);
            GameManager.Instance.messageControl.ShowMessage(SelectionStrings[SelectedSlots.Count]);
        }
    }

    private string TargetString()
    {
        string s = $"{confirmString} on ";
        for (int i = 0; i < SelectedSlots.Count; i++)
        {
            s += SelectedSlots[i].SlotTitle;
            if (i < SelectedSlots.Count - 1) { s += ", "; }
        }

        return s;
    }
    public void Clear()
    {
        SelectedSlots.Clear();
    }

    
    public static SlotSelector CreateMulti(List<string> msgs, string confirm, List<CardSlot> targets, int totalSelections, bool confirmEach)
    {
        SlotSelector sel = new SlotSelector(msgs, confirm, totalSelections, targets.ToArray(), confirmEach);
        sel.ConfirmEach = confirmEach;
        return sel;
    }

    public static SlotSelector AttackSlots(Player defender, int attacks, bool canDirectAttack = false)
    {
        List<CardSlot> targets = new List<CardSlot>();
        List<string> msgs = new List<string>();
        for (int i = 0; i < defender.gameField.ElestralSlots(false).Count; i++)
        {
            CardSlot slot = defender.gameField.ElestralSlots(false)[i];
            if (slot.MainCard != null) { targets.Add(slot); }
        }
        if (!canDirectAttack) { canDirectAttack = targets.Count == 0; }
        if (canDirectAttack)
        {
            targets.Add(defender.gameField.SpiritDeckSlot);
        }

        for (int i = 0; i < attacks; i++)
        {
            msgs.Add("Select Attack Target");
        }
        return CreateMulti(msgs, "Attack", targets, attacks, true);
    }

   
}


