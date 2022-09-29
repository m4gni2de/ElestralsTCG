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
        #endregion

        public static void Load(Player p, List<GameCard> cards)
        {
            if (GameManager.Instance == null) { return; }
            GameManager.Instance.locationMenu.LoadMenu(p, cards);
        }

        public void LoadMenu(Player p, List<GameCard> cards)
        {
            
            int modelCount = MenuItems.Count;
            if (modelCount < cards.Count)
            {
                CreateModels(cards.Count - modelCount);
            }
            Open();
            DisplayMenu(p, cards);

        }

        protected void DisplayMenu(Player player, List<GameCard> cards)
        {
            Refresh();
            
            List<CardSlot> slots = player.gameField.cardSlots;
            for (int i = 0; i < cards.Count; i++)
            {
                MenuItems[i].LoadCard(cards[i], slots);
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

       
    }
}

