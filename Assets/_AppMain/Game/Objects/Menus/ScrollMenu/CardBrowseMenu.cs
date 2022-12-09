using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardsUI.Filtering;
using UnityEngine.Events;

namespace Gameplay.Menus
{
    public class CardBrowseMenu : ScrollMenu
    {
        public class BrowseArgs
        {
            public List<GameCard> Selections { get; set; }
            public CardMode CastMode { get; set; }
            public int Minimum { get; set; }
            public int Maximum { get; set; }
            public bool IsConfirm { get; set; }

            public CardSlot SelectedSlot { get; set; }
            public GameCard SourceCard { get; set; }

            public BrowseArgs(CardBrowseMenu menu, CardMode mode)
            {
                Selections = new List<GameCard>();
                Selections.AddRange(menu.SelectedCards);
                CastMode = mode;
                Minimum = menu.minSelectCount;
                Maximum = menu.maxSelectCount;
                IsConfirm = menu.IsConfirmed;
                SelectedSlot = menu.SelectedSlot;
                SourceCard = menu.SourceCard;

            }
        }
        public BrowseArgs menuArgs { get; set; }
        public event Action<BrowseArgs> OnClosed;
       
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
        private MagicTextBox TitleText;

        public MagicToggleGroup CardModeGroup;
        public MagicToggle AttackToggle, DefenseToggle;
        protected bool isCastMode = false;
        #endregion

        #region Menu Origin Properties
        protected CardSlot SelectedSlot { get; private set; }
        protected GameCard SourceCard { get; private set; }
        #endregion

        #region Events

        private UnityEvent _OnSelection = null;
        public UnityEvent OnSelection { get { _OnSelection ??= new UnityEvent(); return _OnSelection; } }

        //public event Action<CardBrowseMenu> OnSelectionConfirm;

        public event Action<GameCard, bool> OnCardSelected;
        #endregion

        #region Overrides

        #endregion

        #region Click and Holding Cards
        protected void TrySelectCard(int index)
        {
            if (Scroll.velocity.magnitude > 0) { return; }
            SelectCard(index);
        }
        
        protected void SelectCard(int index)
        {
            GameCard v = cards[index];
            CardView clone = clones[index];
            
            if (SelectedCards.Contains(v))
            {
                SelectedCards.Remove(v);
                OnCardSelected?.Invoke(v, false);
                ToggleSelect(clone, false);
            }
            else
            {
                if (SelectedCards.Count < maxSelectCount)
                {
                    SelectedCards.Add(v);
                    OnCardSelected?.Invoke(v, true);
                    ToggleSelect(clone, true);
                    
                }

            }

            
            ConfirmButton.interactable = (SelectedCards.Count >= minSelectCount && SelectedCards.Count <= maxSelectCount);
        }

        protected void ToggleSelect(CardView clone, bool isSelected)
        {
            if (isSelected)
            {
                clone.SetAlpha(1f);

            }
            else
            {
                clone.SetAlpha(.5f);
            }
        }

        protected void TryHoldCard(int index)
        {
            StartCoroutine(AwaitCardHold(index));
        }

        protected IEnumerator AwaitCardHold(int index)
        {
            GameCard v = cards[index];
            CardView clone = clones[index];
            ToggleScrolling(false);
            ToggleSelect(clone, true);
            GameManager.Instance.DisplayCard(v);

            do
            {
                yield return null;
            } while (true && Input.GetMouseButton(0) && IsOpen && clone.touch.IsPointerOverMe());

            GameManager.Instance.HideDisplayCard();
            ToggleSelect(clone, false);
            ToggleScrolling(false);
        }
        #endregion


        protected int maxSelectCount, minSelectCount;
        private int _prevMaxCount, _prevMinCount;

        public event Action<List<GameCard>, CardMode> OnCastClose;
        public event Action<List<GameCard>> OnMenuClose;

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
            SourceCard = null;
            SelectedSlot = null;
            if (Scroll.verticalScrollbar != null) { Scroll.verticalScrollbar.value = 0f; }
            if (Scroll.horizontalScrollbar != null) { Scroll.horizontalScrollbar.value = 0f; }
            CardModeGroup.Refresh();
        }

        
        public void LoadCards(List<GameCard> cards, string title, bool faceUp = true)
        {
            LoadCards(cards, title, faceUp, 0, 0);
            

        }

