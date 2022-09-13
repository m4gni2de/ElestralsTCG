using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalUtilities
{
    public class GenericNumber<T>
    {
        private T _Value { get; set; }
        public T Value
        {
            get { return _Value; }
        }

        private Type ValueType => Value.GetType();

        #region Numeric Conversions
        public object TrueValue
        {
            get
            {
                if (ValueType == typeof(int)) { return IntValue; }
                if (ValueType == typeof(float)) { return FloatValue; }
                if (ValueType == typeof(decimal)) { return DecimalValue; }
                if (ValueType == typeof(double)) { return DoubleValue; }

                return Value;
            }
        }
        public int IntValue
        {
            get
            {
                int val = int.Parse(Value.ToString());
                return val;
            }
        }
        public float FloatValue
        {
            get
            {
                float val = float.Parse(Value.ToString());
                return val;
            }
        }
        public double DoubleValue
        {
            get
            {
                double val = double.Parse(Value.ToString());
                return val;
            }
        }

        public decimal DecimalValue
        {
            get
            {
                decimal val = decimal.Parse(Value.ToString());
                return val;
            }
        }

        public bool IsInt => ValueType == typeof(int);
        public bool IsFloat => ValueType == typeof(float);
        public bool IsDecimal => ValueType == typeof(decimal);
        public bool IsDouble => ValueType == typeof(double);
        #endregion
        #region Operators
        public static int operator +(GenericNumber<T> a, int b) => a.IntValue + b;
        public static float operator +(GenericNumber<T> a, float b) => a.FloatValue + b;
        public static decimal operator +(GenericNumber<T> a, decimal b) => a.DecimalValue + b;
        public static double operator +(GenericNumber<T> a, double b) => a.DoubleValue + b;

        public static int operator -(GenericNumber<T> a, int b) => a.IntValue - b;
        public static float operator -(GenericNumber<T> a, float b) => a.FloatValue - b;
        public static decimal operator -(GenericNumber<T> a, decimal b) => a.DecimalValue - b;
        public static double operator -(GenericNumber<T> a, double b) => a.DoubleValue - b;
        public static bool operator >(GenericNumber<T> a, GenericNumber<T> b) => a.FloatValue > b.FloatValue;
        public static bool operator <(GenericNumber<T> a, GenericNumber<T> b) => a.FloatValue < b.FloatValue;
        public static bool operator ==(GenericNumber<T> a, GenericNumber<T> b)
        {
            if (a.IsInt) { return a.IntValue == b.IntValue; }
            if (a.IsFloat) { return a.FloatValue == b.FloatValue; }
            if (a.IsDecimal) { return a.DecimalValue == b.DecimalValue; }
            if (a.IsDouble) { return a.DoubleValue == b.DoubleValue; }
            return false;
        }
        public static bool operator !=(GenericNumber<T> a, GenericNumber<T> b)
        {
            if (a.IsInt) { return a.IntValue != b.IntValue; }
            if (a.IsFloat) { return a.FloatValue != b.FloatValue; }
            if (a.IsDecimal) { return a.DecimalValue != b.DecimalValue; }
            if (a.IsDouble) { return a.DoubleValue != b.DoubleValue; }
            return true;
        }
        #endregion

        #region Comparer Overrides
        public override bool Equals(object obj)
        {
            return obj is GenericNumber<T> number &&
                   EqualityComparer<T>.Default.Equals(_Value, number._Value) &&
                   EqualityComparer<T>.Default.Equals(Value, number.Value) &&
                   EqualityComparer<Type>.Default.Equals(ValueType, number.ValueType) &&
                   EqualityComparer<object>.Default.Equals(TrueValue, number.TrueValue) &&
                   IntValue == number.IntValue &&
                   FloatValue == number.FloatValue &&
                   DoubleValue == number.DoubleValue &&
                   DecimalValue == number.DecimalValue &&
                   IsInt == number.IsInt &&
                   IsFloat == number.IsFloat &&
                   IsDecimal == number.IsDecimal &&
                   IsDouble == number.IsDouble;
        }

        public override int GetHashCode()
        {
            int hashCode = 1046367059;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(_Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ValueType);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(TrueValue);
            hashCode = hashCode * -1521134295 + IntValue.GetHashCode();
            hashCode = hashCode * -1521134295 + FloatValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DoubleValue.GetHashCode();
            hashCode = hashCode * -1521134295 + DecimalValue.GetHashCode();
            hashCode = hashCode * -1521134295 + IsInt.GetHashCode();
            hashCode = hashCode * -1521134295 + IsFloat.GetHashCode();
            hashCode = hashCode * -1521134295 + IsDecimal.GetHashCode();
            hashCode = hashCode * -1521134295 + IsDouble.GetHashCode();
            return hashCode;
        }
        #endregion

        public GenericNumber(T value)
        {
            if (!value.IsNumeric()) { throw new System.Exception("Generic number class can only accept numeric values!"); }
            _Value = value;
        }


    }
}
