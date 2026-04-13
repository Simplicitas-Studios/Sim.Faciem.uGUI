using UnityEngine;

namespace Sim.Faciem.uGUI
{
    public abstract class SimConverterBaseBehaviour : MonoBehaviour
    {
        internal abstract object Convert(object obj);
    }
}