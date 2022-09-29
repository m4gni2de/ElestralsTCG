using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


namespace Gameplay.Menus
{
    public class VmLocation : ViewModel
    {
        private GameCard card;
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
            card = null;
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
            card = c;
            _startLocation = c.CurrentSlot;
            slotLocations.AddRange(slots);
            ddLocations.AddOptions(SlotOptions);
            toggle.isOn = true;
            Show();
        }

        #region Value Change Watchers
        public void OnToggleValueChange()
        {
            ddLocations.interactable = toggle.isOn;
        }
        public void OnDropdownChange()
        {

        }
        #endregion
    }
}

