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


        public List<CardObject> _cards = new List<CardObject>();
        public CardObject template;
       
        public Transform ScrollContent;
        public GameObject CardScroll;
        public Canvas _displayCanvas;

       
        public CardObject displayCard;

        public static async Task<PackGenerator> LoadGenerator()
        {
            GameObject go = await AssetPipeline.GameObjectCloneAsync("PackGenerator", WorldCanvas.Instance.transform);
            PackGenerator g = go.GetComponent<PackGenerator>();
            g.LoadAssets();
            return g;
           
        }
        
        private void Start()
        {
           
        }

        private async void LoadAssets()
        {
            GameObject go = await AssetPipeline.GameObjectCloneAsync(CardObject.CardKey, ScrollContent);
            template = go.GetComponent<CardObject>();
            displayCard = Instantiate(template, _displayCanvas.transform);
            displayCard.SetScale(new Vector2(70f, 70f));
            displayCard.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            displayCard.touch.OnClickEvent.AddListener(() => HideDisplay());

            Open();

        }

        private void Open()
        {
            _cards.Add(template);
            for (int i = 0; i < 9; i++)
            {
                CardObject c = Instantiate(template, ScrollContent);
                _cards.Add(c);
            }
            Refresh();

            _displayCanvas.gameObject.SetActive(false);
            displayCard.DisplayBack();
            boosterSet = new BoosterSet("bs");
        }
        public void Refresh()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].DisplayBack();
                _cards[i].touch.OnClickEvent.RemoveAllListeners();
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

                _cards[i].LoadCard(p.cards[i], false);
                CardObject c = _cards[i];
                _cards[i].touch.OnClickEvent.AddListener(() => DisplayCard(c));
            }
        }

        protected void DisplayCard(CardObject obj)
        {
            _displayCanvas.gameObject.SetActive(true);
            displayCard.LoadCard(obj.ActiveCard);
            
           
            CardScroll.SetActive(false);

            string sortLayerName = "InputMenus";
            _displayCanvas.overrideSorting = true;
            _displayCanvas.sortingLayerName = sortLayerName;
            _displayCanvas.sortingOrder = 0;
            displayCard.SetSortingLayer(sortLayerName);
            

        }
        public void HideDisplay()
        {
            _displayCanvas.gameObject.SetActive(false);
            CardScroll.SetActive(true);

        }
    }
}

