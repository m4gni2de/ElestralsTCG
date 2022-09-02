using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Packs
{
    public class PackGenerator : MonoBehaviour
    {
        public static PackGenerator Instance { get; private set; }

        public BoosterSet boosterSet;
        public BlisterPack pack;

        public List<CardView> cardList = new List<CardView>();
        public CardView templateCard;
        public CardDisplay cardDisplay;
       
        public Transform ScrollContent;
        public GameObject CardScroll;

     

        public static PackGenerator LoadGenerator()
        {
            GameObject go = AssetPipeline.GameObjectClone("PackGenerator", WorldCanvas.Instance.transform);
            PackGenerator g = go.GetComponent<PackGenerator>();
            g.Open();
            return g;
           
        }
        
        private void Start()
        {
           
        }

       
        //private void Open()
        //{
        //    _cards.Add(template);
        //    for (int i = 0; i < 9; i++)
        //    {
        //        CardObject c = Instantiate(template, ScrollContent);
        //        _cards.Add(c);
        //    }
        //    Refresh();

        //    _displayCanvas.gameObject.SetActive(false);
        //    displayCard.DisplayBack();
        //    boosterSet = new BoosterSet("bs");
        //}


        private void Open()
        {
            cardList.Add(templateCard);
            for (int i = 0; i < 9; i++)
            {
                CardView c = Instantiate(templateCard, ScrollContent);
                cardList.Add(c);
            }
            Refresh();

            
            cardDisplay.LoadBack();
            cardDisplay.gameObject.SetActive(false);
            boosterSet = new BoosterSet("bs");
        }
        public void Refresh()
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].LoadCard();
            }
        }

        public void GeneratePack()
        {
            Refresh();
            pack = new BlisterPack(boosterSet);
            ShowPack(pack);
        }

        public void ShowPack(BlisterPack p)
        {
            for (int i = 0; i < p.cards.Count; i++)
            {
                cardList[i].LoadCard(p.cards[i]);
            }
        }

        public void DisplayCard(CardView obj)
        {
            
            cardDisplay.LoadCard(obj);
            
           
            CardScroll.SetActive(false);

            string sortLayerName = "InputMenus";
            cardDisplay.SetSortingLayer(sortLayerName);
            

        }
        public void HideDisplay()
        {
            cardDisplay.gameObject.SetActive(false);
            CardScroll.SetActive(true);

        }
    }
}

