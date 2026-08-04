using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Android;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

public static class ZeldaXRBuild
{
    private const string OpenXRLoader = "UnityEngine.XR.OpenXR.OpenXRLoader";
    private const string OpenXRPackage = "com.unity.xr.openxr";
    private static string ToolchainRoot
    {
        get { return System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, ".unity-toolchain"); }
    }

    [MenuItem("Zelda VR/Configure OpenXR")]
    public static void ConfigureOpenXR()
    {
        ConfigureAndroidToolchain();
        ExcludeDesktopPluginsFromAndroid();
        ConfigureLoader(BuildTargetGroup.Standalone);
        ConfigureLoader(BuildTargetGroup.Android);
        EnableControllerProfiles(BuildTargetGroup.Standalone);
        EnableControllerProfiles(BuildTargetGroup.Android);

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, "com.foolserrand.zeldavr");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.foolserrand.zeldavr");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        // Quest OpenXR with OpenGLES3 requires Linear color space.
        PlayerSettings.colorSpace = ColorSpace.Linear;
        AssetDatabase.SaveAssets();
    }

    private static void ConfigureAndroidToolchain()
    {
        AndroidExternalToolsSettings.jdkRootPath = System.IO.Path.Combine(ToolchainRoot, "OpenJDK");
        AndroidExternalToolsSettings.ndkRootPath = System.IO.Path.Combine(ToolchainRoot, "NDK");
        AndroidExternalToolsSettings.sdkRootPath = System.IO.Path.Combine(ToolchainRoot, "SDK");
    }

    private static void ExcludeDesktopPluginsFromAndroid()
    {
        foreach (string path in AssetDatabase.GetAllAssetPaths())
        {
            if (!path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;

            PluginImporter plugin = AssetImporter.GetAtPath(path) as PluginImporter;
            if (plugin == null) continue;

            plugin.SetExcludeFromAnyPlatform(BuildTarget.Android, true);
            plugin.SaveAndReimport();
        }
    }

    private static void ConfigureLoader(BuildTargetGroup group)
    {
        string[] assets = AssetDatabase.FindAssets("t:XRGeneralSettingsPerBuildTarget");
        XRGeneralSettingsPerBuildTarget perTargetSettings = assets.Length == 0
            ? null
            : AssetDatabase.LoadAssetAtPath<XRGeneralSettingsPerBuildTarget>(AssetDatabase.GUIDToAssetPath(assets[0]));

        if (perTargetSettings == null)
        {
            Debug.LogWarning("Could not find the XR settings asset. Use Project Settings > XR Plug-in Management once, then run this command again.");
            return;
        }

        if (!perTargetSettings.HasManagerSettingsForBuildTarget(group))
            perTargetSettings.CreateDefaultManagerSettingsForBuildTarget(group);

        XRManagerSettings manager = perTargetSettings.ManagerSettingsForBuildTarget(group);
        if (manager == null || !XRPackageMetadataStore.AssignLoader(manager, OpenXRLoader, group))
            Debug.LogWarning("Could not assign the OpenXR loader for " + group + ". Use Project Settings > XR Plug-in Management.");

        EditorUtility.SetDirty(perTargetSettings);
    }

    private static void EnableControllerProfiles(BuildTargetGroup group)
    {
        OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(group);
        if (settings == null) return;

        EnableFeature<OculusTouchControllerProfile>(settings);
        EnableFeature<HTCViveControllerProfile>(settings);
        EnableFeature<KHRSimpleControllerProfile>(settings);
        EnableFeature<ValveIndexControllerProfile>(settings);
        EnableFeature<MicrosoftMotionControllerProfile>(settings);
        EnableFeature<MetaQuestTouchProControllerProfile>(settings);
        EnableFeature<MetaQuestTouchPlusControllerProfile>(settings);
        if (group == BuildTargetGroup.Android)
            EnableFeature<MetaQuestFeature>(settings);
        EditorUtility.SetDirty(settings);
    }

    private static void EnableFeature<T>(OpenXRSettings settings) where T : OpenXRFeature
    {
        T feature = settings.GetFeature<T>();
        if (feature != null) feature.enabled = true;
    }

    [MenuItem("Zelda VR/Build/Quest APK")]
    public static void BuildQuestMenu() { BuildQuest(); }

    [MenuItem("Zelda VR/Build/Windows OpenXR")]
    public static void BuildWindowsMenu() { BuildWindows(); }

    public static void BuildQuest()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        ConfigureOpenXR();
        Build(BuildTarget.Android, "Builds/Quest/ZeldaVR.apk");
    }

    public static void BuildWindows()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        ConfigureOpenXR();
        Build(BuildTarget.StandaloneWindows64, "Builds/Windows/ZeldaVR.exe");
    }

    private static void Build(BuildTarget target, string location)
    {
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = location,
            target = target,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Zelda VR build failed: " + report.summary.result);
    }
}
