using R3;

namespace Sim.Faciem.uGUI
{
    internal interface ISimDataBindingInfo
    {
        Observable<string> TextualValue { get; }
    }
}