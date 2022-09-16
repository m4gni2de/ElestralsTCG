using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using System;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine.UIElements;
using Gameplay.CardActions;

namespace Gameplay
{
    [System.Serializable]
    public class CardActionData
    {
        public string actionKey;
        private Dictionary<string, object> _actionValues = null;
        public Dictionary<string, object> actionValues { get { _actionValues ??= new Dictionary<string, object>(); return _actionValues; } }

        #region Indexers/Field Functions
        protected string GetActionType
        {
            get
            {
                return (string)actionValues["action_type"];
            }
        }

        public object this[string field]
        {
            get
            {
                return actionValues[field];
            }
        }


        public T Value<T>(string field)
        {
            return (T)actionValues[field];
        }
        
        /// <summary>
        /// Get the count of fields that have the same prefix, but different suffix. Example being spirit fields are named 'spirit_1', 'spirit_2', etc
        /// </summary>
        /// <param name="containsKey"></param>
        /// <returns></returns>
        protected int CountOfBaseField(string containsKey)
        {
            int count = 0;
            foreach (var item in actionValues)
            {
                string lowerKey = item.Key.ToLower();
                if (lowerKey.Contains(containsKey.ToLower()))
                 {
                    count += 1;
                }
            }
            return count;
        }

        public int CountOfSpiritFields()
        {
            int count = 0;
            foreach (var item in actionValues)
            {
                string lowerKey = item.Key.ToLower();
                if (lowerKey.Contains("spirit_"))
                {
                    count += 1;
                }
            }
            return count;
        }
        #endregion

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
                JSONObject o = new JSONObject();
                foreach (var item in actionValues)
                {
                    AddValue(o, item);
                }

                return o.Print();

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


        #region Data to Action
        public static void FromData(string jsonData)
        {
            JSONObject obj = new JSONObject(jsonData);

            if (obj.type != JSONObject.Type.Null)
            {
                CardActionData data = new CardActionData(obj);
                string acType = data.GetActionType;
            }
           
        }

        protected static CardAction ParseData(CardActionData data)
        {
            string acType = data.GetActionType.ToLower();

            switch (acType)
            {
                case "draw":
                    return DrawAction.FromData(data);
                case "attack":
                    return DrawAction.FromData(data);
            }

            return null;
        }


        #endregion
    }
}

