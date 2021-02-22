using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// ReSharper disable HeapView.PossibleBoxingAllocation
namespace MSDF.DataChecker.Common.Enumerations
{
    [Serializable]
    [DebuggerDisplay("{DisplayName} - {Value}")]
    public abstract class Enumeration<TEnumeration> : Enumeration<TEnumeration, int>
        where TEnumeration : Enumeration<TEnumeration>
    {
        protected Enumeration(int value, string displayName)
            : base(value, displayName) { }

        public static TEnumeration FromInt32(int value) => FromValue(value);

        public static bool TryFromInt32(int listItemValue, out TEnumeration result) => TryParse(listItemValue, out result);
    }

    [Serializable]
    [DebuggerDisplay("{DisplayName} - {Value}")]
    public abstract class Enumeration<TEnumeration, TValue> : IComparable<TEnumeration>, IEquatable<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration, TValue>
        where TValue : IComparable
    {
        private static readonly Lazy<TEnumeration[]> _enumerations = new Lazy<TEnumeration[]>(GetEnumerations);

        protected Enumeration(TValue value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public TValue Value { get; }

        public string DisplayName { get; }

        public int CompareTo(TEnumeration other) => Value.CompareTo(other.Value);

        public bool Equals(TEnumeration other) => other != null && Value.Equals(other.Value);

        public override string ToString() => DisplayName;

        public static TEnumeration[] GetAll() => _enumerations.Value;

        private static TEnumeration[] GetEnumerations()
        {
            Type enumerationType = typeof(TEnumeration);

            return enumerationType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => enumerationType.IsAssignableFrom(info.FieldType))
                .Select(info => info.GetValue(null))
                .Cast<TEnumeration>()
                .ToArray();
        }

        public override bool Equals(object obj) => Equals(obj as TEnumeration);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Enumeration<TEnumeration, TValue> left, Enumeration<TEnumeration, TValue> right)
            => Equals(left, right);

        public static bool operator !=(Enumeration<TEnumeration, TValue> left, Enumeration<TEnumeration, TValue> right)
            => !Equals(left, right);

        public static TEnumeration FromValue(TValue value) => Parse(value, "value", item => item.Value.Equals(value));

        public static TEnumeration Parse(string displayName)
            => Parse(displayName, "display name", item => item.DisplayName == displayName);

        public static bool TryParse(Func<TEnumeration, bool> predicate, out TEnumeration result)
        {
            result = GetAll().SingleOrDefault(predicate);
            return result != null;
        }

        private static TEnumeration Parse(object value, string description, Func<TEnumeration, bool> predicate)
        {
            if (!TryParse(predicate, out TEnumeration result))
            {
                string message = $"'{value}' is not a valid {description} in {typeof(TEnumeration)}";
                throw new ArgumentException(message, nameof(value));
            }

            return result;
        }

        public static bool TryParse(TValue value, out TEnumeration result) => TryParse(e => e.Value.Equals(value), out result);

        public static bool TryParse(string displayName, out TEnumeration result)
            => TryParse(e => e.DisplayName == displayName, out result);
    }
}
