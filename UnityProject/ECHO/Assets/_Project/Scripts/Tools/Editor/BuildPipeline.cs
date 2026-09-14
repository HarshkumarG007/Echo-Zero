using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

namespace EchoZero.Tools.Editor
{
    public static class BuildPipeline
    {
        [MenuItem("ECHO//ZERO/Build Windows 64-bit (IL2CPP)")]
        public static void BuildWindows64()
        {
            Debug.Log("[BuildPipeline] Starting Windows 64-bit IL2CPP build...");

            // Ensure build directory exists
            string buildPath = Path.Combine(Application.dataPath, "../Builds/Windows/ECHO_ZERO.exe");
            string buildDir = Path.GetDirectoryName(buildPath);
            if (!Directory.Exists(buildDir))
            {
                Directory.CreateDirectory(buildDir);
            }

            // Configure Player Settings for IL2CPP Windows 64
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
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
