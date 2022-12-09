using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;

/// <summary>
/// Possibly Depreciated
/// </summary>
public class HandMenu : EdgeMenu, iScaleCard
{
    #region Interface
    public Vector2 CardScale
    {
        get
        {
            return _cardSlot.CardScale;
        }
    }
    public string SortLayer { get { return _cardSlot.SortLayer; } }
    public bool IsClicked { get; set; }

   
    #endregion

    #region Properties
    public GameObject MenuUI;
    [SerializeField]
    private ScrollRect _scrollView;
    public RectTransform Content { get { return _scrollView.content; } }

    [SerializeField]
    private CardSlot _cardSlot;


    #endregion

   
    protected override void Open()
    {
        base.Open();
        MenuUI.SetActive(true);

        if (GameManager.Instance != null && GameManager.Instance.ActivePlayer != null && GameManager.Instance.ActivePlayer == GameManager.ActiveGame.You)
        {

        }
        

    }
    protected override void Close()
    {
        base.Close();
        MenuUI.SetActive(false);
    }

    public void AddCard(GameCard card)
    {
        Open();
        //card.cardObject.transform.SetParent(Content);
        //card.cardObject.transform.SetAsFirstSibling();
        //card.cardObject.SetScale(CardScale);
        //card.cardObject.SetSortingLayer(SortLayer);
        card.cardObject.SetAsChild(Content, CardScale, SortLayer, 0);
        card.cardObject.Flip();

        if (card.location != CardLocation.Hand)
        {
            TouchObject to = card.cardObject.touch;
            //to.AddClickListener(() => ClickCard(card));
            //to.AddHoldListener(() => DragCard(card));
            //to.OnClickEvent.AddListener(() => ClickCard(card));
            //to.OnHoldEvent.AddListener(() => DragCard(card));
        }
        

    }

 

    #region Touch Cards

    //public void ClickCard(GameCard card)
    //{
    //    GameManager.Instance.SelectCard(card);
    //}

    //public void DragCard(GameCard card)
    //{
    //    StartCoroutine(DoDragCard(card));
    //}

    //protected IEnumerator DoDragCard(GameCard card)
    //{
        
    //    //card.cardObject.transform.SetParent(transform);
    //    Vector2 newScale = new Vector2(card.cardObject.transform.localScale.x / 2f, card.cardObject.transform.localScale.y / 2f);
    //    //card.cardObject.SetScale(newScale);

    //    card.cardObject.SetAsChild(transform, newScale, SortLayer);

    //    Field f = GameManager.Instance.arena.GetPlayerField(GameManager.ActiveGame.You);
    //    do
    //    {
    //        Close();
    //        DoFreeze();
    //        yield return new WaitForEndOfFrame();
    //        var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    //        card.cardObject.transform.position = new Vector3(newPos.x, newPos.y, -2f);
    //        f.ValidateSlots(card);

    //    } while (true && Input.GetMouseButton(0));

    //    CardSlot slot = f.SelectedSlot;

    //    if (slot == null)
    //    {
    //        AddCard(card);
    //        f.SetSlot();
    //    }
    //    else
    //    {
    //        if (slot.ValidateCard(card))
    //        {
    //            //_cardSlot.RemoveCard(card);
    //            slot.AllocateTo(card);
    //        }
    //        else
    //        {
    //            AddCard(card);
    //        }
    //        f.SetSlot();
    //    }
    //    DoThaw();
    //}
    #endregion

    private void Update()
    {
        if (!IsClicked && Input.GetMouseButton(0))
        {
            
        }
        else
        {
            if (!Input.GetMouseButton(0)) { IsClicked = false; }
        }

        

    }


   

}
