using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CardsUI;
using CardsUI.Glowing;
using CardsUI.Stones;
using FX;
using GlobalUtilities;
using TMPro;

using UnityEngine;
using UnityEngine.Profiling;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;
#endif
public class CardConfig : MonoBehaviour
{

#region Functions
    public int BaseSortOrder { get { return cardImageSp.sortingOrder; } }
    public string BaseSortLayer { get { return cardImageSp.sortingLayerName; } }

    
    private Dictionary<Renderer, int> _spriteOffset = null;
    private Dictionary<Renderer, int> SpritesSortOffset
    {
        get
        {
            if (_spriteOffset == null) 
            { 
                _spriteOffset = new Dictionary<Renderer, int>();
                Renderer[] rends = GetComponentsInChildren<Renderer>(true);

                for (int i = 0; i < rends.Length; i++)
                {
                    SetSpriteOffset(rends[i], 0);
                }
              
            }
            return _spriteOffset;
        }
    }

    private void SetSpriteOffset(Renderer rend, int offset)
    {
        if (!SpritesSortOffset.ContainsKey(rend))
        {
            SpritesSortOffset.Add(rend, offset);
        }
        else
        {
            SpritesSortOffset[rend] = offset;
        }
    }

   
    private List<SpriteRenderer> SharedRenderers
    {
        get
        {
            List<SpriteRenderer> list = new List<SpriteRenderer>();
            list.Add(cardBg);
            list.Add(cardCover);
            list.Add(raritySp);
            list.Add(cardBorder);
            return list;
        }
    }

    private List<SpriteRenderer> allSp = null;
    private List<SpriteRenderer> CardRenderers
    {
        get
        {
            if (allSp == null)
            {
                allSp = new List<SpriteRenderer>();
                allSp.AddRange(SharedRenderers);
                allSp.Add(cardImageSp);
                allSp.Add(frameSp);
                allSp.Add(glowL);
                allSp.Add(glowR);
                allSp.Add(stoneSp);
                allSp.Add(runeSpL);
                allSp.Add(runeSpR);
                allSp.Add(swordSp);
                allSp.Add(shieldSp);
                allSp.Add(raritySp);
                allSp.Add(stamp1);
                allSp.Add(stamp2);
                allSp.Add(subSp1);
                allSp.Add(subSp2);

                allSp.Add(centerStone);
                allSp.Add(leftStone);
                allSp.Add(rightStone);
                allSp.Add(leftCenterStone);
                allSp.Add(rightCenterStone);


                allSp.Add(spiritStone);
                allSp.Add(spiritGlowL);
                allSp.Add(spiritGlowR);
            }

            return allSp;
        }
    }

   
#endregion


#region Card UI Properties
    public SpriteRenderer cardImageSp;
    public SpriteRenderer cardBorder;
    public SpriteRenderer cardBg;
    public SpriteRenderer cardCover;
    public SpriteRenderer frameSp;
    public SpriteRenderer glowL;
    public SpriteRenderer glowR;
    public SpriteRenderer stoneSp;
    public SpriteRenderer runeSpL;
    public SpriteRenderer runeSpR;
    public SpriteRenderer swordSp;
    public SpriteRenderer shieldSp;
    [Header("Rarity")]
    public SpriteRenderer raritySp;

    private SpriteGradient _rarityColors = null;
    protected SpriteGradient RarityColors
    {
        get
        {
            _rarityColors ??= raritySp.GetComponent<SpriteGradient>();
            return _rarityColors;
        }
    }

    [Header("Lower Icons")]
    public SpriteRenderer stamp1;
    public SpriteRenderer stamp2;
    public SpriteRenderer subSp1;
    public SpriteRenderer subSp2;
    public SpriteRenderer centerStone, leftStone, rightStone, leftCenterStone, rightCenterStone;

    [Header("Spirit UI")]
    public SpriteRenderer spiritStone;
    public SpriteRenderer spiritGlowL;
    public SpriteRenderer spiritGlowR;

    [Header("Full Art Mask")]
    public SpriteMask artMask;

