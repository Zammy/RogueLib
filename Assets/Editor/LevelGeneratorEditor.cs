using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Step"))
        {
            bool done = ((LevelGenerator)target).Step();
            if (done)
            {
                EditorUtility.DisplayDialog("Gen", "Generated all rooms!", "ok");
            }
        }

        if (GUILayout.Button("GenAndDrawAll"))
        {
            ((LevelGenerator)target).GenerateAndDrawWholeDungeon();
        }
    }

}