        public void CastLoad(List<GameCard> cards, string title, bool faceUp, int minSelectable, int maxSelectable, GameCard toEnchant, bool isAdding)
        {
            LoadCards(cards, title, faceUp, minSelectable, maxSelectable);
            CastMode(toEnchant, null, isAdding);
        }
        public void LoadCards(List<GameCard> cards, string title, bool faceUp, int minSelectable, int maxSelectable)
        {
            if (IsOpen) { return; }
            DisplayManager.AddAction(Cancel);

            Refresh();
            maxSelectCount = maxSelectable;
            minSelectCount = minSelectable;
            _prevMaxCount = maxSelectable;
            _prevMinCount = minSelectable;

            
            for (int i = 0; i < cards.Count; i++)
            {
                this.cards.Add(cards[i]);
                CardView co = CardFactory.Copy(cards[i].cardObject, Content, cards[i].card);
                DisplayCard(cards[i], co, faceUp);
                clones.Add(co);
                co.transform.localPosition = new Vector3(co.transform.localPosition.x, co.transform.localPosition.y, -2f);
            }

            Open();
            TitleText.SetText(title);
            
            ShowButtons();

        }
        public void CastMode(GameCard card, CardSlot to = null, bool isAdding = true)
        {
            DefenseToggle.OnToggleChanged -= CheckForFaceDownRune;
            SourceCard = card;
            if (to == null)
            {
                SelectedSlot = card.CurrentSlot;
            }
            else
            {
                SelectedSlot = to;
            }
            if (!isAdding)
            {
                CardModeGroup.Unload();
                CardModeGroup.Refresh();
            }
            else
            {

                isCastMode = true;
                if (card.CardType == CardType.Elestral)
                {
                    CardModeGroup.SetToggleText(AttackToggle, "Attack Mode");
                    CardModeGroup.SetToggleText(DefenseToggle, "Defense Mode");

                }
                else if (card.CardType == CardType.Rune)
                {
                    TitleText.SaveCurrent();
                    CardModeGroup.SetToggleText(AttackToggle, "Face-Up");
                    CardModeGroup.SetToggleText(DefenseToggle, "Face-Down");
                    DefenseToggle.OnToggleChanged += CheckForFaceDownRune;
                }

                DefenseToggle.Show();
                AttackToggle.Show();
            }
        }
        


        protected void CheckForFaceDownRune(MagicToggle toggle)
        {
            bool isOn = toggle.IsOn;
            if (isOn)
            {
                string newText = $"Play {SourceCard.cardStats.title} Face-Down?";
                TitleText.SetText(newText);
                maxSelectCount = 0;
                minSelectCount = 0;
                SelectNone();
                ConfirmButton.interactable = true;
            }
            else
            {
                TitleText.LoadSavedText();
                maxSelectCount = _prevMaxCount;
                minSelectCount = _prevMinCount;
                ShowButtons();
            }
        }

       
       protected void ShowButtons()
        {
            ConfirmButton.gameObject.SetActive(maxSelectCount > 0);
            ConfirmButton.interactable = (SelectedCards.Count == maxSelectCount || SelectedCards.Count >= minSelectCount);
            
        }

        protected void DisplayCard(GameCard card, CardView co, bool faceUp)
        {
            
            
            co.touch.ClearAll();
            co.SetAsChild(Content, CardScale, SortLayer, 0);
            
            if (minSelectCount > 0)
            {
                ToggleSelect(co, false);
            }
            int index = cards.Count -1;
            co.touch.AddClickListener(() => TrySelectCard(index));
            co.touch.AddHoldListener(() => TryHoldCard(index), .4f);
            co.touch.IsMaskable = false;
            co.touch.bypassFreeze = true;
            co.SetSortingOrder(5);

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
                CardView clone = clones[i];
                ToggleSelect(clone, false);
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


        public void Cancel()
        {
            IsConfirmed = false;
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
                if (isCastMode)
                {
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
                
            }

            menuArgs = new BrowseArgs(this, cMode);


            OnCastClose?.Invoke(selected, cMode);
            OnMenuClose?.Invoke(selected);
            Refresh();
            base.Close();
            DefenseToggle.OnToggleChanged -= CheckForFaceDownRune;
            CardModeGroup.Unload();
            isCastMode = false;
            TitleText.ClearSaved();
            

            DisplayManager.RemoveAction(Cancel);
            OnClosed?.Invoke(menuArgs);

        }

        #endregion


        

    }
}

