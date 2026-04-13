using System.Collections.Generic;
using Plugins.Sim.Faciem.Shared;
using Sim.Faciem.Controls;
using Sim.Faciem.uGUI.Editor.Internal;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor
{
    [CustomPropertyDrawer(typeof(BindableProperty<>))]
    public class BindablePropertyPropertyDrawer : PropertyDrawer
    {
        private readonly Color _bindingAccentColor = new(87/255f, 133/255f, 217/255f, 1);
        private static DebugPropertyVisitor s_DebugPropertyVisitor = new();
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = property.FindPropertyRelative("_value");
            
            var root = new VisualElement();

            var valueField = new PropertyField(valueProperty)
            {
                label = property.displayName
            };

            var bindingIcon = new VisualElement
            {
                style =
                {
                    backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("Binding").image as Texture2D),
                    position = Position.Absolute,
                    left = -12,
                    top = 2,
                    width = 16,
                    height = 16,
                    unityBackgroundImageTintColor = _bindingAccentColor,
                    display = DisplayStyle.None
                }
            };
            root.Add(bindingIcon);
            
            root.schedule.Execute(() => bindingIcon.style.display = property.boxedValue is IBindableProperty
                {
                    BindingInfo: { IsDefault: true }
                }
                    ? DisplayStyle.None
                    : DisplayStyle.Flex)
                .Every(200);

            root.Add(valueField);
            
            var path = new PropertyPath(property.propertyPath);
            s_DebugPropertyVisitor.Path = path;
            var target = property.serializedObject.targetObject;
            if (PropertyContainer.TryAccept(s_DebugPropertyVisitor, ref target))
            {
                if (s_DebugPropertyVisitor.Value is IRuntimeBindableProperty runtimeBindableProperty)
                {
                    valueField.SetEnabled(false);

                    var bindablePropertyElement = BindablePropertyElement.CreateElement(runtimeBindableProperty);
                    root.Add(bindablePropertyElement);
                }   
            }
            s_DebugPropertyVisitor.Reset();
            
            return root;
        }
        
        private void AddBinding(SerializedProperty property)
        {
            Debug.Log($"Add binding for {property.propertyPath}");
            // Open your binding popup here
        }

        private void ClearBinding(SerializedProperty property)
        {
            Debug.Log($"Clear binding for {property.propertyPath}");
            // Remove serialized binding info here
        }
    }
}