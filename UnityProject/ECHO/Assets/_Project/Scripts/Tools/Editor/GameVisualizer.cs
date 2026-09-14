using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EchoZero.UI;
using EchoZero.Gameplay;
using EchoZero.Narrative;

namespace EchoZero.Tools.Editor
{
    public class GameVisualizer
    {
        [MenuItem("ECHO/Add Visuals and UXML")]
        public static void AddVisuals()
        {
            // 1. UXML
            var uiObj = GameObject.Find("[UI]");
            if (uiObj != null)
            {
                var canvas = uiObj.GetComponent<Canvas>();
                if (canvas != null) Object.DestroyImmediate(canvas);
                
                var canvasScaler = uiObj.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (canvasScaler != null) Object.DestroyImmediate(canvasScaler);
                
                var raycaster = uiObj.GetComponent<UnityEngine.UI.GraphicRaycaster>();
                if (raycaster != null) Object.DestroyImmediate(raycaster);

                var uiDoc = uiObj.GetComponent<UIDocument>();
                if (uiDoc == null) uiDoc = uiObj.AddComponent<UIDocument>();

                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Project/UI/GameplayUI.uxml");
                if (visualTree != null)
                {
                    uiDoc.visualTreeAsset = visualTree;
                    uiDoc.panelSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.PanelSettings>("Assets/Settings/PanelSettings.asset"); // Might be null, Unity will create default
                    Debug.Log("[GameVisualizer] Assigned GameplayUI.uxml to [UI] UIDocument.");
                }
            }

            // 2. DRIFT Mesh
            var drift = GameObject.Find("[DRIFT]");
            if (drift != null && drift.transform.childCount == 0)
            {
                var driftMesh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                driftMesh.transform.SetParent(drift.transform);
                driftMesh.transform.localPosition = Vector3.zero;
                
                var renderer = driftMesh.GetComponent<MeshRenderer>();
                var redMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                redMat.color = Color.red;
                renderer.sharedMaterial = redMat;
                
                Object.DestroyImmediate(driftMesh.GetComponent<Collider>());
                Debug.Log("[GameVisualizer] Added Red Capsule to [DRIFT].");
            }

            // 3. Memory Fragment
            if (GameObject.Find("MemoryFragment_Test") == null)
            {
                var frag = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frag.name = "MemoryFragment_Test";
                frag.transform.position = new Vector3(3, 1, 3);
                frag.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                frag.GetComponent<BoxCollider>().isTrigger = true;
                frag.AddComponent<MemoryFragmentPickup>();

                var renderer = frag.GetComponent<MeshRenderer>();
                var blueMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                blueMat.color = Color.cyan;
                blueMat.SetFloat("_Metallic", 0.8f);
                blueMat.SetFloat("_Smoothness", 0.9f);
                renderer.sharedMaterial = blueMat;
                
                Debug.Log("[GameVisualizer] Added MemoryFragment to Scene.");
            }

            // 4. Floor Material
            var floor = GameObject.Find("Floor");
            if (floor != null)
            {
                var renderer = floor.GetComponent<MeshRenderer>();
                var darkMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                darkMat.color = new Color(0.2f, 0.2f, 0.2f);
                renderer.sharedMaterial = darkMat;
            }

            // Save the scene
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            Debug.Log("[GameVisualizer] Visuals updated and scene saved.");
        }
    }
}
