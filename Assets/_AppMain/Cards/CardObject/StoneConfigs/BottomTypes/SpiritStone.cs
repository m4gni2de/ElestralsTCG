using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardsUI.Glowing;

public class SpiritStone : StoneVariant
{

    public override void Set(Card card)
    {
        base.Set(card);
    }

    protected override void SetStones(Card card)
    {
        Stones.SetLargeStone(card);
    }

    protected override void SetSprite(Card card)
    {
        string frameString = FrameString(card);
        FrameSp.sprite = AssetPipeline.ByKey<Sprite>(frameString);
        LeftSp.color = CardUI.TextColor(card.SpiritsReq[0].Code);
        RightSp.color = CardUI.TextColor(card.SpiritsReq[0].Code);
    }


}
