#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ConfigureAndroidBuild
{
    static ConfigureAndroidBuild()
    {
        PlayerSettings.companyName = "ParhamAzangoo";
        PlayerSettings.productName = "COD & PUBG Battlegrounds";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.parhamazangoo.codpubg");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true)
        };
        EnsureShaderIncluded();
    }

    // All game materials are created at runtime, so Unity would strip the
    // Standard shader from the build (nothing in the project references it)
    // and every object would render pink/magenta. Creating a real material
    // asset that uses Standard forces Unity to include the shader in builds.
    private static void EnsureShaderIncluded()
    {
        try
        {
            string matPath = "Assets/Resources/DefaultMat.mat";
            if (System.IO.File.Exists(matPath)) return;
            Shader standard = Shader.Find("Standard");
            if (standard == null) return;
            Material mat = new Material(standard);
            mat.name = "DefaultMat";
            mat.color = Color.white;
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.Refresh();
            Debug.Log("[ConfigureAndroidBuild] Created DefaultMat.mat so the Standard shader is included in builds.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[ConfigureAndroidBuild] EnsureShaderIncluded failed: " + e.Message);
        }
    }
}
#endif
