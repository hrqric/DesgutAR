using System.IO;
using CardapioRA;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
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

        [MenuItem("Cardapio RA/Preparar projeto MVP")]
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
            if (AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(LibraryPath) != null)
                return;

            var library = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();
            AssetDatabase.CreateAsset(library, LibraryPath);
        }
    }
}

