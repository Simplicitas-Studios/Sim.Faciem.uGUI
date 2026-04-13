using System;
using R3;

namespace Sim.Faciem.uGUI.Internal
{
    public static class ReactiveTypeUtils
    {
        public static bool IsObservableType(Type type)
        {
            var currentType = type;
            
            do
            {
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Observable<>))
                {
                    return true;
                }
                
                currentType = currentType.BaseType;
                
            } while (currentType != null);

            return false;
        }
    }
}