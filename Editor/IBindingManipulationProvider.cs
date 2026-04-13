using R3;

namespace Sim.Faciem.uGUI.Editor
{
    public interface IBindingManipulationProvider
    {
        ReactiveProperty<IBindableProperty> BindableProperty { get; }
    }
}