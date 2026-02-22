
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(StairsGenerator))]
    public class StairsGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var taget = (StairsGenerator)target;
            
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
                taget.Generate();
        }
    }