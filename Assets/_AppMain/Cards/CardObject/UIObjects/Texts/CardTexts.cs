using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;


namespace CardsUI
{
    public class CardTexts : MonoBehaviour
    {
        #region Static Functions
        private static Dictionary<string, string> _FormatCharacters = null;
        public static Dictionary<string, string> FormatCharacters
        {
            get
            {
                if (_FormatCharacters == null)
                {
                    _FormatCharacters = new Dictionary<string, string>();
                    _FormatCharacters.Add("<", ">");
                }
                return _FormatCharacters;
            }
        }

        private static string SpiritEffectWaypoint = "Spirit";
        private static string RegularEffectWaypoint = "Regular";
        #endregion

        #region Properties
        private List<MagicTextBox> _allTexts = null;
        public List<MagicTextBox> AllTexts
        {
            get
            {
                _allTexts ??= CollectionHelpers.ListWith(title, artist, cardNumber, edition, Effect, Attack, Defense, trademark);
                return _allTexts;
            }
        }

        public MagicTextBox title;
        public MagicTextBox artist;
        public MagicTextBox cardNumber;
        public MagicTextBox edition;
        public MagicTextBox Effect;
        public MagicTextBox Attack;
        public MagicTextBox Defense;
        public MagicTextBox trademark;

        private Dictionary<MagicTextBox, Color> _defaultColors = null;
        public Dictionary<MagicTextBox, Color> DefaultColors
        {
            get
            {
                _defaultColors ??= new Dictionary<MagicTextBox, Color>();
                return _defaultColors;
            }
        }

        private DynamicObject dynamicEffectText
        {
            get
            {
                return Effect.GetComponent<DynamicObject>();
            }
        }
        #endregion

       

        //private List<SpecialWord> _specialWords = null;
        //protected List<SpecialWord> specialWords
        //{
        //    get
        //    {
        //        _specialWords ??= new List<SpecialWord>();
        //        return _specialWords;
        //    }
        //    private set { _specialWords = value; }
        //}

        //public class SpecialWord
        //{
        //    public string word;
        //    public int start;
        //    public int end;
        //    public List<int> chars = new List<int>();
        //    public Color32 wordColor;
        //    public SpecialWord(string w, int startIndex, int endIndex, Color32 color)
        //    {
        //        word = w;
        //        start = startIndex;
        //        end = endIndex;
        //        wordColor = color;

        //        for (int i = startIndex; i <= endIndex; i++)
        //        {
        //            chars.Add(i);
        //        }
        //    }

        //}


        #region Set-Up
        public void SetBlank()
        {
            Effect.Blank();
            artist.Blank();
            cardNumber.Blank();
            trademark.Blank();
            edition.Blank();
            title.Blank();
            Attack.Blank();
            Defense.Blank();
        }
        public virtual void LoadTexts(Card card)
        {
            //specialWords.Clear();

            string cardTitle = card.cardData.cardName;
            if (card.cardData.rarity == Rarity.Stellar || card.cardData.artType == ArtType.Stellar)
            {   
                title.SetStellar(card.cardData.cardName);
            }
            else
            {
                title.SetText(cardTitle);
            }


            dynamicEffectText.MoveToWaypoint(RegularEffectWaypoint);
            Effect.SetText(card.cardData.effect);
            artist.SetText(card.cardData.artist);
            cardNumber.SetText($"{card.cardData.setNumber}/{CardLibrary.SetCount(card.cardData.setCode)}");
            trademark.SetText("@ 2022 Elestrals LLC");
            edition.SetText("1st");
            

            if (card.CardType == CardType.Elestral)
            {
                
                DoElestral((Elestral)card);
            }
            else if (card.CardType == CardType.Rune)
            {
                DoRune((Rune)card);
            }
            else if (card.CardType == CardType.Spirit)
            {
                DoSpirit(card);
            }
            
            CreateDefaults();
        }






        private void DoElestral(Elestral e)
        {
            bool isGold = e.IsGold;
            
            if (isGold)
            {
                Attack.SetText(e.Data.attack.ToString(), Color.yellow, false);
                Defense.SetText(e.Data.defense.ToString(), Color.yellow, false);
            }
            else
            {
                Attack.SetText(e.Data.attack.ToString(), e.OfElement(0).Code.TextColor(), false);
                Defense.SetText(e.Data.defense.ToString(), e.OfElement(1).Code.TextColor(), false);
            }
            
            if (!e.isFullArt)
            {
                if (isGold)
                {
                    title.SetColor(Color.yellow);
                    Effect.SetColor(Color.yellow);
                }
                else
                {
                    title.SetColor(Color.black);
                    Effect.SetColor(Color.black);
                    title.SetMaterial();
                    Effect.SetMaterial();

                }
                
            }
            else
            {
                if (isGold)
                {
                    title.SetColor(Color.yellow);
                    Effect.SetColor(Color.yellow);
                    title.SetMaterial();
                    Effect.SetMaterial();
                }
                else
                {
                    title.SetColor(ElementCode.Any.TextColor());
                    Effect.SetColor(ElementCode.Any.TextColor());

                    Material titleMat = e.OfElement(0).FontMaterial("Name");
                    Material effectMat = e.OfElement(0).FontMaterial("Effect");
                    if (titleMat != null) { title.SetMaterial(titleMat); Effect.SetMaterial(effectMat); }
                }
               




            }

           

           

        }

