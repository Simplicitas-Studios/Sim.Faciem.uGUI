using System;
using Sim.Faciem.uGUI.Binding;

namespace Sim.Faciem.uGUI
{
    [Serializable]
    public class GenericBindableProperty : IBindableProperty, IDisposable
    {
        public SimComponentPropertyPath Target;
        
        public SimBindingInfo BindingInfo;

        internal SimGenericRuntimeBinding RuntimeBinding;
        
        public Type BoundType => Type.GetType(Target.Type);

        SimBindingInfo IBindableProperty.BindingInfo
        {
            get => BindingInfo;
            set => BindingInfo = value;
        }
        
        public void CreateBinding()
        {
            RuntimeBinding ??= SimBindingFactory.CreateGenericBinding(this);
        }

        public void Dispose()
        {
            RuntimeBinding?.Dispose();
        }
    }
}