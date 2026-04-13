using Plugins.Sim.Faciem.Editor.DI;

namespace Sim.Faciem.uGUI.Editor
{
    public class FaciemUGUIEditorInstaller : EditorServiceInstaller
    {
        public override void Install(IEditorInjector injector)
        {
            injector.Register<IBindingManipulationProvider, BindingManipulationProvider>();   
        }
    }
}