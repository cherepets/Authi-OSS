using System;
using System.Linq;
using System.Reflection;

namespace Authi.Common.Extensions
{
    internal static class TypeExt
    {
        public static bool HasParameterlessConstructor(this Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return type.GetConstructor(flags, null, Type.EmptyTypes, null) != null;
        }

        public static bool HasAttribute<T>(this TypeInfo type) where T : Attribute
        {
            var attr = typeof(T);
            return type.CustomAttributes.Any(t => t.AttributeType == attr);
        }
    }
}
