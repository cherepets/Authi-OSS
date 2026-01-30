using System;
using System.Linq;
using System.Reflection;

namespace Authi.Common.Extensions
{
    public static class ObjectExt
    {
        public static T MapPropertiesTo<T>(this object source)
        {
            return source.MapPropertiesTo(Activator.CreateInstance<T>());
        }

        public static T MapPropertiesTo<T>(this object source, T destination)
        {
            var sourceProperties = source
                .GetType()
                .GetProperties()
                .Where(x => x.CanRead)
                .ToList();

            var destinationProperties = typeof(T)
                .GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var sourceProperty in sourceProperties)
            {
                if (destinationProperties.FirstOrDefault(x => x.Name == sourceProperty.Name) is PropertyInfo destinationProperty)
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                }
            }

            return destination;
        }

        public static T Customize<T>(this T obj, Action<T> customization)
        {
            customization?.Invoke(obj);
            return obj;
        }
    }
}
