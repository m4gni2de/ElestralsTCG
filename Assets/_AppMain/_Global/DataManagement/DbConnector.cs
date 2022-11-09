using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Databases
{
    public class DbConnector : MonoBehaviour
    {
        public enum DbMode
        {
            Internal = 0,
            Player = 1,
        }

        public string fileName;
        protected string dbPath
        {
            get
            {
                return $"Assets/_AppMain/_Global/DataManagement/Databases/{fileName}.bytes";
            }
        }

        public enum OverridePathMode
        {
            Absolute,
            RelativeToPersistentData
        }


        public SQLiteConnection _conn;
        public TextAsset databaseFile;

        public string overrideBasePath = "";

        public OverridePathMode overridePathMode;

        public bool changeWorkingName;

        public string workingName = "";

        public bool overwriteIfExists;

        /// <summary>
        /// This is only for Editor Mode. Allows changes to be made to the Database from Unity Editor. In a live build, this is not used. 
        /// </summary>
        public bool overwriteMaster = false;

        public bool debugTrace;

        public DatabaseCreatedDelegate databaseCreated;


        public bool DebugTrace
        {
            get
            {
                return debugTrace;
            }
            set
            {
                debugTrace = value;
                if (_conn != null)
                {
                    _conn.Trace = value;
                }
            }
        }

        public TimeSpan BusyTimeout
        {
            get
            {
                Initialize(forceReinitialization: false);
                return _conn.BusyTimeout;
            }
            set
            {
                Initialize(forceReinitialization: false);
                _conn.BusyTimeout = value;
            }
        }

        public bool IsInTransaction
        {
            get
            {
                Initialize(forceReinitialization: false);
                return _conn.IsInTransaction;
            }
        }

        public IEnumerable<TableMapping> TableMappings
        {
            get
            {
                Initialize(forceReinitialization: false);
                return _conn.TableMappings;
            }
        }

        private void Awake()
        {
            LoadRuntimeLibrary();

#if UNITY_EDITOR

            if (overwriteMaster)
            {
                Initialize();
            }
            else
            {
                if (!ClonesManager.IsClone())
                {
                    Initialize(forceReinitialization: false);
                }

            }
#else
 if (overwriteMaster)
            {
                Initialize();
            }
            else
            {
                Initialize(forceReinitialization: false);
            }
#endif



        }

        public async Task<bool> ConnectAsync()
        {
            AsyncOperationHandle<TextAsset> db = Addressables.LoadAssetAsync<TextAsset>(fileName);

            await db.Task;
#if UNITY_EDITOR
            return InitializeEditor(db);
#else
            return InitializePlayer(db);
#endif


        }
#if UNITY_EDITOR
        private bool InitializeEditor(AsyncOperationHandle<TextAsset> db)
        {
            if (db.Status == AsyncOperationStatus.Succeeded && db.IsDone)
            {
                databaseFile = db.Result;
                if (ClonesManager.IsClone())
                {
                    Initialize();
                }
                else
                {
                    Initialize(true);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
#endif
        private bool InitializePlayer(AsyncOperationHandle<TextAsset> db)
        {
            if (db.Status == AsyncOperationStatus.Succeeded && db.IsDone)
            {
                databaseFile = db.Result;
                Initialize(true);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Flush()
        {
            Close();
            Dispose();
            if (overwriteMaster)
            {
                OverrideMaster();
            }
        }


        //public DbConnector(TextAsset dbFile, bool overWrite = false, bool debugLog = true)
        //{

        //    databaseFile = dbFile;
        //    overwriteIfExists = overWrite;
        //    overwriteMaster = true;
        //    DebugTrace = debugLog;
        //    LoadRuntimeLibrary();
        //    Initialize(false);


        //}
        private DbConnector(TextAsset dbFile, bool overWrite, bool debugLog, bool overrideMaster)
        {
            databaseFile = dbFile;
            overwriteIfExists = overWrite;
            overwriteMaster = overrideMaster;
            DebugTrace = debugLog;
            LoadRuntimeLibrary();
            Initialize(false);
        }

        public static DbConnector EditorDb(TextAsset dbFile)
        {
            return new DbConnector(dbFile, true, false, true);
        }

        private void LoadRuntimeLibrary()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    {
                        string path = Application.dataPath + "/../";
                        try
                        {
                            if (File.Exists(Path.Combine(path, "sqlite3.dll")))
                            {
                                File.Delete(Path.Combine(path, "sqlite3.dll"));
                            }
                        }
                        catch
                        {
                        }

                        if (is64Bit())
                        {
                            RuntimeHelper.CreateFileFromEmbeddedResource("SimpleSQL.Resources.sqlite3.dll_64.resource", Path.Combine(path, "sqlite3.dll"));
                        }
                        else
                        {
                            RuntimeHelper.CreateFileFromEmbeddedResource("SimpleSQL.Resources.sqlite3.dll_32.resource", Path.Combine(path, "sqlite3.dll"));
                        }

                        break;
                    }
                default:
                    {
                        string path = Application.dataPath + "/../";
                        if (is64Bit())
                        {
                            RuntimeHelper.CreateFileFromEmbeddedResource("SimpleSQL.Resources.sqlite3.dll_64.resource", Path.Combine(path, "sqlite3.dll"));
                        }
                        else
                        {
                            RuntimeHelper.CreateFileFromEmbeddedResource("SimpleSQL.Resources.sqlite3.dll_32.resource", Path.Combine(path, "sqlite3.dll"));
                        }

                        break;
                    }
            }
        }

        private bool is64Bit()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (!string.IsNullOrEmpty(environmentVariable) && !(environmentVariable.Substring(0, 3) == "x86"))
            {
                return true;
            }

            return false;
        }

        public void OverrideMaster()
        {
            //string path = _conn.DatabasePath;
#if UNITY_EDITOR

            string cloneLoc = Path.Combine(string.IsNullOrEmpty(overrideBasePath) ? Application.persistentDataPath : Path.Combine((overridePathMode == OverridePathMode.Absolute) ? "" : Application.persistentDataPath, overrideBasePath), changeWorkingName ? workingName.Trim() : (databaseFile.name + ".bytes"));
            bool flag = File.Exists(cloneLoc);

            string origLoc = AssetDatabase.GetAssetPath(databaseFile);


            bool flag2 = true;
            try
            {


                using (FileStream fs = File.OpenRead(cloneLoc))
                {

                    byte[] cloneBytes = StreamToBytes(fs);
                    File.WriteAllBytes(origLoc, cloneBytes);
                    fs.Close();
                }



                if (databaseCreated != null)
                {
                    databaseCreated(cloneLoc);
                }


            }
            catch (Exception ex)
            {
                flag2 = false;
                Debug.LogError("Failed to open database at the working path: " + cloneLoc);
                Debug.LogError(ex.Message);
            }


            //if (flag2)
            //{

            //    CreateConnection(cloneLoc);
            //    _conn.Trace = debugTrace;
            //}


            AssetDatabase.Refresh();
#endif

        }


        public void Initialize()
        {
#if UNITY_EDITOR
            if (_conn == null)
            {


                string path = AssetDatabase.GetAssetPath(databaseFile);
                Close();
                Dispose();


                if (databaseCreated != null)
                {
                    databaseCreated(path);
                }

                CreateConnection(path);
                _conn.Trace = debugTrace;
            }
#endif
        }

        
        /// <summary>
        /// If using a clone of the working DB, use this method to create the clone, or make changes to clone DB
        /// </summary>
        public virtual void Initialize(bool forceReinitialization)
        {
            if (_conn != null && !forceReinitialization)
            {
                return;
            }

            if (changeWorkingName && workingName.Trim() == "")
            {
                Debug.LogError("If you want to change the database's working name, then you will need to supply a new working name in the SimpleSQLManager [" + name + "]");
                return;
            }

            Close();
            Dispose();
            string text = Path.Combine(string.IsNullOrEmpty(overrideBasePath) ? Application.persistentDataPath : Path.Combine((overridePathMode == OverridePathMode.Absolute) ? "" : Application.persistentDataPath, overrideBasePath), changeWorkingName ? workingName.Trim() : (databaseFile.name + ".bytes"));
            bool flag = File.Exists(text);

            bool flag2 = true;
            if ((overwriteIfExists && flag) || !flag)
            {
                try
                {
                    if (flag)
                    {
                        File.Delete(text);
                    }

                    File.WriteAllBytes(text, databaseFile.bytes);
                    if (databaseCreated != null)
                    {
                        databaseCreated(text);
                    }
                }
                catch
                {
                    flag2 = false;
                    Debug.LogError("Failed to open database at the working path: " + text);
                }
            }

            if (flag2)
            {
                CreateConnection(text);
                _conn.Trace = debugTrace;
            }
        }

        protected virtual void CreateConnection(string documentsPath)
        {
            _conn = new SQLiteConnection(documentsPath);
        }

        private static byte[] StreamToBytes(Stream input)
        {
            int capacity = (int)(input.CanSeek ? input.Length : 0);
            using (MemoryStream memoryStream = new MemoryStream(capacity))
            {
                byte[] array = new byte[4096];
                int num;
                do
                {
                    num = input.Read(array, 0, array.Length);
                    memoryStream.Write(array, 0, num);
                }
                while (num != 0);
                return memoryStream.ToArray();
            }
        }

        public void Close()
        {
            if (_conn != null)
            {
                if (debugTrace)
                {
                    Debug.Log(name + ": closing connection");
                }

                _conn.Close();
            }
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                if (debugTrace)
                {
                    Debug.Log(name + ": disposing connection");
                }

                _conn.Dispose();
                _conn = null;
            }
        }


        private void OnApplicationQuit()
        {
            Flush();
        }

        public TableMapping GetMapping(Type type)
        {
            Initialize(forceReinitialization: false);
            return _conn.GetMapping(type);
        }

        public int CreateTable(Type type, string tableName)
        {
            Initialize(forceReinitialization: false);
            return _conn.CreateTable(type, tableName);
        }
        public int CreateTable<T>()
        {
            Initialize(forceReinitialization: false);
            return _conn.CreateTable<T>();
        }


        public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
        {
            Initialize(forceReinitialization: false);
            return _conn.CreateCommand(cmdText, ps);
        }



        public int Execute(string query, params object[] args)
        {
            Initialize(forceReinitialization: false);
            return _conn.Execute(query, args);
        }

        public int ExecuteWithResult(out SQLite3.Result result, out string errorMessage, string query, params object[] args)
        {
            Initialize(forceReinitialization: false);
            return _conn.Execute(out result, out errorMessage, query, args);
        }

        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            Initialize(forceReinitialization: false);
            return _conn.Query<T>(query, args);
        }

        public List<object> Query(TableMapping map, string query, params object[] args)
        {
            Initialize(forceReinitialization: false);
            return _conn.Query(map, query, args);
        }

        public SimpleDataTable QueryGeneric(string query, params object[] args)
        {
            SQLiteCommand sQLiteCommand = CreateCommand(query, args);
            return sQLiteCommand.ExecuteQueryGeneric();
        }



        public T QueryFirstRecord<T>(out bool recordExists, string query, params object[] args) where T : new()
        {
            Initialize(forceReinitialization: false);
            List<T> list = _conn.Query<T>(query, args);
            if (list.Count > 0)
            {
                recordExists = true;
                return list[0];
            }

            recordExists = false;
            return default(T);
        }

        public object QueryFirstRecord(out bool recordExists, TableMapping map, string query, params object[] args)
        {
            Initialize(forceReinitialization: false);
            List<object> list = _conn.Query(map, query, args);
            if (list.Count > 0)
            {
                recordExists = true;
                return list[0];
            }

            recordExists = false;
            return null;
        }

        public TableQuery<T> Table<T>() where T : new()
        {
            Initialize(forceReinitialization: false);
            return _conn.Table<T>();
        }

        public List<TableInfo> GetTableInfo(string tableName)
        {
            string query = "pragma table_info(\"" + tableName + "\")";
            List<TableInfo> list = _conn.Query<TableInfo>(query, new object[0]);

            return list;
        }

        public T Get<T>(object pk) where T : new()
        {
            Initialize(forceReinitialization: false);
            return _conn.Get<T>(pk);
        }

        public void BeginTransaction()
        {
            Initialize(forceReinitialization: false);
            _conn.BeginTransaction();
        }

        public void Rollback()
        {
            Initialize(forceReinitialization: false);
            _conn.Rollback();
        }

        public void Commit()
        {
            Initialize(forceReinitialization: false);
            _conn.Commit();
        }

        public void RunInTransaction(Action action)
        {
            Initialize(forceReinitialization: false);
            _conn.RunInTransaction(action);
        }

        public int InsertAll(IEnumerable objects, out long lastRowID)
        {
            Initialize(forceReinitialization: false);
            return _conn.InsertAll(objects, out lastRowID);
        }

        public int InsertAll(IEnumerable objects)
        {
            long lastRowID = -1L;
            return InsertAll(objects, out lastRowID);
        }


        public int Insert(object obj, string tableName)
        {
            long rowID = -1L;
            return _conn.Insert(obj, "", obj.GetType(), tableName, out rowID);
        }

        public int UpdateTable(object obj, string tableName)
        {
            Initialize(forceReinitialization: false);
            return _conn.Update(obj, obj.GetType(), tableName);
        }


        public int Delete<T>(T obj)
        {
            Initialize(forceReinitialization: false);
            return _conn.Delete(obj);
        }

        public int Delete(object obj, Type objType, string tableName)
        {
            Initialize(forceReinitialization: false);
            return _conn.Delete(obj, objType, tableName);
        }


    }
}
