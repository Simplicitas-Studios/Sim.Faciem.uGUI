using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sim.Faciem.uGUI
{
    [Serializable]
    public struct SimBindingInfo : IEquatable<SimBindingInfo>
    {
        [SerializeField]
        private string _propertyPath;

        public BindingType BindingType;
        
        public SimDataSourceMonoBehaviour DataSource;

        public List<SimConverterBaseBehaviour> Converters;
        
        public SimPropertyPath PropertyPath
        {
            get => new(_propertyPath);
            set => _propertyPath = value.ToString();
        }

        public bool IsDefault => string.IsNullOrEmpty(_propertyPath) && DataSource == null;

        public bool Equals(SimBindingInfo other)
        {
            return _propertyPath == other._propertyPath && Equals(DataSource, other.DataSource)
                && Converters.SequenceEqual(other.Converters);
        }

        public override bool Equals(object obj)
        {
            return obj is SimBindingInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_propertyPath, DataSource, Converters);
        }
    }
}