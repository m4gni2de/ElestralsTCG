using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using System;
using Newtonsoft.Json.Schema;
using static Gameplay.GameState;
using TouchControls;
using UnityEngine.UIElements;

public class SingleSlot : CardSlot, iMainCard
{
   
    

 
    #region Interface
    public void AddMainCard(GameCard card)
    {
        MainCard = card;
        
    }
    protected override void SetMainCard(GameCard card)
    {
        if (card.CardType != CardType.Spirit)
        {
            AddMainCard(card);
        }
    }
    #endregion

    #region Functions
    public List<GameCard> EnchantingSpirits
    {
        get
        {
            List<GameCard> list = new List<GameCard>();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].CardType == CardType.Spirit)
                {
                    list.Add(cards[i]);
                }
            }
            return list;
        }
    }
    #endregion

    private TouchGroup _touchGroup = null;
    public TouchGroup touchGroup
    {
        get
        {
           if (_touchGroup == null)
            {
                if (GetComponent<TouchGroup>() != null) { _touchGroup = GetComponent<TouchGroup>(); }
                else
                { _touchGroup = gameObject.AddComponent<TouchGroup>();
                }
            }
            return _touchGroup;
        }
    }

    private void Reset()
    {
        if (GetComponent<TouchGroup>() == null)
        {
            gameObject.AddComponent<TouchGroup>();
        }
    }
    protected override void SetSlot()
    {
        
    }

   
    protected override void ToggleSelect(bool isSelected)
    {
       if (!isSelected)
        {
            if (SelectedCard != null)
            {
                SelectedCard.SelectCard(false);
            }
        }
        else
        {
            if (SelectedCard != null)
            {
                SelectedCard.SelectCard(true);
            }
        }
    }

    public override void RemoveCard(GameCard card)
    {
        base.RemoveCard(card);
        if (MainCard != null && MainCard == card)
        {

            MainCard = null;
        }
       

    }

    public override void AllocateTo(GameCard card,bool sendToServer = true)
    {

        card.RemoveFromSlot();
        cards.Add(card);
        card.AllocateTo(this, sendToServer);
        if (card.CardType != CardType.Spirit)
        {
            SetMainCard(card);
        }
        else
        {
            //Enchant(card);
        }
        DisplayCardObject(card);

    }

    protected override void DisplayCardObject(GameCard card)
    {
        CardView c = card.cardObject;
        string sortLayer = SortLayer;
        float height = rect.sizeDelta.y;
        float offsetVal = .10f;
        float offsetHeight = height * offsetVal;

        
        SetCommands(card);
        int zOrder = 0;
        if (card.cardStats.cardType != CardType.Spirit)
        {
            sortLayer = "CardL2";
            offsetHeight *= -1;
            
        }
        else
        {
            offsetHeight *= (1 + cards.Count);
            int sortOrder = (cards.Count + 1) * -2;
            c.SetSortingOrder(sortOrder);
            zOrder = -cards.Count - 1;
            c.touch.IsMaskable = true;

        }


        c.SetAsChild(transform, CardScale, sortLayer);
        c.transform.localPosition = new Vector3(0f, offsetHeight, zOrder);



        SetRotation(card);
        SetOrientation(card);
    }

    protected override void SetCommands(GameCard card)
    {
        TouchObject to = card.cardObject.touch;
        to.RemoveFromGroup();
        touchGroup.Add(to);
        to.AddClickListener(() => ClickCard(card));
        //to.AddHoldListener(() => GameManager.Instance.DragCard(card, this));
        to.AddHoldListener(() => DragCard(card));

    }

    protected virtual void SetOrientation(GameCard card)
    {
        card.cardObject.Flip();
    }
    protected virtual void SetRotation(GameCard card)
    {
        card.SetCardMode(card.mode);
    }
    
    protected void Refresh()
    {
        GameManager.Instance.browseMenu.SelectedCards.Clear();
        SelectedCard = null;
        GameManager.SelectedCard = null;
    }


    #region Commands

    #region Nexus Command
    protected void NexusCommand()
    {
        List<CardSlot> yourCards = new List<CardSlot>();
        yourCards.AddRange(Owner.gameField.ElestralSlots(false));
        yourCards.AddRange(Owner.gameField.RuneSlots(false));


        List<string> msgs = new List<string>();
        msgs.Add("Select Card to take Spirits from.");
        msgs.Add("Select Card to recieve Spirits.");
        SlotSelector sourceSelect = SlotSelector.CreateMulti(msgs, "Nexus Card", yourCards, 2, true);
        GameManager.Instance.SetSelector(sourceSelect);
        sourceSelect.OnSelectionHandled += AwaitNexusSource;
        ClosePopMenu(true);
    }

    protected void AwaitNexusSource(bool isConfirm, SlotSelector sel)
    {
        sel.OnSelectionHandled -= AwaitNexusSource;
        if (isConfirm)
        {
            GameCard source = sel.SelectedSlots[0].MainCard;
            GameCard target = sel.SelectedSlots[1].MainCard;
            SingleSlot sourceSlot = (SingleSlot)source.CurrentSlot;
            int maxCount = sourceSlot.EnchantingSpirits.Count;
            int minCount = 1;
            List<GameCard> toShow = sourceSlot.EnchantingSpirits;
            string title = $"Select up to {maxCount} Spirits to Nexus from {source.cardStats.title} to {target.cardStats.title}!";
            ShowCardBrowse(toShow, title, true, maxCount, minCount);
            GameManager.Instance.browseMenu.OnMenuClose += DoNexusCommand;
            GameManager.Instance.browseMenu.EnchantMode(source, false);
        }
        else
        {
            GameManager.Instance.SetSelector();
        }
    }

    
    protected virtual void DoNexusCommand(List<GameCard> selected)
    {
        GameManager.Instance.browseMenu.OnMenuClose -= DoNexusCommand;
        if (selected.Count == 0)
        {
            GameManager.Instance.currentSelector.OnSelectionHandled += AwaitNexusSource;
            GameManager.Instance.currentSelector.UndoSelect();
            return;
        }
        SlotSelector selector = GameManager.Instance.currentSelector;

        GameCard source = selector.SelectedSlots[0].MainCard;
        GameCard target = selector.SelectedSlots[1].MainCard;
        source.SelectCard(false);
        target.SelectCard(false);
        GameManager.Instance.Nexus(Owner, source, target, selected);
        GameManager.Instance.SetSelector();
        Refresh();
    }

    

   
    protected void ShowCardBrowse(List<GameCard> toShow, string title, bool faceUp, int max, int min)
    {
        GameManager.Instance.browseMenu.LoadCards(toShow, title, faceUp, max, min);
        ClosePopMenu();
        
    }
    #endregion

    #endregion

}
