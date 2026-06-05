using System;
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

            root.schedule.Execute(() =>
                {
                    try
                    {
                        bindingIcon.style.display = property?.boxedValue is IBindableProperty
                        {
                            BindingInfo: { IsDefault: true }
                        }
                            ? DisplayStyle.None
                            : DisplayStyle.Flex;
                    }
                    catch (Exception)
                    {
                        // Swallow
                    }
                })
                .Every(200);

            if (valueProperty.hasChildren)
            {
                var valueField = new PropertyField(valueProperty)
                {
                    label = property.displayName
                };

                root.Add(valueField);
            }
            else
            {
                root.Add(new Label(property.displayName));

                root.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    evt.menu.ClearItems(); // removes default items

                    evt.menu.AppendAction(
                        "Edit Binding",
                        _ =>
                        {
                            BindablePropertyContextMenu.EditBinding(property.boxedValue as IBindableProperty, property);
                        }
                    );

                    // TODO allow deleting binding
                    // evt.menu.AppendAction(
                    //     "Delete Binding",
                    //     _ =>
                    //     {
                    //         var currentIndex = autoBindingComponent.Bindings.IndexOf(binding);
                    //         bindingListProperty.DeleteArrayElementAtIndex(currentIndex);
                    //         serializedObject.ApplyModifiedProperties();
                    //         DrawBindings(autoBindingComponent, bindingListProperty, root);
                    //     }
                    // );
                }));
            }

            var path = new PropertyPath(property.propertyPath);
            s_DebugPropertyVisitor.Path = path;
            var target = property.serializedObject.targetObject;

            if (PropertyContainer.TryAccept(s_DebugPropertyVisitor, ref target))
            {
                if (s_DebugPropertyVisitor.Value is IRuntimeBindableProperty runtimeBindableProperty && runtimeBindableProperty.BindingInfo.DataSource != null)
                {
                    // valueField.SetEnabled(false);

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
