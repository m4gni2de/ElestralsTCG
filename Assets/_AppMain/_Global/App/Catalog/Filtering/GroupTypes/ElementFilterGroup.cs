using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardsUI.Filtering
{
    public class ElementFilterGroup : FilterGroup
    {
        private bool _MultiTypeMode = false;
        public bool MultiTypeMode
        {
            get
            {
                return _MultiTypeMode;
            }
            set
            {
                _MultiTypeMode = value;
                if (value == true)
                {
                    joinWord = Joiner.And;
                }
                else
                {
                    joinWord = Joiner.Or;
                }
            }
        }


        public void ToggleMultiType()
        {
            _MultiTypeMode = !MultiTypeMode;

        }

        public override bool Validate()
        {
            bool validate = true;
            if (MultiTypeMode)
            {
                if (CheckedCount > 3)
                {
                    validate = false;
                    App.ShowMessage("Multi Type Mode is selected but more than 3 Elements are checked.");
                }
            }
            return validate;
        }
    }

}
