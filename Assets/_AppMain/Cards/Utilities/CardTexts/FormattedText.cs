using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using GlobalUtilities;
using Elements;
using System.Globalization;

public class FormattedText : MonoBehaviour
{
    #region Properties
    private TMP_Text _TextBox = null;
    public TMP_Text TextBox
    {
        get
        {
            if (_TextBox == null)
            {
                _TextBox = GetComponent<TMP_Text>();
            }
            return _TextBox;
        }
    }
   
    
    public string Content { get { return TextBox.text; } }

    public Color TextColor
    {
        get
        {
            return TextBox.color;
        }
        private set
        {
            TextBox.color = value;
        }
    }


    #endregion
    #region Custom Font Formatting
    private static Dictionary<string, CustomGlyph> _customChars = null;
    public static Dictionary<string, CustomGlyph> CustomChars
    {
        get
        {
            if (_customChars == null)
            {
                _customChars = new Dictionary<string, CustomGlyph>();
                List<zCustomText> dtos = DataService.GetAll<zCustomText>("zCustomText");

                for (int i = 0; i < dtos.Count; i++)
                {
                    CustomGlyph c = new CustomGlyph(dtos[i]);
                    _customChars.Add(c.EncodedText, c);
                }
            }
            return _customChars;
        }
    }

    private static Regex m_RegexExpression = new Regex(@"(?<!\\)(?:\\u[0-9a-fA-F]{4}|\\U[0-9a-fA-F]{8})");

    public static void FormatTextBox(TMP_Text textLabel, string stringWithUnicodeChars)
    {
        string plainText = stringWithUnicodeChars;

        stringWithUnicodeChars = m_RegexExpression.Replace(stringWithUnicodeChars,
            match =>
            {
                if (match.Value.StartsWith("\\U"))
                {
                    string st = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\U", " "), NumberStyles.HexNumber));
                    return st;
                }

                string stl = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\u", " "), NumberStyles.HexNumber));
                return stl;
            });

        textLabel.text = stringWithUnicodeChars;

    }

    #endregion

    #region Font Materials
    [SerializeField]
    private Material defaultMaterial = null;

    private Material _tempMaterial = null;
    private Material tempMaterial
    {
        get { return _tempMaterial; }
        set
        {
            if (value == null)
            {
                if (_tempMaterial != null) { Destroy(_tempMaterial); }
                _tempMaterial = null;
                FontMaterial = defaultMaterial; 
            }
            else
            {
                if (_tempMaterial != null) { Destroy(_tempMaterial); }
                _tempMaterial = value;
               FontMaterial = value;
            }
        }
    }
    protected Material FontMaterial
    {
        get { return TextBox.fontSharedMaterial; }
        set { TextBox.fontSharedMaterial = value; }
    }
   
    #endregion



    private void Awake()
    {
        defaultMaterial = new Material(TextBox.fontSharedMaterial);
    }
    
    public void SetText(string txt, bool colorSpirits = false)
    {
        SetText(txt, Color.clear, colorSpirits); 
    }
   
    public void SetText(string txt, Color col, bool colorSpirits)
    {
        
        Show();
        //CustomFont.FormatEffect(txt, TextBox, colorSpirits);
        Format(txt, colorSpirits);
        if (col != Color.clear)
        {
            SetColor(col);
        }
        
    }
   
    public void Hide(bool blank)
    {

        gameObject.SetActive(false);
        if (blank)
        {
            Blank();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Blank()
    {
        TextBox.text = "";
    }
    public void SetStellar(string txt)
    {
        Show();
        string newText = $"{CustomGlyph.Stellar.UnicodeString} {txt}";
        //CustomFont.FormatTextBox(newText, TextBox);
        FormatTextBox(TextBox,newText);
    }

    public void SetColor(Color col)
    {
        Show();
        TextColor = col;
    }

    public void SetMaterial(Material mat = null)
    {
        if (mat != null)
        {
            tempMaterial = new Material(mat);
        }
        else
        {
            tempMaterial = null;
        }
        
    }

    private void Format(string effect, bool colorSpirits)
    {
        string newEffect = effect;

        foreach (var item in CustomChars)
        {
            
            if (newEffect.Contains(item.Key))
            {
               
                string newUnicode = $" {item.Value.UnicodeString} ";
                if (colorSpirits)
                {
                    newUnicode = $" {item.Value.UnicodeWithColor} ";
                }

                newEffect = newEffect.Replace(item.Key, newUnicode);


            }
        }

        FormatTextBox(TextBox,newEffect);
        //FormatTextBox(newEffect, textLabel);
    }



    private void OnDestroy()
    {
        if (tempMaterial != null) { Destroy(tempMaterial); tempMaterial = null; }
        if (defaultMaterial != null) { Destroy(defaultMaterial); defaultMaterial = null; }
    }

    private void ColorMesh(string st)
    {
        TMP_TextInfo textInfo = TextBox.textInfo;
        Color32[] newVertexColors;
        Color32 c0 = Color.blue;
        Color32 cBase = Color.white;

        for (int c = 0; c < st.Length; c++)
        {
            // Get the index of the material used by the current character.
            //int materialIndex = textInfo.characterInfo[st[c]].materialReferenceIndex;
            int materialIndex = 0;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[st[c]].vertexIndex;

            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;


            TextBox.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        }
    }
    private static void ColorChar(string st, TMP_Text text)
    {
        TMP_TextInfo textInfo = text.textInfo;
        Color32[] newVertexColors;
        Color32 c0 = Color.blue;
        Color32 cBase = Color.white;

        for (int c = 0; c < st.Length; c++)
        {
            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[st[c]].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[st[c]].vertexIndex;

            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;


            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        }
    }
}
