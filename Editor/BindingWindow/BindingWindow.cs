using Cysharp.Threading.Tasks;
using Plugins.Sim.Faciem.Editor;
using UnityEngine;

namespace Sim.Faciem.uGUI.Editor.BindingWindow
{
    public class BindingWindow : FaciemEditorWindow
    {
        public BindingWindow()
        {
            titleContent = new GUIContent("Binding Setup");
        }

        protected override async UniTask NavigateTo()
        {
            await Navigation.Navigate(WellKnownEditorViewIds.Sim_Faciem_uGUI_BindingWindow, WindowRegionName);
        }

        protected override async UniTask NavigateAway()
        {
            await Navigation.Clear(WindowRegionName);
        }
    }
}