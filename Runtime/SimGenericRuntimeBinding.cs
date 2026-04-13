using System;
using R3;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    public class SimGenericRuntimeBinding : IDisposable
    {
        private readonly IDisposable _subscription;
        
        public SimGenericRuntimeBinding(Observable<object> propertyChange, Component target, Action<object, object> setter, SimPropertyPath propertyPath)
        {
            _subscription = propertyChange
                .Subscribe(value =>
                {
                    setter(target, value);
                    SimGenericBindingPostApply.PostApply(target, propertyPath);
                });
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}