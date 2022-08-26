using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class DbManager : MonoBehaviour
{
    private TextAsset dbFile;
    public DbConnector _conn;

    private void Awake()
    {
       
    }

    public async Task<bool> ConnectAsync()
    {
        AsyncOperationHandle<TextAsset> db = Addressables.LoadAssetAsync<TextAsset>("dbInternal");

        await db.Task;

        if (db.Status == AsyncOperationStatus.Succeeded && db.IsDone)
        {
            dbFile = db.Result;
            _conn = new DbConnector(dbFile, true, false);
            return true;
        }
        else
        {
            return false;
        }

    }
    public async void Connect()
    {
        dbFile = await AssetPipeline.ByKeyAsync<TextAsset>("dbInternal");
        _conn = new DbConnector(dbFile, false, false);

        //_conn = ConnectionManager.db;

//#if UNITY_EDITOR
//        _conn = ConnectionManager.db;
//#else
//dbFile = await AssetPipeline.ByKeyAsync<TextAsset>("dbInternal");
//_conn = new DbConnector(dbFile, false, false);
//#endif
    }
}
