using System;
using System.Collections.Generic;
using System.Linq;

namespace DecimalInternetClock.Helpers
{
    public static class ExtensionMethodsNumericGeneric
    {
        static ExtensionMethodsNumericGeneric()
        {
            ValueTypesDictionary = new Dictionary<Type, ENumericValueTypes>();
            foreach (KeyValuePair<Type, ENumericValueTypes> item in IntegerValueTypesDictionary.Union(FloatValueTypesDictionary))
                ValueTypesDictionary.Add(item.Key, item.Value);
        }

        public enum ENumericValueTypes
        {
            //_byte,
            _Byte,
            _SByte,
            //_short,
            _Int16,
            //_int,
            _Int32,
            //_long,
            _Int64,
            //_ushort,
            _UInt16,
            //_uint,
            _UInt32,
            //_ulong,
            _UInt64,
            //_float,
            _Single,
            //_double,
            _Double,
            _Char
        }

        #region public static Dictionary<Type, ENumericValueTypes> ValueTypesDictionary

        public static Dictionary<Type, ENumericValueTypes> IntegerValueTypesDictionary = new Dictionary<Type, ENumericValueTypes>()
        {
            {typeof(Byte), ENumericValueTypes._Byte},
            {typeof(SByte), ENumericValueTypes._SByte},
            {typeof(Char), ENumericValueTypes._Char},
            {typeof(Int16), ENumericValueTypes._Int16},
            {typeof(Int32),ENumericValueTypes._Int32},
            {typeof(Int64),ENumericValueTypes._Int64},
            {typeof(UInt16),ENumericValueTypes._UInt16},
            {typeof(UInt32),ENumericValueTypes._UInt32},
            {typeof(UInt64),ENumericValueTypes._UInt64},
        };

        public static Dictionary<Type, ENumericValueTypes> ValueTypesDictionary;

        public static Dictionary<Type, ENumericValueTypes> FloatValueTypesDictionary = new Dictionary<Type, ENumericValueTypes>()
        {
            {typeof(Single),ENumericValueTypes._Single},
            {typeof(Double),ENumericValueTypes._Double},
        };

        #endregion public static Dictionary<Type, ENumericValueTypes> ValueTypesDictionary

        public static double CalculateAverage<T>(this T value1_in, T value2_in)

            where T : struct, IEquatable<T>, IConvertible, IComparable<T>
        {
            if (!typeof(T).IsValueType || !ValueTypesDictionary.ContainsKey(typeof(T)))
                throw new NotSupportedException();

            //double conversion is necessary to prevent overflow in integer addition
            switch (ValueTypesDictionary[typeof(T)])
            {
                //case ENumericValueTypes._byte:
                case ENumericValueTypes._Byte:
                    return ((double)value1_in.ToByte(null) + (double)value2_in.ToByte(null)) / 2.0;
                case ENumericValueTypes._SByte:
                    return ((double)value1_in.ToSByte(null) + (double)value2_in.ToSByte(null)) / 2.0;
                //case ENumericValueTypes._short:
                case ENumericValueTypes._Int16:
                    return ((double)value1_in.ToInt16(null) + (double)value2_in.ToInt16(null)) / 2.0;
                //case ENumericValueTypes._int:
                case ENumericValueTypes._Int32:
                    return ((double)value1_in.ToInt32(null) + (double)value2_in.ToInt32(null)) / 2.0;
                //case ENumericValueTypes._long:
                case ENumericValueTypes._Int64:
                    return ((double)value1_in.ToInt64(null) + (double)value2_in.ToInt64(null)) / 2.0;
                //case ENumericValueTypes._ushort:
                case ENumericValueTypes._UInt16:
                    return ((double)value1_in.ToUInt16(null) + (double)value2_in.ToUInt16(null)) / 2.0;
                //case ENumericValueTypes._uint:
                case ENumericValueTypes._UInt32:
                    return ((double)value1_in.ToUInt32(null) + (double)value2_in.ToUInt32(null)) / 2.0;
                //case ENumericValueTypes._ulong:
                case ENumericValueTypes._UInt64:
                    return ((double)value1_in.ToUInt64(null) + (double)value2_in.ToUInt64(null)) / 2.0;
                //case ENumericValueTypes._float:
                case ENumericValueTypes._Single:
                    return ((double)value1_in.ToSingle(null) + (double)value2_in.ToSingle(null)) / 2.0;
                //case ENumericValueTypes._double:
                case ENumericValueTypes._Double:
                    return ((double)value1_in.ToDouble(null) + (double)value2_in.ToDouble(null)) / 2.0;
                default:
                    throw new NotSupportedException();
            }
        }

        public static T AddWithValue<T>(this T value1_in, T value2_in)
            where T : struct, IEquatable<T>, IConvertible, IComparable<T>
        {
            if (!typeof(T).IsValueType || !ValueTypesDictionary.ContainsKey(typeof(T)))
                throw new NotSupportedException();

            switch (ValueTypesDictionary[typeof(T)])
            {
                //case ENumericValueTypes._byte:
                case ENumericValueTypes._Byte:
                    return (T)(object)(value1_in.ToByte(null) + value2_in.ToByte(null));
                case ENumericValueTypes._SByte:
                    return (T)(object)(value1_in.ToSByte(null) + value2_in.ToSByte(null));
                //case ENumericValueTypes._short:
                case ENumericValueTypes._Int16:
                    return (T)(object)(value1_in.ToInt16(null) + value2_in.ToInt16(null));
                //case ENumericValueTypes._int:
                case ENumericValueTypes._Int32:
                    return (T)(object)(value1_in.ToInt32(null) + value2_in.ToInt32(null));
                //case ENumericValueTypes._long:
                case ENumericValueTypes._Int64:
                    return (T)(object)(value1_in.ToInt64(null) + value2_in.ToInt64(null));
                //case ENumericValueTypes._ushort:
                case ENumericValueTypes._UInt16:
                    return (T)(object)(value1_in.ToUInt16(null) + value2_in.ToUInt16(null));
                //case ENumericValueTypes._uint:
                case ENumericValueTypes._UInt32:
                    return (T)(object)(value1_in.ToUInt32(null) + value2_in.ToUInt32(null));
                //case ENumericValueTypes._ulong:
                case ENumericValueTypes._UInt64:
                    return (T)(object)(value1_in.ToUInt64(null) + value2_in.ToUInt64(null));
                //case ENumericValueTypes._float:
                case ENumericValueTypes._Single:
                    return (T)(object)(value1_in.ToSingle(null) + value2_in.ToSingle(null));
                //case ENumericValueTypes._double:
                case ENumericValueTypes._Double:
                    return (T)(object)(value1_in.ToDouble(null) + value2_in.ToDouble(null));
                default:
                    throw new NotSupportedException();
            }
        }

        public static object DefaultValue(this Type t)
        {
            if (!t.IsValueType)
                throw new ArgumentException("Only value types are acceptable");
            if (t == typeof(DateTime)) // HACK
            {
                return new DateTime();
            }
            return Convert.ChangeType(0, t);
        }

        public static T DefaultValue<T>()
            where T : struct, IEquatable<T>, IConvertible, IComparable<T>
        {
            return (T)DefaultValue(typeof(T));
        }

        public static bool IsIntegerType(this object value)
        {
            return IntegerValueTypesDictionary.ContainsKey(value.GetType());
        }
    }
}