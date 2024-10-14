#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildTool : EditorWindow
{
    private static string buildFolder = string.Empty;
    private static string macOsBuildPath = string.Empty;
    private static string windowsBuildPath = string.Empty;
    private static string androidBuildPath = string.Empty;
    private static string buildName = "bunnyrobots-at-work";
    private static bool developmentBuild = false;

    [MenuItem("Tools/Build for All Platforms")]
    public static void ShowWindow()
    {
        GetWindow<BuildTool>("Multi-Platform Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("Multi-Platform Build Settings", EditorStyles.boldLabel);
        
        GUILayout.Label("Build Name", EditorStyles.label);
        buildName = EditorGUILayout.TextField(buildName);
        developmentBuild = EditorGUILayout.Toggle("Developmnet build", developmentBuild);

        if (GUILayout.Button("Build All"))
        {
            BuildAllPlatforms();
        }
    }

    private void BuildAllPlatforms()
    {
        buildFolder = EditorUtility.OpenFolderPanel("Select MacOS Build Folder", buildFolder, "");

        macOsBuildPath = Path.Combine(buildFolder, "MacOS");
        windowsBuildPath = Path.Combine(buildFolder, "Windows");
        androidBuildPath = Path.Combine(buildFolder, "Android");
        
        if (!Directory.Exists(macOsBuildPath)) Directory.CreateDirectory(macOsBuildPath);
        if (!Directory.Exists(windowsBuildPath)) Directory.CreateDirectory(windowsBuildPath);
        if (!Directory.Exists(androidBuildPath)) Directory.CreateDirectory(androidBuildPath);

        var buildOptions = BuildOptions.None;
        if (developmentBuild) buildOptions |= BuildOptions.Development;

        SetIcon("Icons/icon_PC", BuildTargetGroup.Standalone);
        BuildPlayerOptions macOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = Path.Combine(macOsBuildPath, buildName + ".app"),
            target = BuildTarget.StandaloneOSX,
            options = buildOptions
        };
        BuildPipeline.BuildPlayer(macOptions);

        SetIcon("Icons/icon_PC", BuildTargetGroup.Standalone);
        BuildPlayerOptions windowsOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = Path.Combine(windowsBuildPath, buildName + ".exe"),
            target = BuildTarget.StandaloneWindows64,
            options = buildOptions
        };
        BuildPipeline.BuildPlayer(windowsOptions);

        SetIcon("Icons/icon_Mobile", BuildTargetGroup.Android);
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        
        BuildPlayerOptions androidOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = Path.Combine(androidBuildPath, buildName + ".apk"),
            target = BuildTarget.Android,
            options = buildOptions
        };
        BuildPipeline.BuildPlayer(androidOptions);

        Debug.Log("Builds completed successfully!");
    }

    private string[] GetScenes()
    {
        return EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
    }

    private void SetIcon(string iconPath, BuildTargetGroup buildTargetGroup)
    {
        var icon = Resources.Load<Texture2D>(iconPath);

        int[] iconSizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Standalone);

        Texture2D[] icons = new Texture2D[iconSizes.Length];

        for (int i = 0; i < iconSizes.Length; i++)
        {
            icons[i] = ResizeTexture(icon, iconSizes[i], iconSizes[i]);
        }

        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, icons, IconKind.Any);
    }
    
    private static Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;

        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
}

#endif
