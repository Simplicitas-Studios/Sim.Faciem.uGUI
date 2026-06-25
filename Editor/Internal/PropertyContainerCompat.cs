using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Sim.Faciem.uGUI.Editor.Controls;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor.Internal
{
    internal static class PropertyContainerCompat
    {
        private static readonly PropertyPathVisitor s_propertyPathVisitor = new();
        private static readonly Dictionary<Type, List<(SimPropertyPath Path, Type Type)>> s_recursivePropertyCache = new();
        private static readonly Dictionary<Type, bool> s_hasPropertiesCache = new();

        public static IEnumerable<(SimPropertyPath, Type)> GetAllPropertiesRecursive(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (s_recursivePropertyCache.TryGetValue(type, out var cachedProperties))
            {
                return cachedProperties;
            }

            var path = new SimPropertyPath(new PropertyPath());
            var properties = GetAllPropertiesRecursive(type, path, new HashSet<Type>())
                .Select(x => (x.Item1, x.Item2))
                .ToList();

            s_recursivePropertyCache[type] = properties;
            return properties;
        }

        private static IEnumerable<(SimPropertyPath, Type)> GetAllPropertiesRecursive(Type type, SimPropertyPath path, HashSet<Type> recursionStack)
        {
            if (type == null || !recursionStack.Add(type))
            {
                yield break;
            }

            try
            {
                foreach (var property in GetProperties(type))
                {
                    var childPath = SimPropertyPath.AppendPath(path, property.Name);
                    var valueType = property.DeclaredValueType();

                    var unpackedType = TryUnpackReactiveTypes(valueType, out var wasReactiveType);

                    if (wasReactiveType)
                    {
                        childPath = SimPropertyPath.AppendSubscription(childPath);
                    }

                    yield return (childPath, unpackedType);

                    if (!HasProperties(unpackedType) || recursionStack.Contains(unpackedType))
                    {
                        continue;
                    }

                    foreach (var child in GetAllPropertiesRecursive(unpackedType, childPath, recursionStack))
                    {
                        yield return child;
                    }
                }
            }
            finally
            {
                recursionStack.Remove(type);
            }
        }

        public static IEnumerable<IProperty> GetProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var bag = PropertyBag.GetPropertyBag(type);
            if (bag == null)
            {
                yield break;
            }

            var list = new List<IProperty>();
            s_propertyPathVisitor.Properties = list;
            bag.Accept(s_propertyPathVisitor);

            foreach (var property in list)
            {
                yield return property;
            }
        }

        public static bool HasProperties(Type type)
        {
            if (type == null || type == typeof(VisualTreeAsset))
            {
                return false;
            }

            if (s_hasPropertiesCache.TryGetValue(type, out var cachedHasProperties))
            {
                return cachedHasProperties;
            }

            var bag = PropertyBag.GetPropertyBag(type);
            if (bag == null)
            {
                s_hasPropertiesCache[type] = false;
                return false;
            }

            var list = new List<IProperty>();
            s_propertyPathVisitor.Properties = list;
            bag.Accept(s_propertyPathVisitor);

            var hasAny = list.Count > 0;
            s_hasPropertiesCache[type] = hasAny;
            return hasAny;
        }

        private static Type TryUnpackReactiveTypes(Type valueType, out bool didUnpack)
        {
            didUnpack = false;
            if (valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(Observable<>)
                    || valueType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>)
                    || valueType.GetGenericTypeDefinition() == typeof(ReadOnlyReactiveProperty<>)))
            {
                var observableType = valueType.GetGenericArguments().First();
                didUnpack = true;
                return observableType;
            }

            return valueType;
        }
    }
}
