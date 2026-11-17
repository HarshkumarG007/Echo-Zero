using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EchoZero.UI;
using EchoZero.Gameplay;
using EchoZero.Narrative;

namespace EchoZero.Tools.Editor
{
    /// <summary>
    /// Editor utility for attaching procedural PBR visuals and UI Toolkit documents to Aerie.unity.
    /// Follows ADR-0007 (Procedural PBR Asset Architecture).
    /// TASK: TASK-041
    /// </summary>
    public class GameVisualizer
    {
        [MenuItem("ECHO/Add Visuals and UXML")]
        public static void AddVisuals()
        {
            // 1. UI Toolkit Document binding
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
                    uiDoc.panelSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.PanelSettings>("Assets/Settings/PanelSettings.asset");
                    Debug.Log("[GameVisualizer] Assigned GameplayUI.uxml to [UI] UIDocument.");
                }
            }

            // 2. DRIFT Compound Mesh
            var drift = GameObject.Find("[DRIFT]");
            if (drift != null)
            {
                // Clear any existing legacy visual children
                for (int i = drift.transform.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(drift.transform.GetChild(i).gameObject);
                }

                AerieArtFactory.CreateDriftVisual(drift.transform);
                Debug.Log("[GameVisualizer] Created stylized compound visual for [DRIFT].");
            }

            // 3. Memory Fragment Compound Crystal
            var fragObj = GameObject.Find("MemoryFragment_Test");
            if (fragObj == null)
            {
                fragObj = new GameObject("MemoryFragment_Test");
                fragObj.transform.position = new Vector3(3, 1, 3);

                var boxCol = fragObj.AddComponent<BoxCollider>();
                boxCol.isTrigger = true;
                boxCol.size = new Vector3(1.2f, 1.2f, 1.2f);

                var rb = fragObj.AddComponent<Rigidbody>();
                rb.isKinematic = true;

                fragObj.AddComponent<MemoryFragmentPickup>();
            }

            // Replace visual representation with compound crystal
            for (int i = fragObj.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(fragObj.transform.GetChild(i).gameObject);
            }
            AerieArtFactory.CreateMemoryCrystalVisual(fragObj.transform);
            Debug.Log("[GameVisualizer] Created stylized compound crystal for MemoryFragment.");

            // 4. Floor & Wall PBR Materials
            var floor = GameObject.Find("Floor");
            if (floor != null)
            {
                var renderer = floor.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = AerieArtFactory.GetFloorMaterial();
                }
            }

            string[] wallNames = { "Wall_North", "Wall_South", "Wall_East", "Wall_West" };
            Material steelMat = AerieArtFactory.GetSteelMaterial();
            foreach (var wName in wallNames)
            {
                var wall = GameObject.Find(wName);
                if (wall != null)
                {
                    var renderer = wall.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = steelMat;
                    }
                }
            }

            // 5. Choice Pedestals
            var pedestalA = GameObject.Find("Pedestal_A");
            if (pedestalA != null && pedestalA.transform.childCount == 0)
            {
                AerieArtFactory.CreatePedestalVisual(pedestalA.transform, "Pedestal_A", false);
            }

            var pedestalB = GameObject.Find("Pedestal_B");
            if (pedestalB != null && pedestalB.transform.childCount == 0)
            {
                AerieArtFactory.CreatePedestalVisual(pedestalB.transform, "Pedestal_B", false);
            }

            // Save the scene
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            Debug.Log("[GameVisualizer] Visuals updated and scene saved.");
        }
    }
}
