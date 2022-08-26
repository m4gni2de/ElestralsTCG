using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        #endregion
        public FormattedText title;
        public TMP_Text artist;
        public TMP_Text cardNumber;
        public TMP_Text trademark;
        public TMP_Text edition;
        public FormattedText Effect;


        private List<SpecialWord> _specialWords = null;
        protected List<SpecialWord> specialWords
        {
            get
            {
                _specialWords ??= new List<SpecialWord>();
                return _specialWords;
            }
            private set { _specialWords = value; }
        }

        public class SpecialWord
        {
            public string word;
            public int start;
            public int end;
            public int effectIndex;
            public List<int> chars = new List<int>();
            public Color32 wordColor;
            public SpecialWord(string w, int startIndex, int endIndex, int index, Color32 color)
            {
                word = w;
                start = startIndex;
                end = endIndex;
                effectIndex = index;
                wordColor = color;

                for (int i = startIndex; i <= endIndex; i++)
                {
                    chars.Add(i);
                }
            }

        }


        public virtual void SetTexts(Card card)
        {
            specialWords.Clear();

            string cardTitle = card.cardData.cardName;
            if (card.cardData.rarity == Rarity.Stellar)
            {   
                title.SetStellar(card.cardData.cardName);
                //CustomFont.Format(cardTtle, )
            }
            else
            {
                title.SetText(cardTitle);
            }
            
            Effect.SetText(card.cardData.effect);
            artist.text = card.cardData.artist;
            cardNumber.text = $"{card.cardData.setNumber}/?";
            trademark.text = "@ 2022 Elestrals LLC";
            edition.text = "1st";
        }


        public void SetBlank()
        {
            Effect.Blank();
            artist.text = "";
            cardNumber.text = "";
            trademark.text = "";
            edition.text = "";
            title.Blank();
        }
        //private static string EffectText(string effect)
        //{
           
        //}

        #region Text Formatting

        
        public void EffectFormatter(string effect, int effectIndex)
        {

            int start = 0;
            bool hasStarted = false;
            string curWord = "";
            string curChar = "";
            for (int i = 0; i < effect.Length; i++)
            {
                string c = effect[i].ToString();
                if (hasStarted)
                {
                    curWord += c;
                    if (c == FormatCharacters[curChar])
                    {
                        Color32 col = TextColor(c);
                        SpecialWord word = new SpecialWord(curWord, start, i, effectIndex, col);
                        specialWords.Add(word);
                        curWord = "";
                        curChar = "";
                        hasStarted = false;
                    }

                }
                else if (FormatCharacters.ContainsKey(c))
                {
                    curChar = c;
                    curWord = c;
                    hasStarted = true;
                    start = i;
                }

            }


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

        }

        public void TextFormatter(TMP_Text text, int effectIndex)
        {
            TMP_TextInfo textInfo = text.textInfo;
            Color32[] newVertexColors;
            Color32 c0 = text.color;
            Color32 cBase = Color.white;

            for (int i = 0; i < specialWords.Count; i++)
            {
                SpecialWord sw = specialWords[i];
                if (sw.effectIndex == effectIndex)
                {

                    c0 = sw.wordColor;
                    for (int c = 0; c < sw.chars.Count; c++)
                    {
                        // Get the index of the material used by the current character.
                        int materialIndex = textInfo.characterInfo[sw.chars[c]].materialReferenceIndex;

                        // Get the vertex colors of the mesh used by this text element (character or sprite).
                        newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                        // Get the index of the first vertex used by this text element.
                        int vertexIndex = textInfo.characterInfo[sw.chars[c]].vertexIndex;

                        newVertexColors[vertexIndex + 0] = c0;
                        newVertexColors[vertexIndex + 1] = c0;
                        newVertexColors[vertexIndex + 2] = c0;
                        newVertexColors[vertexIndex + 3] = c0;


                        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    }
                }
            }

            //TMP_TextInfo info = text.textInfo;

            //info.characterInfo[0].co
        }

        public Color32 TextColor(string s)
        {
            if (s == "[" || s == "]")
            {
                return new Color32((byte)0, (byte)149, (byte)255, 255);
            }
            if (s == "<" || s == ">")
            {
                return new Color32((byte)255, (byte)0, (byte)0, 255);
            }
            if (s == "{" || s == "}")
            {
                return new Color32((byte)203, (byte)73, (byte)148, 255);
            }

            return new Color32(255, 255, 255, 255);
        }
        #endregion
    }
}

