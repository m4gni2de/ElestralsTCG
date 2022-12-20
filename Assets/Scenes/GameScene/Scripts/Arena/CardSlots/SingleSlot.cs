using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using System;
using Newtonsoft.Json.Schema;
using static Gameplay.GameState;
using TouchControls;
using UnityEngine.UIElements;
using Gameplay.CardActions;
using Gameplay.Turns;
using static Gameplay.Menus.CardBrowseMenu;

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
               
                if (GetComponent<TouchGroup>() != null) 
                { 
                    _touchGroup = GetComponent<TouchGroup>(); 
                }
                else
                { 
                    _touchGroup = gameObject.AddComponent<TouchGroup>();
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


    protected override bool GetIsInPlay()
    {
        return true;
    }

    public override void RemoveCard(GameCard card)
    {
        base.RemoveCard(card);
        touchGroup.Remove(card.cardObject.touch);
        if (MainCard != null && MainCard == card)
        {

            MainCard = null;
        }
       

    }

    public override void AllocateTo(GameCard card,bool sendToServer = true)
    {
        card.cardObject.Show();
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
            int sortOrder = (-20 * cards.Count) + 1;
            offsetHeight *= -1;
            c.SetSortingOrder(sortOrder);

        }
        else
        {
            offsetHeight *= (1 + cards.Count);
            int sortOrder = ((20 * cards.Count) + 1) * -2;
            //int sortOrder = (cards.Count + 1) * -2;
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
        to.ClearAll();
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
    
    protected override void Refresh()
    {
        base.Refresh();
        GameManager.Instance.browseMenu.SelectedCards.Clear();
        SelectedCard = null;
        GameManager.SelectedCard = null;
        GameManager.Instance.cardSlotMenu.Close();
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
            BrowseCards(toShow, title, true, minCount, maxCount);
            GameManager.Instance.browseMenu.OnMenuClose += DoNexusCommand;
            GameManager.Instance.browseMenu.CastMode(source, null, false);
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


   

    /// <summary>
    /// Start the action of Casting a card to the selected slot directly. 
    /// </summary>
    #region Cast to Slot Directly
    public virtual void CastToSlotCommand(GameCard card, CardSlot from)
    {
        int castCount = card.card.SpiritsReq.Count;
        List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

        string title = $"Select {CardUI.AnySpiritUnicode(castCount)} Spirits for Cast of {card.card.cardData.cardName}";
        GameManager.Instance.browseMenu.LoadCards(toShow, title, true, castCount, castCount);
        GameManager.Instance.browseMenu.CastMode(card, this);
        ClosePopMenu(true);
        BrowseMenu.OnClosed += CastToClose;
    }

    protected void CastToClose(BrowseArgs args)
    {
        BrowseMenu.OnClosed -= CastToClose;
        if (args.CastMode == CardMode.None) { Refresh(); args.SourceCard.ReAddToSlot(false); return; }

        List<GameCard> cardsList = new List<GameCard>();
        for (int i = 0; i < args.Selections.Count; i++)
        {
            cardsList.Add(args.Selections[i]);
        }

        if (args.SourceCard.card.CardType == CardType.Rune)
        {
            RuneCastClose(args.SourceCard, args.SelectedSlot, cardsList, args.CastMode);
        }
        else
        {
            GameManager.Instance.Cast(Owner, args.SourceCard, cardsList, args.SelectedSlot, args.CastMode);
            Refresh();
        }
    }
    #endregion

    #endregion



    #region Enchant/DisEnchant Commands
    public void EnchantCommand(int spiritCount = 1)
    {

        List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

        string title = $"Select {CardUI.AnySpiritUnicode(spiritCount)} to Enchant {SelectedCard.name} with.";
        BrowseMenu.LoadCards(toShow, title, true, spiritCount, spiritCount);
        BrowseMenu.CastMode(SelectedCard);
        ClosePopMenu(true);
        BrowseMenu.OnCastClose += AwaitEnchantClose;
    }

    public void DisEnchantCommand(int spiritCount = 1)
    {
        List<GameCard> toShow = new List<GameCard>();
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].CardType == CardType.Spirit)
            {
                toShow.Add(cards[i]);
            }
        }
        int maxEnchantCount = toShow.Count;


        string title = $"Select {CardUI.AnySpiritUnicode(spiritCount)} to DisEnchant from {SelectedCard.name}";
        BrowseMenu.LoadCards(toShow, title, true, spiritCount, maxEnchantCount);
        BrowseMenu.CastMode(SelectedCard, null, false);
        ClosePopMenu(true);
        BrowseMenu.OnMenuClose += AwaitDisEnchantClose;
    }
    protected void AwaitDisEnchantClose(List<GameCard> selectedCards)
    {
        BrowseMenu.OnMenuClose -= AwaitDisEnchantClose;
        if (selectedCards.Count == 0) { Refresh(); return; }
        List<GameCard> cardsList = new List<GameCard>();
        for (int i = 0; i < selectedCards.Count; i++)
        {
            cardsList.Add(selectedCards[i]);
        }
        GameManager.Instance.DisEnchant(Owner, MainCard, cardsList, Owner.gameField.UnderworldSlot);
       

    }

    protected virtual void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
    {
        BrowseMenu.OnCastClose -= AwaitEnchantClose;
        if (cMode == CardMode.None) { return; }

        List<GameCard> cardsList = new List<GameCard>();
        for (int i = 0; i < selectedCards.Count; i++)
        {
            cardsList.Add(selectedCards[i]);
        }
        GameManager.Instance.Enchant(Owner, MainCard, cardsList);
        Refresh();

    }
    #endregion


    #endregion

    protected virtual void ChangeModeCommand()
    {
       
    }


    protected virtual void DestroyCommand()
    {
        if (MainCard != null)
        {
            App.AskYesNo($"Send {MainCard.cardStats.title} and all linked cards to the Underworld?", DestroyResponse);

        }
        ClosePopMenu(false);
    }

    protected virtual void DestroyResponse(bool confirm)
    {
        if (confirm)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                GameManager.Instance.MoveCard(Owner, cards[i], cards[i].Owner.gameField.UnderworldSlot);
            }
            GameManager.Instance.cardSlotMenu.Close();
        }
        Refresh();
    }
    

}
