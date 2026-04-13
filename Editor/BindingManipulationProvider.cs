using R3;

namespace Sim.Faciem.uGUI.Editor
{
    public class BindingManipulationProvider : IBindingManipulationProvider
    {
        public ReactiveProperty<IBindableProperty> BindableProperty { get; } = new();
    }
}