using System;
using R3;
using Unity.Properties;
using UnityEngine;

namespace Sim.Faciem.uGUI.Binding
{
    internal static class SimBindingFactory
    {
        private static readonly SimObservablePropertyPathVisitor s_observablePathVisitor = new();
        private static readonly SimPropertyPathVisitor s_propertyPathVisitor = new();
        
        public static SimDataBinding<T> CreateBinding<T>(SimBindingInfo bindingInfo)
        {
            if (bindingInfo.BindingType == BindingType.BindToUI)
            {
                var propertyPath = bindingInfo.PropertyPath;

                var propertyPathParts = propertyPath.ExtractPaths();

                Observable<object> propertyChanges = null;
                
                foreach (var simPropertyPathPart in propertyPathParts)
                {
                    if (simPropertyPathPart.Kind == SimPropertyPathPartKind.Observable)
                    {
                        if (propertyChanges == null)
                        {
                            s_observablePathVisitor.Path = simPropertyPathPart.Path;
                            if (!PropertyContainer.TryAccept(s_observablePathVisitor, ref bindingInfo.DataSource))
                            {
                                Debug.LogError("Could not evaluate observable property path");
                                return null;
                            }
                            
                            var getter = s_observablePathVisitor.PropertyGetter;
                            s_observablePathVisitor.Reset();
                            propertyChanges = getter(bindingInfo.DataSource);
                        }
                        else
                        {
                            Func<object, Observable<object>> getter = null;
                            propertyChanges = propertyChanges
                                .Select(subDataSource =>
                                {
                                    if (getter == null)
                                    {
                                        s_observablePathVisitor.Path = simPropertyPathPart.Path;
                                        if (!PropertyContainer.TryAccept(s_observablePathVisitor, ref subDataSource))
                                        {
                                            Debug.LogError("Could not evaluate sub-dataContext-property path");
                                            return Observable.Never<object>();
                                        }

                                        getter = s_observablePathVisitor.PropertyGetter;
                                        s_observablePathVisitor.Reset();
                                    }
                                    
                                    var observable = getter(subDataSource);
                                    
                                    return observable;
                                })
                                .Switch();
                        }
                    }

                    if (simPropertyPathPart.Kind == SimPropertyPathPartKind.Property)
                    {
                        if (simPropertyPathPart.Path.IsEmpty)
                        {
                            continue;
                        }
                        
                        if (propertyChanges != null)
                        {
                            Func<object, object> getter = null;
                            
                            propertyChanges = propertyChanges
                                .Where(dataSource => dataSource != null)
                                .Select(dataSource =>
                                {
                                    if (getter == null)
                                    {
                                        s_propertyPathVisitor.Path = simPropertyPathPart.Path;
                                        if (!PropertyContainer.TryAccept(s_propertyPathVisitor, ref dataSource))
                                        {
                                            Debug.LogError("Could not evaluate property path");
                                            return null;
                                        }

                                        getter = s_propertyPathVisitor.Getter;
                                        s_propertyPathVisitor.Reset();
                                    }

                                    var value =  getter(dataSource);

                                    foreach (var simConverterBaseBehaviour in bindingInfo.Converters)
                                    {
                                        value = simConverterBaseBehaviour.Convert(value);
                                    }
                                    
                                    return value;
                                });
                        }
                    }
                }

                if (propertyChanges == null)
                {
                    return new SimOneWayToUIBinding<T>(Observable.Never<T>());
                }
                
                var obs = propertyChanges
                    .OfType<object, T>();
                
                return new SimOneWayToUIBinding<T>(obs);
            }
            
            return null;
        }

        public static SimGenericRuntimeBinding CreateGenericBinding(GenericBindableProperty genericBindableProperty)
        {
            var sourceBinding = CreateBinding<object>(genericBindableProperty.BindingInfo);

            var parts = genericBindableProperty.Target.PropertyPath.ExtractPaths();
            if (parts.Length > 1)
            {
                Debug.LogError("Cannot set value to an observable property");
                return null;
            }
            
            s_propertyPathVisitor.Path = parts[0].Path;
            var targetComponent = genericBindableProperty.Target.Component;
            if (!PropertyContainer.TryAccept(s_propertyPathVisitor, ref targetComponent))
            {
                Debug.LogError("Could not evaluate property path");
                return null;
            }

            var setter = s_propertyPathVisitor.Setter;
            s_propertyPathVisitor.Reset();

            return new SimGenericRuntimeBinding(sourceBinding.Value, targetComponent, setter, genericBindableProperty.Target.PropertyPath);
        }
    }
}