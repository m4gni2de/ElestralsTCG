using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Menus
{
    public class MagicToggleGroup : MonoBehaviour
    {
        #region Properties
        public enum ToggleMode
        {
            Single = 0,
            Multi = 1,
        }

        public ToggleMode toggleMode;
        private int _ToggleLimit = 1;
        public int ToggleLimit
        {
            get
            {
                if (toggleMode == ToggleMode.Single)
                {
                    _ToggleLimit = 1;
                }
                return _ToggleLimit;
            }
        }

        public List<MagicToggle> Toggles = new List<MagicToggle>();

        [SerializeField]
        protected List<MagicToggle> m_toggled = null;
        protected List<MagicToggle> toggled
        {
            get
            {
                List<MagicToggle> list = new List<MagicToggle>();
                for (int i = 0; i < Toggles.Count; i++)
                {
                    if (Toggles[i].IsOn)
                    {
                        list.Add(Toggles[i]);
                    }
                }
                m_toggled = list;
                return m_toggled;
            }
        }
        #endregion

        private void Awake()
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                Toggles[i].OnToggleChanged += ToggleChanged;
            }
        }

        private void OnEnable()
        {
            Load();
        }
        public void Refresh()
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                Toggles[i].Hide();
            }
        }

        public void ToggleChanged(MagicToggle toggle)
        {
            
            if (toggle.IsOn)
            {
                
                if (toggled.Count > ToggleLimit)
                {
                    List<MagicToggle> byDate = m_toggled;
                    byDate.Sort(ByDateDescend);
                    byDate[0].Toggle(false);
                }
            }
        }

        public List<string> GetToggledValues()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < toggled.Count; i++)
            {
                list.Add(toggled[i].valName);
            }
            return list;
        }
        public string GetToggledValue()
        {
            if (toggled.Count > 1)
            {
                App.LogWarning("This group is set to Multi Mode, so there may be more than 1 Toggled item.");
                return "";
            }
            if (toggled.Count == 0)
            {
                App.LogWarning("There are no Toggled items");
                return "";
            }
            return toggled[0].valName;
        }

        public void SetToggleText(string toggleKey, string txt)
        {
            MagicToggle t = GetToggleByKey(toggleKey);
            SetToggleText(t, txt);
        }
        public void SetToggleText(MagicToggle toggle, string txt)
        {
            toggle.SetText(txt);
        }

        protected MagicToggle GetToggleByKey(string toggleKey)
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i].valName.ToLower() == toggleKey.ToLower())
                {
                    return Toggles[i];
                }
            }
            return null;
        }


        public void Load()
        {
            Unload();
            Toggles[0].Toggle(true);
            for (int i = 0; i < Toggles.Count; i++)
            {
                Toggles[i].OnToggleChanged -= ToggleChanged;
                Toggles[i].OnToggleChanged += ToggleChanged;
            }
            
        }
        public void Unload()
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                Toggles[i].OnToggleChanged -= ToggleChanged;
                Toggles[i].Toggle(false);
            }
        }


        #region Sorters
        public int ByDate(MagicToggle x, MagicToggle y)
        {
            if (x.whenToggled > y.whenToggled) { return 1; }
            if (x.whenToggled < y.whenToggled) { return -1; }
            return 0;
        }

        public int ByDateDescend(MagicToggle x, MagicToggle y)
        {
            if (x.whenToggled > y.whenToggled) { return -1; }
            if (x.whenToggled < y.whenToggled) { return 1; }
            return 0;
        }
        #endregion

    }
}

