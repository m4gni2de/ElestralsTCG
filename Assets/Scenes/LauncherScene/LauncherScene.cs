using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Users;

public class LauncherScene : MonoBehaviour
{
    #region Scene Loading
    public static string SceneName
    {
        get
        {
            return SceneHelpers.SceneName(typeof(LauncherScene));
        }
    }
    public static void LoadScene()
    {
        App.ChangeScene(SceneName);
    }
    #endregion




}
