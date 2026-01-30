using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Linq;
using System.Reflection;

namespace Authi.App.WinUI.Converters
{
    public class ConvertersDictionary : ResourceDictionary
    {
        public ConvertersDictionary()
        {
            var assembly = typeof(ConvertersDictionary).GetTypeInfo().Assembly;
            var baseType = typeof(IValueConverter).GetTypeInfo();
            var converters = assembly.DefinedTypes.Where(baseType.IsAssignableFrom).ToList();
            foreach (var converter in converters)
            {
                var name = converter.Name.Replace("Converter", string.Empty);
                var value = Activator.CreateInstance(converter.AsType());
                this[name] = value;
            }
        }
    }
}
