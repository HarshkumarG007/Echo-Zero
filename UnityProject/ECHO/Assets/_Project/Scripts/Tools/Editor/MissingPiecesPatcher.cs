using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EchoZero.Narrative;

namespace EchoZero.Tools.Editor
{
    public class MissingPiecesPatcher
    {
        [MenuItem("ECHO/Patch Missing Pieces")]
        public static void PatchAll()
        {
            // 1. UI Rendering (PanelSettings)
            var panelSettingsPath = "Assets/Settings/PanelSettings.asset";
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelSettingsPath);
            if (panelSettings == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                    AssetDatabase.CreateFolder("Assets", "Settings");

                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panelSettings, panelSettingsPath);
                Debug.Log("[MissingPiecesPatcher] Created PanelSettings.asset");
            }

            var uiDoc = GameObject.Find("[UI]")?.GetComponent<UIDocument>();
            if (uiDoc != null && uiDoc.panelSettings == null)
            {
                uiDoc.panelSettings = panelSettings;
                Debug.Log("[MissingPiecesPatcher] Assigned PanelSettings to [UI].");
            }

            // 2. Memory Fragment Data
            var fragPath = "Assets/_Project/Data/Narrative/Fragment_TheFirstMemory.asset";
            var fragSO = AssetDatabase.LoadAssetAtPath<MemoryFragmentSO>(fragPath);
            if (fragSO == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/Data/Narrative"))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/_Project/Data"))
                        AssetDatabase.CreateFolder("Assets/_Project", "Data");
                    AssetDatabase.CreateFolder("Assets/_Project/Data", "Narrative");
                }

                fragSO = ScriptableObject.CreateInstance<MemoryFragmentSO>();
                fragSO.fragmentId = "FRAG_FIRST_MEMORY";
                fragSO.narrativeFlagOnCollect = "HAS_FIRST_MEMORY";
                fragSO.editorNotes = "The first memory the player discovers.";
                AssetDatabase.CreateAsset(fragSO, fragPath);
                Debug.Log("[MissingPiecesPatcher] Created MemoryFragmentSO.");
            }

            var fragCube = GameObject.Find("MemoryFragment_Test");
            if (fragCube != null)
            {
                var pickup = fragCube.GetComponent<MemoryFragmentPickup>();
                if (pickup != null)
                {
                    SerializedObject so = new SerializedObject(pickup);
                    so.FindProperty("_fragment").objectReferenceValue = fragSO;
                    so.ApplyModifiedProperties();
                    Debug.Log("[MissingPiecesPatcher] Assigned MemoryFragmentSO to MemoryFragment_Test.");
                }

                var rb = fragCube.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = fragCube.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }

            // 3. Fix Floating Capsule
            var driftMesh = GameObject.Find("[DRIFT]/Capsule");
            if (driftMesh != null)
            {
                driftMesh.transform.localPosition = Vector3.zero;
            }

            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            Debug.Log("[MissingPiecesPatcher] All missing pieces patched!");
        }
    }
}
