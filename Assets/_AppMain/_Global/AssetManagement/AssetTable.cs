using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetTable 
{
    private static Dictionary<string, string> _Assets = null;
    public static Dictionary<string, string> Assets
    {
        get
        {
            if (_Assets == null)
            {
                _Assets = new Dictionary<string, string>();
            }
            return _Assets;
        }
    }

    
    
   

}
