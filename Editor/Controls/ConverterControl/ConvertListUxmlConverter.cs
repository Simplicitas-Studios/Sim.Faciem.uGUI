using System.Collections.Generic;
using UnityEditor.UIElements;

namespace Sim.Faciem.uGUI.Editor.Controls.ConverterControl
{
    public class ConvertListUxmlConverter : UxmlAttributeConverter<List<SimConverterBase>>
    {
        public override List<SimConverterBase> FromString(string value)
        {
            return new List<SimConverterBase>();
        }
    }
}
