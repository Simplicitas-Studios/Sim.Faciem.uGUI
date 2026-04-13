using System.Linq;
using Plugins.Sim.Faciem.Shared;
using R3;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor
{
    [CustomEditor(typeof(SimAutoBindingComponent))]
    public class SimAutoBindingComponentEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var bindingListProperty = serializedObject.FindProperty(nameof(SimAutoBindingComponent.Bindings));
            
            if (serializedObject.targetObject is SimAutoBindingComponent autoBindingComponent)
            {
                DrawBindings(autoBindingComponent, bindingListProperty, root);

                var disposables = root.RegisterDisposableBag();
                
                disposables.Add(
                BindablePropertyContextMenu.EditDone
                    .Subscribe(_ =>
                    {
                        serializedObject.Update();
                        DrawBindings(autoBindingComponent, bindingListProperty, root);
                    }));
            }

            
            return root;
        }

        private void DrawBindings(
            SimAutoBindingComponent autoBindingComponent,
            SerializedProperty bindingListProperty,
            VisualElement root)
        {
            root.Clear();

            if (!autoBindingComponent.Bindings.Any())
            {
                root.Add(new Label("No Bindings configured!")
                {
                    style =
                    {
                        fontSize = 15,
                        marginTop = 4,
                        color = BindablePropertyElement.BindingAccentColor,
                        unityTextAlign = TextAnchor.MiddleCenter
                    }
                });
                return;
            }
            
            for (var index = 0; index < autoBindingComponent.Bindings.Count; index++)
            {
                var binding = autoBindingComponent.Bindings[index];
                var property = bindingListProperty.GetArrayElementAtIndex(index);

                var bindingInfoRoot = new VisualElement
                {
                    style =
                    {
                        alignItems = Align.Stretch,
                        borderBottomColor = BindablePropertyElement.BackgroundColor,
                        borderTopColor = BindablePropertyElement.BackgroundColor,
                        borderLeftColor = BindablePropertyElement.BackgroundColor,
                        borderRightColor = BindablePropertyElement.BackgroundColor,
                        borderBottomWidth = 2,
                        borderTopWidth = 2,
                        borderLeftWidth = 2,
                        borderRightWidth = 2,
                        borderBottomLeftRadius = 2,
                        borderTopLeftRadius = 2,
                        borderBottomRightRadius = 2,
                        borderTopRightRadius = 2,
                        paddingLeft = 8,
                        paddingRight = 8,
                        paddingBottom = 8,
                        paddingTop = 8,
                    }
                };

                var targetInfo = new VisualElement
                {
                    style =
                    {
                        fontSize = 13,
                        paddingBottom = 8,
                        paddingTop = 8,
                        paddingLeft = 8,
                        paddingRight = 8,
                        backgroundColor = BindablePropertyElement.BackgroundColor,
                        borderBottomColor = BindablePropertyElement.BindingAccentColor,
                        borderTopColor = BindablePropertyElement.BindingAccentColor,
                        borderLeftColor = BindablePropertyElement.BindingAccentColor,
                        borderRightColor = BindablePropertyElement.BindingAccentColor,
                        borderBottomWidth = 2,
                        borderTopWidth = 2,
                        borderLeftWidth = 2,
                        borderRightWidth = 2,
                        borderBottomLeftRadius = 2,
                        borderTopLeftRadius = 2,
                        borderBottomRightRadius = 2,
                        borderTopRightRadius = 2,
                    }
                };

                var row1Container = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        marginBottom = 4
                    }
                };
                row1Container.Add(new Label("Target:")
                {
                    style =
                    {
                        minWidth = 100
                    }
                });
                var boundLabel = new Label
                {
                    text = $"{binding.Target.Component.GetType()} - {binding.Target.Path}"
                };
                row1Container.Add(boundLabel);

                var row2Container = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        overflow = Overflow.Hidden,
                        marginBottom = 4
                    }
                };
                row2Container.Add(new Label("Type:")
                {
                    style =
                    {
                        minWidth = 100
                    }
                });
                var boundTypeLabel = new Label
                {
                    text = $"{binding.Target.Type}",
                    style =
                    {
                        textOverflow = TextOverflow.Ellipsis,
                    }
                };
                row2Container.Add(boundTypeLabel);

                targetInfo.Add(row1Container);
                targetInfo.Add(row2Container);
                bindingInfoRoot.Add(targetInfo);
                bindingInfoRoot.Add(new Label("↓")
                {
                    style =
                    {
                        fontSize = 18,
                        unityFontStyleAndWeight = FontStyle.Bold,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        marginTop = 4,
                        color = BindablePropertyElement.BindingAccentColor,
                    }
                });

                var boundInfos = BindablePropertyElement.CreateElement(binding);
                bindingInfoRoot.Add(boundInfos);

                bindingInfoRoot.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    evt.menu.ClearItems(); // removes default items

                    evt.menu.AppendAction(
                        "Edit Binding",
                        _ =>
                        {
                            BindablePropertyContextMenu.EditBinding(binding, property);
                        }
                    );

                    evt.menu.AppendAction(
                        "Delete Binding",
                        _ =>
                        {
                            var currentIndex = autoBindingComponent.Bindings.IndexOf(binding);
                            bindingListProperty.DeleteArrayElementAtIndex(currentIndex);
                            serializedObject.ApplyModifiedProperties();
                            DrawBindings(autoBindingComponent, bindingListProperty, root);
                        }
                    );
                }));

                root.Add(bindingInfoRoot);
            }
        }
    }
}