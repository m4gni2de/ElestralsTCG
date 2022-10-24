using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;

namespace Gameplay.Menus
{
    public class LocationMenu : GameMenu
    {
        #region Properties
        public bool ShowMovedCards { get; private set;}
        private List<VmLocation> _menuItems = null;
        public List<VmLocation> MenuItems
        {
            get
            {
                _menuItems ??= new List<VmLocation>();
                return _menuItems;
            }
        }
        public VmLocation _template;
        public Transform Content;
        #endregion
       
        #region Overrides
        protected override void Setup()
        {
            _template.Hide();
            if (!MenuItems.Contains(_template))
            {
                MenuItems.Add(_template);
            }
            base.Setup();
        }
        public override void Open()
        {
            base.Open();

        }
        public override void Refresh()
        {
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItems[i].Clear();
            }
        }
        public override void Close()
        {
            
            base.Close();
            
        }
        #endregion

        #region Functions
       
        public bool Validate()
        {
           
            Dictionary<CardSlot, VmLocation> constraintedSlots = new Dictionary<CardSlot, VmLocation>();
            for (int i = 0; i < MenuItems.Count; i++)
            {
                if (!MenuItems[i].ValidatePlacement())
                {
                    string error = $"Error! {MenuItems[i].Location.SlotTitle} is not a valid placement.";
                    return GameMessage.Error(error);
                }
                else
                {
        
                    CardSlot selected = MenuItems[i].Location;
                    if (selected is SingleSlot)
                    {
                        if (constraintedSlots.ContainsKey(MenuItems[i].Location))
                        {
                            if (MenuItems[i].Card.CardType != CardType.Spirit)
                            {
                                string error = $"Error! {MenuItems[i].Location.SlotTitle} already has a placement";
                                return GameMessage.Error(error);
                            }
                            else
                            {
                                constraintedSlots.Add(MenuItems[i].Location, MenuItems[i]);
                            }
                           
                        }
                        else
                        {
                            if (MenuItems[i].Card.CardType != CardType.Spirit)
                            {
                                constraintedSlots.Add(MenuItems[i].Location, MenuItems[i]);
                            }
                        }
                       
                    }
                }

            }

            return true;
        }

        #endregion

        public static void Load(Player p, List<GameCard> cards, bool showOnMove = false)
        {
            if (GameManager.Instance == null) { return; }
            GameManager.Instance.locationMenu.LoadMenu(p, cards, showOnMove);
        }

        public void LoadMenu(Player p, List<GameCard> cards, bool showOnMove)
        {
            
            int modelCount = MenuItems.Count;
            if (modelCount < cards.Count)
            {
                CreateModels(cards.Count - modelCount);
            }
            ShowMovedCards = showOnMove;
            Open();
            DisplayMenu(p, cards);

        }

        protected void DisplayMenu(Player player, List<GameCard> cards)
        {
            Refresh();
            
            
            for (int i = 0; i < cards.Count; i++)
            {
                MenuItems[i].LoadCard(cards[i], player.gameField.cardSlots);
            }
        }

        protected void CreateModels(int count)
        {
            for (int i = 0; i < count; i++)
            {
                VmLocation vm = Instantiate(_template, Content);
                MenuItems.Add(vm);
            }
        }


        public void ConfirmButton()
        {
            if (Validate())
            {
                for (int i = 0; i < MenuItems.Count; i++)
                {
                    MenuItems[i].Confirm(ShowMovedCards);
                }
                Close();
            }
           
        }
        public void CancelButton()
        {
            for (int i = 0; i < MenuItems.Count; i++)
            {
                MenuItems[i].Cancel();
            }
            Close();
        }
       
    }
}

