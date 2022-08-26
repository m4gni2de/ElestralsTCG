using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatalogScene : MonoBehaviour   
{
    public CardCatalog Catalog;


    void Start()
    {
        if (Catalog != null)
        {
            Catalog.Open();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
