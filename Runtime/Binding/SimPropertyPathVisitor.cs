using System;
using Unity.Properties;

namespace Sim.Faciem.uGUI.Binding
{
    public class SimPropertyPathVisitor : PathVisitor
    {
        public Func<object, object> Getter { get; private set; }
        
        public Action<object, object> Setter { get; private set; }
        
        protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
        {
            Getter = source =>
            {
                var typed = (TContainer)source;
                return property.GetValue(ref typed);
            };

            Setter = (source, value) =>
            {
                var typed = (TContainer)source;

                var typedValue = value == null
                    ? default
                    : (TValue)value;
                
                property.SetValue(ref typed, typedValue);
            };
        }
    }
}