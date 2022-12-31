using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

class PreBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
  
    public void OnPreprocessBuild(BuildReport report)
    {
        //// Do the preprocessing here
        //string defaultBuild = "tts-002";
        //VersionData current = App.CurrentVersion;

        //if (current.versionKey.ToLower() != defaultBuild.ToLower())
        //{
        //    if (current.versionKey.ToLower() == App.AppVersionKey.ToLower())
        //    {
        //        //throw new BuildFailedException($"Hardcoded App Version of '{App.AppVersionKey}' in App.cs ");
        //    }
        //}
    }
}
