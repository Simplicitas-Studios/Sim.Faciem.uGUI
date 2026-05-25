using UnityEngine;

namespace Sim.Faciem.uGUI
{
    public abstract class SimConverterBase : ScriptableObject
    {
        internal abstract object Convert(object obj);
    }
}
