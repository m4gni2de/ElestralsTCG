using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Databases.Views;
using static UnityEngine.Rendering.DebugUI;


namespace Cards
{
    public class CardService : DataService
    {

        public static readonly string ElestralDTOTable = "cElestralDTO";
        public static readonly string RuneDTOTable = "cRuneDTO";
        public static readonly string vElestralsView = "vElestrals";
        public static readonly string vRunesView = "vRunes";
        


        public static readonly string BaseCardView = "qBaseCard";
        public static readonly string qUniqueCardView = "qUniqueCard";
        public static readonly string CardArtView = "qCardArt";

        
        public static readonly string CardDTOTable = "CardDTO";



        #region Converters

        #endregion

        #region Find Cards

        public static qBaseCard CardBySetKey(string setKey)
        {
            string query = $"setKey = '{setKey}'";
            qBaseCard dto = GetFirstWhere<qBaseCard>(BaseCardView, query);
            if (dto != null) { return dto; } return null;
        }

        public static List<qBaseCard> CardsWithTitle(string title)
        {
            string query = $"title LIKE '%{title}%'";
            List<qBaseCard> list = ListByQuery<qBaseCard>(BaseCardView, query);
            return list;
        }
        public static ElestralData FindElestralCard(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            qBaseCard dto = ByKey<qBaseCard>(BaseCardView, "cardKey", key);
            ElestralData data = new ElestralData(dto);
            return data;
        }
        public static ElestralData FindElestral(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            qUniqueCard dto = ByKey<qUniqueCard>(qUniqueCardView, "setKey", key);
            ElestralData data = new ElestralData(dto);
            return data;
        }
        public static RuneData FindRuneCard(string key)
        {

            qBaseCard dto = ByKey<qBaseCard>(BaseCardView, "cardKey", key);
            RuneData data = new RuneData(dto);
            return data;
        }
        public static RuneData FindRune(string key)
        {

            qUniqueCard dto = ByKey<qUniqueCard>(qUniqueCardView, "setKey", key);
            RuneData data = new RuneData(dto);
            return data;
        }
        public static CardData FindSpiritCard(string key)
        {

            qBaseCard dto = ByKey<qBaseCard>(BaseCardView, "cardKey", key);
            CardData data = new CardData(dto);
            return data;
        }
        public static CardData FindSpirit(string key)
        {

            qUniqueCard dto = ByKey<qUniqueCard>(qUniqueCardView, "setKey", key);
            CardData data = new CardData(dto);
            return data;
        }

        public static CardData FindCard(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            qBaseCard dto = ByKey<qBaseCard>(BaseCardView, "cardKey", key);
            CardData data = new CardData(dto);
            return data;
        }
        #endregion

        #region Find Filtered Cards
        public static List<qUniqueCard> BySet(string table, string setName)
        {
            string query = $"setName = '{setName}'";
            List<qUniqueCard> list = ListByQuery<qUniqueCard>(table, query);
            return list;
        }
        #endregion
        public static T FindCardWithTitle<T>(string table, string title, string set) where T : new()
        {
            string query = $"title = '{title}' AND setName = '{set}'";
            T dto = ByQuery<T>(table, query);
            return dto;

        }



        #region Elestrals

        #endregion
        public static string CardArtString(string cardKey, string colName)
        {
            string qWhere = $"{colName} = '{cardKey}'";
            qBaseCard dto = GetFirstWhere<qBaseCard>(BaseCardView, qWhere);
            return dto.image;
        }

        public static string CardArtFile(string imageKey)
        {
            string qWhere = $"cardKey = '{imageKey}'";
            //Debug.Log(imageKey);
            qCardArt dto = GetFirstWhere<qCardArt>(CardArtView, qWhere);
            return dto.image;
        }


        #region Runes


        #endregion


        #region Network Card
        public static Decks.Decklist.DeckCard DeckCardFromDownload(string cardKey)
        {
            qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", cardKey);
            return new Decks.Decklist.DeckCard { cardType = (CardType)dto.cardClass, copy = 1, key = cardKey };
        }
        public static Decks.Decklist.DeckCard DeckCardFromDTO(DeckCardDTO card)
        {
            qUniqueCard dto = ByKey<qUniqueCard>(qUniqueCardView, "setKey", card.setKey);
            return new Decks.Decklist.DeckCard { cardType = (CardType)dto.cardClass, copy = card.qty, key = card.setKey };
        }
        #endregion
    }
}

