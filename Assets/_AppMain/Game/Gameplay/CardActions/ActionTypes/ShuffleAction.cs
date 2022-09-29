using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Gameplay.CardActions;
using Decks;
using Gameplay.Decks;


[System.Serializable]
public class ShuffleAction : CardAction
{
    #region Properties
    public Deck sourceDeck { get; set; }
    [SerializeField]
    protected List<string> oldOrder = null;
    [SerializeField]
    protected List<string> newOrder = null;

    protected override ActionCategory GetCategory()
    {
        return ActionCategory.Shuffle;
    }
    #endregion

    protected override CardActionData GetActionData()
    {
       

        CardActionData data = new CardActionData(this);
        data.SetPlayer(player);
        data.AddData("deck", (int)sourceDeck.deckType);
        data.AddData("old_order", oldOrder.ToJson());
        data.AddData("new_order", newOrder.ToJson());
        return data;
    }


    #region Initialization

    public ShuffleAction(Player p, Deck deck) : base(p)
    {
        sourceDeck = deck;
        oldOrder = new List<string>();
        newOrder = new List<string>();
        for (int i = 0; i < deck.InOrder.Count; i++)
        {
            oldOrder.Add(deck.InOrder[i].cardId);
        }
        actionTime = 1.5f;
        _declaredMessage = $"Shuffle their deck.";
        _actionMessage = $"{p.userId} shuffles their deck!";
    }

    public static ShuffleAction Shuffle(Player p, Deck deck)
    {
        return new ShuffleAction(p, deck);
    }
    #endregion


    public override IEnumerator PerformAction()
    {
        yield return DoShuffle();

    }

    private Dictionary<GameCard, Vector3> TargetPositions()
    {
        CardSlot deckSlot = player.gameField.DeckSlot;
        Vector2 topBound = new Vector2(deckSlot.transform.position.x, (deckSlot.transform.position.y + (deckSlot.rect.sizeDelta.y * .5f)));
        Vector2 bottomBound = new Vector2(deckSlot.transform.position.x, (deckSlot.transform.position.y - (deckSlot.rect.sizeDelta.y * .5f)));

        Dictionary<GameCard, Vector3> targetPos = new Dictionary<GameCard, Vector3>();

        for (int i = 0; i < sourceDeck.InOrder.Count; i++)
        {
            int rand = Random.Range(0, 2);
            float targetY = deckSlot.transform.position.y;
            if (rand == 0)
            {
                targetY = Random.Range(deckSlot.transform.position.y, topBound.y);
            }
            else
            {
                targetY = Random.Range(bottomBound.y, deckSlot.transform.position.y);
            }

            Vector3 direction = GetDirection(sourceDeck.InOrder[i], new Vector2(deckSlot.transform.position.x, targetY));
            targetPos.Add(sourceDeck.InOrder[i], direction);
        }


        return targetPos;
    }


    protected IEnumerator DoShuffle()
    {
        float acumTime = 0f;
        Dictionary<GameCard, Vector3> targetPos = TargetPositions();
        float directionMod = 1f;

        do
        {
            foreach (var item in targetPos)
            {
                item.Key.cardObject.transform.position += (directionMod * (item.Value * Time.deltaTime));
            }
            yield return new WaitForEndOfFrame();
            acumTime += Time.deltaTime;
            if (acumTime >= actionTime / 2f)
            {
                directionMod = -1f;
            }

        } while (Validate(acumTime, actionTime));
        sourceDeck.Shuffle();

        for (int i = 0; i < sourceDeck.InOrder.Count; i++)
        {
            newOrder.Add(sourceDeck.InOrder[i].cardId);
        }
        End(ActionResult.Succeed);
    }
}
