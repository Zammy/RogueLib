using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Gen"))
        {
            ((LevelGenerator)target).Go();
        }
    }

}
