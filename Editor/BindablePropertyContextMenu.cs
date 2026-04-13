using System.Linq;
using Bebop.Monads;
using Plugins.Sim.Faciem.Editor.DI;
using R3;
using Sim.Faciem.uGUI.Editor.Internal;
using UnityEditor;
using UnityEngine;
using Unit = R3.Unit;

namespace Sim.Faciem.uGUI.Editor
{
    public static class BindablePropertyContextMenu
    {
        private static readonly Subject<Unit> s_editDoneSubject = new();
        private static IBindingManipulationProvider s_bindingManipulationProvider;
        private static SerializedProperty s_lastProperty;
        private static IMaybe<SimComponentPropertyPath> s_lastPropertyTarget;
        private static bool s_ignoreSelf;

        public static Observable<Unit> EditDone => s_editDoneSubject;
        
        [InitializeOnLoadMethod]
        public static void Init()
        {
            EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
            s_bindingManipulationProvider = EditorInjector.Instance.ResolveInstance<IBindingManipulationProvider>();

            s_bindingManipulationProvider.BindableProperty
                .Where(_ => s_lastProperty != null && !s_lastPropertyTarget.HasValue && !s_ignoreSelf)
                .Subscribe(propertyChanged =>
                {
                    var bindingInfo = s_lastProperty.FindPropertyRelative(nameof(IBindableProperty.BindingInfo));
                    bindingInfo.boxedValue = propertyChanged.BindingInfo;
                    bindingInfo.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(bindingInfo.serializedObject.targetObject);
                    EditorWindow.focusedWindow?.Repaint();
                    s_editDoneSubject.OnNext(Unit.Default);
                });

            s_bindingManipulationProvider.BindableProperty
                .Where(_ => s_lastProperty != null && s_lastPropertyTarget.HasValue && !s_ignoreSelf)
                .Subscribe(propertyChanged =>
                {
                    if (s_lastProperty.serializedObject.targetObject is not Component component)
                    {
                        return;
                    }
                    
                    if (!component.gameObject.TryGetComponent<SimAutoBindingComponent>(out var bindingComponent))
                    {
                        bindingComponent = component.gameObject.AddComponent<SimAutoBindingComponent>();
                    }

                    if (s_lastProperty.boxedValue is GenericBindableProperty genericBindable)
                    {
                        genericBindable.BindingInfo = propertyChanged.BindingInfo;
                        EditorUtility.SetDirty(s_lastProperty.serializedObject.targetObject);
                    }
                    else
                    {
                        var binding = bindingComponent.Bindings
                            .FirstOrDefault(x => x.Target.Equals(s_lastPropertyTarget.Value)) ?? new GenericBindableProperty 
                        { 
                            Target = s_lastPropertyTarget.Value
                        };

                        binding.BindingInfo = propertyChanged.BindingInfo;
                        bindingComponent.Bindings.Add(binding);
                    }
                    
                    s_editDoneSubject.OnNext(Unit.Default);
                    EditorUtility.SetDirty(bindingComponent);
                });
        }

        private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
        {
            s_lastPropertyTarget = Maybe.Nothing<SimComponentPropertyPath>();
            if (property.name == "value")
            {
                // The serialized field is the value from SerializableReactiveProperty, we have to move up two parents
                // to check if the parent is of type BindableProperty
                var parentProperty = property.FindParentProperty()?.FindParentProperty();
                if (parentProperty != null && parentProperty.type == typeof(BindableProperty<>).Name
                    && parentProperty.boxedValue is IBindableProperty bindableProperty)
                {
                    var manipulationItemName = bindableProperty.BindingInfo.IsDefault
                        ? "Add Binding"
                        : "Edit Binding";
                    
                    menu.AddItem(new GUIContent(manipulationItemName), false, () =>
                    {
                        s_lastProperty = parentProperty;
                        s_ignoreSelf = true;
                        s_bindingManipulationProvider.BindableProperty.Value = bindableProperty;
                        s_bindingManipulationProvider.BindableProperty.ForceNotify();
                        s_ignoreSelf = false;
                        var bindingWindow = EditorWindow.GetWindow<BindingWindow.BindingWindow>();
                        bindingWindow.Show();
                    });

                    if (!bindableProperty.BindingInfo.IsDefault)
                    {
                        menu.AddItem(new GUIContent("Remove Binding"), false, () =>
                        {
                            var bindingInfo = parentProperty.FindPropertyRelative(nameof(IBindableProperty.BindingInfo));
                            bindingInfo.boxedValue = default(SimBindingInfo);
                            bindingInfo.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(bindingInfo.serializedObject.targetObject);
                            EditorWindow.focusedWindow?.Repaint();
                        });
                    }
                }
            }

            var targetObject = property.serializedObject.targetObject;
            var availableProperties = PropertyContainerCompat.GetAllPropertiesRecursive(targetObject.GetType())
                .ToList();
            var compatibleProperty =
                availableProperties.FirstOrDefault(x => x.Item1.Path.ToString().Equals(property.propertyPath));

            if (compatibleProperty.Item2 != null && targetObject is Component component)
            {
                menu.AddItem(new GUIContent("Add Binding"), false, () =>
                {
                    s_lastProperty = property;
                    var genericBindableProperty = new GenericBindableProperty
                    {
                        Target = new SimComponentPropertyPath
                        {
                            Component = component,
                            PropertyPath = compatibleProperty.Item1,
                            Type = compatibleProperty.Item2.AssemblyQualifiedName
                        }
                    };

                    s_lastPropertyTarget = Maybe.From(genericBindableProperty.Target);
                    s_ignoreSelf = true;
                    s_bindingManipulationProvider.BindableProperty.Value = genericBindableProperty;
                    s_bindingManipulationProvider.BindableProperty.ForceNotify();
                    s_ignoreSelf = false;
                    var bindingWindow = EditorWindow.GetWindow<BindingWindow.BindingWindow>();
                    bindingWindow.Show();
                });
            }
        }

        public static void EditBinding(GenericBindableProperty bindableProperty, SerializedProperty property)
        {
            s_lastProperty = property;
            s_lastPropertyTarget = Maybe.From(bindableProperty.Target);
            s_ignoreSelf = true;
            s_bindingManipulationProvider.BindableProperty.Value = bindableProperty;
            s_bindingManipulationProvider.BindableProperty.ForceNotify();
            s_ignoreSelf = false;
            var bindingWindow = EditorWindow.GetWindow<BindingWindow.BindingWindow>();
            bindingWindow.Show();
        }
    }
}