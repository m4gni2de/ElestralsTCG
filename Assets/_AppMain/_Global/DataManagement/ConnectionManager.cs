
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConnectionManager
{
    #region Properties
    public static string dbName = "dbInternal";
    public static TextAsset dbAsset = null;
    private static ConnectionManager _Instance = null;
    public static ConnectionManager Instance { get { return _Instance; } }

    private DbConnector _conn = null;
    public DbConnector conn
    {
        get
        {
            if (_conn == null)
            {
                dbAsset = AssetPipeline.ByKey<TextAsset>(dbName);
                if (dbAsset != null)
                {
                    _conn = DbConnector.EditorDb(dbAsset);
                }
                else
                {
                    App.LogError($"Database Asset name '{dbName}' does not exist as an Addressable. Please check your spelling and try again.");
                }
            }
            return _conn;
        }
    }

    #endregion


#if UNITY_EDITOR


    public static bool Connect(string databaseName = "")
    {
        if (_Instance == null)
        {
            if (EditorApplication.isPlaying)
            {
                return App.LogFatal($"This is an Editor Only database manager. Cannot be started in play mode.");
            }


            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = dbName;
            }
            if (databaseName.ToLower() != dbName.ToLower())
            {
                return App.LogFatal($"Database {databaseName} does not match Database on file.");
            }

           _Instance = new ConnectionManager();
            Instance.Do(databaseName);
           
        }
        return true;


    }

#endif 
    private void Do(string databaseName)
    {
        dbName = databaseName;
        dbAsset = AssetPipeline.ByKey<TextAsset>(databaseName);
        if (dbAsset != null)
        {
            _conn = DbConnector.EditorDb(dbAsset);
        }
        else
        {
            App.LogError($"Database Asset name {databaseName} does not exist as an Addressable. Please check your spelling and try again.");
        }
    }


   
    public static void Disconnect()
    {

        if (_Instance != null && _Instance._conn != null)
        {
            _Instance.conn.Flush();
            _Instance = null;
        }

    }

  

    

    




}
