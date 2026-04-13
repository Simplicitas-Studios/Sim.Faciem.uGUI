using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    public class SimAutoBindingComponent : MonoBehaviour
    {
        public List<GenericBindableProperty> Bindings = new();

        private void Awake()
        {
            foreach (var genericBindableProperty in Bindings)
            {
                genericBindableProperty.CreateBinding();
                genericBindableProperty.AddTo(this);
            }
        }
    }
}