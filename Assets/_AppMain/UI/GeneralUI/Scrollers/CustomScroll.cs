using System;
using System.Collections;
using System.Collections.Generic;
using GameActions;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CustomScroll : MonoBehaviour
{

    #region Properties
    [SerializeField] protected ScrollRect Scroll;
    protected Scrollbar scrollBar
    {
        get
        {
            if (Scroll.horizontal) { return Scroll.horizontalScrollbar; }
            return Scroll.verticalScrollbar;
        }
    }
    protected RectTransform Content { get { return Scroll.content; } }

    #region Grid
    [SerializeField] protected GridLayoutGroup Grid;
    protected GridSettings gridSettings;
    public GridSettings GetGridSettings() { return gridSettings; }

    private void UpdateGrid()
    {
        gridSettings.SetGrid(Grid);
    }
    private void LoadGridSetings(GridSettings sett)
    {
        gridSettings = sett;
        //float freeWidth = Scroll.GetComponent<RectTransform>().FreeWidth(Scroll.verticalScrollbar.handleRect, gridSettings.TotalPadding());
        //gridSettings.SetGridSize(Grid, freeWidth);
        gridSettings.OnSettingsUpdate -= UpdateGrid;
        gridSettings.OnSettingsUpdate += UpdateGrid;
    }

    #endregion

    #region Cells
    protected List<object> _dataContext = null;
    protected List<object> DataContext
    {
        get
        {
            _dataContext ??= new List<object>();
            return _dataContext;
        }
    }

    private Type _dataType = null;
    public Type DataType { get { return _dataType; } }

    protected iGridCell _templateItem = null;
    protected iGridCell templateItem
    {
        get
        {
            if (_templateItem == null)
            {
                _templateItem = Content.GetChild(0).GetComponent<iGridCell>();
                _templateItem.Hide();
            }
            return _templateItem;
        }
    }

    protected List<iGridCell> _cells = null;
    public List<iGridCell> Cells { get { _cells ??= new List<iGridCell>(); return _cells; } }

    protected List<T> CellsAs<T>()
    {
        List<T> list = new List<T>();

        for (int i = 0; i < Cells.Count; i++)
        {
            T item = As<T>(Cells[i]);
            list.Add(item);
        }
        return list;
    }
    protected T As<T>(iGridCell cell)
    {
        return (T)cell;
    }


    public iGameAction OnLoadCell;
    private void LoadCell(int index)
    {
        iGridCell cell = Cells[index];
        object data = DataContext[index];
        cell.LoadData(data, index);
        OnLoadCell?.Invoke(cell, data);
    }
    #endregion

    private bool _isLoaded = false;
    public bool IsLoaded { get => _isLoaded; }
    private bool _isDirty = false;
    public bool IsDirty { get { return _isDirty; } private set { _isDirty = value; } }
    #endregion

    #region Initialization
    private void Refresh()
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            Cells[i].Remove();

        }
        Cells.Clear();
        DataContext.Clear();
        _dataType = null;
        IsDirty = false;
    }
    public void Initialize(GridSettings sett, Action<iGridCell, object> setAction = null)
    {
        LoadGridSetings(sett);
        templateItem.Hide();
        if (setAction != null)
        {
            OnLoadCell = GameAction.Create(setAction);
        }
        
    }
   
    public void SetDataContext<T>(List<T> items)
    {
        Refresh();
        _dataType = typeof(T);
        for (int i = 0; i < items.Count; i++)
        {
            DataContext.Add(items[i]);
        }
        LoadData();
    }
   
    private void LoadData()
    {
        
        for (int i = 0; i < DataContext.Count; i++)
        {
            CreateCell(i);
        }
    }
   
    public void Toggle(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
    #endregion

    #region Cell Management
    private void CreateCell(int index)
    {
        GameObject go = Instantiate(templateItem.GetGameObject(), Content);
        Cells.Add(go.GetComponent<iGridCell>());
        LoadCell(index);
    }
    #endregion

    #region Data Context Changing
   
   
    public void AddData<T>(T obj)
    {
        if (typeof(T) != DataType) { App.LogFatal($"Object of Type '{typeof(T)}' does not match Data Context items Type of '{DataType}'"); }
        DataContext.Add(obj);
        IsDirty = true;
        SyncCellsWithData();
    }

    private void SyncCellsWithData()
    {
        if (!IsDirty) { return; }
        int cellCount = Cells.Count;
        int dataCount = DataContext.Count;

        if (dataCount > cellCount)
        {
            int diff = dataCount - cellCount;

            for (int i = 0; i < diff; i++)
            {
                CreateCell(cellCount + i);
            }
        }

        IsDirty = false;
    }
    #endregion

    #region Clean Up
    private void OnDestroy()
    {
        gridSettings.OnSettingsUpdate -= UpdateGrid;
    }
    #endregion
}
