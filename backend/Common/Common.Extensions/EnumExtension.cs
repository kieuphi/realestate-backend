using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Common.Extensions
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),
                false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }

        public static bool In<T>(this T me, params T[] set)
        {
            return set.Contains(me);
        }

        public static bool In<T>(this T me, IEnumerable<T> set)
        {
            return set.Contains(me);
        }

        public static bool NotIn<T>(this T me, params T[] set)
        {
            return !set.Contains(me);
        }

        public static bool NotIn<T>(this T me, IEnumerable<T> set)
        {
            return !set.Contains(me);
        }
    }
}
