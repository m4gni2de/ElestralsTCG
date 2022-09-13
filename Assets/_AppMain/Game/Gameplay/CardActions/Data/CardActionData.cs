using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using System;

namespace Gameplay
{
    [System.Serializable]
    public class CardActionData
    {
        public string actionKey;
        private Dictionary<string, object> _actionValues = null;
        public Dictionary<string, object> actionValues { get { _actionValues ??= new Dictionary<string, object>(); return _actionValues; } }


        public CardActionData(CardAction ac)
        {
            CreateData(ac.id);
        }
        public CardActionData(JSONObject obj)
        {
            int count = 0;
            foreach (var item in obj)
            {
                if (count == 0)
                {
                    actionKey = obj[0].stringValue;
                    
                }
                else
                {
                    object val = GetValue(obj[count]);
                    actionValues.Add(obj.keys[count], val);
                }
                count += 1;
            }
        }
        public void CreateData(string actionId)
        {
            actionKey = actionId;
            actionValues.Add("actionKey", actionId);
        }

        public void AddData(string key, object val)
        {
            if (!actionValues.ContainsKey(key))
            {
                actionValues.Add(key, val);
            }
        }
        public void SetData(string key, object val)
        {
            if (actionValues.ContainsKey(key))
            {
                actionValues[key] = val;
            }
        }

       


        public string GetJson
        {
            get
            {
                string json = "";

                JSONObject o = new JSONObject();
   

                foreach (var item in actionValues)
                {
                    AddValue(o, item);
                    
                    
                }

                return o.Print(true);

            }
        }

        protected void AddValue(JSONObject o, KeyValuePair<string, object> item)
        {
            if (item.Value.GetType() == typeof(string)) { o.AddField(item.Key, item.Value.ToString()); }
            if (item.Value.GetType() == typeof(int)) { o.AddField(item.Key, (int)item.Value); }
            if (item.Value.GetType() == typeof(float)) { o.AddField(item.Key, (float)item.Value); }
        }

        protected object GetValue(JSONObject o)
        {
            Type ty = o.GetType();

            if (ty == typeof(string))
            {
                return o.stringValue;
            }
            if (ty == typeof(int))
            {
                return o.intValue;
            }
            if (ty == typeof(float))
            {
                return o.floatValue;
            }

            return o.stringValue;
        }
    }
}

