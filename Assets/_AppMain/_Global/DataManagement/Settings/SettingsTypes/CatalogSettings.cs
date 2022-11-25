using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    [System.Serializable]
    public class CatalogSettings : ISettingsType<CatalogSettings>
    {
        /// <summary>
        /// If true, then cards with the same art and effect, but in different sets, will display if the set the duplicate card is in is being searched. Examples are cards in Base Set 1 and Starter Deck 1.
        /// They have a different set number, but are the same card, effect and art wise.
        /// </summary>
        public bool displayDuplicates;

        /// <summary>
        /// If true, groups cards with the same effect, but different arts. A toggle on the display for the base card will allow you to cycle between the alt arts.
        /// </summary>
        public bool groupAltArts;



        public CatalogSettings Default
        {
            get
            {
                return new CatalogSettings
                {
                    displayDuplicates = false,
                    groupAltArts = true
                };
            }
        }
    }
}

