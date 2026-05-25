namespace Sim.Faciem.uGUI
{
    public abstract class SimConverter<TFrom, TTo> : SimConverterBase
    {
        internal sealed override object Convert(object obj)
        {
            return Convert((TFrom)obj);
        }

        public abstract TTo Convert(TFrom from);
    }
}
