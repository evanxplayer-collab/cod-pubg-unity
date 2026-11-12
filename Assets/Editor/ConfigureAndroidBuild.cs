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
            // Standard material (forces the Standard shader into the build).
            string matPath = "Assets/Resources/DefaultMat.mat";
            if (!System.IO.File.Exists(matPath))
            {
                Shader standard = Shader.Find("Standard");
                if (standard != null)
                {
                    Material mat = new Material(standard);
                    mat.name = "DefaultMat";
                    mat.color = Color.white;
                    AssetDatabase.CreateAsset(mat, matPath);
                }
            }

            // Skybox material (forces the Procedural skybox shader into the build).
            string skyPath = "Assets/Resources/SkyboxMat.mat";
            if (!System.IO.File.Exists(skyPath))
            {
                Shader procedural = Shader.Find("Procedural");
                if (procedural != null)
                {
                    Material sky = new Material(procedural);
                    sky.name = "SkyboxMat";
                    sky.SetFloat("_Exposure", 1.1f);
                    sky.SetColor("_SkyTint", new Color(0.45f, 0.62f, 0.9f));
                    sky.SetColor("_GroundColor", new Color(0.35f, 0.3f, 0.22f));
                    AssetDatabase.CreateAsset(sky, skyPath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("[ConfigureAndroidBuild] Ensured Standard + Procedural skybox shaders are included in builds.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[ConfigureAndroidBuild] EnsureShaderIncluded failed: " + e.Message);
        }
    }
}
#endif
