using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScrollCell : MonoBehaviour, iGridCell
{
    #region Interface
    public virtual int Index => throw new System.NotImplementedException();

    public virtual void Clear()
    {
        
    }

    public virtual GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void Hide()
    {
       gameObject.SetActive(false);
    }

    public abstract void LoadData(object data, int index);

    public abstract void Remove();

    public virtual void SetInsideView(bool isInside)
    {
       
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    #endregion

    #region Properties
    protected string _cellName;
    public string CellName { get { return _cellName; } set { _cellName = value; } }
    #endregion

   
}
