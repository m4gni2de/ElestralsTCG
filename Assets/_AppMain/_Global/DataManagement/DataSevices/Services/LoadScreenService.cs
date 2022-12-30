using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using SimpleSQL;

namespace Databases
{
    public class zLoadScreen
    {
        [PrimaryKey]
        public int screenKey { get; set; }
        public string assetName { get; set; }
    }
}
public class LoadScreenService : DataService
{
    private static readonly string TableName = "zLoadScreen";
    private static readonly string PkColumn = "screenKey";

    public static string ScreenAssetString(int screenKey)
    {
        zLoadScreen dto = ByKey<zLoadScreen, int>(TableName, PkColumn, screenKey);
        return dto.assetName;

    }

    public static int RandomScreenIndex()
    {
        List<zLoadScreen> list = GetAll<zLoadScreen>(TableName);

        if (list.Count > 0)
        {
            int rand = Random.Range(0, list.Count);
            return list[rand].screenKey;
        }
        return 0;
    }
}
