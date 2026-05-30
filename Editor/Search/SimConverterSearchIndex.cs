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

            indexer.IndexProperty<string, Shader>(context.documentIndex, "converter_base", nameof(SimConverterBase), false);

        }
    }
}
