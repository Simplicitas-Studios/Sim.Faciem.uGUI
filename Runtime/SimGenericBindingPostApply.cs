using System;
using System.Collections.Generic;

namespace Sim.Faciem.uGUI
{
    public static class SimGenericBindingPostApply
    {
        private static readonly Dictionary<Type, List<Action<object, SimPropertyPath>>> s_postApplyActions = new();

        public static void RegisterPostApplyAction<T>(Action<object, SimPropertyPath> action)
        {
            var type = typeof(T);

            if (!s_postApplyActions.TryGetValue(type, out var list))
            {
                list = new List<Action<object, SimPropertyPath>>();
                s_postApplyActions.Add(type, list);
            }
            
            list.Add(action);
        }

        internal static void PostApply(object source, SimPropertyPath propertyPath)
        {
            var type = source.GetType();

            if (!s_postApplyActions.TryGetValue(type, out var actions))
            {
                return;
            }
            foreach (var action in actions)
            {
                action(source, propertyPath);
            }
        }
    }
}