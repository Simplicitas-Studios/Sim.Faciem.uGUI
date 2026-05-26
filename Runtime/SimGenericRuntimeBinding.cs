using System;
using R3;
using Sim.Faciem.uGUI.Internal;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    public class SimGenericRuntimeBinding : IDisposable
    {
        private readonly IDisposable _subscription;

        public SimGenericRuntimeBinding(Observable<object> propertyChange, Component target,
            Action<object, object> setter, SimPropertyPath propertyPath)
        {
            _subscription = propertyChange
                .Subscribe(value =>
                {
                    try
                    {
                        setter(target, value);
                        SimGenericBindingPostApply.PostApply(target, propertyPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error for binding at {target.transform.GetTransformPath()} for property path {propertyPath}:\n{e}");
                    }
                });
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
