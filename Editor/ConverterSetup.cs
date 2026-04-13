using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor
{
    public static class ConverterSetup
    {
        [InitializeOnLoadMethod]
        public static void Setup()
        {
            ConverterGroups.RegisterGlobalConverter((ref Object value) => value as SimDataSourceMonoBehaviour);
        }
    }
}