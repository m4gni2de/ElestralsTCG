using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TouchSprite : TouchObject
{


    #region Click Area
    
    private List<Vector2> _outline = null;
    public List<Vector2> Outline
    {
        get
        {
            if (_outline == null || _outline.Count == 0)
            {
                _outline = sp.sprite.SetLocalOutline(Source);
            }
            return _outline;
        }
    }
    #endregion

    #region Overrides
    public override bool IsPointerOverMe()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        return sp.IsPointInside(mousePos);
    }
    protected override void SetBounds()
    {
        Source = GetComponent<Transform>();

    }
    #endregion

    #region Properties
    [SerializeField]
    private SpriteRenderer _sp;
    public SpriteRenderer sp { get { return _sp; } }
    #endregion

   
    #region Initilization
    
    #endregion

    //public override bool Validate()
    //{
    //    if (!IsPointerOverMe()) { return false; }

    //    return ErrorList.Count == 0;
        
    //}


   

    #region Comparing
    public override float GetSortValue()
    {
        int layerVal = sp.sortingLayerID;
        return layerVal + sp.sortingOrder;

    }
    #endregion

    #region Touch Inputs
    //protected override void StartClick()
    //{
    //    _isClicked = true;
    //    DoFreeze();
    //    StartCoroutine(WhileClicked());


    //}

    #endregion


    




}