        private void DoRune(Rune r)
        {
            Attack.Hide(true);
            Defense.Hide(true);

            bool isGold = r.IsGold;

            if (!r.isFullArt)
            {
                if (isGold)
                {
                    title.SetColor(Color.yellow);
                    Effect.SetColor(Color.yellow);
                }
                else
                {
                    title.SetColor(Color.black);
                    Effect.SetColor(Color.black);
                    title.SetMaterial();
                    Effect.SetMaterial();
                }
               
            }
            else
            {
                if (isGold)
                {
                    title.SetColor(Color.yellow);
                    Effect.SetColor(Color.yellow);
                    title.SetMaterial();
                    Effect.SetMaterial();
                }
                else
                {
                    title.SetColor(ElementCode.Any.TextColor());
                    Effect.SetColor(ElementCode.Any.TextColor());
                    Material titleMat = r.OfElement(0).FontMaterial("Name");
                    Material effectMat = r.OfElement(0).FontMaterial("Effect");
                    if (titleMat != null) { title.SetMaterial(titleMat); Effect.SetMaterial(effectMat); }
                }
               

                
            }
            
        }

        private void DoSpirit(Card card)
        {
            Attack.Hide(true);
            Defense.Hide(true);
            dynamicEffectText.MoveToWaypoint(SpiritEffectWaypoint);
            if (card.IsGold)
            {
                title.SetColor(Color.yellow);
                Effect.SetColor(Color.yellow);
            }
            else
            {
                title.SetMaterial();
                Effect.SetMaterial();
            }
           
        }
#endregion


        #region Text Formatting
        public void CreateDefaults()
        {
            DefaultColors.Clear();

            for (int i = 0; i < AllTexts.Count; i++)
            {
                MagicTextBox t = AllTexts[i];
                DefaultColors.Add(t, t.TextColor);
            }
        }
        public void SetToDefaults()
        {
            foreach (var item in DefaultColors)
            {
                item.Key.SetColor(item.Value);
            }
        }
        public void SetAlpha(float alpha)
        {
            foreach (var item in AllTexts)
            {
                Color col = new Color(item.TextColor.r, item.TextColor.g, item.TextColor.b, alpha);
                item.SetColor(col);
            }
        }
        public void SetColors(Color color)
        {
            foreach (var item in AllTexts)
            {
                Color col = new Color(item.TextColor.r, item.TextColor.g, item.TextColor.b, color.a);
                item.SetColor(col);
            }
        }

        public void SetSortingLayer(string layerName)
        {
            foreach (MagicTextBox item in AllTexts)
            {
                item.SortLayer = layerName;
            }
        }

        public void SetSortingOrder(int sortOrder)
        {
            foreach (MagicTextBox item in AllTexts)
            {
                item.SortOrder = sortOrder;
            }
        }

        public void ChangeSortingOrder(int changeVal)
        {
            foreach (MagicTextBox item in AllTexts)
            {
                item.SortOrder += changeVal;
            }
        }



        //public void EffectFormatter(string effect)
        //{

        //    int start = 0;
        //    bool hasStarted = false;
        //    string curWord = "";
        //    string curChar = "";
        //    for (int i = 0; i < effect.Length; i++)
        //    {
        //        string c = effect[i].ToString();
        //        if (hasStarted)
        //        {
        //            curWord += c;
        //            if (c == FormatCharacters[curChar])
        //            {
        //                Color32 col = TextColor(c);
        //                SpecialWord word = new SpecialWord(curWord, start, i, col);
        //                specialWords.Add(word);
        //                curWord = "";
        //                curChar = "";
        //                hasStarted = false;
        //            }

        //        }
        //        else if (FormatCharacters.ContainsKey(c))
        //        {
        //            curChar = c;
        //            curWord = c;
        //            hasStarted = true;
        //            start = i;
        //        }

        //    }


        //for (int i = 0; i < card.cardEffectList.Count; i++)
        //{
        //    CardEffect ce = card.cardEffectList[i];
        //    if (ce.effectIndex == 0)
        //    {
        //        if (effect.ToLower().Contains(ce.name.ToLower()))
        //        {
        //            Debug.Log(effect.IndexOf(ce.name.ToLower()));
        //        }
        //    }
        //}
        //if (effect.Contains())

        //}

        //public void TextFormatter(TMP_Text text)
        //{
        //    TMP_TextInfo textInfo = text.textInfo;
        //    Color32[] newVertexColors;
        //    Color32 c0 = text.color;
        //    Color32 cBase = Color.white;

        //    for (int i = 0; i < specialWords.Count; i++)
        //    {
        //        SpecialWord sw = specialWords[i];
        //        c0 = sw.wordColor;
        //        for (int c = 0; c < sw.chars.Count; c++)
        //        {
        //            // Get the index of the material used by the current character.
        //            int materialIndex = textInfo.characterInfo[sw.chars[c]].materialReferenceIndex;

        //            // Get the vertex colors of the mesh used by this text element (character or sprite).
        //            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

        //            // Get the index of the first vertex used by this text element.
        //            int vertexIndex = textInfo.characterInfo[sw.chars[c]].vertexIndex;

        //            newVertexColors[vertexIndex + 0] = c0;
        //            newVertexColors[vertexIndex + 1] = c0;
        //            newVertexColors[vertexIndex + 2] = c0;
        //            newVertexColors[vertexIndex + 3] = c0;


        //            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        //        }
        //    }

        //    //TMP_TextInfo info = text.textInfo;

        //    //info.characterInfo[0].co
        //}

        //public Color32 TextColor(string s)
        //{
        //    if (s == "[" || s == "]")
        //    {
        //        return new Color32((byte)0, (byte)149, (byte)255, 255);
        //    }
        //    if (s == "<" || s == ">")
        //    {
        //        return new Color32((byte)255, (byte)0, (byte)0, 255);
        //    }
        //    if (s == "{" || s == "}")
        //    {
        //        return new Color32((byte)203, (byte)73, (byte)148, 255);
        //    }

        //    return new Color32(255, 255, 255, 255);
        //}
        #endregion


    }
}

