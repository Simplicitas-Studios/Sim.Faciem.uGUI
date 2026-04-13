namespace Sim.Faciem.uGUI
{
    internal interface IRuntimeBindableProperty : IBindableProperty
    {
        ISimDataBindingInfo RuntimeBindingInfo { get; }
    }
}