using System;
using System.Linq;
using R3;
using Sim.Faciem.uGUI.Editor.Internal;
using UnityEditor.IMGUI.Controls;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Sim.Faciem.uGUI.Editor.Controls
{
    internal sealed class PropertyPathTreeView : TreeView
    {
        private readonly Type _rootType;
        private readonly Type _expectedValueType;
        private readonly Action<string, Type> _onSelected;

        public PropertyPathTreeView(
            TreeViewState state,
            Type rootType,
            Type expectedValueType,
            Action<string, Type> onSelected)
            : base(state)
        {
            _rootType = rootType;
            _expectedValueType = expectedValueType;
            _onSelected = onSelected;

            Reload();
        }
        

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            return 30;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item;

            // Use the full row rect
            Rect rowRect = args.rowRect;

            // Optional: indent manually if you want tree structure
            rowRect.x += 6;

            // Create a centered style based on EditorStyles.label
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft
            };

            // Draw the label
            EditorGUI.LabelField(rowRect, item.displayName, centeredStyle);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1 };
            int id = 1;
            
            BuildProperties(
                root,
                ref id,
                _rootType,
                new SimPropertyPath(new PropertyPath()));

            if (!root.hasChildren)
            {
                root.AddChild(new  TreeViewItem { id = 0, depth = -1 , displayName = "No compatible Properties available!"});
            }
            return root;
        }

        private void BuildProperties(
            TreeViewItem parent,
            ref int id,
            Type type,
            SimPropertyPath path)
        {
            foreach (var property in PropertyContainerCompat.GetProperties(type))
            {
                var childPath = SimPropertyPath.AppendPath(path, property.Name);
                var valueType = property.DeclaredValueType();

                var unpackedType = TryUnpackReactiveTypes(valueType, out var wasReactiveType);

                if (wasReactiveType)
                {
                    childPath = SimPropertyPath.AppendSubscription(childPath);
                }

                if (CheckCompatibility(unpackedType))
                {
                    var item = new PropertyPathTreeViewItem(
                        id++,
                        parent.depth + 1,
                        childPath.ToString(),
                        childPath.ToString(),
                        unpackedType);
                    
                    parent.AddChild(item);   
                }
                
                if (PropertyContainerCompat.HasProperties(unpackedType))
                {
                    BuildProperties(parent, ref id, unpackedType, childPath);
                }
            }
        }

        private Type TryUnpackReactiveTypes(Type valueType, out bool didUnpack)
        {
            didUnpack = false;
            if(valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(Observable<>)
                                           || valueType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>)
                                           || valueType.GetGenericTypeDefinition() == typeof(ReadOnlyReactiveProperty<>)))
            {
                var observableType = valueType.GetGenericArguments().First();
                didUnpack = true;
                return observableType;
            }

            return valueType;
        }
        
        private bool CheckCompatibility(Type valueType)
        {
            return _expectedValueType == null || _expectedValueType.IsAssignableFrom(valueType);
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            if (item is PropertyPathTreeViewItem pathItem)
            {
                _onSelected?.Invoke(pathItem.PropertyPath, pathItem.PropertyType);
            }
        }
    }

}