using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{


    public class HandSlot : CardSlot
    {
        public ScrollRect m_Scroll;
        protected RectTransform Content { get { return m_Scroll.content; } }

        [SerializeField]
        private SpriteDisplay sp;

        protected override void SetSlot()
        {
            orientation = Orientation.Vertical;
            slotType = CardLocation.Hand;

            
        }
        public override void AllocateTo(GameCard card)
        {
            cards.Add(card);
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
            
            card.SetSlot(index);
            card.AllocateTo(slotType);


            DisplayCardObject(card);
            SetCommands(card);
        }

        protected override void DisplayCardObject(GameCard card)
        {
            card.cardObject.SetAsChild(Content, CardScale, SortLayer, 0);
            card.cardObject.Flip();
        }
        protected override void SetCommands(GameCard card)
        {
            TouchObject to = card.cardObject.touch;
            to.OnClickEvent.AddListener(() => ClickCard(card));
            to.OnHoldEvent.AddListener(() => GameManager.Instance.DragCard(card, this));
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.card.CardType == CardType.Spirit) { return false; }
            return true;
        }



        protected override void GetSpriteRenderer()
        {

        }

        public override void SetValidate(bool isValid)
        {
            if (isValid)
            {
                sp.SetColor(new Color(Color.green.r, Color.green.g, Color.green.g, .45f));
            }
            else
            {
                sp.SetColor(new Color(Color.red.r, Color.red.g, Color.red.g, .45f));
            }
        }
        public override void ClearValidation()
        {
            sp.SetColor(Color.clear);
        }

    }
}
