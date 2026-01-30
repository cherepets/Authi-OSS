using Authi.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Authi.Common.Services
{
    public class ServiceLocator
    {
        private static ServiceLocator? _current;

        public static ServiceLocator Current
        {
            get => _current ?? throw new NullReferenceException($"{nameof(ServiceLocator)} was not initialized before use.");
            private set => _current = value;
        }

        public IReadOnlyDictionary<string, TypeInfo> Services { get; }

        protected ServiceLocator(IReadOnlyDictionary<string, TypeInfo> services)
        {
            Services = services;
        }

        public static void Init(params Assembly[] assemblies)
        {
            var orderedTypes = assemblies
                .Select(assembly => assembly.DefinedTypes)
                .SelectMany(typeInfo => typeInfo)
                .ToList();
            var serviceDeclarations = orderedTypes
                .Where(IsService)
                .ToList();
            var concreteTypes = orderedTypes
                .Where(IsConcrete)
                .ToList();

            var services = serviceDeclarations.ToDictionary(
                declaration => declaration.Name,
                declaration => concreteTypes.FirstOrDefault(declaration.IsAssignableFrom)
                    ?? throw new NotImplementedException($"Service [{declaration.Name}] has no implementation.")
            );

            Current = new ServiceLocator(services);
        }

        private static bool IsService(TypeInfo typeInfo)
            => typeInfo.HasAttribute<ServiceAttribute>();

        private static bool IsConcrete(TypeInfo typeInfo)
            => !typeInfo.IsAbstract && !typeInfo.IsInterface;
    }
}
