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

            if (GUILayout.Button("GenAndDrawRooms"))
            {
                ((BSPGenerator)target).GenAndDrawRooms();
            }
        }

    }
}