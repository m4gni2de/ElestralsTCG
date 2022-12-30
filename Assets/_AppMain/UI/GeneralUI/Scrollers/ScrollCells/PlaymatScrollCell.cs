using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaymatScrollCell : BaseScrollCell
{
    #region Properties
    [SerializeField] protected SpriteDisplay cellSp;
    protected TouchObject touch { get; private set; }
    private int _maxIndex = -1;
    public int matIndex
    {
        get
        {
            return _maxIndex;
        }
        protected set
        {
            if (value == _maxIndex) { return; }
            _maxIndex = value;
            if (value > -1)
            {
                Sprite sp = CardFactory.PlaymattSprite(value);
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
        matIndex = -1;
        touch.ClearAll();
    }
    public override void LoadData(object data, int index)
    {
        if (data.GetType() != typeof(int)) { return; }
        matIndex = (int)data;
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
