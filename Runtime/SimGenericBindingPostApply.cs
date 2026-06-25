using System;
using System.Collections.Generic;

namespace Sim.Faciem.uGUI
{
    public static class SimGenericBindingApplyActions
    {
        private static readonly Dictionary<Type, List<Action<object, SimPropertyPath>>> s_postApplyActions = new();
        private static readonly Dictionary<Type, List<Func<object, object, SimPropertyPath, bool>>> s_applyActions = new();

        public static void RegisterApplyAction<T>(Func<object, object, SimPropertyPath, bool> func)
        {
            var type = typeof(T);

            if (!s_applyActions.TryGetValue(type, out var list))
            {
                list = new List<Func<object, object, SimPropertyPath, bool>>();
                s_applyActions.Add(type, list);
            }

            list.Add(func);
        }

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

        internal static bool ApplyAction(object source, object value, SimPropertyPath propertyPath)
        {
            var type = source.GetType();

            if (!s_applyActions.TryGetValue(type, out var actions))
            {
                return false;
            }

            foreach (var action in actions)
            {
                if (action(source, value, propertyPath))
                {
                    return true;
                }
            }

            return false;
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
