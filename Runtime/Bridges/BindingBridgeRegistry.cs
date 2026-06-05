using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sim.Faciem.uGUI.Bridges
{
    public static class BindingBridgeRegistry
    {
        private static readonly Dictionary<Type, Type> s_bindingBridgeRegistry = new()
        {
            [typeof(SpriteRenderer)] = typeof(SpriteRendererBindingBridge)
        };

        public static void RegisterBindingBridge<T, TComponent>()
            where T : BindingBridge<TComponent>
            where TComponent : Component =>
            s_bindingBridgeRegistry[typeof(TComponent)] = typeof(T);

        internal static bool IsBindingBridge(Type componentType) => s_bindingBridgeRegistry.ContainsKey(componentType);

        internal static bool HasExistingBindingBridge(Type componentType, Component component)
        {
            if (s_bindingBridgeRegistry.TryGetValue(componentType, out var bridgeType))
            {
                return component.TryGetComponent(bridgeType, out _);
            }

            return false;
        }

        internal static bool TryResolveBindingBridge(Type componentType, Component component, out BindingBridgeBase bridge)
        {
            if(s_bindingBridgeRegistry.TryGetValue(componentType, out var bridgeType))
            {
                if (!component.TryGetComponent(bridgeType, out var foundBridge)
                    || foundBridge is not BindingBridgeBase bridgeInstance
                    || !bridgeInstance.InternalComponent.Equals(component))
                {
                    bridgeInstance = component.gameObject.AddComponent(bridgeType) as BindingBridgeBase;
                    bridgeInstance!.InternalComponent = component;
                    bridgeInstance.ActivatedProperties = bridgeInstance.CollectBindableProperties()
                        .Select(_ => false)
                        .ToList();
                }

                bridge = bridgeInstance;
                return true;
            }

            bridge = null;
            return false;
        }
    }
}
