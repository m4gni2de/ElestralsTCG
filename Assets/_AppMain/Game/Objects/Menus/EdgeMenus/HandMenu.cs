using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;

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

        if (GameManager.ActivePlayer != null && GameManager.ActivePlayer == GameManager.ActiveGame.You)
        {
            //AddTouchButtons(GameManager.ActivePlayer);
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
            to.OnClickEvent.AddListener(() => ClickCard(card));
            to.OnHoldEvent.AddListener(() => DragCard(card));
        }
        

    }

 

    #region Touch Cards

    public void ClickCard(GameCard card)
    {
        GameManager.Instance.SelectCard(card);
    }

    public void DragCard(GameCard card)
    {
        StartCoroutine(DoDragCard(card));
    }

    protected IEnumerator DoDragCard(GameCard card)
    {
        
        //card.cardObject.transform.SetParent(transform);
        Vector2 newScale = new Vector2(card.cardObject.Container.transform.localScale.x / 2f, card.cardObject.Container.transform.localScale.y / 2f);
        //card.cardObject.SetScale(newScale);

        card.cardObject.SetAsChild(transform, newScale);

        Field f = GameManager.Instance.arena.GetPlayerField(GameManager.ActiveGame.You);
        do
        {
            Close();
            GameManager.Instance.Freeze(true);
            yield return new WaitForEndOfFrame();
            var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            card.cardObject.transform.position = new Vector3(newPos.x, newPos.y, -2f);
            f.ValidateSlots(card);

        } while (true && Input.GetMouseButton(0));

        CardSlot slot = f.SelectedSlot;

        if (slot == null)
        {
            AddCard(card);
            f.SetSlot();
        }
        else
        {
            if (slot.ValidateCard(card))
            {
                _cardSlot.RemoveCard(card);
                slot.AllocateTo(card);
            }
            else
            {
                AddCard(card);
            }
            f.SetSlot();
        }
        GameManager.Instance.Freeze(false);
    }
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
