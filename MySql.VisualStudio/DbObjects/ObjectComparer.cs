using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MySql.Data.VisualStudio.DbObjects
{
    class ObjectComparer
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
    }
}
