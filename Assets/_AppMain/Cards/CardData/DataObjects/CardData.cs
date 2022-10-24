using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;


namespace Cards
{

    
    [System.Serializable]
    public class CardData : iCardData
    {
        public string cardKey { get; set; }
        public string cardName { get; set; }
        public int cardType { get; set; }
        public int cost1 { get; set; }
        public int cost2 { get; set; }
        public int cost3 { get; set; }
        public string artist { get; set; }
        public string effect { get; set; }
        public Rarity rarity { get; set; }
        public string setCode { get; set; }
        public int setNumber { get; set; }
        public ArtType artType { get; set; }
        public string image { get; set; }
        

       
        public CardData(CardDTO dto)
        {
            cardKey = dto.cardKey;
            cardName = dto.title;
            cardType = dto.cardClass;
            cost1 = dto.cost1;
            if (!dto.cost2.HasValue) { cost2 = -1; } else { cost2 = dto.cost2.Value; }
            if (!dto.cost3.HasValue) { cost3 = -1; } else { cost3 = dto.cost3.Value; }
            artist = dto.artist;
            if (!string.IsNullOrEmpty(dto.effect))
            {
                effect = dto.effect;
            }
            else
            {
                effect = "";
            }

            rarity = (Rarity)dto.rarity;
            setCode = dto.setName;
            setNumber = dto.setNumber;
            artType = (ArtType)dto.artType;
            image = CardService.CardArtFile(cardKey);


        }

        public CardData(qCards dto)
        {
            cardKey = dto.setKey;
            cardName = dto.title;
            cardType = dto.cardClass;
            cost1 = dto.cost1;
            if (!dto.cost2.HasValue) { cost2 = -1; } else { cost2 = dto.cost2.Value; }
            if (!dto.cost3.HasValue) { cost3 = -1; } else { cost3 = dto.cost3.Value; }
            artist = dto.artist;
            if (!string.IsNullOrEmpty(dto.effect))
            {
                effect = dto.effect;
            }
            else
            {
                effect = "";
            }

            rarity = (Rarity)dto.rarity;
            setCode = dto.setName;
            setNumber = dto.setNumber;
            artType = (ArtType)dto.artType;

           
            image = CardService.CardArtFile(cardKey);

            //if (!string.IsNullOrEmpty(dto.image))
            //{
            //    image = dto.image;
            //}
            //else
            //{
            //    image = "";
            //}



        }


    }

    [System.Serializable]
    public class ElestralData : CardData
    {
        public int attack { get; set; }
        public int defense { get; set; }

        public Elestral.SubClass subType1 { get; set; }
        public Elestral.SubClass subType2 { get; set; }
        
        

       
        public ElestralData(CardDTO dto) : base(dto)
        {
            if (dto.attack.HasValue) { attack = dto.attack.Value; }
            if (dto.defense.HasValue) { defense = dto.defense.Value; }
            if (dto.subType1.HasValue) { subType1 = (Elestral.SubClass)dto.subType1.Value; } else { subType1 = Elestral.SubClass.None; }
            if (dto.subType2.HasValue) { subType2 = (Elestral.SubClass)dto.subType2.Value; } else { subType2 = Elestral.SubClass.None; }
        }

        public ElestralData(qCards dto) : base(dto)
        {
            if (dto.attack.HasValue) { attack = dto.attack.Value; }
            if (dto.defense.HasValue) { defense = dto.defense.Value; }
            if (dto.subType1.HasValue) { subType1 = (Elestral.SubClass)dto.subType1.Value; } else { subType1 = Elestral.SubClass.None; }
            if (dto.subType2.HasValue) { subType2 = (Elestral.SubClass)dto.subType2.Value; } else { subType2 = Elestral.SubClass.None; }
        }



    }

    [System.Serializable]
    public class RuneData : CardData
    {
        public Rune.RuneType runeType { get; set; }

        public RuneData(CardDTO dto) :base(dto)
        {
            runeType = (Rune.RuneType)dto.subType1;
        }

        public RuneData(qCards dto) : base(dto)
        {
            runeType = (Rune.RuneType)dto.subType1;
        }

    }

    

}

