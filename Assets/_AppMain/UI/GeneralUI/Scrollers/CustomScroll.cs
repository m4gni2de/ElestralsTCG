using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameActions;
using TouchControls;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif


public class CustomScroll : MonoBehaviour
{

    #region Indexors/Conveters
    public List<T> DataContextAs<T>()
    {
        List<T> list = new List<T>();
        for (int i = 0; i < DataContext.Count; i++)
        {
            list.Add((T)DataContext[i]);
        }
        return list;
    }
    public object this[int index]
    {
        get
        {
            if (index > DataContext.Count - 1) { return DataContext.Last(); }
            if (index < 0) { return DataContext.First(); }
            return DataContext[index];
        }
    }
    public T ContextAtIndex<T>(int index)
    {
        return (T)this[index];
    }
    public int this [object val]
    {
        get
        {
            for (int i = 0; i < DataContext.Count; i++)
            {
                if (DataContext[i] == val) { return i; }
            }
            return -1;
        }
    }


    #endregion

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
    [SerializeField] private TouchGroup _touchGroup = null;
    public TouchGroup touchGroup
    {
        get
        {
            _touchGroup ??= GetComponent<TouchGroup>();
            return _touchGroup;
        }
    }
    protected RectTransform Content { get { return Scroll.content; } }
    protected RectTransform Viewport { get { return Scroll.viewport; } }


    #region Grid
    [SerializeField] protected GridLayoutGroup Grid;
    protected GridSettings gridSettings;
    public GridSettings GetGridSettings() { return gridSettings; }

    protected void UpdateGrid()
    {
        gridSettings.SetGrid(Grid);
    }
    protected virtual void LoadGridSetings(GridSettings sett)
    {
        gridSettings = sett;
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
        private set
        {
            _dataContext = value;
        }
    }

    
   

    protected Type _dataType = null;
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

    public List<iGridCell> VisibleCards()
    {
        List<iGridCell> list = new List<iGridCell>();
        for (int i = 0; i < Cells.Count; i++)
        {
            if (Cells[i].GetGameObject().gameObject.activeSelf == true) { list.Add(Cells[i]); }
        }
        return list;
    }


   
   
    #endregion

    private bool _isLoaded = false;
    public bool IsLoaded { get => _isLoaded; set => _isLoaded = value; }
    protected bool _isDirty = false;
    public bool IsDirty { get { return _isDirty; } protected set { _isDirty = value; } }
    #endregion

    #region Sorting
    private iSorter _scrollSorter = null;
    public iSorter Sorter
    {
        get
        {
            _scrollSorter ??= new Sorter();
            return _scrollSorter;
        }
        set
        {
            _scrollSorter = value;
        }
    }

    public void SetSorter(iSorter sorter)
    {
        this.Sorter = sorter;
    }
    #endregion

