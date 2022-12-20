using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;
using Elements;

namespace Databases
{
    public class ElementService 
    {

        #region Static Functions
        private static List<ElementData> _elementsList = null;
        public static List<ElementData> ElementsList
        {
            get
            {
                if (_elementsList == null)
                {
                    _elementsList = new List<ElementData>();
                    List<ElementDTO> list = AllElements();

                    for (int i = 0; i < list.Count; i++)
                    {
                        ElementData data = new ElementData(list[i]);
                        _elementsList.Add(data);
                    }
                }
                return _elementsList;

            }
        }
        #endregion

        private static readonly string tableName = "zElementDTO";

        public static List<ElementDTO> AllElements()
        {
            List<ElementDTO> list = DataService.GetAll<ElementDTO>(tableName);
            return list;

        }
        public static ElementDTO GetElement(int code)
        {
            if (code < 0) { code = -1; }

            string where = $" where typeKey = {code}";

            List<ElementDTO> list = DataService.GetAllWhere<ElementDTO>(tableName, where);
            return list[0];
        }
    }

    public static class ElementHelpers
    {
        public static List<ElementCode> AsCodes(this List<Element> list)
        {
            List<ElementCode> results = new List<ElementCode>();
            for (int i = 0; i < list.Count; i++)
            {
                results.Add(list[i].Code);
            }
            return results;
        }
    }
}

