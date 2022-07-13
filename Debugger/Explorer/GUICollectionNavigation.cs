using ModTools.UI;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUICollectionNavigation
    {
        public static void SetUpCollectionNavigation(
            string collectionLabel,
            SceneExplorerState state,
            ReferenceChain refChain,
            ReferenceChain oldRefChain,
            int collectionSize,
            out uint startIndex,
            out uint endIndex)
        {
            GUILayout.BeginHorizontal();
            SceneExplorerCommon.InsertIndent(refChain.Indentation);

            GUILayout.Label($"{collectionLabel} size: {collectionSize}");

            if (!state.SelectedArrayStartIndices.TryGetValue(refChain.UniqueId, out startIndex))
            {
                state.SelectedArrayStartIndices.Add(refChain.UniqueId, 0);
            }

            if (!state.SelectedArrayEndIndices.TryGetValue(refChain.UniqueId, out endIndex))
            {
                state.SelectedArrayEndIndices.Add(refChain.UniqueId, 32);
                endIndex = 32;
            }

            startIndex = GUIControls.NumericValueField($"{oldRefChain}.arrayStart", "Start index", startIndex);
            endIndex = GUIControls.NumericValueField($"{oldRefChain}.arrayEnd", "End index", endIndex);
            GUILayout.Label("(32 items max)");
            var pageSize = (uint)Mathf.Clamp(endIndex - startIndex + 1, 1, Mathf.Min(32, collectionSize - startIndex, endIndex + 1));
            if (GUILayout.Button("◄", GUILayout.ExpandWidth(false)))
            {
                startIndex -= pageSize;
                endIndex -= pageSize;
            }

            if (GUILayout.Button("►", GUILayout.ExpandWidth(false)))
            {
                startIndex += pageSize;
                endIndex += pageSize;
            }

            startIndex = (uint)Mathf.Clamp(startIndex, 0, collectionSize - pageSize);
            endIndex = (uint)Mathf.Max(0, Mathf.Clamp(endIndex, pageSize - 1, collectionSize - 1));
            if (startIndex > endIndex)
            {
                endIndex = startIndex;
            }

            if (endIndex - startIndex > 32)
            {
                endIndex = startIndex + 32;
                endIndex = (uint)Mathf.Max(0, Mathf.Clamp(endIndex, 32, collectionSize - 1));
            }

            state.SelectedArrayStartIndices[refChain.UniqueId] = startIndex;
            state.SelectedArrayEndIndices[refChain.UniqueId] = endIndex;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}