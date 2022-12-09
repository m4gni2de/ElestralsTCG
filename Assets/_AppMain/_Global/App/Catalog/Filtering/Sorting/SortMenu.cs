using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalUtilities;
using TMPro;

#if UNITY_EDITOR
using UnityEditor.U2D.Path;
#endif


namespace CardsUI.Filtering
{


    public class SortMenu : MonoBehaviour
    {
        #region Sorting Mechanics

        private List<DataSorter> _sorters = null;
        public List<DataSorter> Sorters { get { _sorters ??= DefaultCardSort; return _sorters; } }
        private static List<DataSorter> DefaultCardSort
        {
            get
            {
                List<DataSorter> list = new List<DataSorter>();

                DataSorter byDate = new DataSorter(SortBy.CardSetDate, SortDirection.ASC);
                DataSorter byNumber = new DataSorter(SortBy.CardSetNumber, SortDirection.ASC);
                list.Add(byDate);
                list.Add(byNumber);
                return list;
            }
        }
        #endregion

        #region Properties
        
        [SerializeField] private vmSorter template;
        private List<vmSorter> _activeSorters = null;
        public List<vmSorter> SortOrder { get { _activeSorters ??= new List<vmSorter>(); return _activeSorters; } }
        [SerializeField] private RectTransform Content;
        [Header("Add/Remove Sorter")]
        [SerializeField] private GameObject AddSortObject;
        [SerializeField] private Button addSortButton;
        [SerializeField] private GameObject removeSortButton;

        [Header("Dropdown Options")]
        #region Dropdown
        [SerializeField] private TMP_Dropdown dropDown;
        private int _selectionIndex = -1;
        private int selectIndex { get { return _selectionIndex; } set { _selectionIndex = value; } }
        private List<SortBy> _sortList = null;
        protected List<SortBy> SortList
        {
            get
            {
                if (_sortList == null)
                {
                    _sortList = new List<SortBy>();
                    for (int i = 0; i < Enums.GetNames(typeof(SortBy)).Count; i++)
                    {
                        _sortList.Add((SortBy)i);
                    }
                }
                return _sortList;
            }
        }
        protected List<TMP_Dropdown.OptionData> AvailableSortOptions
        {
            get
            {
                List<SortBy> currentSorts = new List<SortBy>();
                for (int i = 0; i < SortOrder.Count; i++)
                {
                    currentSorts.Add(SortOrder[i].sortBy);
                }


                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                for (int i = 0; i < SortList.Count; i++)
                {
                    if (!currentSorts.Contains(SortList[i]))
                    {
                        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(SortList[i].ToString());
                        options.Add(optionData);
                    }
                }
                return options;
            }
        }
        public bool IsSelecting { get { return dropDown.gameObject.activeSelf; } set { dropDown.gameObject.SetActive(value);} }
        #endregion

        #region Sort Creation/Removal
        protected vmSorter SelectedSorter = null;
        #endregion



        #endregion





        [SerializeField] private GameObject chooseSortObject;



        #region Menu Management
        private void Refresh()
        {
            dropDown.ClearOptions();

        }
        public void Open()
        {
            ToggleAddRemoveButtons(true);
            chooseSortObject.gameObject.SetActive(false);
        }

        public void Close()
        {

        }

        #endregion

        #region Adding/Removing
        private void CreateNewSorter()
        {
            SelectedSorter = Instantiate(template, Content.transform);
            chooseSortObject.gameObject.SetActive(true);
            chooseSortObject.transform.position = new Vector2(chooseSortObject.transform.position.x, SelectedSorter.transform.position.y);

        }
        public void ClickAddNewButton()
        {
            Refresh();
            dropDown.AddOptions(AvailableSortOptions);
            if (dropDown.options.Count > 0) { dropDown.value = 0; LoadSortOptions(); }

        }
        public void ClickRemoveButton()
        {
            if (IsSelecting)
            {
                CancelSelection();
            }
        }

        private void LoadSortOptions()
        {
            IsSelecting = true;
            ToggleAddRemoveButtons(false);
            CreateNewSorter();


        }
        private void ToggleAddRemoveButtons(bool isAdding)
        {
            addSortButton.gameObject.SetActive(isAdding);
            removeSortButton.gameObject.SetActive(!isAdding);
        }


        private void ConfirmSelection()
        {
            SortOrder.Add(SelectedSorter);
            SelectedSorter = null;
            EndSelection();
        }
        private void CancelSelection()
        {
            IsSelecting = false;
            Destroy(SelectedSorter.gameObject);
            SelectedSorter = null;
            EndSelection();
        }
        private void EndSelection()
        {
            chooseSortObject.gameObject.SetActive(false);
            ToggleAddRemoveButtons(true);
        }
        #endregion

        public void ToggleGlobalDirection(SortDirection dir)
        {
            for (int i = 0; i < Sorters.Count; i++)
            {
                Sorters[i].ChangeDirection(dir);
            }
        }

    }
}




      


