using System;
using Plugins.Sim.Faciem.Shared;
using R3;

namespace Sim.Faciem.uGUI
{
    public abstract class SimDataBinding<T> : IDisposable, ISimDataBindingInfo
    {
        protected DisposableBagHolder Disposables { get; }
        
        public ReactiveProperty<T> Value { get; }

        public Observable<string> TextualValue => Value.Select(currentValue => currentValue.ToString());
        
        public SimDataBinding()
        {
            Disposables = new DisposableBagHolder();
            Value = new ReactiveProperty<T>();
            Disposables.Add(Value);
        }

        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}