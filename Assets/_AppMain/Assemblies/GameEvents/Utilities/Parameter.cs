using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    public class Parameter<T> : iParameter
    { 
        
        #region Interface
        public object GetValue() { return Value; }
        #endregion


        private string _name;
        public string Name { get { return _name; } }
        private int _index;
        public int index { get { return _index; } }

        #region Value from Event
        private bool _hasValue = false;
        public bool HasValue
        {
            get
            {
                return _hasValue;
            }
        }
        private T _Value = default(T);
        public T Value
        {
            get
            {
                if (HasValue) { return _Value; } else { return default(T); }
            }
        }
        #endregion

        #region Initialization
        public Parameter(string paramName, int index)
        {
            _name = paramName;
            _index = index;
            _hasValue = false;
        }

        public static Parameter<T> Set(string paramName, int index = 0)
        {
            return new Parameter<T>(paramName, index);
        }
        #endregion

        public void SetValue(object val)
        {
            _Value = (T)val;
            _hasValue = true;
        }
    }
}