    #region Text Properties
    [SerializeField]
    private CardTexts cardTexts;
    #endregion

    #endregion

    #region Coloring Properties
    private Dictionary<SpriteRenderer, Color> _defaultColors = new Dictionary<SpriteRenderer, Color>();
    public Dictionary<SpriteRenderer, Color> DefaultColors
    {
        get
        {
            _defaultColors ??= new Dictionary<SpriteRenderer, Color>();
            return _defaultColors;
        }
    }

    #endregion

    private void Awake()
    {
        
    }


   
    public void LoadBlank()
    {
        stamp1.sprite = null;
        stamp2.sprite = null;
        centerStone.sprite = null;
        leftStone.sprite = null;
        rightStone.sprite = null;
        leftCenterStone.sprite = null;
        rightCenterStone.sprite = null;
        subSp1.sprite = null;
        subSp2.sprite = null;
        runeSpL.sprite = null;
        runeSpR.sprite = null;
        spiritStone.sprite = null;
        spiritGlowL.sprite = null;
        spiritGlowR.sprite = null;

        frameSp.gameObject.SetActive(false);
        glowL.gameObject.SetActive(false);
        glowR.gameObject.SetActive(false);
        RarityColors.enabled = false;
        raritySp.color = Color.white;

        cardTexts.SetBlank();
        raritySp.sprite = null;
        stoneSp.sprite = null;
        cardImageSp.sprite = null;
        cardBg.gameObject.SetActive(false);
        cardCover.sprite = CardFactory.cardBackSp;
    }
    public void LoadCard(Card card)
    {
        CardType ty = card.CardType;

        bool isFullArt = card.isFullArt;

        frameSp.gameObject.SetActive(true);
        glowL.gameObject.SetActive(!isFullArt);
        glowR.gameObject.SetActive(!isFullArt);
        cardBg.gameObject.SetActive(!isFullArt);

        
        stamp2.sprite = null;
        centerStone.sprite = null;
        leftStone.sprite = null;
        rightStone.sprite = null;
        leftCenterStone.sprite = null;
        rightCenterStone.sprite = null;

        spiritStone.sprite = null;
        spiritGlowL.sprite = null;
        spiritGlowR.sprite = null;

        
        raritySp.sprite = CardFactory.GetRaritySprite(card.GetRarity());

        cardTexts.LoadTexts(card);
        if (ty == CardType.Elestral)
        {
            DoElestral(card);
        }
        else if (ty == CardType.Rune)
        {
            DoRune(card);
        }
        else
        {
            DoOther(card);
        }

        LoadSprites(card);

        
        SetDefaultColors();
        UpdateMask();

    }

    private void DoElestral(Card card)
    {
        runeSpL.sprite = null;
        runeSpR.sprite = null;

        Elestral e = (Elestral)card;

        if (!card.isFullArt)
        {
            stoneSp.sprite = CardFactory.Instance.elestralStone;
        }
        else
        {
            stoneSp.sprite = CardFactory.Instance.faElestralStone;
        }

        swordSp.sprite = CardFactory.Instance.swordSp;
        shieldSp.sprite = CardFactory.Instance.shieldSp;

        if (e.Data.subType1 != Elestral.SubClass.None)
        {
            subSp1.sprite = CardFactory.GetSubClassSprite(e.Data.subType1);
        }
        else
        {
            subSp1.sprite = null;
        }
        if (e.Data.subType2 != Elestral.SubClass.None)
        {
            subSp2.sprite = CardFactory.GetSubClassSprite(e.Data.subType2);
        }
        else
        {
            subSp2.sprite = null;
        }

    }
    private void DoRune(Card card)
    {
        shieldSp.sprite = null;
        swordSp.sprite = null;
       

        Rune r = (Rune)card;
        if (!card.isFullArt)
        {
            stoneSp.sprite = CardFactory.Instance.runeStone;
        }
        else
        {
            stoneSp.sprite = CardFactory.Instance.faRuneStone;
            

        }

        runeSpL.sprite = CardFactory.GetRuneSprite(r.GetRuneType);
        runeSpR.sprite = CardFactory.GetRuneSprite(r.GetRuneType);
    }



