using System;
using UnityEditor.IMGUI.Controls;

namespace Sim.Faciem.uGUI.Editor.Controls
{
    internal sealed class PropertyPathTreeViewItem : TreeViewItem
    {
        public string PropertyPath { get; }
        
        public Type PropertyType { get; }

        public PropertyPathTreeViewItem(int id, int depth, string displayName, string propertyPath, Type propertyType)
            : base(id, depth, displayName)
        {
            PropertyPath = propertyPath;
            PropertyType = propertyType;
        }
    }
}