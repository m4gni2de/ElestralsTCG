using System.Collections.Generic;
using Databases;
using TMPro;
using UnityEngine.UI;


namespace Gameplay.Menus
{
    public class VmLocation : ViewModel
    {
        private GameCard _card;
        public GameCard Card { get { return _card; } }
        public SpriteDisplay sp;
        public Toggle toggle;
        public TMP_Dropdown ddLocations;
        private List<CardSlot> _slotLocations = null;
        public List<CardSlot> slotLocations { get { _slotLocations ??= new List<CardSlot>(); return _slotLocations; } }

        protected List<TMP_Dropdown.OptionData> SlotOptions
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                for (int i = 0; i < slotLocations.Count; i++)
                {
                    TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(slotLocations[i].SlotLocationName);
                    options.Add(optionData);
                }
                return options;
            }
        }
        

        protected CardSlot _startLocation { get; set; }
        protected CardSlot _newLocation { get; set; }
        public CardSlot Location
        {
            get
            {
                if (!ddLocations.interactable || ddLocations.options.Count == 0) { return _startLocation; }
                return slotLocations[ddLocations.value];
            }
        }

        public bool IsSelected { get { return toggle.isOn; } }

        private void Awake()
        {
            
        }
        public override void Refresh()
        {
            _card = null;
            toggle.isOn = false;
            ddLocations.ClearOptions();
            slotLocations.Clear();
            sp.Clear();
        }

        public void Clear()
        {
            Refresh();
            Hide();
            
        }
        public void LoadCard(GameCard c, List<CardSlot> slots)
        {
            _card = c;
            _startLocation = c.CurrentSlot;
            slotLocations.AddRange(ValidSlots(c, slots));
            ddLocations.AddOptions(SlotOptions);
            toggle.isOn = true;
            sp.SetSprite(CardLibrary.GetCardArt(c.card));
            Show();
        }

        #region Value Change Watchers
        public void OnToggleValueChange()
        {
            ddLocations.interactable = toggle.isOn;
        }
        public void OnDropdownChange()
        {
            _newLocation = Location;
        }
        #endregion

        public void Confirm(bool showOnMove)
        {
            if (_startLocation != _newLocation)
            {
                if (showOnMove)
                {
                    _card.FlipCard(false, true);
                }
                GameManager.Instance.MoveCard(Player.LocalPlayer, _card, _newLocation);
            }
            
        }
        public void Cancel()
        {
            Clear();
        }

        #region Functions
        protected List<CardSlot> ValidSlots(GameCard card, List<CardSlot> allSlots)
        {
            List<CardSlot> slots = new List<CardSlot>();
            for (int i = 0; i < allSlots.Count; i++)
            {
                if (allSlots[i].ValidateCard(card))
                {
                    slots.Add(allSlots[i]);
                }
            }
            return slots;
        }
        public bool ValidatePlacement()
        {
            if (_startLocation != _newLocation)
            {
                return _newLocation.ValidateCard(_card);
            }

            return true;
            
        }
        #endregion
    }
}

