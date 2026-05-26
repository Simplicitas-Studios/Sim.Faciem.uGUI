using UnityEngine;

namespace Sim.Faciem.uGUI.Internal
{
    internal static class TransformExtensions
    {
        public static string GetTransformPath(this Transform transform)
        {
            var path = transform.name;

            while (transform.parent != null)
            {
                path = $"{transform.parent.name}/{path}";
                transform = transform.parent;
            }

            return path;
        }
    }
}
