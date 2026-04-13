namespace Sim.Faciem.uGUI
{
    public abstract class SimConverterBehaviour<TFrom, TTo> : SimConverterBaseBehaviour
    {
        internal sealed override object Convert(object obj)
        {
            return Convert((TFrom)obj);
        }

        public abstract TTo Convert(TFrom from);
    }
}