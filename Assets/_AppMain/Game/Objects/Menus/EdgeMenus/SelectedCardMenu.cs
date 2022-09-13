using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCardMenu : EdgeMenu
{
    public GameObject CardImage;
    private SpriteRenderer _CardSp = null;
    protected SpriteRenderer CardSp { get { _CardSp ??= CardImage.GetComponent<SpriteRenderer>(); return _CardSp; } }
}
