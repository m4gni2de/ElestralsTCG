using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System;

public class VersionData
{
    public string versionKey { get; set; }
    public DateTime whenVersion { get; set; }
    public int mainBuildIndex { get; set; }
    public int buildOrder { get; set; }
    public string title { get; set; }  


    public VersionData(AppVersionDTO dto)
    {
        versionKey = dto.vKey;
        mainBuildIndex = dto.mainBuild;
        buildOrder = dto.buildNumber;
        title = dto.vName;
        whenVersion = DateTime.Parse(dto.vWhen);
    }

    public bool IsLatestVersion()
    {
        VersionData latest = VersionService.GetLatestVersion(mainBuildIndex);
        return buildOrder >= latest.buildOrder;
    }
   
}
