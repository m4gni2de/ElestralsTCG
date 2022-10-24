using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Databases.Views;


namespace Cards
{
    public class CardService : DataService
    {

        public static readonly string ElestralDTOTable = "cElestralDTO";
        public static readonly string RuneDTOTable = "cRuneDTO";
        public static readonly string vElestralsView = "vElestrals";
        public static readonly string vRunesView = "vRunes";
        


        public static readonly string CardTable = "qCard";
        public static readonly string CardsByImageTable = "qCards";
        public static readonly string CardArtView = "qCardArt";





        #region Converters

        #endregion

        #region Find Cards
        public static ElestralData FindElestralCard(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            CardDTO dto = ByKey<CardDTO>(CardTable, "cardKey", key);
            ElestralData data = new ElestralData(dto);
            return data;
        }
        public static ElestralData FindElestral(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            qCards dto = ByKey<qCards>(CardsByImageTable, "setKey", key);
            ElestralData data = new ElestralData(dto);
            return data;
        }
        public static RuneData FindRuneCard(string key)
        {

            CardDTO dto = ByKey<CardDTO>(CardTable, "cardKey", key);
            RuneData data = new RuneData(dto);
            return data;
        }
        public static RuneData FindRune(string key)
        {

            qCards dto = ByKey<qCards>(CardsByImageTable, "setKey", key);
            RuneData data = new RuneData(dto);
            return data;
        }
        public static CardData FindSpiritCard(string key)
        {

            CardDTO dto = ByKey<CardDTO>(CardTable, "cardKey", key);
            CardData data = new CardData(dto);
            return data;
        }
        public static CardData FindSpirit(string key)
        {

            qCards dto = ByKey<qCards>(CardsByImageTable, "setKey", key);
            CardData data = new CardData(dto);
            return data;
        }

        public static CardData FindCard(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            CardDTO dto = ByKey<CardDTO>(CardTable, "cardKey", key);
            CardData data = new CardData(dto);
            return data;
        }
        #endregion

        #region Find Filtered Cards
        public static List<CardDTO> BySet(string table, string setName)
        {
            string query = $"setName = '{setName}'";
            List<CardDTO> list = ListByQuery<CardDTO>(table, query);
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
            CardDTO dto = GetFirstWhere<CardDTO>(CardTable, qWhere);
            return dto.image;
        }

        public static string CardArtFile(string imageKey)
        {
            string qWhere = $"cardKey = '{imageKey}'";
            qCardArt dto = GetFirstWhere<qCardArt>(CardArtView, qWhere);
            return dto.image;
        }


        #region Runes


        #endregion


        #region Network Card
        public static Decks.Decklist.DeckCard DeckCardFromDownload(string cardKey)
        {
            qCards dto = CardService.ByKey<qCards>(CardService.CardsByImageTable, "setKey", cardKey);
            return new Decks.Decklist.DeckCard { cardType = (CardType)dto.cardClass, copy = 1, key = cardKey };
        }
        public static Decks.Decklist.DeckCard DeckCardFromDTO(DeckCardDTO card)
        {
            qCards dto = ByKey<qCards>(CardsByImageTable, "setKey", card.setKey);
            return new Decks.Decklist.DeckCard { cardType = (CardType)dto.cardClass, copy = card.qty, key = card.setKey };
        }
        #endregion
    }
}

