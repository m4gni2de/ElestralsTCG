using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;

public class ConnectionManager
{
    #region Properties
    public static string dbName = "dbInternal";
    public static TextAsset dbAsset = null;
    private static ConnectionManager _Instance = null;
    public static ConnectionManager Instance { get { return _Instance; } }

    private DbConnector _conn = null;
    protected DbConnector conn
    {
        get
        {
            if (_conn == null)
            {
                //if (AppManager.Instance == null)
                //{
                //    AppManager.Create();
                //}
                //TextAsset db = AssetPipeline.ByKey<TextAsset>(dbName);
                //_conn = new DbConnector(db);
            }
            return _conn;
        }
    }
    public static DbConnector db
    {
        get
        {
            if (!IsConnected())
            {
                Connect("dbInternal");
            }
            return Instance.conn;
        }
    }
    #endregion


    //#region Services
    //private ServiceManager _services = null;
    //public ServiceManager Services
    //{
    //    get
    //    {
    //        if (_services == null)
    //        {
    //            _services = new ServiceManager();

    //        }
    //        return _services;

    //    }
    //}
    //#endregion

    #region Connection Config
    public static bool Connect(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = dbName;
        }
        if (databaseName.ToLower() != dbName.ToLower())
        {
            //return Log.Warn($"Database {databaseName} does not match Database on file.");
            return false;
        }

        DoConnect(databaseName);
        return IsConnected();

    }
    protected static void DoConnect(string databaseName)
    {
        if (!IsConnected())
        {
            _Instance = new ConnectionManager(databaseName);
        }

    }

    public static void Disconnect()
    {
        if (IsConnected())
        {
            Instance.conn.Flush();
        }
    }

    public static bool IsConnected()
    {
        return _Instance != null;
    }
    #endregion

    ConnectionManager(string databaseName)
    {
        Do(databaseName);


        
    }

    private void Do(string databaseName)
    {
        dbName = databaseName;
        dbAsset = AssetPipeline.ByKey<TextAsset>(dbName);
        if (dbAsset != null)
        {
            //_conn = new DbConnector(dbAsset);
        }
        else
        {
            //Log.Stop($"Database Asset name {databaseName} does not exist as an Addressable. Please check your spelling and try again.");
        }
    }

    




}
