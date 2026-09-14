using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

namespace EchoZero.Tools.Editor
{
    public static class BuildPipeline
    {
        [MenuItem("ECHO//ZERO/Build Windows 64-bit (IL2CPP)")]
        public static void BuildWindows64_IL2CPP()
        {
            BuildWindows64(ScriptingImplementation.IL2CPP);
        }

        [MenuItem("ECHO//ZERO/Build Windows 64-bit (Mono)")]
        public static void BuildWindows64_Mono()
        {
            BuildWindows64(ScriptingImplementation.Mono2x);
        }

        private static void BuildWindows64(ScriptingImplementation scriptingBackend)
        {
            Debug.Log($"[BuildPipeline] Starting Windows 64-bit {scriptingBackend} build...");

            // Ensure build directory exists
            string backendFolder = scriptingBackend == ScriptingImplementation.IL2CPP ? "Windows_IL2CPP" : "Windows_Mono";
            string buildPath = Path.Combine(Application.dataPath, $"../Builds/{backendFolder}/ECHO_ZERO.exe");
            string buildDir = Path.GetDirectoryName(buildPath);
            if (!Directory.Exists(buildDir))
            {
                Directory.CreateDirectory(buildDir);
            }

            // Configure Player Settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, scriptingBackend);
            PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1); // 1 = x64

            // Define scenes (order matters)
            string[] scenes = 
            {
                "Assets/_Project/World/Scenes/Bootstrap.unity",
                "Assets/_Project/World/Scenes/AerieUpperScene.unity",
                "Assets/_Project/World/Scenes/AerieLowerScene.unity"
            };

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[BuildPipeline] Build succeeded: {summary.totalSize} bytes");
                Debug.Log($"[BuildPipeline] Output path: {buildPath}");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("[BuildPipeline] Build failed!");
            }
        }
    }
}
