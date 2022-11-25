using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using SimpleSQL;

public class PlayerService
{

    protected static DbConnector db { get { return AppManager.Instance.dbPlayer; } }


    public static string PrimaryKey<T>() where T : new()
    {
        TableMapping mapping = db.GetMapping(typeof(T));
        return mapping.PK.Name;
    }
    public static string PrimaryKey(string tableName)
    {
        List<TableInfo> table = db.GetTableInfo(tableName);
        for (int i = 0; i < table.Count; i++)
        {
            TableInfo t = table[i];
            if (t.pk != 0) { return t.name; }
        }
        return string.Empty;
    }


    public static T ByKey<T>(string tableName, string colName, string colValue) where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {colName} = ?";
        T dto = db.QueryFirstRecord<T>(out bool exists, query, colValue);

        return dto;
        //if (exists) { return dto; } else { Game.LogFatalError(db, objName, $"Column {colName} does not exists in Database."); return default(T); }
    }

    public static T ByQuery<T>(string tableName, string queryWhere) where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {queryWhere}";
        if (string.IsNullOrEmpty(queryWhere))
        {
            query = $"SELECT * FROM {tableName}";
        }
        T dto = db.QueryFirstRecord<T>(out bool exists, query);

        return dto;
    }

    public static List<T> ListByQuery<T>(string tableName, string queryWhere = "") where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {queryWhere}";
        if (string.IsNullOrEmpty(queryWhere))
        {
            query = $"SELECT * FROM {tableName}";
        }
        List<T> dto = db.Query<T>(query);

        return dto;
    }

    public static T ByPk<T>(string tableName, string value) where T : new()
    {
        string pk = PrimaryKey<T>();
        return ByKey<T>(tableName, pk, value);

    }





    public static List<T> GetAll<T>(string tableName) where T : new()
    {
        string query = $"SELECT * FROM {tableName}";
        return db.Query<T>(query);
    }

    public static List<T> GetAllWhere<T>(string tableName, string whereClause) where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {whereClause}";
        return db.Query<T>(query);
    }

    public static T GetFirstWhere<T>(string tableName, string whereClause) where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {whereClause}";
        bool exists;
        T dto = db.QueryFirstRecord<T>(out exists, query);
        return dto;

    }

    #region Key For Key

    public static bool KeyExists<T>(T obj, string key) where T : new()
    {
        TableMapping map = db.GetMapping(typeof(T));
        //if (map.PK == null) { Game.LogFatalError(db, objName, $"Table {map.TableName} does not have a Primary Key. Use function that requires Column Name."); return false; }

        string query = $"SELECT * FROM {map.TableName} WHERE {map.PK.Name} = ?";
        db.QueryFirstRecord<T>(out bool exists, query, key);
        return exists;
    }
    public static bool KeyExists<T>(string tableName, string colName, string colValue) where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {colName} = ?";
        db.QueryFirstRecord<T>(out bool exists, query, colValue);
        return exists;
    }
    public static bool KeyExists<T>(string tableName, string colName, string colValue, out T val) where T : new()
    {
        string query = $"SELECT * FROM {tableName} WHERE {colName} = ?";
        val = db.QueryFirstRecord<T>(out bool exists, query, colValue);
        if (!exists)
        {
            val = default(T);
        }
        return exists;
    }

    protected static bool KeyExists(TableMapping map, string tableName, string colName, string colValue)
    {
        string query = $"SELECT * FROM {tableName} WHERE {colName} = ?";
        db.QueryFirstRecord(out bool exists, map, query, colValue);

        return exists;
    }

    #endregion

    public static int Count(string tableName)
    {

        SimpleDataTable item = db.QueryGeneric("SELECT COUNT() FROM " + tableName);
        return (int)item.rows[0].fields[0];
    }

    public static void DoQuery(string query)
    {
        db.QueryGeneric(query);
        db.Commit();
    }


    #region Saving
    public static void Save<T>(T obj, string keyValue) where T : new()
    {
        bool exists = KeyExists<T>(obj, keyValue);
        TableMapping map = db.GetMapping(typeof(T));
        if (exists)
        {

            db.UpdateTable(obj, map.TableName);
        }
        else
        {
            db.Insert(obj, map.TableName);
        }
        db.Commit();

    }

    public static void Save<T>(T obj, string tableName, string colName, string colValue)
    {
        TableMapping mapping = db.GetMapping(typeof(T));
        bool exists = KeyExists(mapping, tableName, colName, colValue);
        if (exists)
        {
            db.UpdateTable(obj, tableName);
        }
        else
        {
            db.Insert(obj, tableName);
        }

        db.Commit();
    }

    public static void OverrideAndSave<T>(T obj, string tableName, string colName, string colValue)
    {
        TableMapping mapping = db.GetMapping(typeof(T));
        bool exists = KeyExists(mapping, tableName, colName, colValue);
    }

    public static void Insert<T>(T obj, string tableName, string colName, string colValue)
    {
        TableMapping mapping = db.GetMapping(typeof(T));
        bool exists = KeyExists(mapping, tableName, colName, colValue);
        if (!exists)
        {
            db.Insert(obj, tableName);
        }

    }
    public static void Insert<T>(T obj, string tableName)
    {
        db.Insert(obj, tableName);
    }

    public static bool Delete(string tableName, string queryWhere)
    {
        string query = $"DELETE FROM {tableName} WHERE {queryWhere}";
        db.QueryGeneric(query);
        db.Commit();
        return true;
    }
    #endregion

}
