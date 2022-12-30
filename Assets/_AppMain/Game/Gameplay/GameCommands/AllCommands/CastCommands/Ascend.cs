using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using Gameplay.Menus;
using Gameplay.Turns;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Commands
{
    public class Ascend : GameCommand
    {
        #region Overrides
        protected override string DefaultKey { get { return "Ascend"; } }
        //public override CardAction commandAction { get; set; }
        public override bool CanActivate
        {
            get
            {
                if (tributedCard == null) { return false; }
                if (ascendCard == null) { return false; }
                if (spiritsUsed == null || spiritsUsed.Count == 0) { return false; }
                if (cardFrom == null) { return false; }
                if (cardTo == null) { return false; }
                return true;
            }
        }
        public override void CompleteCommand()
        {
            bool isYours = player == GameManager.ActiveGame.You;
            if (isYours)
            {
                if (ascendCard == null)
                {
                    ChooseCard(SelectMode.Ascend);
                }
                else
                {
                    if (tributedCard == null)
                    {
                        ChooseCard(SelectMode.Tribute);
                    }
                }
            }

        }

        public override void Refresh()
        {
            if (selectMode == SelectMode.Tribute)
            {
                tributedCard = null;
                cardTo = null;
            }
            if (selectMode == SelectMode.Ascend)
            {
                ascendCard = null;
                cardFrom = null;
            }

            spiritsUsed.Clear();
        }

        public override void Resolve(Result res)
        {
            base.Resolve(res);
            if (res == Result.Cancelled)
            {
                OnActionReady?.Invoke(this, false);
            }
            else
            {
                Do();
            }
            
        }


        public event Action<Ascend, bool> OnActionReady;
        public override void Do()
        {
            commandAction = AscendAction.Create(player, ascendCard, tributedCard, SpiritsTaken, CatalystSpirit, ascendMode);
            AscendAction ac = commandAction as AscendAction;
            OnActionReady?.Invoke(this, true);
        }
        #endregion

        #region Enum
        public enum SelectMode
        {
            None = 0,
            Tribute = 1,
            Ascend = 2,
        }

        #endregion

        #region Properties
        private SelectMode selectMode { get; set; }


        protected GameCard tributedCard { get; set; }
        protected GameCard ascendCard { get; set; }
        protected List<GameCard> spiritsUsed { get; set; } = new List<GameCard>();
        protected CardSlot cardFrom { get; set; }
        protected CardSlot cardTo { get; set; }

        protected List<GameCard> ascendOptions { get; set; } = new List<GameCard>();
        protected List<GameCard> tributeOptions { get; set; } = new List<GameCard>();
        
        protected bool forceMode { get; set; }
        protected CardMode ascendMode { get; set; }
        public GameCard CatalystSpirit
        {
            get
            {
                if (spiritsUsed.Count > 0) { return spiritsUsed[0]; }return null;
            }
        }
        public List<GameCard> SpiritsTaken
        {
            get
            {
                List<GameCard> list = new List<GameCard>();
                if (spiritsUsed.Count > 1)
                {
                    for (int i = 1; i < spiritsUsed.Count; i++)
                    {
                        list.Add(spiritsUsed[i]);
                    }
                }
                return list;
            }
        }

        #endregion

       

        #region Initialization
        public Ascend(Player player, GameCard tributed = null, GameCard ascended = null, List<GameCard> spirits = null)
        {
            SetCommand(DefaultKey);
            this.player = player;
            if (tributed != null)
            {
                tributedCard = tributed;
                cardTo = tributedCard.CurrentSlot;
            }
            else
            {
                tributedCard = null;
                cardTo = null;
            }
            
            
            if (ascended != null) { ascendCard = ascended; cardFrom = ascended.CurrentSlot; } else { ascendCard = null;  cardFrom = null; }
            spiritsUsed = new List<GameCard>();
            if (spirits != null)
            {
                for (int i = 0; i < spirits.Count; i++)
                {
                    spiritsUsed.Add(spiritsUsed[i]);
                }
            }

        }

        public static Ascend FromAscendedChosen(Player player, GameCard ascended)
        {
            Ascend a = new Ascend(player, null, ascended, null);
            List<CardSlot> elestralSlots = player.gameField.ElestralSlots(false);
            List<GameCard> onField = new List<GameCard>();
            for (int i = 0; i < elestralSlots.Count; i++)
            {
                onField.Add(elestralSlots[i].MainCard);
            }
            a.SetTributeOptions(onField);
            return a;
            
        }
        public static Ascend FromTributedChosen(Player player, GameCard tributed, List<GameCard> ascendOptions)
        {
            Ascend a = new Ascend(player,tributed, null, null);
            a.SetAscendOptions(ascendOptions);
            return a;
        }
        #endregion

        #region Completion Steps
        public void SetAscendOptions(List<GameCard> options)
        {
            ascendOptions = options;
            selectMode = SelectMode.Ascend;
        }
        public void SetTributeOptions(List<GameCard> options)
        {
            tributeOptions = options;
            selectMode = SelectMode.Tribute;
        }
        public void SetForceMode(bool force, CardMode mode = CardMode.None)
        {
            if (force)
            {
                ascendMode = mode;
            }
            forceMode = force;
        }
        public void ChooseCard(SelectMode selMode)
        {

            selectMode = selMode;
            string title = $"Select Elestral to Ascend from {tributedCard.cardName}";
            List<GameCard> options = ascendOptions;
            if (selectMode == SelectMode.Tribute)
            {
                title = $"Select Elestral to Tribute for the Ascension of {ascendCard.cardName}";
                options = tributeOptions;
            }

            GameManager.Instance.browseMenu.LoadCards(options, title, true, 1, 1);
            GameManager.Instance.browseMenu.OnClosed += AwaitSelection;
        }

        protected void AwaitSelection(CardBrowseMenu.BrowseArgs args)
        {
            GameManager.Instance.browseMenu.OnClosed -= AwaitSelection;
            if (!args.IsConfirm) { Refresh(); return; }
            ascendCard = args.Selections[0];
            if (selectMode == SelectMode.Tribute)
            {
                tributedCard = args.Selections[0];
                
            }
            string title = $"Select Catalyst Spirit to Ascend from {tributedCard.cardName} to {ascendCard.cardName}.";
            if (!forceMode)
            {
                GameManager.Instance.browseMenu.CastLoad(player.gameField.SpiritDeckSlot.cards, title, true, 1, 1, ascendCard, true);
            }
            else
            {
                GameManager.Instance.browseMenu.CastLoad(player.gameField.SpiritDeckSlot.cards, title, true, 1, 1, ascendCard, true, ascendMode);
            }
            
            GameManager.Instance.browseMenu.OnClosed += TryAscend;
        }


        protected void TryAscend(CardBrowseMenu.BrowseArgs args)
        {
            GameManager.Instance.browseMenu.OnClosed -= TryAscend;
            if (!args.IsConfirm)
            {
                Refresh();
                CompleteCommand();
            }
            else
            {
                ascendMode = args.CastMode;
                spiritsUsed.Add(args.Selections[0]);
                spiritsUsed.AddRange(tributedCard.EnchantingSpirits);
                Resolve(Result.Succeed);
            }

        }
        #endregion
    }
}
