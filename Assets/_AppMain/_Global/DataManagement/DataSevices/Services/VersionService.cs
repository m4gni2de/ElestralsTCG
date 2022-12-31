using System.Collections;
using System.Collections.Generic;
using SimpleSQL;
using UnityEngine;
using Databases;

public class VersionService : DataService
{
    public static readonly string VersionTable = "AppVersionDTO";
    private static readonly string DateColumn = "vWhen";
    private static readonly string BuildColumn = "mainBuild";
    private static readonly string KeyColumn = "vKey";

    public static AppVersionDTO LatestVersionForBuild(int buildIndex)
    {
        string qWhere = $"{BuildColumn} = {buildIndex} ORDER BY {DateColumn} DESC";
        AppVersionDTO latest = GetFirstWhere<AppVersionDTO>(VersionTable, qWhere);
        if (latest != null)
        {
            return latest;
        }
        return null;
    }
    public static VersionData GetLatestVersion(int buildIndex)
    {
        AppVersionDTO dto = LatestVersionForBuild(buildIndex);
        return new VersionData(dto);
    }

    public static string LatestVersionKey(int buildIndex)
    {
        string qWhere = $"{BuildColumn} = {buildIndex} ORDER BY {DateColumn} DESC";
        AppVersionDTO latest = GetFirstWhere<AppVersionDTO>(VersionTable, qWhere);
        if (latest != null)
        {
            return latest.vKey;
        }
        return "";
    }

    public static VersionData GetVersion(string versionKey)
    {
        string qWhere = $"{KeyColumn} = '{versionKey}'";
        AppVersionDTO dto = GetFirstWhere<AppVersionDTO>(VersionTable, qWhere);
        return new VersionData(dto);
    }

}


namespace Databases
{

    [System.Serializable]
    public class AppVersionDTO
    {
        [PrimaryKey]
        public string vKey { get; set; }
        public int mainBuild { get; set; }
        public int buildNumber { get; set; }
        public string vName { get; set; }
        public string vWhen { get; set; }
    }
}

