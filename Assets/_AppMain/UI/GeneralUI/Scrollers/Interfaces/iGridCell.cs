using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iGridCell 
{
    GameObject GetGameObject();
    int Index { get; }
    void LoadData(object data, int index);
    void Clear();
    void Hide();
    void Show();
    void Remove();
}
