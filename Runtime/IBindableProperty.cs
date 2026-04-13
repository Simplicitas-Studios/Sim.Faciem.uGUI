using System;

namespace Sim.Faciem.uGUI
{
    public interface IBindableProperty
    {
        Type BoundType { get; }
        
        SimBindingInfo BindingInfo { get; set;  }
    }
}