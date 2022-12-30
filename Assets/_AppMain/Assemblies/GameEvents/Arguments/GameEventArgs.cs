using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

namespace GameEvents
{
    public class GameEventArgs 
    {
        #region Properties
        private Dictionary<string, object> _eventArgs = null;
        public Dictionary<string, object> EventArgs
        {
            get
            {
                _eventArgs ??= new Dictionary<string, object>();
                return _eventArgs;
            }
        }
        #endregion

        protected object this[string key]
        {
            get
            {
                if (EventArgs.ContainsKey(key))
                {
                    return EventArgs[key];  
                }
                return null;
            }
        }

        #region Initialization
        public GameEventArgs(iGameEvent ev)
        {
            for (int i = 0; i < ev.Parameters.Count; i++)
            {
                iParameter p = ev.Parameters[i];
                EventArgs.Add(p.Name, p.GetValue());
            }
        }

       
        #endregion

        public object GetArgValue(string argKey)
        {
            return this[argKey];
        }
        public object GetArgProperty(string argKey, string propName)
        {
            object obj = this[argKey];
            if (obj == null) { return null; }

            propName = propName.ToLower();
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            

            foreach (var prop in props)
            {
                if (prop.Name.ToLower() == propName)
                {
                    object propVal = prop.GetValue(obj);
                    if (propVal.GetType().IsEnum) { return (int)propVal; }
                    return propVal;
                }
            }

            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                if (field.Name.ToLower() == propName)
                {
                    object fieldVal = field.GetValue(obj);
                    if (fieldVal.GetType().IsEnum) { return (int)fieldVal; }
                    return fieldVal;
                }
            }

            return null;
        }

        protected object GetValue(object val)
        {
            if (val.GetType().IsEnum) { return (int)val; }
            if (val.GetType() == typeof(bool)) { return BoolToInt((bool)val); }
            if (val.GetType() == typeof(string)) { return val.ToString(); }
            if (val.GetType() == typeof(int)) { return (int)val; }
            if (val.GetType() == typeof(float)) { return (float)val; }
            return val;
        }
        private bool IntToBool(int a)
        {
            if (a == 0) { return false; }
            return true;
        }
        private int BoolToInt(bool a)
        {
            if (a == false) { return 0; }
            return 1;
        }
    }
}