    private void DoOther(Card card)
    {
        shieldSp.sprite = null;
        swordSp.sprite = null;
        runeSpL.sprite = null;
        runeSpR.sprite = null;
        raritySp.sprite = null;

        if (card.isFullArt)
        {
            stoneSp.sprite = CardFactory.Instance.spiritStone;
        }
        else
        {
            stoneSp.sprite = CardFactory.Instance.spiritStone;

        }

        spiritGlowL.sprite = CardFactory.Instance.spiritGlowL;
        spiritGlowR.sprite = CardFactory.Instance.spiritGlowR;
        spiritStone.sprite = CardFactory.GetLargeStoneSprite(card.SpiritsReq[0].Code);

        spiritGlowL.color = CardUI.TextColor(card.SpiritsReq[0].Code);
        spiritGlowR.color = CardUI.TextColor(card.SpiritsReq[0].Code);


    }


    public void LoadSprites(Card card)
    {
        
        cardImageSp.sprite = CardFactory.CardImage(card);
        if (!card.isFullArt)
        {
            cardBg.sprite = CardFactory.GetBackground(card.SpiritsReq[0].Code);
            glowL.sprite = CardFactory.GetGlowSprite(card.SpiritsReq[0].Code);
            if (card.SpiritsReq.Count > 1)
            {
                glowR.sprite = CardFactory.GetGlowSprite(card.SpiritsReq[1].Code);
            }
            else
            {
                glowR.sprite = glowL.sprite;
            }
        }
        else
        {
            cardBg.gameObject.SetActive(false);
        }

        stamp1.sprite = CardFactory.GetSetStamp(card.cardData.setCode);


        if (card.CardType == CardType.Elestral || card.CardType == CardType.Rune)
        {
            LoadStones(card);
            SetRarity(card);
        }

        
      
    }


#region Type Stones
    private void LoadStones(Card card)
    {
        int count = card.SpiritsReq.Count;

        if (count == 0)
        {
            Debug.Log(card.cardData.cardName);
        }
        SpriteRenderer[] stones = UseStones(count);

        for (int i = 0; i < stones.Length; i++)
        {
            SpriteRenderer s = stones[i];
            s.sprite = CardFactory.GetTypeStoneSprite(card.SpiritsReq[i].Code);
        }
    }


    protected SpriteRenderer[] UseStones(int count)
    {
        SpriteRenderer[] stones = new SpriteRenderer[count];


        if (count == 1)
        {
            stones[0] = centerStone;
            return stones;
        }
        if (count == 2)
        {
            stones[0] = leftCenterStone;
            stones[1] = rightCenterStone;
        }
        else
        {
            stones[0] = centerStone;
            stones[1] = leftStone;
            stones[2] = rightStone;
        }

        return stones;
    }

#endregion

#region Rarity
    protected void SetRarity(Card card)
    {
        
        int count = card.DifferentElements.Count;


        //Gradient rarityGrad = SpriteFactory.CreateGradient(card.DifferentElements);
        //tex = SpriteFactory.GradientTexture(rarityGrad, raritySp.sprite.texture);

        //Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        //raritySp.sprite = sprite;

        bool isAny = false;
        for (int i = 0; i < card.DifferentElements.Count; i++)
        {
            if (card.DifferentElements[i].Code == ElementCode.Any)
            {
                isAny = true;
            }
        }

        if (isAny)
        {
            Color c1 = Element.TextColor(ElementCode.Fire);
            Color c2 = Element.TextColor(ElementCode.Water);
            Color c3 = Element.TextColor(ElementCode.Wind);
            Color c4 = Element.TextColor(ElementCode.Thunder);

            List<Color> list = CollectionHelpers.ListWith(c1, c2, c3, c4);
            RarityColors.SetColors(1f, list.ToArray());
            return;
        }
        if (count == 1)
        {
            Color c1 = card.SpiritsReq[0].ElementColor();
            RarityColors.SetSingleColor(c1);
        }
        else if (count == 2)
        {
            Color c1 = card.SpiritsReq[0].ElementColor();
            Color c2 = card.SpiritsReq[1].ElementColor();
            RarityColors.SetDualGradient(c1, c2);
        }
        else if (count == 3)
        {
            Color c1 = card.SpiritsReq[0].ElementColor();
            Color c2 = card.SpiritsReq[1].ElementColor();
            Color c3 = card.SpiritsReq[2].ElementColor();

            List<Color> list = CollectionHelpers.ListWith(c1, c2, c3);
            RarityColors.SetColors(1f, list.ToArray());
        }
    }
#endregion

