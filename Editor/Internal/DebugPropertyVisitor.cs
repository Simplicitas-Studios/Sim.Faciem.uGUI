using Unity.Properties;

namespace Sim.Faciem.uGUI.Editor.Internal
{
    public class DebugPropertyVisitor : PathVisitor
    {
        public object Value { get; private set; }
        
        protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
        {
            Value = value;
        }
    }
}