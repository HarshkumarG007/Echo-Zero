using UnityEngine;
using UnityEditor;
using System.IO;

namespace EchoZero.Tools.Editor
{
    /// <summary>
    /// Procedural PBR Asset and Compound Mesh Factory for URP.
    /// Generates standardized materials and compound visual meshes for ECHO//ZERO.
    /// Follows ADR-0007 (Procedural PBR Asset Architecture).
    /// TASK: TASK-041
    /// </summary>
    public static class AerieArtFactory
    {
        private const string MaterialsDirectory = "Assets/_Project/Art/Materials";
        private const string URP_LIT_SHADER = "Universal Render Pipeline/Lit";

        /// <summary>
        /// Material definitions following the color palette from docs/design.md.
        /// </summary>
        public static class Palette
        {
            public static readonly Color SteelBase = new Color(0.12f, 0.15f, 0.18f); // #1E2530
            public static readonly Color FloorBase = new Color(0.08f, 0.10f, 0.13f); // #141A22
            public static readonly Color CrystalCyan = new Color(0.39f, 0.78f, 1.0f); // #64C8FF
            public static readonly Color CrystalCyanEmission = new Color(0.20f, 0.60f, 1.0f) * 2.5f;
            public static readonly Color DriftAmber = new Color(1.0f, 0.42f, 0.29f); // #FF6B4A
            public static readonly Color DriftAmberEmission = new Color(1.0f, 0.30f, 0.15f) * 3.0f;
            public static readonly Color PedestalBase = new Color(0.05f, 0.07f, 0.09f); // #0D1117
            public static readonly Color PedestalGold = new Color(1.0f, 0.82f, 0.39f); // #FFD264
            public static readonly Color PedestalGoldEmission = new Color(1.0f, 0.70f, 0.20f) * 2.0f;
        }

        /// <summary>
        /// Ensures all standard PBR materials exist on disk with correct URP Lit properties.
        /// </summary>
        public static Material GetOrCreateMaterial(string materialName, Color baseColor, float metallic, float smoothness, Color? emissionColor = null)
        {
            if (!Directory.Exists(MaterialsDirectory))
            {
                Directory.CreateDirectory(MaterialsDirectory);
            }

            string assetPath = $"{MaterialsDirectory}/{materialName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            Shader urpShader = Shader.Find(URP_LIT_SHADER);
            if (urpShader == null)
            {
                urpShader = Shader.Find("Standard"); // Fallback
            }

            if (mat == null)
            {
                mat = new Material(urpShader);
                AssetDatabase.CreateAsset(mat, assetPath);
            }
            else if (mat.shader != urpShader && urpShader != null)
            {
                mat.shader = urpShader;
            }

            // Configure URP Lit PBR properties
            mat.SetColor("_BaseColor", baseColor);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);

