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
            //_conn = new DbConnector(dbFile, true, false);
            _conn.databaseFile = dbFile;
            _conn.Initialize(true);
            return true;
        }
        else
        {
            return false;
        }

    }
    


    private void OnApplicationQuit()
    {
        if (_conn != null)
        {
            _conn.Flush();
        }
        
    }

    private void OnDestroy()
    {
        if(_conn != null)
        {
            _conn.Flush();
        }
        
    }
}
