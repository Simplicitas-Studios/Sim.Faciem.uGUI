using System;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    [Serializable]
    public struct SimComponentPropertyPath : IEquatable<SimComponentPropertyPath>
    {
        public Component Component;

        public string Path;
        
        public string Type;

        public SimPropertyPath PropertyPath
        {
            get => new(Path);
            set => Path = value.Path.ToString();
        }
        
        public bool Equals(SimComponentPropertyPath other)
        {
            return Equals(Component, other.Component) && PropertyPath.Equals(other.PropertyPath) && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is SimComponentPropertyPath other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Component, PropertyPath, Type);
        }
    }
}