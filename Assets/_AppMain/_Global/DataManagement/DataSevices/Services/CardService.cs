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
        public static RuneData FindRuneCard(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            CardDTO dto = ByKey<CardDTO>(CardTable, "cardKey", key);
            RuneData data = new RuneData(dto);
            return data;
        }
        public static CardData FindSpiritCard(string key)
        {
            //CardDTO dto = ByPk<CardDTO>(CardTable, key);
            CardDTO dto = ByKey<CardDTO>(CardTable, "cardKey", key);
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
            string qWhere= $"{colName} = '{cardKey}'";
            CardDTO dto = GetFirstWhere<CardDTO>(CardTable, qWhere);
            return dto.image;
        }


        #region Runes

       
        #endregion
    }
}

