using System.Collections.Generic;
using Sim.Faciem.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor
{
    public static class BindablePropertyElement
    {
        public static readonly Color BindingAccentColor = new(87/255f, 133/255f, 217/255f, 1);
        public static readonly Color BackgroundColor = new(80 / 255f, 80 / 255f, 80 / 255f, 1);

        public static VisualElement CreateElement(IBindableProperty bindableProperty)
        {
            var runtimeInfoContainer = new VisualElement
            {
                style =
                {
                    fontSize = 13,
                    marginTop = 4,
                    paddingBottom = 8,
                    paddingTop = 8,
                    paddingLeft = 8,
                    paddingRight = 8,
                    backgroundColor = BackgroundColor,
                    borderBottomColor = BindingAccentColor,
                    borderTopColor = BindingAccentColor,
                    borderLeftColor = BindingAccentColor,
                    borderRightColor = BindingAccentColor,
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
            row1Container.Add(new Label("Bound to: ")
            {
                style =
                {
                    minWidth = 100
                }
            });
            var dataSourceLink = new HyperLinkLabel
            {
                LinkIds = new List<string> { "1" },
                InstanceIds = new List<int> { bindableProperty.BindingInfo.DataSource.GetInstanceID() },
                focusable = true,
                LinkHoverColor = BindingAccentColor,
                LinkColor = Color.white,
                pickingMode = PickingMode.Position,
                style =
                {
                    flexGrow = 1
                },
                text = $"<link=\"1\">{bindableProperty.BindingInfo.DataSource.name} - {bindableProperty.BindingInfo.DataSource.GetType().Name}<link>"
            };
            row1Container.Add(dataSourceLink);

            runtimeInfoContainer.Add(row1Container);

            var row2Container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            row2Container.Add(new Label("Path: ")
            {
                style =
                {
                    minWidth = 100
                }
            });
            row2Container.Add(new Label(bindableProperty.BindingInfo.PropertyPath.ToString())
            {
                style =
                {
                    flexGrow = 1
                }
            });
            runtimeInfoContainer.Add(row2Container);

            return runtimeInfoContainer;
        }
    }
}