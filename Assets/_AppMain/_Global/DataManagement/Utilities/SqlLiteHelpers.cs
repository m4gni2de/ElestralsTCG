using System.Collections;
using System.Collections.Generic;
using SimpleSQL;
using System;
using System.Linq;

namespace Databases
{


    public static class SQLiteHelpers
    {

        public static int Insert(this SQLiteConnection conn, object obj, string extra, Type objType, string tableName, out long rowID)
        {
            rowID = -1L;
            if (obj == null || (object)objType == null)
            {
                return 0;
            }

            TableMapping mapping = conn.GetMapping(objType);

            TableMapping.Column[] insertColumns = mapping.InsertColumns;
            object[] array = new object[insertColumns.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = insertColumns[i].GetValue(obj);
            }

            string query = mapping.InsertSql(tableName, extra);
            int result = conn.Execute(query, array.ToArray());
            if (mapping.HasAutoIncPK)
            {
                rowID = SQLite3.LastInsertRowid(conn.Handle);
                mapping.SetAutoIncPK(obj, rowID);
            }

            return result;
        }

        public static string InsertSql(this TableMapping mapping, string tableName, string extra)
        {
            TableMapping.Column[] insertColumns = mapping.InsertColumns;
            string _insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", tableName, string.Join(",", insertColumns.Select((TableMapping.Column c) => "\"" + c.Name + "\"").ToArray()), string.Join(",", insertColumns.Select((TableMapping.Column c) => "?").ToArray()), extra);

            return _insertSql;

        }

        public static int Update(this SQLiteConnection conn, object obj, Type objType, string tableName)
        {
            if (obj == null || (object)objType == null)
            {
                return 0;
            }

            TableMapping mapping = conn.GetMapping(objType);
            TableMapping.Column pk = mapping.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot update " + tableName + ": it has no PK");
            }

            IEnumerable<TableMapping.Column> source = mapping.Columns.Where((TableMapping.Column p) => p != pk);
            IEnumerable<object> collection = source.Select((TableMapping.Column c) => c.GetValue(obj));
            List<object> list = new List<object>(collection);
            list.Add(pk.GetValue(obj));
            string query = string.Format("update \"{0}\" set {1} where {2} = ? ", tableName, string.Join(",", source.Select((TableMapping.Column c) => "\"" + c.Name + "\" = ? ").ToArray()), pk.Name);
            return conn.Execute(query, list.ToArray());
        }



        public static int Delete(this SQLiteConnection conn, object obj, Type objType, string tableName)
        {
            TableMapping mapping = conn.GetMapping(objType);
            TableMapping.Column pK = mapping.PK;
            if (pK == null)
            {
                throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
            }

            string query = $"delete from \"{tableName}\" where \"{pK.Name}\" = ?";
            return conn.Execute(query, pK.GetValue(obj));
        }

        public static int CreateTable(this SQLiteConnection conn, Type type, string tableName)
        {
            TableMapping value = conn.GetMapping(type);
            string str = "create table \"" + tableName + "\"(\n";
            IEnumerable<string> source = value.Columns.Select((TableMapping.Column p) => Orm.SqlDecl(p));
            string str2 = string.Join(",\n", source.ToArray());
            str += str2;
            str += ")";
            int num = 0;
            try
            {
                conn.Execute(str);
                num = 1;
            }
            catch (SQLiteException)
            {
            }

            if (num == 0)
            {
                MigrateTable(conn, value, tableName);
            }

            foreach (TableMapping.Column item in value.Columns.Where((TableMapping.Column x) => x.IsIndexed))
            {
                string arg = tableName + "_" + item.Name;
                string query = $"create index if not exists \"{arg}\" on \"{tableName}\"(\"{item.Name}\")";
                num += conn.Execute(query);
            }

            return num;
        }


        private static void MigrateTable(SQLiteConnection conn, TableMapping map, string tableName)
        {
            string query = "pragma table_info(\"" + tableName + "\")";
            List<TableInfo> list = conn.Query<TableInfo>(query, new object[0]);
            List<TableMapping.Column> list2 = new List<TableMapping.Column>();
            TableMapping.Column[] columns = map.Columns;
            foreach (TableMapping.Column column in columns)
            {
                bool flag = false;
                foreach (TableInfo item in list)
                {
                    flag = (column.Name == item.name);
                    if (flag)
                    {
                        break;
                    }
                }

                if (!flag)
                {
                    list2.Add(column);
                }
            }

            foreach (TableMapping.Column item2 in list2)
            {
                string query2 = "alter table \"" + tableName + "\" add column " + Orm.SqlDecl(item2);

                conn.Execute(query2);
            }
        }


    }
}