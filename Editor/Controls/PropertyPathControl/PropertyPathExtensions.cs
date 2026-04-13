using Unity.Properties;

namespace Sim.Faciem.uGUI.Editor.Controls
{
    internal static class PropertyPathExtensions
    {
        public static PropertyPath Append(this PropertyPath path, string propertyName)
        {
            return path.IsEmpty ? new PropertyPath(propertyName) : new PropertyPath($"{path}.{propertyName}");
        }
    }
}