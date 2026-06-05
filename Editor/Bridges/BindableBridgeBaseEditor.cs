using Sim.Faciem.uGUI.Bridges;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor.Bridges
{
    [CustomEditor(typeof(BindingBridgeBase), true)]
    public class BindableBridgeBaseEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            if(serializedObject.targetObject is not BindingBridgeBase bridge)
            {
                return root;
            }

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            root.schedule
                .Execute(() =>
                {
                    for (int index = 0; index < bridge.ActivatedProperties.Count; index++)
                    {
                        bool bridgeActivatedProperty = bridge.ActivatedProperties[index];
                        if (root.childCount > index + 1)
                        {
                            var item = root[index + 2];
                            item.style.display =
                                bridgeActivatedProperty ? DisplayStyle.Flex : DisplayStyle.None;
                        }
                    }
                })
                .Every(500);

            return root;
        }
    }
}
