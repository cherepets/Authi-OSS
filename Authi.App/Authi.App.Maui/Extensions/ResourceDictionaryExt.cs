using Microsoft.Maui.Controls;
using System;

namespace Authi.App.Maui.Extensions
{
    public static class ResourceDictionaryExt
    {
        public static ResourceDictionary Merge<T>(this ResourceDictionary resources) where T : ResourceDictionary
        {
            resources.MergedDictionaries.Add(Activator.CreateInstance<T>());
            return resources;
        }
    }
}
