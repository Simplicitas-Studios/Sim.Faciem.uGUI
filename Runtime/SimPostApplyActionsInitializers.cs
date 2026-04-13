

using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif

namespace Sim.Faciem.uGUI
{
    public class SimPostApplyActionsInitializers
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void Initialize()
        {
            SimGenericBindingPostApply.RegisterPostApplyAction<TextMeshProUGUI>(((o, path) =>
            {
                if (o is TextMeshProUGUI tmp)
                {
                    tmp.SetText(tmp.text);
                }
            }));
            
            SimGenericBindingPostApply.RegisterPostApplyAction<TextMeshPro>(((o, path) =>
            {
                if (o is TextMeshPro tmp)
                {
                    tmp.SetText(tmp.text);
                }
            }));
        }
    }
}