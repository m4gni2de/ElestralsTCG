using System;
using System.Collections;
using System.Collections.Generic;
using GlobalUtilities;
using UnityEngine;
using UnityEngine.UI;

public class vmSorter : ViewModel
{
    #region Events
    public event Action<vmSorter> OnSorterChanged;
    private void RaiseSorterChanged()
    {
        OnSorterChanged?.Invoke(this);
    }
    #endregion


    #region Properties
    protected DataSorter _sorter;
    public DataSorter Sorter { get { return _sorter; } }
    public SortBy sortBy { get { return _sorter.sortBy; } }
    public SortDirection direction { get { return _sorter.sortDirection; } }

    protected int _index;
    public int sortOrder { get { return _index; } }
    #endregion

    #region UI
    [SerializeField] private MagicTextBox titleText;
    [SerializeField] private SpriteDisplay spDirection;
    [SerializeField] private Button btnDiretion;
    private Vector3 upRotation = new Vector3(0f, 0f, 90f);
    private Vector3 downRotation = new Vector3(0f, 0f, -90f);
    #endregion


    #region Intialization
    public void Load(int index, SortBy sortBy, SortDirection dir)
    {
        btnDiretion.interactable = true;
        _sorter = new DataSorter(sortBy, dir);
        this._index = index;

        titleText.SetText(sortBy.ToString());
        SetArrow();
        Show();
    }

    #endregion

    #region Direction Controls
    private void SetArrow()
    {
        if (direction == SortDirection.ASC) { spDirection.Rotate(upRotation); } else { spDirection.Rotate(downRotation); }
    }
    public void DirectionClick()
    {
        SortDirection other = SortDirection.ASC;
        if (direction == SortDirection.ASC) { other = SortDirection.DESC; }
        Sorter.ChangeDirection(other);
        SetArrow();
        RaiseSorterChanged();
    }
    #endregion


}
