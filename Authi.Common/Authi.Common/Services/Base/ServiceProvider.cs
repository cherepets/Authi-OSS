using System;

namespace Authi.Common.Services
{
    public interface IServiceProvider
    {
        object GetNewInstance(string typeName);
        T Get<T>() where T : class;

        public T GetNewInstance<T>() => (T)GetNewInstance(typeof(T).Name);
        public object GetNewInstance(Type type) => GetNewInstance(type.Name);
    }

    public class ServiceProvider : IServiceProvider
    {
        public static IServiceProvider Current { get; protected set; }

        static ServiceProvider()
        {
            Current = new ServiceProvider();
        }

        protected ServiceProvider() { }

        public virtual object GetNewInstance(string typeName)
            => Activator.CreateInstance(ServiceLocator.Current.Services[typeName].AsType())
            ?? throw new ArgumentException($"Can't create instance of a type [{typeName}].");

        public virtual T Get<T>() where T : class
            => LazyResolver<T>.Resolve();

        internal static class LazyResolver<T> where T : class
        {
            public static T Resolve() => _impl ??= Current.GetNewInstance<T>();
            private static T? _impl;
        }
    }
}
