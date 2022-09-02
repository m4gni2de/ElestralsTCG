using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RemoteAssetHelpers 
{
    private static Dictionary<Type, string> _Assets = null;
    public static Dictionary<Type, string> Assets
    {
        get
        {
            if (_Assets == null)
            {
                _Assets = GetAssets();
            }
            return _Assets;
        }
    }
    private static Dictionary<Type, string> GetAssets()
    {
        Dictionary<Type, string> assets = new Dictionary<Type, string>();
        assets.Add(typeof(NumberInput), "NumberInput");


        return assets;
    }
    public static string GetAssetName<T>()
    {
        if (Assets.ContainsKey(typeof(T)))
        {
            return Assets[typeof(T)];
        }

        App.LogFatal($"No asset of Type {typeof(T).ToString()} exists in Assets Dictionary. Manually add it to RemoteAssetHelpers.cs before trying again.");
        return "";
    }
}
