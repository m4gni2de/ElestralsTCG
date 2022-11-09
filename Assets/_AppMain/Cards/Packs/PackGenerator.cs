using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Packs
{
    public class PackGenerator : MonoBehaviour
    {
        private static PackGenerator _instance;
        public static PackGenerator Instance
        {
            get => _instance;
            private set
            {
                if (_instance == null)
                    _instance = value;
                else if (_instance != value)
                {
                    Debug.Log($"{nameof(GameLogic)} instance already exists, destroying duplicate!");
                    Destroy(value);
                }
            }
        }

        public BoosterSet boosterSet;
        public BlisterPack pack;

        public List<CardView> cardList = new List<CardView>();
        public CardView templateCard;
        public CardView cardDisplay;
       
        public Transform ScrollContent;
        public GameObject CardScroll;

        
        [SerializeField]
        private GridLayoutGroup Grid;
        private GridSettings gridSettings;

     

        public static PackGenerator LoadGenerator()
        {
            if (Instance == null)
            {
                GameObject go = AssetPipeline.GameObjectClone("PackGenerator", WorldCanvas.Instance.transform);
                //GameObject go = AssetPipeline.WorldObjectClone("PackGenerator");
                PackGenerator g = go.GetComponent<PackGenerator>();
                Instance = g;
                Instance.Create();

            }
            else
            {
                Instance.Display();
            }
            
            return Instance;



        }

        private void Awake()
        {
           
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


        
        private void Create()
        {
            Display();
            
            templateCard.MatchSize(Grid.cellSize);

            cardList.Add(templateCard);
            for (int i = 0; i < 9; i++)
            {
                CardView c = Instantiate(templateCard, ScrollContent);
                cardList.Add(c);
            }
            Refresh();
            boosterSet = new BoosterSet("bs");

            GeneratePack();
        }
        private void Display()
        {
            gameObject.SetActive(true);
        }
        public static void CloseGenerator()
        {
            if (Instance != null)
            {
                Instance.Refresh();
                Instance.gameObject.SetActive(false);
                
            }

        }
        public void Refresh()
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].LoadCard();
            }
            cardDisplay.LoadCard();
            cardDisplay.gameObject.SetActive(false);
            CardScroll.SetActive(true);
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
            
            cardDisplay.LoadCard(obj.ActiveCard);
            
           
            CardScroll.SetActive(false);

            string sortLayerName = "InputMenus";
            cardDisplay.SetSortingLayer(sortLayerName);
            //DisplayManager.SetAction(() => HideDisplay());
            DisplayManager.AddAction(HideDisplay);


        }
        public void HideDisplay()
        {
            cardDisplay.gameObject.SetActive(false);
            CardScroll.SetActive(true);
            //DisplayManager.RemoveAction(() => HideDisplay());
            DisplayManager.RemoveAction(HideDisplay);

        }


        
    }
}

