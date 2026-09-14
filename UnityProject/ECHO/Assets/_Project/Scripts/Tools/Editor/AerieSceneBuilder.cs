using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using EchoZero.Gameplay;
using EchoZero.Gameplay.Recall;

namespace EchoZero.Tools.Editor
{
    public class AerieSceneBuilder
    {
        [MenuItem("ECHO/Build Aerie Graybox Scene")]
        public static void BuildAerieScene()
        {
            // Create a new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Create Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.position = new Vector3(0, -0.5f, 0);
            floor.transform.localScale = new Vector3(20, 1, 20);

            // Create Walls
            CreateWall("Wall_North", new Vector3(0, 2, 10), new Vector3(20, 4, 1));
            CreateWall("Wall_South", new Vector3(0, 2, -10), new Vector3(20, 4, 1));
            CreateWall("Wall_East", new Vector3(10, 2, 0), new Vector3(1, 4, 20));
            CreateWall("Wall_West", new Vector3(-10, 2, 0), new Vector3(1, 4, 20));

            // Create Player
            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0, 1, 0);
            
            var cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);

            // Needs Input component but since we are doing Input System later, we just add the controllers
            player.AddComponent<PlayerController>();
            player.AddComponent<RecallAbility>();

            // Create Camera Root
            GameObject cameraRoot = new GameObject("CameraRoot");
            cameraRoot.transform.SetParent(player.transform);
            cameraRoot.transform.localPosition = new Vector3(0, 1.6f, 0);

            // Move main camera to camera root
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.SetParent(cameraRoot.transform);
                mainCam.transform.localPosition = Vector3.zero;
                mainCam.transform.localRotation = Quaternion.identity;
            }

            // Assign Camera Root to Player Controller using SerializedObject
            SerializedObject so = new SerializedObject(player.GetComponent<PlayerController>());
            so.FindProperty("_cameraRoot").objectReferenceValue = cameraRoot.transform;
            so.ApplyModifiedProperties();

            // Save the scene
            string scenePath = "Assets/_Project/World/Scenes/Aerie.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"[AerieSceneBuilder] Scene built and saved to {scenePath}");
        }

        private static void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
        }
    }
}
