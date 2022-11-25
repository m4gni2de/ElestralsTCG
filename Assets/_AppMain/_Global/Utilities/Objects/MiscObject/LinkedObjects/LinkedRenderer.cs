using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedRenderer
{
    public iSortRenderer linkedRend;
    /// <summary>
    /// linkedRend's Sort Order - source Renderer's Sort Order
    /// </summary>
    public int sortDistance;


    public LinkedRenderer(iSortRenderer rend, int sourceRendererOrder)
    {
        linkedRend = rend;
        sortDistance = rend.SortOrder - sourceRendererOrder;
    }


    public void UpdateSortOrder(int newOrder)
    {
        int linkedOrder = newOrder + sortDistance;
        linkedRend.SortOrder = linkedOrder;
    }

    public void UpdateSortLayer(string layer)
    {
        linkedRend.SortLayer = layer;
    }
}
