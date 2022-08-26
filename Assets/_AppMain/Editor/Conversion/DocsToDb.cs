using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System;
using Cards;
using UnityEditor;
using SimpleSQL;


[System.Serializable]
public class AllElestrals
{
    [PrimaryKey]
    public int uniqueId { get; set; }
    public string species { get; set; }
}
namespace Conversion
{
    public static class DocsToDb
    {
        private readonly static string TableName = "elestralsDoc";
        private readonly static string CardTableName = "CardDTO";
        private readonly static string ElestralTableName = "ElestralDTO";

        //[MenuItem("Data Converting/Elestrals")]
        //public static void ConvertElestrals()
        //{
        //    List<SheetsElestralsDTO> dtos = ElestralList;

        //    List<CardDTO> cards = new List<CardDTO>();
        //    List<ElestralDTO> elestrals = new List<ElestralDTO>();

        //    for (int i = 0; i < dtos.Count; i++)
        //    {
        //        CardDTO cDto = FromSheets(dtos[i], i);
        //        cards.Add(cDto);

        //        if (cDto.cardClass == (int)CardType.Elestral)
        //        {
        //            ElestralDTO eDto = FromSheets(dtos[i]);
        //            elestrals.Add(eDto);
        //        }
                
        //    }

        //    SaveConvertedElestrals(cards, elestrals);
        //}

        //[MenuItem("Data Converting/Effects")]
        //public static void EffectChange()
        //{
        //    List<SheetsElestralsDTO> dtos = ElestralList;
        //    List<CardDTO> cards = CardService.GetAll<CardDTO>("CardDTO");

        //    for (int i = 0; i < dtos.Count; i++)
        //    {
        //        for (int j = 0; j < cards.Count; j++)
        //        {
        //            if (dtos[i].ElestralName.ToLower().Contains(cards[j].title.ToLower()))
        //            {
        //                if (!string.IsNullOrEmpty(dtos[i].Effect))
        //                {
        //                    cards[j].effect = dtos[i].Effect;
        //                    DataService.Save<CardDTO>(cards[j], CardTableName, "cardKey", cards[j].cardKey);
        //                }
                        

        //            }
        //        }
        //    }
        //}

        //[MenuItem("Data Converting/Update Card Key")]
        //public static void UpdateCardKey()
        //{
        //    string setName = "BaseSet";
        //    string prefix = "bs_";

        //    List<CardDTO> cards = DataService.GetAll<CardDTO>(CardTableName);

        //    List<string> UniqueElestrals = new List<string>();


        //    List<ElestralDTO> elestrals = new List<ElestralDTO>();

        //    for (int i = 0; i < cards.Count; i++)
        //    {
        //        //List<string> countOf = UniqueElestrals.FindAll(delegate (string s) { return s.ToLower() == $"{cards[i].title.ToLower()}"; });
        //        //int uniqueCount = countOf.Count;

        //        //string uString = uniqueCount.ToString();
        //        //if (uniqueCount < 9)
        //        //{
        //        //    uString = "00" + uniqueCount.ToString();
        //        //}
        //        //else if (uniqueCount < 99)
        //        //{
        //        //    uString = "0" + uniqueCount.ToString();
        //        //}

        //        //string newKey = $"{prefix}{uString}";

        //        string uString = i.ToString();
        //        if (i < 10)
        //        {
        //            uString = "00" + i.ToString();
        //        }
        //        else if (i < 100)
        //        {
        //            uString = "0" + i.ToString();
        //        }
        //        string newKey = "bs_" + uString;
        //        string oldKey = cards[i].cardKey;
        //        //cards[i].cardKey = newKey;

        //        //DataService.Save<CardDTO>(cards[i], CardTableName, "cardKey", oldKey);

        //        string query = $"UPDATE CardDTO set cardKey = '{newKey}' WHERE cardKey = '{oldKey}'";
        //        DataService.DoQuery(query);

        //    }




        //}


        //[MenuItem("Data Converting/Image")]
        //public static void DoCardImage()
        //{
        //    List<CardDTO> cards = CardService.GetAll<CardDTO>(CardTableName);
        //    List<CardDTO> newCards = new List<CardDTO>();

        //    for (int i = 0; i < cards.Count; i++)
        //    {
        //        string title = cards[i].title.ToLower();
        //        title = title.Replace(" ", "");
        //        title = title.Replace("'", "");
        //        string image = $"{cards[i].setName}_{title}";
        //        cards[i].imageFile = image;
        //    }

        //    SaveCards(cards);
        //}

        private static void SaveCards(List<CardDTO> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                DataService.Save<CardDTO>(cards[i], CardTableName, "cardKey", cards[i].cardKey);
            }
        }

        private static void SaveConvertedElestrals(List<CardDTO> cards, List<ElestralDTO> elestrals)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                DataService.Save<CardDTO>(cards[i], CardTableName, "cardKey", cards[i].cardKey);
            }

            for (int i = 0; i < elestrals.Count; i++)
            {
                DataService.Save<ElestralDTO>(elestrals[i], ElestralTableName, "cardKey", elestrals[i].eleKey);
            }
        }

        private static List<SheetsElestralsDTO> ElestralList
        {
            get
            {
                return DataService.GetAll<SheetsElestralsDTO>(TableName);
            }
        }

        #region Column Conversions
        private static  ElementCode ElementToCode(string ele)
        {
            if (string.IsNullOrEmpty(ele)) { return ElementCode.None; }
            for (int i = 0; i < Enum.GetNames(typeof(ElementCode)).Length; i++)
            {
                ElementCode code = (ElementCode)i;
                if (ele.ToLower() == code.ToString().ToLower())
                {
                    return code;
                }
            }

            return ElementCode.None;
        }

        private static Elestral.SubClass SubClassToEnum(string subclass)
        {
            if (string.IsNullOrEmpty(subclass)) { return Elestral.SubClass.None; }

            for (int i = 0; i < Enum.GetNames(typeof(ElementCode)).Length; i++)
            {
                Elestral.SubClass code = (Elestral.SubClass)i;
                if (subclass.ToLower() == code.ToString().ToLower())
                {
                    return code;
                }
            }

            return Elestral.SubClass.None;

        }
        #endregion


        #region Converted DTOs
        private static CardDTO FromSheets(SheetsElestralsDTO dto, int count)
        {
            CardDTO card = new CardDTO
            {
                cardKey = dto.Codename,
                title = dto.ElestralName,
                cardClass = 1,
                artist = dto.Artist,
                effect = dto.Effect,
                
            };

            
            card.cost1 = (int)ElementToCode(dto.Element1);
            card.cost2 = (int)ElementToCode(dto.Element2);
            card.cost3 = (int)ElementToCode(dto.Element3);
            //card.rarity = 1;
            //card.setName = "First Set";

            return card;


        }

        private static ElestralDTO FromSheets(SheetsElestralsDTO dto)
        {
            ElestralDTO card = new ElestralDTO
            {
                eleKey = dto.Codename,
                species = dto.ElestralName,
                attack = dto.Attack,
                defense = dto.Defense,
                subClass1 = (int)SubClassToEnum(dto.SubClass),
                subClass2 = (int)SubClassToEnum(dto.SubClass2)
            };

            return card;
        }
        #endregion

    }
}

