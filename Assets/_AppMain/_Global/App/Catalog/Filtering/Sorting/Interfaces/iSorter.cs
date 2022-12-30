using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iSorter
{
    public List<T> SortItems<T>(List<T> items);
}
