using System.Collections.Generic;
using UnityEditor.UIElements;

namespace Sim.Faciem.uGUI.Editor.Controls.ConverterControl
{
    public class ConvertListUxmlConverter : UxmlAttributeConverter<List<SimConverterBaseBehaviour>>
    {
        public override List<SimConverterBaseBehaviour> FromString(string value)
        {
            return new List<SimConverterBaseBehaviour>();
        }
    }
}