using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cards;
using CardsUI.Glowing;

namespace CardsUI
{
    public class ElestralTexts : CardTexts
    {
        public TMP_Text Attack;
        public TMP_Text Defense;

        public override void SetTexts(Card card)
        {
            
            base.SetTexts(card);
            if (card.CardType != CardType.Elestral)
            {
                Attack.gameObject.SetActive(false);
                Defense.gameObject.SetActive(false);
                Attack.color = Color.white;
                Defense.color = Color.white;
            }
            else
            {
                ElestralData elData = (ElestralData)card.cardData;

                Attack.gameObject.SetActive(true);
                Defense.gameObject.SetActive(true);
                Attack.text = elData.attack.ToString();
                Defense.text = elData.defense.ToString();
                Attack.color = card.OfElement(0).Code.TextColor();
                Defense.color = card.OfElement(1).Code.TextColor();
            }


           
            
        }

        protected override List<Renderer> GetTextRenderers()
        {
            List<Renderer> list = new List<Renderer>();
            list.AddRange(title.GetComponentsInChildren<Renderer>(true));
            list.AddRange(artist.GetComponentsInChildren<Renderer>(true));
            list.AddRange(cardNumber.GetComponentsInChildren<Renderer>(true));
            list.AddRange(trademark.GetComponentsInChildren<Renderer>(true));
            list.AddRange(edition.GetComponentsInChildren<Renderer>(true));
            list.AddRange(Effect.GetComponentsInChildren<Renderer>(true));
            list.AddRange(Attack.GetComponentsInChildren<Renderer>(true));
            list.AddRange(Defense.GetComponentsInChildren<Renderer>(true));
            return list;
        }
    }
}