            if (emissionColor.HasValue && emissionColor.Value != Color.black)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor.Value);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }

            EditorUtility.SetDirty(mat);
            return mat;
        }

        public static Material GetSteelMaterial() =>
            GetOrCreateMaterial("Mat_Aerie_Structure_Steel", Palette.SteelBase, 0.75f, 0.65f);

        public static Material GetFloorMaterial() =>
            GetOrCreateMaterial("Mat_Aerie_Structure_Floor", Palette.FloorBase, 0.40f, 0.40f);

        public static Material GetCrystalCyanMaterial() =>
            GetOrCreateMaterial("Mat_Aerie_Crystal_Reconstructed", Palette.CrystalCyan, 0.20f, 0.95f, Palette.CrystalCyanEmission);

        public static Material GetDriftAmberMaterial() =>
            GetOrCreateMaterial("Mat_Aerie_Crystal_Drift", Palette.DriftAmber, 0.80f, 0.85f, Palette.DriftAmberEmission);

        public static Material GetPedestalBaseMaterial() =>
            GetOrCreateMaterial("Mat_Aerie_Pedestal_Base", Palette.PedestalBase, 0.50f, 0.30f);

        public static Material GetPedestalGoldMaterial() =>
            GetOrCreateMaterial("Mat_Aerie_Pedestal_Chosen", Palette.PedestalGold, 0.30f, 0.90f, Palette.PedestalGoldEmission);

        /// <summary>
        /// Constructs a stylized crystalline memory fragment compound object.
        /// </summary>
        public static GameObject CreateMemoryCrystalVisual(Transform parent)
        {
            var visualRoot = new GameObject("Crystal_VisualCompound");
            visualRoot.transform.SetParent(parent, false);
            visualRoot.transform.localPosition = Vector3.zero;

            Material crystalMat = GetCrystalCyanMaterial();

            // 1. Central Diamond Crystal (rotated 45 degrees along X and Z axes)
            var centralCore = GameObject.CreatePrimitive(PrimitiveType.Cube);
            centralCore.name = "Core_Diamond";
            centralCore.transform.SetParent(visualRoot.transform, false);
            centralCore.transform.localScale = new Vector3(0.45f, 0.75f, 0.45f);
            centralCore.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
            Object.DestroyImmediate(centralCore.GetComponent<Collider>());
            centralCore.GetComponent<MeshRenderer>().sharedMaterial = crystalMat;

            // 2. Upper shard
            var upperShard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            upperShard.name = "Upper_Facet";
            upperShard.transform.SetParent(visualRoot.transform, false);
            upperShard.transform.localPosition = new Vector3(0, 0.35f, 0);
            upperShard.transform.localScale = new Vector3(0.20f, 0.35f, 0.20f);
            upperShard.transform.localRotation = Quaternion.Euler(0, 45f, 0);
            Object.DestroyImmediate(upperShard.GetComponent<Collider>());
            upperShard.GetComponent<MeshRenderer>().sharedMaterial = crystalMat;

            // 3. Lower shard
            var lowerShard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lowerShard.name = "Lower_Facet";
            lowerShard.transform.SetParent(visualRoot.transform, false);
            lowerShard.transform.localPosition = new Vector3(0, -0.35f, 0);
            lowerShard.transform.localScale = new Vector3(0.20f, 0.35f, 0.20f);
            lowerShard.transform.localRotation = Quaternion.Euler(0, 45f, 0);
            Object.DestroyImmediate(lowerShard.GetComponent<Collider>());
            lowerShard.GetComponent<MeshRenderer>().sharedMaterial = crystalMat;

            return visualRoot;
        }

        /// <summary>
        /// Constructs a stylized compound model for the Drift anomaly.
        /// </summary>
        public static GameObject CreateDriftVisual(Transform parent)
        {
            var visualRoot = new GameObject("Drift_VisualCompound");
            visualRoot.transform.SetParent(parent, false);
            visualRoot.transform.localPosition = Vector3.zero;

            Material driftMat = GetDriftAmberMaterial();
            Material steelMat = GetSteelMaterial();

            // 1. Central Core Sphere
            var core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "Drift_Core";
            core.transform.SetParent(visualRoot.transform, false);
            core.transform.localPosition = new Vector3(0, 1.0f, 0);
            core.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            Object.DestroyImmediate(core.GetComponent<Collider>());
            core.GetComponent<MeshRenderer>().sharedMaterial = driftMat;

            // 2. Orbiting Corrupted Shards (Surrounding carapace)
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shard.name = $"Carapace_Shard_{i}";
                shard.transform.SetParent(visualRoot.transform, false);
                shard.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.7f, 1.0f + (i % 2 == 0 ? 0.2f : -0.2f), Mathf.Sin(angle) * 0.7f);
                shard.transform.localScale = new Vector3(0.25f, 0.6f, 0.15f);
                shard.transform.localRotation = Quaternion.Euler(15f * (i % 2 == 0 ? 1 : -1), i * 90f, 10f);
                Object.DestroyImmediate(shard.GetComponent<Collider>());
                shard.GetComponent<MeshRenderer>().sharedMaterial = steelMat;
            }

            return visualRoot;
        }

        /// <summary>
        /// Constructs a stylized stepped architectural pedestal.
        /// </summary>
        public static GameObject CreatePedestalVisual(Transform parent, string name, bool isChosen = false)
        {
            var visualRoot = new GameObject($"{name}_VisualCompound");
            visualRoot.transform.SetParent(parent, false);
            visualRoot.transform.localPosition = Vector3.zero;

            Material baseMat = GetPedestalBaseMaterial();
            Material accentMat = isChosen ? GetPedestalGoldMaterial() : GetCrystalCyanMaterial();

            // Base plinth
            var basePlinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basePlinth.name = "Plinth_Base";
            basePlinth.transform.SetParent(visualRoot.transform, false);
            basePlinth.transform.localPosition = new Vector3(0, 0.25f, 0);
            basePlinth.transform.localScale = new Vector3(1.2f, 0.25f, 1.2f);
            Object.DestroyImmediate(basePlinth.GetComponent<Collider>());
            basePlinth.GetComponent<MeshRenderer>().sharedMaterial = baseMat;

            // Pillar
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "Plinth_Pillar";
            pillar.transform.SetParent(visualRoot.transform, false);
            pillar.transform.localPosition = new Vector3(0, 0.75f, 0);
            pillar.transform.localScale = new Vector3(0.6f, 0.5f, 0.6f);
            Object.DestroyImmediate(pillar.GetComponent<Collider>());
            pillar.GetComponent<MeshRenderer>().sharedMaterial = baseMat;

            // Upper Chalice / Receptacle Ring
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Receptacle_Ring";
            ring.transform.SetParent(visualRoot.transform, false);
            ring.transform.localPosition = new Vector3(0, 1.25f, 0);
            ring.transform.localScale = new Vector3(0.9f, 0.1f, 0.9f);
            Object.DestroyImmediate(ring.GetComponent<Collider>());
            ring.GetComponent<MeshRenderer>().sharedMaterial = accentMat;

            return visualRoot;
        }
    }
}
