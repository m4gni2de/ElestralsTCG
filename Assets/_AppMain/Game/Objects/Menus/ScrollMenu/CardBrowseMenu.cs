using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardsUI.Filtering;

namespace Gameplay.Menus
{
    public class CardBrowseMenu : ScrollMenu
    {
        private Dictionary<GameCard, GameCard.VisualInfo> _VisualInfo = null;
        public Dictionary<GameCard, GameCard.VisualInfo> m_VisualInfo
        {
            get
            {
                _VisualInfo ??= new Dictionary<GameCard, GameCard.VisualInfo>();
                return _VisualInfo;
            }
        }

        private List<GameCard> _SelectedCards = null;
        public List<GameCard> SelectedCards
        {
            get
            {
                _SelectedCards ??= new List<GameCard>();
                return _SelectedCards;
            }
        }

        #region Buttons/UI
        [Header("UI")]
        [SerializeField]
        private Button ConfirmButton;
        private bool IsConfirmed = false;
        [SerializeField]
        private Button CancelButton;
        [SerializeField]
        private TMP_Text TitleText;

        public GameToggleGroup CardModeGroup;
        public GameToggle AttackToggle, DefenseToggle;
        #endregion

        protected void SelectCard(int index)
        {
            GameCard v = cards[index];
            CardView clone = clones[index];
            if (SelectedCards.Contains(v))
            {
                SelectedCards.Remove(v);
                clone.SelectCard(false);
            }
            else
            {
                if (SelectedCards.Count < maxSelectCount)
                {
                    SelectedCards.Add(v);
                    clone.SelectCard(true);
                }

            }

            ConfirmButton.interactable = SelectedCards.Count == maxSelectCount;
        }
        protected int maxSelectCount, minSelectCount;
        private int _prevMaxCount, _prevMinCount;

        public event Action<List<GameCard>, CardMode> OnMenuClose;

        public List<CardView> clones = new List<CardView>();


        protected void Refresh()
        {
            IsConfirmed = false;
            for (int i = 0; i < clones.Count; i++)
            {
                Destroy(clones[i].gameObject);
            }
            clones.Clear();
            cards.Clear();
            SelectedCards.Clear();
            ConfirmButton.gameObject.SetActive(false);
            ConfirmButton.interactable = false;
            
            CardModeGroup.Refresh();
        }
        public void LoadCards(List<GameCard> cards, string title, bool faceUp = true)
        {
            LoadCards(cards, title, faceUp, 0, 0);
            

        }

        public void LoadCards(List<GameCard> cards, string title, bool faceUp, int maxSelectable, int minSelectable)
        {
            if (IsOpen) { return; }

            Refresh();
            maxSelectCount = maxSelectable;
            minSelectCount = minSelectable;
            _prevMaxCount = maxSelectable;
            _prevMinCount = minSelectable;

            
            for (int i = 0; i < cards.Count; i++)
            {
                this.cards.Add(cards[i]);
                //CardView co = cards[i].cardObject;
                CardView co = Instantiate(cards[i].cardObject, Content);
                co.LoadCard(cards[i].card);
                DisplayCard(cards[i], co, faceUp);
                clones.Add(co);
                co.transform.localPosition = new Vector3(co.transform.localPosition.x, co.transform.localPosition.y, -2f);
            }

            Open();
            TitleText.text = title;
            ShowButtons();

        }
        public void EnchantMode(GameCard card)
        {
            DefenseToggle.OnToggleChanged -= CheckForFaceDownRune;
            if (card.CardType == CardType.Elestral)
            {
                CardModeGroup.SetToggleText(AttackToggle, "Attack Mode");
                CardModeGroup.SetToggleText(DefenseToggle, "Defense Mode");
               
            }
            else if (card.CardType == CardType.Rune)
            {
                CardModeGroup.SetToggleText(AttackToggle, "Face-Up");
                CardModeGroup.SetToggleText(DefenseToggle, "Face-Down");
                DefenseToggle.OnToggleChanged += CheckForFaceDownRune;
            }

            DefenseToggle.Show();
            AttackToggle.Show();
        }

        protected void CheckForFaceDownRune(GameToggle toggle)
        {
            bool isOn = toggle.IsToggled;
            if (isOn)
            {
                maxSelectCount = 0;
                minSelectCount = 0;
                SelectNone();
                ConfirmButton.interactable = true;
            }
            else
            {
                maxSelectCount = _prevMaxCount;
                minSelectCount = _prevMinCount;
                ShowButtons();
            }
        }

       
       protected void ShowButtons()
        {
            ConfirmButton.gameObject.SetActive(maxSelectCount > 0);
            ConfirmButton.interactable = SelectedCards.Count == maxSelectCount;
            
        }

        protected void DisplayCard(GameCard card, CardView co, bool faceUp)
        {
            
            
            co.touch.ClearAll();
            //m_VisualInfo.Add(card, card.m_VisualInfo);
            co.SetAsChild(Content, CardScale, SortLayer, 0);
            
            if (minSelectCount > 0)
            {
                co.SelectCard(false);
                //card.SelectCard(false);
            }
            int index = cards.Count -1;
            //co.touch.OnClickEvent.AddListener(() => SelectCard(index));
            co.touch.AddClickListener(() => SelectCard(index));
            co.touch.IsMaskable = false;
            co.touch.bypassFreeze = true;

            if (faceUp)
            {
                if (!card.IsFaceUp) { co.Flip(); }
            }
            else
            {
                if (card.IsFaceUp) { co.Flip(true); }
            }
            

        }

        #region Selection Commands
        public void SelectMax()
        {
            for (int i = 0; i < maxSelectCount; i++)
            {
                SelectCard(i);
            }
        }
        public void SelectNone()
        {
            for (int i = 0; i < clones.Count; i++)
            {
                clones[i].SelectCard(false);
            }
            SelectedCards.Clear();
        }
        #endregion

        #region Closing
        public void Confirm()
        {
            if (SelectedCards.Count < minSelectCount) { return; }
            IsConfirmed = true;
            
            Close();
        }

        protected override void Close()
        {
            List<GameCard> selected = new List<GameCard>();
            CardMode cMode = CardMode.None;
            if (IsConfirmed)
            {
                selected = SelectedCards;

                cMode = CardMode.None;
                string toggleVal = CardModeGroup.GetToggledValue();
                if (!string.IsNullOrEmpty(toggleVal))
                {
                    if (toggleVal.ToLower() == "attack")
                    {
                        cMode = CardMode.Attack;
                    }
                    else
                    {
                        cMode = CardMode.Defense;
                    }
                }
               
               
            }
            OnMenuClose?.Invoke(selected, cMode);
            Refresh();
            base.Close();
            DefenseToggle.OnToggleChanged -= CheckForFaceDownRune;
            CardModeGroup.Unload();
            DoThaw();

        }
        
        #endregion

       


    }
}

