using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Sim.Faciem.uGUI.Editor.Controls;
using Unity.Properties;

namespace Sim.Faciem.uGUI.Editor.Internal
{
    internal static class PropertyContainerCompat
    {
        private static readonly PropertyPathVisitor s_propertyPathVisitor = new();

        public static IEnumerable<(SimPropertyPath, Type)> GetAllPropertiesRecursive(Type type)
        {
            var path = new SimPropertyPath(new PropertyPath());
            return GetAllPropertiesRecursive(type, path);
        }

        private static IEnumerable<(SimPropertyPath, Type)> GetAllPropertiesRecursive(Type type, SimPropertyPath path)
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

                if (!HasProperties(unpackedType))
                {
                    continue;
                }
                
                foreach (var child in GetAllPropertiesRecursive(unpackedType, childPath))
                {
                    yield return child;
                }
            }
        }
        
        public static IEnumerable<IProperty> GetProperties(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var bag = PropertyBag.GetPropertyBag(type);
            if (bag == null)
                yield break;

            var list = new List<IProperty>();
            s_propertyPathVisitor.Properties = list;
            bag.Accept(s_propertyPathVisitor);

            foreach (var property in list)
                yield return property;
        }
        
        public static bool HasProperties(Type type)
        {
            if (type == null) return false;

            // Get the property bag for the type
            var bag = PropertyBag.GetPropertyBag(type);
            if (bag == null)
                return false;

            // Use a visitor to check if the bag has any properties
            var list = new List<IProperty>();
            s_propertyPathVisitor.Properties = list;
            bag.Accept(s_propertyPathVisitor);
            
            var hasAny = list.Count > 0;
            return hasAny;
        }
        
        private static Type TryUnpackReactiveTypes(Type valueType, out bool didUnpack)
        {
            didUnpack = false;
            if(valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(Observable<>)
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