    public void Toggle(bool isOn)
    {
        gameObject.SetActive(isOn);
        if (isOn)
        {
            _spriteOffset = null;
        }
    }

    public void Flip(bool showBack)
    {
        if (showBack)
        {
            cardCover.sprite = CardFactory.cardBackSp;
            cardBg.gameObject.SetActive(false);
            cardBorder.sprite = CardFactory.borderSp;
            cardBorder.color = Color.black;
        }
        else
        {
            cardCover.sprite = null;
            cardBg.gameObject.SetActive(true);
            cardBorder.sprite = CardFactory.borderSp;
            cardBorder.color = Color.black;
        }
    }


    #region Coloring
       
    private void SetDefaultColors()
    {
        DefaultColors.Clear();
        foreach (var item in CardRenderers)
        {
            if (!DefaultColors.ContainsKey(item))
            {
                DefaultColors.Add(item, item.color);
            }
        }
    }

    public void ResetColors()
    {
        foreach (var item in DefaultColors)
        {
            item.Key.color = item.Value;
        }
    }
    public void Mask(Color color)
    {

        foreach (var item in CardRenderers)
        {
            item.color = color;
        }
        cardTexts.SetAlpha(color.a);
        
    }

    public void SetAlpha(float alpha)
    {
        foreach (var item in CardRenderers)
        {
            Color newColor = new Color(item.color.r, item.color.g, item.color.b, alpha);
            item.color = newColor;
        }
        cardTexts.SetAlpha(alpha);
    }
   
    public void Select(Color col)
    {
        cardBorder.color = col;
    }
#endregion

#region Sorting
    public void ChangeSortOrder(int newOrder)
    {
        
        int current = cardBg.sortingOrder;

        
        int diff = newOrder - current;

        SetSpriteOffset(cardBg, -1);
        SetSpriteOffset(cardCover, 8);
        SetSpriteOffset(cardBorder, 1);
        SetSpriteOffset(raritySp, 0);

        foreach (var item in SpritesSortOffset)
        {
            if (item.Value < 0)
            {
                item.Key.sortingOrder = newOrder;
            }
            else if (item.Value == 0)
            {
                item.Key.sortingOrder += diff;
            }
            else
            {
                item.Key.sortingOrder += (newOrder + item.Value);
            }
           
            
        }

        //Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        //for (int i = 0; i < renderers.Length; i++)
        //{
        //    renderers[i].sortingOrder += diff;
        //}
        
        UpdateMask();

    }


    public void ChangeSortLayer(string layer)
    {
       

        SetSpriteOffset(cardBg, -1);
        SetSpriteOffset(cardCover, 8);
        SetSpriteOffset(cardBorder, 1);
        SetSpriteOffset(raritySp, 0);


        foreach (var item in SpritesSortOffset)
        {
            item.Key.sortingLayerName = layer;


        }
        UpdateMask();

    }
#endregion


#region Full Art Mask
    private void UpdateMask()
    {
        artMask.isCustomRangeActive = true;
        artMask.frontSortingOrder = cardImageSp.sortingOrder + 1;
        artMask.backSortingOrder = cardImageSp.sortingOrder - 1;
        artMask.frontSortingLayerID = cardImageSp.sortingLayerID;
        artMask.backSortingLayerID = cardImageSp.sortingLayerID;
    }

#endregion

    private void OnDestroy()
    {
       
    }

}
