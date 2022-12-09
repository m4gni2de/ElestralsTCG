using System.Collections;
using System.Collections.Generic;
using Cards.Collection;
using Cards;
using UnityEngine;
using System.Linq;

namespace Databases
{

    public class CardCollectionService : PlayerService
    {
        protected static readonly string CollectionTable = "CardCollection";


        public static int NewCardsAdded()
        {
            List<qUniqueCard> allCards = CardService.CardsByUniqueArt();
            List<CardCollectionDTO> collected = GetAll<CardCollectionDTO>(CollectionTable);


            List<CardCollectionDTO> notAdded = new List<CardCollectionDTO>();

            for (int i = 0; i < allCards.Count; i++)
            {
                qUniqueCard card = allCards[i];
                bool contains = false;
                foreach (var item in collected)
                {
                    if (item.setKey == card.setKey && item.rarity == card.rarity)
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains) { continue; }
                CardCollectionDTO dto = new CardCollectionDTO { setKey = card.setKey, rarity = card.rarity, qty = 0, colWhen = null };
                notAdded.Add(dto);
                ;
            }

            int count = 0;
            for (int i = 0; i < notAdded.Count; i++)
            {

                Insert(notAdded[i], CollectionTable);
                count += 1;
            }
            db.Commit();
            return count;
        }

        public static int QuantityOf(Card card)
        {
            string qWhere = $"setKey = '{card.cardData.cardKey}' AND rarity = {(int)card.GetRarity}";
            CardCollectionDTO dto = GetFirstWhere<CardCollectionDTO>(CollectionTable, qWhere);
            if (dto != null) { return dto.qty; }
            return 0;
        }

        public static CardCollectionDTO FindData(string setKey, int rarity)
        {
            string qWhere = $"setKey = '{setKey}' AND rarity = {rarity}";
            CardCollectionDTO dto = GetFirstWhere<CardCollectionDTO>(CollectionTable, qWhere);
            if (dto != null) { return dto; }
            return null;
        }
        public static CardCollectionDTO FindData(Card card)
        {
            string qWhere = $"setKey = '{card.cardData.cardKey}' AND rarity = {(int)card.GetRarity}";
            CardCollectionDTO dto = GetFirstWhere<CardCollectionDTO>(CollectionTable, qWhere);
            if (dto != null) { return dto; }
            return null;
        }
        public static void SaveCard(CardCollectionDTO dto)
        {
            if (FindData(dto.setKey, dto.rarity) == null)
            {
                Insert(dto, CollectionTable);
            }
            else
            {
                db.UpdateTable(dto, CollectionTable);
            }
        }

    }
}
