using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using System;
using Gameplay.CardActions;

namespace Gameplay
{
    [System.Serializable]
    public class CardActionData
    {

        

        #region Properties
        public string actionKey;
        private Dictionary<string, object> _actionValues = null;
        public Dictionary<string, object> actionValues { get { _actionValues ??= new Dictionary<string, object>(); return _actionValues; } }


        public List<string> bindings = new List<string>();
        #endregion

       


        #region Indexers/Field Functions
        protected int GetActionType
        {
            get
            {
                return (int)actionValues[CategoryKey];
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

        #region Base Fields
        public static readonly string CategoryKey = "category";
        public void SetCategory(ActionCategory cat)
        {
            SetData(CategoryKey, (int)cat);
        }
        public ActionCategory GetCategory(string valKey = "")
        {
            if (string.IsNullOrEmpty(valKey)) { valKey = CategoryKey; }
            return (ActionCategory)Value<int>(valKey);
        }
        public static readonly string PlayerKey = "player";
        public void SetPlayer(Player p)
        {
            SetData(PlayerKey, p.username);
        }
        public Player FindPlayer(string playerKey = "")
        {
            if (string.IsNullOrEmpty(playerKey)) { playerKey = PlayerKey; }
            return Game.FindPlayer(Value<string>(playerKey));
        }
        public static readonly string SpiritPrefix = "spirit_";
        public void SetSpirit(int index, string cardKey)
        {
            string st = $"{SpiritPrefix}_{index}";
            AddData(st, cardKey);
        }
        public void SetSpiritList(List<GameCard> spirits)
        {
            int count = CountOfSpiritFields();
            for (int i = 0; i < spirits.Count; i++)
            {
                string st = $"{SpiritPrefix}_{i + count}";
                AddData(st, spirits[i].cardId);
            }
            
            
        }
        public static readonly string SourceKey = "source_card";
        public void SetSourceCard(GameCard card)
        {
            AddData(SourceKey, card.cardId);
        }
        public GameCard FindSourceCard(string valKey = "")
        {
            if (string.IsNullOrEmpty(valKey)) { valKey = SourceKey; }
            return Game.FindCard(Value<string>(valKey));
        }
        public static readonly string ResultKey = "result";
        public void SetResult(ActionResult result)
        {
            AddData(ResultKey, (int)result);
        }
        public ActionResult GetResult()
        {
            return (ActionResult)Value<int>(ResultKey);
        }
        #endregion

        public CardActionData(ActionCategory cat)
        {
            CreateData(UniqueString.GetShortId($"ca", 5));
            SetCategory(cat);

        }
        
        public CardActionData(CardAction ac)
        {
            CreateData(ac.id);
            SetCategory(ac.category);
        }
        public CardActionData(JSONObject obj)
        {
            for (int i = 0; i < obj.keys.Count; i++)
            {
                AddData(obj.keys[i], GetValue(obj.list[i]));
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
            else
            {
                SetData(key, val);
            }
        }
        public void SetData(string key, object val)
        {
            if (actionValues.ContainsKey(key))
            {
                actionValues[key] = val;
            }
            else
            {
                AddData(key, val);
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


            if (o.type == JSONObject.Type.Number)
            {
                if (o.isInteger)
                {
                    return o.intValue;
                }
                return o.floatValue;

            }
            if (o.type == JSONObject.Type.String)
            {
                return o.stringValue;
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
                
            }
           
        }

        public static CardAction ParseData(CardActionData data)
        {
            ActionCategory acCat = (ActionCategory)data.GetActionType;

            switch (acCat)
            {
                case ActionCategory.None:
                    return MoveAction.FromData(data);
                case ActionCategory.Draw:
                    return DrawAction.FromData(data);
                case ActionCategory.Shuffle:
                    break;
                case ActionCategory.Enchant:
                    return EnchantAction.FromData(data);
                case ActionCategory.Mode:
                    break;
                case ActionCategory.Attack:
                    break;
                case ActionCategory.Nexus:
                    return NexusAction.FromData(data);
                case ActionCategory.Ascend:
                    return AscendAction.FromData(data);
                default:
                    break;
            }


            return null;
        }


        #endregion


       
        
        
    }
}

