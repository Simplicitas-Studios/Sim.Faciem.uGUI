using System;
using Plugins.Sim.Faciem.Shared;
using R3;
using Sim.Faciem.uGUI.Binding;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    [Serializable]
    public class BindableProperty<T> : IRuntimeBindableProperty, IDisposable
    {
        private DisposableBagHolder _disposables = new();
        internal SimDataBinding<T> RuntimeBinding;

        [SerializeField]
        private SerializableReactiveProperty<T> _value;

        public SimBindingInfo BindingInfo;
        
        public T Value { get =>  _value.Value; set => _value.Value = value; }
        
        public void CreateBinding()
        {
            if (RuntimeBinding == null)
            {
                RuntimeBinding = SimBindingFactory.CreateBinding<T>(BindingInfo);
                _disposables.Add(
                    RuntimeBinding.Value
                        .Subscribe(next => _value.Value = next));
            }
        }

        public Observable<T> ObserveChanges() => _value; 

        Type IBindableProperty.BoundType => typeof(T);

        SimBindingInfo IBindableProperty.BindingInfo
        {
            get => BindingInfo;
            set => BindingInfo = value;
        }

        ISimDataBindingInfo IRuntimeBindableProperty.RuntimeBindingInfo => RuntimeBinding;

        public void Dispose()
        {
            RuntimeBinding?.Dispose();
        }
    }
}