    #region Initialization
    protected virtual void Refresh()
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            Cells[i].Clear();

        }

        DataContext.Clear();
        _dataType = null;
        IsDirty = false;
    }
    public virtual void Initialize(GridSettings sett, Action<iGridCell, object> setAction = null)
    {
        IsLoaded = true;
        LoadGridSetings(sett);
        templateItem.Hide();
        if (setAction != null)
        {
            OnLoadCell = GameAction.Create(setAction);
        }
        
    }
   
    public virtual void SetDataContext<T>(List<T> items)
    {
        Refresh();

        _dataType = typeof(T);
        List<T> sorted = Sorter.SortItems(items);
        for (int i = 0; i < sorted.Count; i++)
        {
            DataContext.Add(sorted[i]);
            LoadCell(i);
        }
        _isDirty = true;
        
        
    }
    public virtual void SetDataContextWithoutLoad<T>(List<T> items)
    {
        Refresh();
        _dataType = typeof(T);
        List<T> sorted = Sorter.SortItems(items);
        for (int i = 0; i < sorted.Count; i++)
        {
            DataContext.Add(sorted[i]);
        }
        _isDirty = true;

    }
   
    public virtual void Toggle(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
    #endregion

    #region Cell Management

    public iGameAction OnLoadCell;
    protected virtual void LoadCell(int index)
    {
        if (Cells.Count <= index)
        {
            SpawnCell();
        }

        iGridCell cell = Cells[index];
        object data = DataContext[index];
        cell.LoadData(data, index);
        ToggleCelVisibility(cell);
        OnLoadCell?.Invoke(cell, data);
    }
    protected virtual void LoadCell(int cellIndex, int dataIndex)
    {
        if (Cells.Count < cellIndex)
        {
            SpawnCell();
        }
        iGridCell cell = Cells[cellIndex];
        cell.LoadData(DataContext[dataIndex], dataIndex);

        OnLoadCell?.Invoke(cell, DataContext[dataIndex]);
    }
    protected void CreateAndLoadCell(int index)
    {
        SpawnCell();
        LoadCell(index);
    }

    protected virtual iGridCell SpawnCell()
    {
        GameObject go = Instantiate(templateItem.GetGameObject(), Content);
        Cells.Add(go.GetComponent<iGridCell>());
        return go.GetComponent<iGridCell>();
    }
    #endregion

    #region Data Context Changing
   
    public void SortItems<T>(List<T> items)
    {
        DataContext.Clear();
        List<T> sorted = Sorter.SortItems(items);

        for (int i = 0; i < sorted.Count; i++)
        {
            DataContext.Add(sorted[i]);
        }


    }

    public void ChangeData<T>(int index, T obj)
    {
        DataContext[index] = obj;
        DataContext = Sorter.SortItems(DataContext);
        int addIndex = DataContext.IndexOf(obj);
        for (int i = addIndex; i < DataContext.Count; i++)
        {
            LoadCell(i);
        }

    }
    public int AddData<T>(T obj)
    {
        if (typeof(T) != DataType) { App.LogFatal($"Object of Type '{typeof(T)}' does not match Data Context items Type of '{DataType}'"); }

        DataContext.Add(obj);

        DataContext = Sorter.SortItems(DataContext);
        int addIndex = DataContext.IndexOf(obj);

        for (int i = addIndex; i < DataContext.Count; i++)
        {
            LoadCell(i);
        }

        return this[obj];
    }

    public void RemoveData(int index)
    {
        DataContext.RemoveAt(index);
        Cells[index].Clear();
        for (int i = index - 1; i < DataContext.Count; i++)
        {
            LoadCell(i);
        }
    }
    public void RemoveData<T>(T obj)
    {
        if (DataContext.Contains(obj))
        {
            int removeIndex = DataContext.IndexOf(obj);
            DataContext.Remove(obj);
            Cells[removeIndex].Clear();

            for (int i = removeIndex - 1; i < DataContext.Count; i++)
            {
                LoadCell(i);
            }
        }

       
    }

    protected virtual void SyncCellsWithData()
    {
        if (!IsDirty) { return; }
        int cellCount = Cells.Count;
        int dataCount = DataContext.Count;

        if (dataCount > cellCount)
        {
            int diff = dataCount - cellCount;

            for (int i = 0; i < diff; i++)
            {
                CreateAndLoadCell(cellCount + i);
            }
        }

        IsDirty = false;
    }

    protected virtual void ReorderCells()
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            Cells[i].Clear();
        }
        for (int i = 0; i < DataContext.Count; i++)
        {
            LoadCell(i);
        }
        _isDirty = true;
    }

    #region Cell Visibility
    public void OnScrollValueChanged(Vector2 pos)
    {
        ScrollValueChange(pos);
    }
    protected virtual void ScrollValueChange(Vector2 pos)
    {
        
        CheckVisibleCells();
    }
    protected virtual void CheckVisibleCells()
    {
        for (int i = 0; i < Cells.Count; i++)
        {
            ToggleCelVisibility(Cells[i]);
        }
    }
    protected void ToggleCelVisibility(iGridCell cell)
    {
        if (cell.IsWithinView(Viewport))
        {
            if (cell.GetGameObject().activeSelf == true && Scroll.gameObject.activeSelf == true)
            {
                cell.SetInsideView(cell.GetGameObject().activeSelf == true);
            }
            else
            {
                cell.SetInsideView(false);
            }
            
        }
        else
        {
            cell.SetInsideView(false);
        }
    }

    #endregion


    #endregion

    #region Scroll Management
    public void EnableScroll(bool enable)
    {
        Scroll.enabled = enable;
    }
    #endregion
    #region Clean Up
    protected void OnDestroy()
    {
        gridSettings.OnSettingsUpdate -= UpdateGrid;
    }
    #endregion
}
