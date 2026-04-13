using System;
using System.Linq;
using Unity.Properties;

namespace Sim.Faciem.uGUI
{
    public readonly struct SimPropertyPath : IEquatable<SimPropertyPath>
    {
        private const string SubscriptionSymbol = "$";
        
        public PropertyPath Path { get; }
        
        public SimPropertyPath(string path) 
            : this(new PropertyPath(path))
        {
        }
        
        public SimPropertyPath(PropertyPath path)
        {
            Path = path;
        }

        public static SimPropertyPath AppendPath(SimPropertyPath path, string newPath)
        {
            return new SimPropertyPath(PropertyPath.Combine(path.Path, newPath));
        }

        public static SimPropertyPath AppendSubscription(SimPropertyPath path)
        {
            return new SimPropertyPath(PropertyPath.Combine(path.Path,SubscriptionSymbol));
        }
        
        public SimPropertyPathPart[] ExtractPaths()
        {
            var pathString = Path.ToString();

            var pathParts = pathString.Split(SubscriptionSymbol);

            return pathParts
                .Select(RemoveStartingDots)
                .Select((x, index) => new SimPropertyPathPart(new PropertyPath(x), index +1 == pathParts.Length 
                    ? SimPropertyPathPartKind.Property
                    : SimPropertyPathPartKind.Observable))
                .ToArray();
        }

        private static string RemoveStartingDots(string path)
        {
            return path.StartsWith(".") ? path[1..] : path;
        }

        public bool Equals(SimPropertyPath other)
        {
            return Path.Equals(other.Path);
        }

        public override string ToString()
        {
            return Path.ToString();
        }
    }
}