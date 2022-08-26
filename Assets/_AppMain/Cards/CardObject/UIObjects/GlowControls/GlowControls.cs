using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FX;


namespace CardsUI.Glowing
{
    public class GlowControls : MonoBehaviour
    {
        public static Color GlowColor(ElementCode code)
        {
            switch (code)
            {
                case ElementCode.None:
                    return Color.clear;
                case ElementCode.Wind:
                    return CardUI.FromHex("#E1F2F9");
                case ElementCode.Dark:
                    return new Color(.83f, .20f, .96f, 1f);
                case ElementCode.Earth:
                    return CardUI.FromHex("#CDFF00");
                case ElementCode.Fire:
                    return CardUI.FromHex("#F15B38");
                case ElementCode.Frost:
                    return new Color(.46f, .39f, .81f, 1f);
                case ElementCode.Light:
                    return new Color(1f, .95f, .36f, 1f);
                case ElementCode.Thunder:
                    return CardUI.FromHex("#FFC16D");
                case ElementCode.Water:
                    return new Color(.17f, .63f, 1f, 1f);
                default:
                    return Color.clear;
            }
        }

        public static Color MaskColor(ElementCode code)
        {
            switch (code)
            {
                case ElementCode.Fire:
                    return new Color(1f, .55f, .41f, .66667f);
                case ElementCode.Thunder:
                    return new Color(1f, .78f, .63f, 1f);
                default:
                    return new Color(1f, 1f, 1f, .66667f);
            }
        }

        private static string GlowAssetString(ElementCode code, bool isRight)
        {
            string prefix = "SG_";
            string suffix = "_L";

            if (isRight) { suffix = "_R"; }

            string glowString = $"{prefix}{code.ToString()}{suffix}";

            return glowString;
        }


        #region Rarity
        public GameObject rarity;

        private SpriteGradient _rarityColors = null;
        private SpriteGradient RarityColors
        {
            get
            {
                if (_rarityColors == null)
                {
                    _rarityColors = rarity.GetComponent<SpriteGradient>();
                    
                }
                return _rarityColors;
            }
        }

        private SpriteRenderer _spRarity = null;
        protected SpriteRenderer spRarity { get { _spRarity ??= rarity.GetComponent<SpriteRenderer>(); return _spRarity; } }

        #endregion


        public GlowObject LeftGlow, RightGlow;

        
        #region Indexing
        public GlowObject this[int index]
        {
            get
            {
                if (index == 0) { return LeftGlow; }
                if (index == 1) { return RightGlow; }

                return LeftGlow;
            }
        }
        #endregion

        public void DoArtChange(bool isFullArt)
        {
            LeftGlow.gameObject.SetActive(!isFullArt);
            RightGlow.gameObject.SetActive(!isFullArt);
        }

        public void SetBlank()
        {
            spRarity.sprite = null;
            LeftGlow.SetBlank();
            RightGlow.SetBlank();

        }

        public void Set(Card card)
        {
            int count = card.DifferentElements.Count;



            string spriteStr = card.GetRarity().SpriteString();
            Sprite baseSp = AssetPipeline.ByKey<Sprite>(spriteStr);

            spRarity.sprite = baseSp;



            if (count == 1)
            {
                OneElement(card.SpiritsReq[0].BaseData.Code);
                
            }
            if (count == 2)
            {
                MultiElement(card.DifferentElements);
                
            }

           
        }

        protected void OneElement(ElementCode code)
        {
            LeftGlow.Set(code);
            RightGlow.Set(code);

            Color c1 = LeftGlow.GlowSp.color;
            RarityColors.SetSingleColor(c1);

           
        }
        protected void MultiElement(List<Element> elements)
        {
            

            for (int i = 0; i < elements.Count; i++)
            {
                GlowObject obj = this[i];
                obj.Set(elements[i].Code);
            }

            //int cont = cols.Length;

            Color c1 = LeftGlow.GlowSp.color;
            Color c2 = RightGlow.GlowSp.color;
            RarityColors.SetDualGradient(c1, c2);


        }


       
    }
}

