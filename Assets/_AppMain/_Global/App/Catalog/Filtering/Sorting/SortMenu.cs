using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalUtilities;


namespace CardsUI.Filtering
{


    public class SortMenu : MonoBehaviour
    {
        #region Sorting Mechanics

        public string SortString
        {
            get
            {
                string s = "";
                for (int i = 0; i < Sorters.Count; i++)
                {
                    s += $"ORDER BY {Sorters[i].sortColumn} {Sorters[i].sortDirection.ToString()}";

                    if (i < Sorters.Count - 1)
                    {
                        s += ", ";
                    }
                }
                return s;
            }
        }
        private List<DataSorter> _sorters = null;
        public List<DataSorter> Sorters { get { _sorters ??= DefaultCardSort; return _sorters; } }
        private static List<DataSorter> DefaultCardSort
        {
            get
            {
                List<DataSorter> list = new List<DataSorter>();

                DataSorter byDate = new DataSorter("setName", SortDirection.ASC);
                DataSorter byNumber = new DataSorter("setNumber", SortDirection.ASC);
                list.Add(byDate);
                list.Add(byNumber);
                return list;
            }
        }
        #endregion

        public Toggle directionToggle;



        #region Menu Management
        public void Open()
        {

        }

        public void Close()
        {

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




      


