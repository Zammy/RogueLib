using UnityEngine;
using UnityEditor;

namespace BSP
{
    [CustomEditor(typeof(BSPGenerator))]
    public class BSPGeneratorEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

//            if (GUILayout.Button("Step"))
//            {
//                bool done = ((LevelGenerator)target).Step();
//                if (done)
//                {
//                    EditorUtility.DisplayDialog("Gen", "Generated all rooms!", "ok");
//                }
//            }

            if (GUILayout.Button("GenAndDrawAll"))
            {
                ((BSPGenerator)target).GenAndDraw();
            }
        }

    }
}