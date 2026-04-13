
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace Sim.Faciem.uGUI.Editor.Controls
{
    public sealed class PropertyPathPickerPopup : PopupWindowContent
    {
        private readonly Vector2 _windowSize;
        private readonly Type _rootType;
        private readonly Type _expectedValueType;
        private readonly Action<string, Type> _onSelected;

        private TreeViewState _treeState;
        private PropertyPathTreeView _treeView;

        public PropertyPathPickerPopup(
            Vector2 windowSize,
            Type rootType,
            Type expectedValueType,
            Action<string, Type> onSelected)
        {
            _windowSize = windowSize;
            _rootType = rootType;
            _expectedValueType = expectedValueType;
            _onSelected = onSelected;
        }

        public override Vector2 GetWindowSize()
            => _windowSize;

        public override void OnOpen()
        {
            _treeState ??= new TreeViewState();
            _treeView = new PropertyPathTreeView(
                _treeState,
                _rootType,
                _expectedValueType,
                (path, type) =>
                {
                    _onSelected(path, type);
                    editorWindow.Close();
                    GUIUtility.ExitGUI();
                });
        }

        public override void OnGUI(Rect rect)
        {
            _treeView.OnGUI(rect);
        }
    }

}