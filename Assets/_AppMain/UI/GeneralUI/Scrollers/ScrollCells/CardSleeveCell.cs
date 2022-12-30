using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardSleeveCell : BaseScrollCell
{
    #region Properties
    [SerializeField] protected SpriteDisplay cellSp;
    protected TouchObject touch { get; private set; }
    private int _sleeveIndex = -1;
    public int sleeveIndex
    {
        get
        {
            return _sleeveIndex;
        }
        protected set
        {
            if (value == _sleeveIndex) { return; }
            _sleeveIndex = value;
            if (value > -1)
            {
                Sprite sp = CardFactory.CardSleeveSprite(value);
                cellSp.SetSprite(sp);
            }
            else
            {
                cellSp.Clear();
            }
        }
    }
    #endregion

    #region Overrides
    public override void Clear()
    {
        base.Clear();
        sleeveIndex = -1;
        touch.ClearAll();
    }
    public override void LoadData(object data, int index)
    {
        if (data.GetType() != typeof(int)) { return; }
        sleeveIndex = (int)data;
    }

    public override void Remove()
    {
        Destroy(gameObject);
    }
    #endregion

    private void Awake()
    {
        
    }

    #region Touch
    public void SetClickListener(UnityAction ac)
    {
        if (touch == null)
        {
            touch = GetComponent<TouchObject>();
        }
        touch.AddClickListener(ac);
    }
    #endregion

}
