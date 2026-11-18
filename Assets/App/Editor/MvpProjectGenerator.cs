using System.IO;
using CardapioRA;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

namespace CardapioRA.Editor
{
    public static class MvpProjectGenerator
    {
        private const string ScenePath = "Assets/Scenes/Mvp.unity";
        private const string LibraryPath = "Assets/App/Resources/MenuReferenceImages.asset";
        private const string MarkerDirectory = "Assets/App/Resources/Markers";
        private const float MarkerWidthMeters = 0.12f;

        [InitializeOnLoadMethod]
        private static void PrepareNewCheckout()
        {
            if (File.Exists(ScenePath))
                return;

            EditorApplication.delayCall += () =>
            {
                if (!File.Exists(ScenePath) && !EditorApplication.isCompiling)
                    Prepare();
            };
        }

        [UnityEditor.MenuItem("Cardapio RA/Preparar projeto MVP")]
        public static void Prepare()
        {
            Directory.CreateDirectory("Assets/Scenes");
            Directory.CreateDirectory("Assets/App/Resources/Models");
            EnsureReferenceLibrary();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var sessionObject = new GameObject("AR Session");
            sessionObject.AddComponent<ARSession>();

            var originObject = new GameObject("XR Origin");
            var origin = originObject.AddComponent<XROrigin>();

            var cameraObject = new GameObject("AR Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(originObject.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0.45f, -2.2f);
            cameraObject.transform.LookAt(Vector3.zero);
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.10f, 0.13f);
            cameraObject.AddComponent<ARCameraManager>();
            cameraObject.AddComponent<ARCameraBackground>();

            // Faz a camera virtual acompanhar o movimento real do celular (sem isso a RA nao fica ancorada).
            var poseDriver = cameraObject.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            poseDriver.positionInput = new UnityEngine.InputSystem.InputActionProperty(
                new UnityEngine.InputSystem.InputAction("Position", binding: "<HandheldARInputDevice>/devicePosition", expectedControlType: "Vector3"));
            poseDriver.rotationInput = new UnityEngine.InputSystem.InputActionProperty(
                new UnityEngine.InputSystem.InputAction("Rotation", binding: "<HandheldARInputDevice>/deviceRotation", expectedControlType: "Quaternion"));
            origin.Camera = camera;

            var imageManager = originObject.AddComponent<ARTrackedImageManager>();
            imageManager.referenceLibrary = AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(LibraryPath);
            imageManager.requestedMaxNumberOfMovingImages = 1;
            var tracker = originObject.AddComponent<ArImageTracker>();

            var simulationAnchor = new GameObject("Simulation Anchor");

            var appObject = new GameObject("Application");
            var bootstrap = appObject.AddComponent<AppBootstrap>();
            var serialized = new SerializedObject(bootstrap);
            serialized.FindProperty("arTracker").objectReferenceValue = tracker;
            serialized.FindProperty("simulationAnchor").objectReferenceValue = simulationAnchor.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            PlayerSettings.productName = "Cardapio RA";
            PlayerSettings.companyName = "Facens UPX";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "br.edu.facens.cardapiora");
            EnableArCoreLoader();

            AssetDatabase.SaveAssets();
            Debug.Log("Cardapio RA: cena, biblioteca de imagens e configuracoes criadas com sucesso.");
        }

        private static void EnableArCoreLoader()
        {
            var buildSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
            if (buildSettings?.AssignedSettings == null)
            {
                Debug.LogWarning("XR Plug-in Management ainda nao criou as configuracoes Android. Ative ARCore em Project Settings > XR Plug-in Management.");
                return;
            }

            if (!XRPackageMetadataStore.AssignLoader(
                    buildSettings.AssignedSettings,
                    "UnityEngine.XR.ARCore.ARCoreLoader",
                    BuildTargetGroup.Android))
            {
                Debug.LogWarning("Nao foi possivel ativar o ARCore automaticamente. Ative-o em Project Settings > XR Plug-in Management > Android.");
            }
        }

        private static void EnsureReferenceLibrary()
        {
            Directory.CreateDirectory(MarkerDirectory);

            var library = AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(LibraryPath);
            if (library == null)
            {
                library = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();
                AssetDatabase.CreateAsset(library, LibraryPath);
            }

            var markerNames = new[] { "marker_prato_001", "marker_prato_002", "marker_prato_003", "marker_prato_004" };
            for (var i = 0; i < markerNames.Length; i++)
            {
                if (ContainsImage(library, markerNames[i]))
                    continue;

                var texture = CreateMarkerTextureAsset(markerNames[i], i);
                library.Add();
                var index = library.count - 1;
                library.SetTexture(index, texture, true);
                library.SetName(index, markerNames[i]);
                library.SetSpecifySize(index, true);
                library.SetSize(index, new Vector2(MarkerWidthMeters, MarkerWidthMeters));
            }

            EditorUtility.SetDirty(library);
        }

        private static bool ContainsImage(XRReferenceImageLibrary library, string imageName)
        {
            for (var i = 0; i < library.count; i++)
            {
                if (library[i].name == imageName)
                    return true;
            }

            return false;
        }

        private static Texture2D CreateMarkerTextureAsset(string markerName, int variant)
        {
            var assetPath = $"{MarkerDirectory}/{markerName}.png";
            var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (existing != null)
                return existing;

            const int size = 512;
            const int cells = 8;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = markerName,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[size * size];
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var cellX = x * cells / size;
                    var cellY = y * cells / size;
                    var border = cellX == 0 || cellY == 0 || cellX == cells - 1 || cellY == cells - 1;
                    var finder = IsFinderCell(cellX, cellY);
                    var bit = ((cellX * 17 + cellY * 31 + variant * 13) ^ (cellX * cellY * 7)) % 5 < 2;
                    pixels[y * size + x] = border || finder || bit
                        ? new Color32(20, 20, 24, 255)
                        : new Color32(245, 242, 232, 255);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            File.WriteAllBytes(assetPath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);

            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }

        private static bool IsFinderCell(int x, int y)
        {
            var topLeft = x is >= 1 and <= 2 && y is >= 1 and <= 2;
            var topRight = x is >= 5 and <= 6 && y is >= 1 and <= 2;
            var bottomLeft = x is >= 1 and <= 2 && y is >= 5 and <= 6;
            return topLeft || topRight || bottomLeft;
        }
    }
}

