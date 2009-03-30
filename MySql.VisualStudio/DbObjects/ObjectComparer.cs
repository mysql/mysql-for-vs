using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DbObjects
{
    class ObjectHelper
    {
        public static bool AreEqual(object one, object two)
        {
            Type firstType = one.GetType();
            Type secondType = two.GetType();
            if (firstType != secondType) return false;

            PropertyInfo[] properties = firstType.GetProperties(
                BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                object firstValue = property.GetValue(one, null);
                if (!(firstValue is IComparable)) continue;

                object secondValue = property.GetValue(two, null);
                if (!firstValue.Equals(secondValue)) return false;
            }
            return true;
        }

        public static void Copy(object from, object to)
        {
            Type firstType = from.GetType();
            Type secondType = to.GetType();

            PropertyInfo[] properties = firstType.GetProperties(
                BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.SetProperty);
            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite) continue;
                object firstValue = property.GetValue(from, null);
                property.SetValue(to, firstValue, null);
            }
        }
    }
}
