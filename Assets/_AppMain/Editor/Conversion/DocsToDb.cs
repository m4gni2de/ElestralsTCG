using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System;
using Cards;
using UnityEditor;
using SimpleSQL;
using Newtonsoft.Json;
using Decks;
using Defective.JSON;
using UnityEditor.ShaderGraph.Serialization;
using Logging;

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
        private readonly static string TableName = "xElestralsDoc";
        private readonly static string RuneTableName = "xRunesDoc";
        private readonly static string CardTableName = "CardDTO";
        private readonly static string ElestralTableName = "ElestralDTO";

        private static List<DocsElestralDTO> CardsFromDocs
        {
            get
            {
                return DataService.GetAll<DocsElestralDTO>(TableName);
            }
        }
        private static List<DocsRuneDTO> RunesFromDocs
        {
            get
            {
                return DataService.GetAll<DocsRuneDTO>(RuneTableName);
            }
        }

        
       

        //[MenuItem("Data Converting/Elestrals")]
        public static void UpdateElestralsFromSheets()
        {
            List<DocsElestralDTO> dtos = CardsFromDocs;
            List<CardDTO> cards = new List<CardDTO>();

            for (int i = 0; i < dtos.Count; i++)
            {
                CardDTO master = UpdatedElestralFromDoc(dtos[i]);
                if (master != null)
                {
                    cards.Add(master);

                }
            }

            foreach (var item in cards)
            {
                //Debug.Log(JsonUtility.ToJson(item, true));
                CardService.UpdateOnly<CardDTO>(item, CardService.CardDTOTable, "cardKey", item.cardKey);
            }
        }
        public static CardDTO UpdatedElestralFromDoc(DocsElestralDTO dto)
        {
            string setKey = $"{dto.SetName}-{dto.SetNumber}";
            qBaseCard card = CardService.CardBySetKey(setKey);
            if (card != null)
            {
                CardDTO master = CardService.ByKey<CardDTO>(CardService.CardDTOTable, "cardKey", card.cardKey);
                if (master != null)
                {
                    master.effect = dto.Effect;
                    if (dto.A.HasValue) { master.attack = dto.A.Value; } else { master.attack = null; }
                    if (dto.D.HasValue) { master.defense = dto.D.Value; } else { master.defense = null; }

                    master.subType1 = (int)SubClassToEnum(dto.Subclass);
                    master.subType2 = (int)SubClassToEnum(dto.Subclass2);
                   

                    if (!string.IsNullOrEmpty(dto.Type1)) { int e = (int)ElementToCode(dto.Type1); master.cost1 = e; } else { master.cost1 = (int)ElementCode.None; }
                    if (!string.IsNullOrEmpty(dto.Type2)) { int e = (int)ElementToCode(dto.Type2); master.cost2 = e; } else { master.cost2 = (int)ElementCode.None; }
                    if (!string.IsNullOrEmpty(dto.Type3)) { int e = (int)ElementToCode(dto.Type3); master.cost3 = e; } else { master.cost3 = (int)ElementCode.None; }

                    return master;
                }
            }
            return null;
        }


        //[MenuItem("Data Converting/Runes")]
        public static void UpdateRunesFromSheets()
        {
            List<DocsRuneDTO> dtos = RunesFromDocs;
            List<CardDTO> cards = new List<CardDTO>();

            for (int i = 0; i < dtos.Count; i++)
            {
                CardDTO master = UpdatedRunesFromDoc(dtos[i]);
                if (master != null)
                {
                    cards.Add(master);

                }
            }

            foreach (var item in cards)
            {
                //Debug.Log(JsonUtility.ToJson(item, true));
                CardService.UpdateOnly<CardDTO>(item, CardService.CardDTOTable, "cardKey", item.cardKey);
            }
        }
        public static CardDTO UpdatedRunesFromDoc(DocsRuneDTO dto)
        {
            string setKey = $"{dto.SetName}-{dto.SetNumber}";
            qBaseCard card = CardService.CardBySetKey(setKey);
            if (card != null)
            {
                CardDTO master = CardService.ByKey<CardDTO>(CardService.CardDTOTable, "cardKey", card.cardKey);
                if (master != null)
                {
                    master.effect = dto.Effect;
                    master.subType1 = (int)RuneTypeToEnum(dto.RuneType);
                    master.subType2 = 0;
                    master.attack = null;
                    master.defense = null;

                    if (!string.IsNullOrEmpty(dto.Type1)) { int e = (int)ElementToCode(dto.Type1); master.cost1 = e; } else { master.cost1 = (int)ElementCode.None; }
                    if (!string.IsNullOrEmpty(dto.Type2)) { int e = (int)ElementToCode(dto.Type2); master.cost2 = e; } else { master.cost2 = (int)ElementCode.None; }
                    if (!string.IsNullOrEmpty(dto.Type3)) { int e = (int)ElementToCode(dto.Type3); master.cost3 = e; } else { master.cost3 = (int)ElementCode.None; }

                    return master;
                }
            }
            return null;
        }

        

        private static void SaveCards(List<qBaseCard> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                DataService.Save<qBaseCard>(cards[i], CardTableName, "cardKey", cards[i].cardKey);
            }
        }

        private static void SaveConvertedElestrals(List<qBaseCard> cards, List<ElestralDTO> elestrals)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                DataService.Save<qBaseCard>(cards[i], CardTableName, "cardKey", cards[i].cardKey);
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

            for (int i = 0; i < Enum.GetNames(typeof(Elestral.SubClass)).Length; i++)
            {
                Elestral.SubClass code = (Elestral.SubClass)i;
                if (subclass.ToLower() == code.ToString().ToLower())
                {
                    return code;
                }
            }

            return Elestral.SubClass.None;

        }

        private static Rune.RuneType RuneTypeToEnum(string subclass)
        {
            if (string.IsNullOrEmpty(subclass)) { return Rune.RuneType.none; }

            for (int i = 0; i < Enum.GetNames(typeof(Rune.RuneType)).Length; i++)
            {
                Rune.RuneType code = (Rune.RuneType)i;
                if (subclass.ToLower() == code.ToString().ToLower())
                {
                    return code;
                }
            }

            return Rune.RuneType.none;

        }
        #endregion


        #region Converted DTOs
        private static qBaseCard FromSheets(SheetsElestralsDTO dto, int count)
        {
            qBaseCard card = new qBaseCard
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


        //[MenuItem("Generate/Find Mentions")]
        public static void FindPairs()
        {
            List<qUniqueCard> all = CardService.GetAll<qUniqueCard>(CardService.qUniqueCardView);

            for (int i = 0; i < all.Count; i++)
            {
                qUniqueCard card = all[i];
                string pairs = $"{card.title} Mentions: ";
                int pairCount = 0;
                List<string> mentions = new List<string>();
                if (string.IsNullOrEmpty(card.effect))
                {
                    continue;
                }
                else
                {
                    foreach (var item in all)
                    {
                       
                        if (item.title != card.title)
                        {
                            
                            if (card.effect.ToLower().Contains(item.title.ToLower())) 
                            {
                                if (!mentions.Contains(item.title))
                                {
                                    mentions.Add(item.title);
                                    pairCount += 1;
                                    pairs += $"{item.title}, ";
                                }
                                
                            }
                        }
                        

                    }

                }

                if (pairCount > 0) { LogController.LogSimple(pairs); }
            }
        }

    }
}

