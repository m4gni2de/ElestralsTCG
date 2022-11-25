using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;


    public class SettingsService : PlayerService
    {
        public static readonly string SettingsTable = "GameSettings";

        public static void Save<T>(string settKey, T obj)
        {
            SettingsDTO dto = SettingsData.Create(settKey, obj);

            string pk = PrimaryKey(SettingsTable);
            if (!KeyExists<SettingsDTO>(SettingsTable, pk, settKey))
            {
                db.Insert(dto, SettingsTable);
            }
            else
            {
                db.UpdateTable(dto, SettingsTable);
            }

            db.Commit();
        }

      
        public static SettingsData FindSettings(string settKey)
        {
            string pk = PrimaryKey(SettingsTable);
            string query = $"SELECT * FROM {SettingsTable} ";
            string wh = $"{pk} = '{settKey}'";


            List<SettingsDTO> results = GetAllWhere<SettingsDTO>(SettingsTable, wh);
            if (results.Count > 0)
            {
                return results[0];
            }
            return SettingsData.Empty;
            
        }

       

    }


