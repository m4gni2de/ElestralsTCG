using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Databases;

namespace Packs
{
    [System.Serializable]
    public class PackData
    {
        public string packId;
        public List<Card> cards = new List<Card>();

        public string[] C = new string[5];
        public string[] UC = new string[3];
        public string[] S = new string[1];
        public string[] R = new string[1];



        public PackData(int index)
        {
            packId = $"pack_{index}";
        }
        public PackData(BoosterSet set)
        {
            packId = $"{set.SetName}_pack_{UniqueString.GetTempId(packId)}";
        }

        public void SetCommons(List<CardData> commons)
        {
            for (int i = 0; i < commons.Count; i++)
            {
                C[i] = commons[i].cardKey;
                cards.Add(Card.FromData(commons[i]));
            }
        }

        public void SetUncommons(List<CardData> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                UC[i] = cards[i].cardKey;
                this.cards.Add(Card.FromData(cards[i]));
            }
        }

        public void SetSpirit(CardData card)
        {
            S[0]= card.cardKey;
            cards.Add(Card.FromData(card));
        }
        public void SetRare(CardData card)
        {
            R[0] = card.cardKey;
            cards.Add(Card.FromData(card));
        }


    }

    [System.Serializable]
    public class Box
    {
        public string boxId;
        public int packCount;


        public PackData[] packs;

        public List<CardData> AllCards = new List<CardData>();
        public List<CardData> Commons = new List<CardData>();
        public List<CardData> Uncommons = new List<CardData>();
        public List<CardData> Spirits = new List<CardData>();
        public List<CardData> Rares = new List<CardData>();
        public List<CardData> Holos = new List<CardData>();
        public List<CardData> FullArts = new List<CardData>();
        public List<CardData> AltSpirits = new List<CardData>();
        public List<CardData> StellarRares = new List<CardData>();

        public Box(BoosterSet bSet, int index)
        {
            boxId = $"{bSet.SetName}_box_{index}";
            packCount = BoosterSet.PacksPerBox;
            packs = new PackData[packCount];
        }

        public void AddPack(PackData data)
        {
            packs[packs.Length - 1] = data;
        }

        public void AddStellar(CardData data)
        {
            StellarRares.Add(data);
        }
        public void AddAltSpirit(CardData data)
        {
            AltSpirits.Add(data);
        }
        public void AddFullArts(CardData data)
        {
            FullArts.Add(data);
        }
        public void AddHolos(CardData data)
        {
            Holos.Add(data);
        }
        public void AddRares(CardData data)
        {
            Rares.Add(data);
        }

    }

    [System.Serializable]
    public class BoosterSet
    {
        #region Static Properties
        public static readonly int Common = 290;
        public static readonly int Uncommon = 139;
        public static readonly int Rare = 63;
        public static readonly int Spirit = 340;
        public static readonly int RareHolo = 27;
        public static readonly int FullArt = 12;
        public static readonly int AltSpirit = 21;
        public static readonly int StellarRare = 1;

        public static readonly int CardsPerPack = 10;
        public static readonly int PacksPerBox = 36;
        public static readonly int BoxesForStellarRare = 10;
        public static readonly int StellarsPerSet = 5;

        //or just use 1/3
        public static readonly float HoloOdds = .333f;
        //or just use 1/18
        public static readonly float FullArtOdds = .055f;
        //or just use 1/360
        public static readonly float StellarOdds = .0027f;
        public static readonly float AltSpiritOdds = .0556f;






        public static int BoxesForCompleteSet
        {
            get { return BoxesForStellarRare * StellarsPerSet; }
        }
        public static int PacksForCompleteSet
        {
            get { return BoxesForCompleteSet * PacksPerBox; }
        }
        public static int CardsForCompleteSet
        {
            get { return PacksForCompleteSet * CardsPerPack; }
        }
        #endregion

        #region Properties
        protected List<CardData> AllCards = new List<CardData>();
        protected List<CardData> Commons = new List<CardData>();
        protected List<CardData> Uncommons = new List<CardData>();
        protected List<CardData> Spirits = new List<CardData>();
        protected List<CardData> Rares = new List<CardData>();
        protected List<CardData> Holos = new List<CardData>();
        protected List<CardData> FullArts = new List<CardData>();
        protected List<CardData> AltSpirits = new List<CardData>();
        protected List<CardData> StellarRares = new List<CardData>();


        public string SetName;
        public List<Box> Boxes = new List<Box>();

        public List<Box> AllBoxes
        {
            get
            {
                List<Box> boxes = new List<Box>();
                for (int i = 0; i < Boxes.Count; i++)
                {
                    boxes.Add(Boxes[i]);
                }
                return boxes;
            }
        }

        #endregion

        public PackData[] Packs;

        public BoosterSet(string setName)
        {
            SetName = setName;

            List<CardDTO> cards = CardService.BySet(CardService.CardTable, setName);


            for (int i = 0; i < cards.Count; i++)
            {
                if ((CardType)cards[i].cardClass == CardType.Spirit)
                {
                    CardData data = new CardData(cards[i]);
                    if (data.rarity == Rarity.SecretRare)
                    {
                        AddAllCards(data, AltSpirits, 1);
                    }
                    else
                    {
                        AddAllCards(data, Spirits, 1);
                    }
                }

                if ((CardType)cards[i].cardClass == CardType.Elestral)
                {
                    ElestralData data = new ElestralData(cards[i]);
                    AddCard(data);

                }
                if ((CardType)cards[i].cardClass == CardType.Rune)
                {
                    RuneData data = new RuneData(cards[i]);
                    AddCard(data);

                }
            }

        }

        public PackData GeneratePack()
        {
            PackData data = new PackData(this);
            data.SetCommons(GetCommons());
            data.SetUncommons(GetUncommons());
            data.SetSpirit(GetSpirit());
            data.SetRare(GetRare());
            return data;
        }

        protected List<CardData> GetCommons()
        {
            List<CardData> commonsPool = new List<CardData>();
            for (int i = 0; i < Commons.Count; i++)
            {
                commonsPool.Add(Commons[i]);
            }
            return GetCards(commonsPool, 5);
        }
        protected List<CardData> GetUncommons()
        {
            List<CardData> pool = new List<CardData>();
            for (int i = 0; i < Uncommons.Count; i++)
            {
                pool.Add(Uncommons[i]);
            }
            return GetCards(pool, 3);
        }
        protected CardData GetSpirit()
        {
            
            int altRand = Random.Range(0, 18);
            int rand = Random.Range(0, Spirits.Count - 1);
            if (altRand == 0)
            {
                return AltSpirits[rand];
            }
            else
            {
                return Spirits[rand];
            }
        }
        protected CardData GetRare()
        {
            List<CardData> cards = new List<CardData>();

            int stellarRand = Random.Range(0, 360);
            if (stellarRand == 0)
            {
                int rand = Random.Range(0, StellarRares.Count - 1);
                return StellarRares[rand];
            }

            int altRand = Random.Range(0, 18);
            if (altRand <= 5)
            {
                int rand = Random.Range(0, Holos.Count - 1);
                return Holos[rand];
            }
            else if (altRand == 6)
            {
                int rand = Random.Range(0, FullArts.Count - 1);
                return FullArts[rand];
            }
            else
            {
                int rand = Random.Range(0, Rares.Count - 1);
                return Rares[rand];
            }
        }
        protected List<CardData> GetCards(List<CardData> pool, int count)
        {
            List<CardData> cards = new List<CardData>();
            for (int i = 0; i < count; i++)
            {
                int rand = Random.Range(0, pool.Count - 1);
                cards.Add(pool[rand]);
                pool.RemoveAt(rand);
            }
            return cards;
        }

       

        public void AddCard(CardData data)
        {
            if (data.rarity == Rarity.Common)
            {
                AddAllCards(data, Commons, 1);
            }
            if (data.rarity == Rarity.Uncommon)
            {
                AddAllCards(data, Uncommons, 1);
            }
            if (data.rarity == Rarity.Rare)
            {
                AddAllCards(data, Rares, 1);
            }
            if (data.rarity == Rarity.HoloRare)
            {
                AddAllCards(data, Holos, 1);
            }
            if (data.rarity == Rarity.SecretRare)
            {
                AddAllCards(data, FullArts, 1);
            }
            if (data.rarity == Rarity.Stellar)
            {
                AddAllCards(data, StellarRares, 1);
            }
        }

        private void AddAllCards(CardData data, List<CardData> list, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(data);
            }
        }


        public void GeneratePacks()
        {
            Packs = new PackData[PacksForCompleteSet];

            List<CardData> commonsPool = new List<CardData>();
            
            
            for (int i = 0; i < Commons.Count; i++)
            {
                commonsPool.Add(Commons[i]);
            }

            List<CardData> ucPool = new List<CardData>();
            for (int i = 0; i < Uncommons.Count; i++)
            {
                ucPool.Add(Uncommons[i]);
            }

            List<CardData> spiritsPool = new List<CardData>();
            for (int i = 0; i < Spirits.Count; i++)
            {
                spiritsPool.Add(Spirits[i]);
            }
            for (int i = 0; i < AltSpirits.Count; i++)
            {
                spiritsPool.Add(AltSpirits[i]);
            }

            List<CardData> raresPool = new List<CardData>();
            for (int i = 0; i < Rares.Count; i++)
            {
                raresPool.Add(Rares[i]);
            }
            for (int i = 0; i < Holos.Count; i++)
            {
                raresPool.Add(Holos[i]);
            }
            for (int i = 0; i < FullArts.Count; i++)
            {
                raresPool.Add(FullArts[i]);
            }
            for (int i = 0; i < StellarRares.Count; i++)
            {
                raresPool.Add(StellarRares[i]);
            }






            for (int i = 0; i < Packs.Length; i++)
            {
                PackData packData = new PackData(i);
                for (int c = 0; c < 5; c++)
                {
                    if (commonsPool.Count > 0)
                    {
                        int rand = Random.Range(0, commonsPool.Count - 1);
                        packData.C[c] = commonsPool[rand].cardKey;
                        commonsPool.Remove(commonsPool[rand]);
                    }
                    
                }
                for (int u = 0; u < 3; u++)
                {
                    if (ucPool.Count > 0)
                    {
                        int rand = Random.Range(0, ucPool.Count - 1);
                        packData.UC[u] = ucPool[rand].cardKey;
                        ucPool.Remove(ucPool[rand]);
                    }
                   
                }

                if (spiritsPool.Count > 0)
                {
                    int sRand = Random.Range(0, spiritsPool.Count - 1);
                    packData.S[0] = spiritsPool[sRand].cardKey;
                    spiritsPool.Remove(spiritsPool[sRand]);
                }
                

                if (raresPool.Count > 0)
                {
                    int rRand = Random.Range(0, raresPool.Count - 1);
                    packData.R[0] = raresPool[rRand].cardKey;
                    raresPool.Remove(raresPool[rRand]);
                }
               
                Packs[i] = packData;
            }

            //GenerateBoxes();
        }

       
        public void GenerateBoxes()
        {
            List<PackData> packData = new List<PackData>();
            for (int i = 0; i < Packs.Length; i++)
            {
                packData.Add(Packs[i]);
            }

            for (int i = 0; i < BoxesForCompleteSet; i++)
            {
                Box b = new Box(this, i);
                Boxes.Add(b);
                for (int p = 0; p < PacksPerBox; p++)
                {
                    b.AddPack(packData[0]);
                    packData.RemoveAt(0);
                }
            }

            AddStellars();

        }

        public void FillSpirits()
        {
            for (int i = 0; i < Packs.Length; i++)
            {

            }
        }
        public void AddStellars()
        {
            List<Box> boxes = AllBoxes;

            for (int i = 0; i < StellarRares.Count; i++)
            {
                int rand = Random.Range(0, boxes.Count);
                boxes[rand].AddStellar(StellarRares[i]);
                boxes.Remove(boxes[rand]);

            }

            AddAltSpirits();
        }

        public void AddAltSpirits()
        {
            List<CardData> cards = new List<CardData>();
            for (int i = 0; i < AltSpirits.Count; i++)
            {
                cards.Add(AltSpirits[i]);
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                int rand = Random.Range(0, cards.Count);
                Boxes[i].AddAltSpirit(cards[rand]);
                cards.Remove(cards[rand]);
                int rand2 = Random.Range(0, cards.Count);
                Boxes[i].AddAltSpirit(cards[rand2]);
                cards.Remove(cards[rand2]);
            }

            AddFullArts();
        }

        public void AddFullArts()
        {
            List<CardData> cards = new List<CardData>();
            for (int i = 0; i < FullArts.Count; i++)
            {
                cards.Add(FullArts[i]);
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                int rand = Random.Range(0, cards.Count);
                Boxes[i].AddFullArts(cards[rand]);
                cards.Remove(cards[rand]);
                int rand2 = Random.Range(0, cards.Count);
                Boxes[i].AddFullArts(cards[rand2]);
                cards.Remove(cards[rand2]);
            }

            AddHolos(12);

        }

        public void AddHolos(int perBox)
        {
            List<CardData> cards = new List<CardData>();
            for (int i = 0; i < Holos.Count; i++)
            {
                cards.Add(Holos[i]);
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int j = 0; j < perBox; j++)
                {
                    int rand = Random.Range(0, cards.Count);
                    Boxes[i].AddHolos(cards[rand]);
                    cards.Remove(cards[rand]);
                }
            }

            AddRares(24);
        }

        public void AddRares(int perBox)
        {
            List<CardData> cards = new List<CardData>();
            for (int i = 0; i < Rares.Count; i++)
            {
                cards.Add(Rares[i]);
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int j = 0; j < perBox; j++)
                {
                    int rand = Random.Range(0, cards.Count);
                    Boxes[i].AddRares(cards[rand]);
                    cards.Remove(cards[rand]);
                }
            }
        }

        public void AddUncommons(int perBox)
        {
            List<CardData> cards = new List<CardData>();
            for (int i = 0; i < Uncommons.Count; i++)
            {
                cards.Add(Uncommons[i]);
            }

            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int j = 0; j < perBox; j++)
                {
                    int rand = Random.Range(0, cards.Count);
                    Boxes[i].AddRares(cards[rand]);
                    cards.Remove(cards[rand]);
                }
            }
        }
    }
}

