using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public static class BuildPostProcess
{
    private const string PLIST_FILE = "Info.plist";
    private const string EXIST_ON_SUSPEND_KEY = "UIApplicationExitsOnSuspend";

    private const string VERSIONING_SYSTEM_KEY = "VERSIONING_SYSTEM";
    private const string CURRENT_PROJECT_VERSION_KEY = "CURRENT_PROJECT_VERSION";
    private const string APPLE_GENERIC_VALUE = "apple-generic";

    private const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";

    private const string CODE_SIGN_STYLE_KEY = "CODE_SIGN_STYLE";
    private const string PROVISIONING_PROFILE_SPECIFIER_KEY = "PROVISIONING_PROFILE_SPECIFIER";
    private const string PROVISIONING_PROFILE_KEY = "PROVISIONING_PROFILE";


    [PostProcessBuild(999)]
    public static void IOSBuildPostProcess(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }
        RemoveDeprecatedInfoPListKeys(pathToBuiltProject);

        string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

#if UNITY_2020_1_OR_NEWER
        var guidProject = pbxProject.GetUnityMainTargetGuid();
#else
        var guidProject = pbxProject.TargetGuidByName(pbxProject.GetUnityMainTargetGuid());
#endif

        Debug.Log("Setting Versioning system to Apple Generic...");
        pbxProject.SetBuildProperty(guidProject, VERSIONING_SYSTEM_KEY, APPLE_GENERIC_VALUE);
        pbxProject.SetBuildProperty(guidProject, CURRENT_PROJECT_VERSION_KEY, "1");

        Debug.Log("Disabling bitcode...");
        pbxProject.SetBuildProperty(guidProject, ENABLE_BITCODE_KEY, "NO");

        Debug.Log("Setting Code sign style to manual and setup provisioning profile specifier...");
        pbxProject.SetBuildProperty(guidProject, CODE_SIGN_STYLE_KEY, "Manual");
        pbxProject.SetBuildProperty(guidProject, PROVISIONING_PROFILE_SPECIFIER_KEY, pbxProject.GetBuildPropertyForAnyConfig(guidProject, PROVISIONING_PROFILE_KEY));
        
        AddImagesXcAssetsToBuildPhases(pbxProject);
        pbxProject.WriteToFile(projectPath);
        FixXCWorkspace(target, pathToBuiltProject);
    }

    private static void FixXCWorkspace(BuildTarget buildTarget, string pathToBuiltProject)
    {
        #if UNITY_IOS
        if (buildTarget != BuildTarget.iOS) return; // Make sure its iOS build
            
        // Getting access to the xcode project file
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);
            
        // Getting the UnityFramework Target and changing build settings
        string target = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        // After we're done editing the build settings we save it 
        pbxProject.WriteToFile(projectPath);
        #endif
    }
    
    private static void AddImagesXcAssetsToBuildPhases(PBXProject project)
    {
        string mainGuid = project.GetUnityMainTargetGuid();
        project.AddFileToBuild(mainGuid, project.AddFile("Unity-iPhone/Images.xcassets", "Images.xcassets"));
    }

    private static void RemoveDeprecatedInfoPListKeys(string pathToBuiltProject)
    {
        string plistPath = Path.Combine(pathToBuiltProject, PLIST_FILE);
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        PlistElementDict rootDict = plist.root;

        if (rootDict.values.ContainsKey(EXIST_ON_SUSPEND_KEY))
        {
            Debug.LogFormat("Removing deprecated key \"{0}\" on \"{1}\" file", EXIST_ON_SUSPEND_KEY, PLIST_FILE);
            rootDict.values.Remove(EXIST_ON_SUSPEND_KEY);
        }

        var usageValue = "Anonymous level difficulty and play time analytics";
        rootDict.SetString("NSUserTrackingUsageDescription", usageValue);
        File.WriteAllText(plistPath, plist.WriteToString());
        Debug.Log($"setting usage description: {usageValue } at: {plistPath}");
    }
}
#endif