using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormattedText : MonoBehaviour
{

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


    private void Awake()
    {
        
    }

    public void SetText(string txt)
    {
        CustomFont.FormatEffect(txt, TextBox);
    }
    public void Blank()
    {
        TextBox.text = "";
    }
    public void SetStellar(string txt)
    {
        string newText = $"{CustomFont.StellarSymbol} {txt}";
        CustomFont.Format(newText, TextBox);
    }
}
