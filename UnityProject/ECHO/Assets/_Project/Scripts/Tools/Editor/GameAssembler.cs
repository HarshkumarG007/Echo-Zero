using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using EchoZero.App;
using EchoZero.Gameplay;
using EchoZero.Gameplay.Recall;
using EchoZero.AI.Drift;
using EchoZero.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace EchoZero.Tools.Editor
{
    public class GameAssembler
    {
        [MenuItem("ECHO/Assemble Full Game Scene")]
        public static void AssembleScene()
        {
            string scenePath = "Assets/_Project/World/Scenes/Aerie.unity";
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. BOOTSTRAP
            if (GameObject.Find("[BOOTSTRAP]") == null)
            {
                var bootstrapObj = new GameObject("[BOOTSTRAP]");
                bootstrapObj.AddComponent<Bootstrap>();
                Debug.Log("[GameAssembler] Added Bootstrap.");
            }

            // 2. PLAYER
            GameObject playerObj = GameObject.Find("Player") ?? GameObject.Find("[PLAYER]");
            if (playerObj == null)
            {
                playerObj = new GameObject("[PLAYER]");
                playerObj.transform.position = new Vector3(0, 1, 0);
            }
            playerObj.name = "[PLAYER]";
            playerObj.tag = "Player";

            if (!playerObj.GetComponent<CharacterController>()) playerObj.AddComponent<CharacterController>();
            if (!playerObj.GetComponent<PlayerController>()) playerObj.AddComponent<PlayerController>();
            
            var playerInput = playerObj.GetComponent<PlayerInput>();
            if (!playerInput) playerInput = playerObj.AddComponent<PlayerInput>();
            
            var recallAbility = playerObj.GetComponent<RecallAbility>();
            if (!recallAbility) recallAbility = playerObj.AddComponent<RecallAbility>();

            // Ensure Player Camera
            GameObject cameraRoot = GameObject.Find("CameraRoot");
            if (cameraRoot == null)
            {
                cameraRoot = new GameObject("CameraRoot");
                cameraRoot.transform.SetParent(playerObj.transform);
                cameraRoot.transform.localPosition = new Vector3(0, 1.6f, 0);
            }

            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                var camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            mainCam.transform.SetParent(cameraRoot.transform);
            mainCam.transform.localPosition = Vector3.zero;

            // Assign dependencies using SerializedObject
            SerializedObject pSo = new SerializedObject(playerObj.GetComponent<PlayerController>());
            pSo.FindProperty("_cameraRoot").objectReferenceValue = cameraRoot.transform;
            pSo.ApplyModifiedProperties();

            // Set up PlayerInput
            var inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (inputActionAsset != null)
            {
                SerializedObject piSo = new SerializedObject(playerInput);
                piSo.FindProperty("m_Actions").objectReferenceValue = inputActionAsset;
                piSo.FindProperty("m_DefaultActionMap").stringValue = "Player"; // Assuming default map is Player
                piSo.FindProperty("m_NotificationBehavior").intValue = (int)PlayerNotifications.SendMessages;
                piSo.ApplyModifiedProperties();
            }

            SerializedObject rSo = new SerializedObject(recallAbility);
            rSo.FindProperty("_recallCamera").objectReferenceValue = mainCam;
            rSo.ApplyModifiedProperties();
            
            Debug.Log("[GameAssembler] Added Player & Camera.");

            // 3. DRIFT AI
            if (GameObject.Find("[DRIFT]") == null)
            {
                var driftObj = new GameObject("[DRIFT]");
                driftObj.transform.position = new Vector3(5, 1, 5);
                driftObj.AddComponent<CharacterController>();
                driftObj.AddComponent<DriftController>();
                Debug.Log("[GameAssembler] Added Drift AI.");
            }

            // 4. UI
            if (GameObject.Find("[UI]") == null)
            {
                var uiObj = new GameObject("[UI]");
                var canvas = uiObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                uiObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                uiObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                uiObj.AddComponent<GameplayUI>();
                uiObj.AddComponent<GameplayUIHook>();
                uiObj.AddComponent<PauseMenuUI>();
                
                // Add EventSystem if missing
                if (GameObject.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    var esObj = new GameObject("EventSystem");
                    esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    esObj.AddComponent<InputSystemUIInputModule>();
                }
                Debug.Log("[GameAssembler] Added UI.");
            }

            // Delete broken GameObjects created manually by the user
            for (int i = 1; i <= 5; i++)
            {
                var go = GameObject.Find($"MyGameObject{i}");
                if (go != null) Object.DestroyImmediate(go);
            }

            // Save Scene
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[GameAssembler] Finished assembling {scenePath}");
        }
    }
}
