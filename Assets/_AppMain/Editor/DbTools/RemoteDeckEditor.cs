#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Configuration;
using Cards;
using Databases;
using Defective.JSON;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static SpriteDisplay;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class RemoteDeckEditor : EditorWindow
{

    [MenuItem("Deck Editor/Remote")]
    private static void OpenWindow()
    {
        RemoteDeckEditor window = EditorWindow.GetWindow<RemoteDeckEditor>("Remote Deck Editor");
    }
    private class Contents
    {
        public static readonly GUIContent RenderTypeLabel = new GUIContent("RenderType", "Does this SpriteDisplay use a SpriteRenderer or Image?");
        public static readonly GUIContent SpriteLabel = new GUIContent("Sprite Renderer", "The Sprite Renderer for this Sprite Display. If left blank, an SpriteRenderer Component will be added automatically.");
        public static readonly GUIContent ImageLabel = new GUIContent("Image", "The Image for this Sprite Display. If left blank, an Image Component will be added automatically.");


        public readonly GUIContent ImportButton = new GUIContent("Import", "Accepts a Datalist of Card Key Strings.");
        
        public Contents()
        {

        }
    }

    private class CardInput
    {
        public string setKey;
        public int qty;
    }

    #region Properties
    private Contents _contents = null;
    private Contents contents { get { _contents ??= new Contents(); return _contents; } }


    private UploadedDeckDTO _deck;
    public UploadedDeckDTO Deck
    {
        get
        {
            _deck ??= new UploadedDeckDTO { deckKey = "", title = "no title", deck = new List<string>() };
            return _deck;
        }
    }
    private string deckList;
    private Vector2 cardScrollPos;

    private List<CardInput> _cards = null;
    private List<CardInput> Cards { get { return _cards; } }

    private Dictionary<string, int> _cardCounts = null;
    private Dictionary<string, int> CardCounts
    {
        get
        {
            return _cardCounts;
        }
    }
    #endregion


    private void OnEnable()
    {

        SetDeck();
    }

    private void SetDeck()
    {
        
    }

    private void OnGUI()
    {
        if (_deck == null) { _deck = new UploadedDeckDTO { deckKey = "", title = "no title", deck = new List<string>() }; }

        if (_cardCounts == null) { _cardCounts = new Dictionary<string, int>(); }

        GUILayout.BeginHorizontal();


        
        deckList = GUILayout.TextArea(deckList, GUILayout.ExpandHeight(true));
        

        
        //bool didImport = false;
        if (GUILayout.Button(contents.ImportButton))
        {
            UploadedDeckDTO imported = FromImport(deckList);
            if (imported != null)
            {
                ImportCardCounts(imported.deck);
                
            }
            else
            {
                ImportCardCounts(new List<string>());
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        SetCardCounts(Deck);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            CardCounts.Add("", 1);
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Print Deck"))
        {
            if (ValidateCards())
            {

            }
            Deck.deck.Clear();
            foreach (var item in CardCounts)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    Deck.deck.Add(item.Key);
                }
            }
            string s = Deck.deck.ToJson();
            deckList = s;
        }
        GUILayout.EndHorizontal();

        


    }


    private void ImportCardCounts(List<string> cards)
    {
       


        CardCounts.Clear();

       
        for (int i = 0; i < cards.Count; i++)
        {
            string s = cards[i];
            if (CardCounts.ContainsKey(s))
            {

                CardCounts[s]++;
            }
            else
            {
                CardCounts.Add(s, 1);
            }
        }

       


    }
    private UploadedDeckDTO FromImport(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) { return null; }
        UploadedDeckDTO deck = new UploadedDeckDTO();
        deck.deckKey = UniqueString.CreateId(7, "dk");
        deck.title = "Imported Deck";
        DataList d = JsonConvert.DeserializeObject<DataList>(json);
        deck.deck = d.items;
       
        return deck;

    }

    private void SetCardCounts(UploadedDeckDTO dto)
    {


        Dictionary<string, int> currentCounts = new Dictionary<string, int>();
       
        foreach (var item in CardCounts)
        {
            currentCounts.Add(item.Key, item.Value);
        }

        GUILayout.BeginHorizontal();

        GUILayout.Label("Card Key");
        GUILayout.Label("Quantity");
        GUILayout.EndHorizontal();

        GUILayout.Space(15f);

       
        cardScrollPos = GUILayout.BeginScrollView(cardScrollPos, false, true);

        int[] options = new int[20];
        string[] optionsStr = new string[20];
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = i;
            optionsStr[i] = i.ToString();
        }

        //bool changed = false;
        
        foreach (var item in currentCounts)
        {

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string cardKey = GUILayout.TextField(item.Key);
            int qty = EditorGUILayout.IntPopup(item.Value, optionsStr, options);
            GUILayout.EndHorizontal();
           
            if (CardCounts.ContainsKey(cardKey))
            {
                if (qty > 0)
                {
                    CardCounts[cardKey] = qty;
                }
                else
                {
                    CardCounts.Remove(cardKey);
                }
                
            }
            else
            {
                CardCounts.Add(cardKey, qty);
            }

        }

        Deck.deck.Clear();
        foreach (var item in CardCounts)
        {
            for (int i = 0; i < item.Value; i++)
            {
                Deck.deck.Add(item.Key);
            }
        }




        GUILayout.EndScrollView();

    }

    private bool ValidateCards()
    {
        List<string> errors = new List<string>();
        //if (ConnectionManager.Connect())
        //{
            

        //    foreach (var item in CardCounts)
        //    {
        //        if (!CardService.KeyExists<qUniqueCard>(CardService.qUniqueCardView, "setKey", item.Key))
        //        {
        //            errors.Add(item.Key);
        //        }
               
        //    }
        //}

        for (int i = 0; i < errors.Count; i++)
        {
            Debug.Log("ERROR -" + errors[i]);
        }
        return errors.Count == 0;
    }
    
}


#endif