using UnityEngine;

namespace Sim.Faciem.uGUI.Bridges
{
    public abstract class BindingBridge<TComponent> : BindingBridgeBase where TComponent : Component
    {
        protected TComponent Component => (TComponent)AttachedComponent;
    }
}
