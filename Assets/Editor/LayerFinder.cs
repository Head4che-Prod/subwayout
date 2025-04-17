using UnityEditor;
using UnityEngine;

namespace Editor
{
    
    /// <summary>
    /// Adds a tool to search for all objects with a paricular layer.
    /// </summary>
    public class LayerFinder : EditorWindow
    {
        private int _layerToSearch = 0;

        [MenuItem("Tools/Find Objects by Layer")]
        public static void ShowWindow()
        {
            GetWindow<LayerFinder>("Find by Layer");
        }

        void OnGUI()
        {
            _layerToSearch = EditorGUILayout.LayerField("Layer", _layerToSearch);

            if (GUILayout.Button("Find"))
            {
                GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

                foreach (GameObject go in allObjects)
                {
                    if (go.layer == _layerToSearch)
                    {
                        Debug.Log("Found: " + go.name, go);
                    }
                }
            }
        }
    }
}