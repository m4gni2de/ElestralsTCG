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
       
        public override void LoadTexts(Card card)
        {
            
            //base.LoadTexts(card);
            //if (card.CardType != CardType.Elestral)
            //{
            //    Attack.gameObject.SetActive(false);
            //    Defense.gameObject.SetActive(false);
            //    Attack.color = Color.white;
            //    Defense.color = Color.white;
            //}
            //else
            //{
            //    ElestralData elData = (ElestralData)card.cardData;

            //    Attack.gameObject.SetActive(true);
            //    Defense.gameObject.SetActive(true);
            //    Attack.text = elData.attack.ToString();
            //    Defense.text = elData.defense.ToString();
            //    Attack.color = card.OfElement(0).Code.TextColor();
            //    Defense.color = card.OfElement(1).Code.TextColor();
            //}


           
            
        }

        
    }
}

