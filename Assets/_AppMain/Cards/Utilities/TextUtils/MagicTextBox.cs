using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using GlobalUtilities;
using Elements;
using System.Globalization;
using UnityEngine.Events;
using System;

public class MagicTextBox : MonoBehaviour, iSortRenderer
{
    #region Operators
    public static implicit operator MagicTextBox(TMP_Text textbox)
    {
        return textbox.gameObject.GetOrAddComponent<MagicTextBox>();
    }
    #endregion

    #region Properties
    
    [SerializeField] private TMP_Text _TextBox = null;
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

    
    public string Content
    {
        get
        {
            return TextBox.text.Trim();
        }
        set
        {
            TextBox.text = value;
        }
    }

    protected string _plainText = "";
    public string PlainText { get { return _plainText.Trim(); } }

    protected bool IsDirty = false;
    protected void SetDirty(bool dirty)
    {
        IsDirty = dirty;
    }
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
    public bool IsContentChanged()
    {
        if (string.IsNullOrEmpty(SavedText)) { return false; }
        return SavedText.Trim() != Content.Trim();
    }

    #region Sort Order/Layer
    private Renderer _rend = null;
    public Renderer textRenderer
    {
        get
        {
            _rend ??= TextBox.GetComponent<Renderer>();
            return _rend;
        }
    }


   
    public int SortOrder
    {
        get { return textRenderer.sortingOrder; }
        set { textRenderer.sortingOrder = value;  }
    }
    public string SortLayer
    {
        get { return SortingLayer.IDToName(textRenderer.sortingLayerID); }
        set { textRenderer.sortingLayerID = SortingLayer.NameToID(value);  }
    }

    private bool _isLoaded = false;
    public bool IsLoaded
    {
        get
        {
            return _isLoaded;
        }
        set
        {
            _isLoaded = value;
            if (value == true) { Load(); }
        }
    }

    protected bool HasInitialText = false;
    #endregion

    #endregion

    #region Customized Options
    [Header("Customizaion")]
    [SerializeField] protected bool SaveInitialText = false;
    #endregion

    
    #region Text Events
    private UnityEvent _OnTextChanged = null;
    public UnityEvent OnTextChanged
    {
        get
        {
            _OnTextChanged ??= new UnityEvent();
            return _OnTextChanged;
        }
    }
    private void TextChanged()
    {
        OnTextChanged.Invoke();
    }

    public void AddTextChangeListener(UnityAction ac)
    {
        RemoveTextChangeListener(ac);
        OnTextChanged.AddListener(ac);
    }
    public void RemoveTextChangeListener(UnityAction ac)
    {
        OnTextChanged.RemoveListener(ac);
    }
    #endregion


    #region Initialize

    private void Awake()
    {
        defaultMaterial = new Material(TextBox.fontSharedMaterial);
    }

    public void Hide(bool blank)
    {

        gameObject.SetActive(false);
        if (blank)
        {
            Blank();
        }
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (!IsLoaded) { IsLoaded = true; }
    }
    protected virtual void Load()
    {
        HasInitialText = false;
        
    }
    public virtual void Refresh(bool clearListeners = true)
    {
        ClearSaved();
        TextHistory.Clear();
        Blank();
        SetDirty(false);
        _isLoaded = false;
        if (clearListeners)
        {
            OnTextChanged.RemoveAllListeners();
        }


    }
    public void Blank()
    {
        Content = "";
        _plainText = "";
        
    }
    #endregion

    #region Set Text
    protected virtual void DisplayText(string formattedString, string plainText)
    {
        Content = formattedString;
        _plainText = plainText;
        TextChanged();
    }

    public virtual void SetText(string txt)
    {
        SetText(txt, Color.clear, false, false);
    }
    public void SetText(string txt, bool saveToHistory)
    {
        SetText(txt, Color.clear, false, saveToHistory);
    }
    public void SetText(string txt, Color col, bool colorSpirits)
    {

        SetText(txt, col, colorSpirits, false);

    }
    public void SetText(string txt, Color col, bool colorSpirits, bool saveToHistory)
    {
        
        Show();

        string formattedText = GetFormattedText(txt, colorSpirits);
        DisplayText(formattedText, txt);

        if (col != Color.clear) { SetColor(col); }

        if (!HasInitialText)
        {
            HasInitialText = true;
            if (SaveInitialText)
            {
                SaveCurrent();
            }
            
        }

        if (saveToHistory)
        {
            ArchiveText();
        }
    }
    
   
    #endregion

    public void SetStellar(string txt)
    {
        Show();
        string newText = $"{CustomGlyph.Stellar.UnicodeString} {txt}";
;
        string unicode;
        newText.UnicodeString(out unicode);

        DisplayText(unicode, txt);
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


    private string GetFormattedText(string originalText, bool colorSpirits)
    {
        string newText = originalText;

        foreach (var item in CustomChars)
        {

            if (newText.Contains(item.Key))
            {

                string newUnicode = $" {item.Value.UnicodeString} ";
                if (colorSpirits)
                {
                    newUnicode = $" {item.Value.UnicodeWithColor} ";
                }

                newText = newText.Replace(item.Key, newUnicode);


            }
        }


        string unicodeChars;
        newText.UnicodeString(out unicodeChars);
        return unicodeChars;
        

    }
   
    public void SetSortLayer(string layerName)
    {
        SortLayer = layerName;
    }
    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    private void OnDestroy()
    {
        if (tempMaterial != null) { Destroy(tempMaterial); tempMaterial = null; }
        if (defaultMaterial != null) { Destroy(defaultMaterial); defaultMaterial = null; }
        OnTextChanged.RemoveAllListeners();
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


    #endregion

    #region Font Materials
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
        set { TextBox.fontSharedMaterial = value; TextBox.ForceMeshUpdate(); }

    }

    #endregion

    #region Text History

    protected string _savedText = "";
    public string SavedText
    {
        get
        {
            return _savedText;
        }
    }
    public void SaveCurrent()
    {
        _savedText = PlainText;

    }
    public void LoadSavedText()
    {
        if (!string.IsNullOrEmpty(_savedText))
        {
            SetText(_savedText);
            _savedText = "";
        }
    }
    public void ClearSaved()
    {
        _savedText = "";
    }
    private List<string> _textHistory = null;
    public List<string> TextHistory
    {
        get
        {
            _textHistory ??= new List<string>();
            return _textHistory;
        }
    }

    public void ArchiveText()
    {
        bool addText = false;
        if (!string.IsNullOrEmpty(PlainText))
        {
            if (TextHistory.Count == 0)
            {
                addText = true;
            }
            else
            {
                string last = TextHistory.Last();
                if (last.Trim() != PlainText)
                {
                    addText = true;
                }
            
            }
        }
        if (addText)
        {
            TextHistory.Add(PlainText);
        }
    }
    #endregion
}
