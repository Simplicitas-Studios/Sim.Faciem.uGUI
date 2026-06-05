using System.Collections.Generic;
using UnityEngine;

namespace Sim.Faciem.uGUI.Bridges
{
    public abstract class BindingBridgeBase : MonoBehaviour
    {
        [SerializeField]
        internal Component AttachedComponent;

        [SerializeField]
        [HideInInspector]
        internal List<bool> ActivatedProperties;

        internal Component InternalComponent
        {
            get => AttachedComponent;
            set => AttachedComponent = value;
        }

        public abstract IEnumerable<IBindableProperty> CollectBindableProperties();

        public abstract bool TryGetBindableProperty(string componentPropertyName, out IBindableProperty bindableProperty);
    }
}
