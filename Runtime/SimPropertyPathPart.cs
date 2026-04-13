using Unity.Properties;

namespace Sim.Faciem.uGUI
{
    public readonly struct SimPropertyPathPart
    {
        public PropertyPath Path { get; }
        
        public SimPropertyPathPartKind Kind { get; }

        public SimPropertyPathPart(PropertyPath path, SimPropertyPathPartKind kind)
        {
            Path = path;
            Kind = kind;
        }
    }
}