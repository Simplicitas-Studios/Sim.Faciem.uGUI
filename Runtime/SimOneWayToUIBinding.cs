using R3;

namespace Sim.Faciem.uGUI
{
    internal class SimOneWayToUIBinding<T> : SimDataBinding<T>
    {
        public SimOneWayToUIBinding(Observable<T> observable)
        {
            Disposables.Add(observable
                .Subscribe(value => Value.Value = value));
        }
    }
}