using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


        public void LoadCards(List<GameCard> cards, bool faceUp = true)
        {
            if (IsOpen) { return; }


            for (int i = 0; i < cards.Count; i++)
            {
                m_VisualInfo.Add(cards[i], cards[i].m_VisualInfo);
                CardObject co = cards[i].cardObject;
                DisplayCard(co);
                if (faceUp)
                {
                    if (!cards[i].IsFaceUp) { cards[i].cardObject.Flip(); }
                }
                else
                {
                    if (cards[i].IsFaceUp) { cards[i].cardObject.Flip(true); }
                }
            }

            Open();
            
        }

        protected void DisplayCard(CardObject co)
        {
            co.SetAsChild(Content, CardScale, SortLayer, 0);
            
        }


        #region Closing
        protected override void Close()
        {
            foreach (var item in m_VisualInfo)
            {
                if (item.Key != null)
                {
                    GameCard g = item.Key;
                    g.m_VisualInfo = item.Value;
                }
               
            }

            m_VisualInfo.Clear();
            base.Close();
        }
        #endregion


    }
}

