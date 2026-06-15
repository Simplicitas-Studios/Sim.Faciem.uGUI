using UnityEditor.Search;
using UnityEngine;

namespace Sim.Faciem.uGUI.Editor.Search
{
    static class SimConverterSearchIndex
    {
        const int version = 1;

        [CustomObjectIndexer(typeof(SimConverterBase), version = version)]
        public static void IndexConverters(CustomObjectIndexerTarget context, ObjectIndexer indexer)
        {
            if (context.target is not SimConverterBase)
                return;

            #if UNITY_6000_3_OR_NEWER
            
            indexer.IndexProperty<string, SimConverterBase>(context.documentIndex, "converter_base", nameof(SimConverterBase), false);

            #elif UNITY_6000_0_OR_NEWER
            
            indexer.IndexProperty<string, SimConverterBase>(context.documentIndex, "converter_base", nameof(SimConverterBase), false, true);
            
            #endif
        }
    }
}
