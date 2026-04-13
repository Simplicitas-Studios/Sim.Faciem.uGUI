using System.Collections.Generic;
using Unity.Properties;

namespace Sim.Faciem.uGUI.Editor.Controls
{
    public class PropertyPathVisitor : IPropertyBagVisitor, ITypeVisitor
    {
        public List<IProperty> Properties { private get; set; }
        
        public void Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
        {
            foreach (var property in properties.GetProperties())
            {
                Properties.Add(property);
            }
        }

        public void Visit<TContainer>()
        {
            var bag = PropertyBag.GetPropertyBag<TContainer>();

            foreach (var property in bag.GetProperties())
            {
                Properties.Add(property);
            }
        }
    }
}