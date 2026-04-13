using System;
using System.Linq;
using System.Reflection;
using R3;
using Sim.Faciem.uGUI.Internal;
using Unity.Properties;
using UnityEngine;

namespace Sim.Faciem.uGUI.Binding
{
    public class SimObservablePropertyPathVisitor : PathVisitor
    {
        public Func<object, Observable<object>> PropertyGetter { get; private set; }

        protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property,
            ref TContainer container, ref TValue value)
        {
            PropertyGetter = source =>
            {
                if (!ReactiveTypeUtils.IsObservableType(typeof(TValue)))
                {
                    Debug.LogError("Only Observables are supported for this path visitor");
                    return null;
                }

                var valueType = typeof(TValue).GetGenericArguments()[0];

                var extensionClass = typeof(ObservableExtensions);

                var method = extensionClass
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m =>
                        m.Name == nameof(ObservableExtensions.Cast) &&
                        m.IsGenericMethodDefinition);

                var castMethod = method.MakeGenericMethod(valueType, typeof(object));

                var typedSource = (TContainer)source;
                var result = castMethod.Invoke(null, new object[] { property.GetValue(ref typedSource) });

                if (result is Observable<object> obj)
                {
                    return obj;
                }

                return null;
            };
        }
    }
}