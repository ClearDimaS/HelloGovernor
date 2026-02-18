using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MyEditorScript {
    static string[] SCENES = FindEnabledEditorScenes();

    static string TARGET_DIR = "CI_Builds";
    private const string BUNDLE_ID_VAR = "BUNDLE_ID";
    private const string BUILD_NAME_VAR = "BUILD_NAME_VAR";
    private const string PRODUCT_NAME_VAR = "PRODUCT_NAME_VAR";
    private const string VERSION_CODE_VAR = "VERSION_CODE_VAR";
    private const string BUILD_TYPE_VAR = "BUILD_TYPE_VAR";
    private const string KEYSTORE_PATH_VAR = "KEYSTORE_PATH_VAR";
    private const string KEYSTORE_PASS_VAR = "KEYSTORE_PASS_VAR";
    private const string KEYSTORE_ALIAS_VAR = "KEYSTORE_ALIAS_VAR";
    private const string KEYSTORE_ALIAS_PASS_VAR = "KEYSTORE_ALIAS_PASS_VAR";
    
    private const string BUILD_NUMBER_IOS_VAR = "BUILD_NUMBER_IOS_VAR";
    private const string BUNDLE_VERSION_ANDROID_VAR = "BUNDLE_VERSION_NUMBER_ANDROID_VAR";
    private const string BUNDLE_VERSION_WEBGL_VAR = "BUNDLE_VERSION_NUMBER_WEBGL_VAR";

    [MenuItem ("Custom/CI/Build iOS")]
    static void PerformIOSBuild ()
    {
        GenericPreBuild();
        UnityEditor.PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        UnityEditor.PlayerSettings.SetIncrementalIl2CppBuild(BuildTargetGroup.iOS, true);

        var dir = Application.productName;
        if (TryGetEnv(BUILD_NAME_VAR, out string buildPath))
        {
            dir = buildPath;
        }
        string target_dir = $"IOS/{dir}";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.iOS,BuildOptions.None);
    }

    [MenuItem ("Custom/CI/Build WebGL")]
    static void PerformWebGL_Build()
    {
        GenericPreBuild();
        UnityEditor.PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        UnityEditor.PlayerSettings.SetIncrementalIl2CppBuild(BuildTargetGroup.iOS, true);
        
        var dir = Application.productName;
        if (TryGetEnv(BUILD_NAME_VAR, out string buildPath))
        {
            dir = buildPath;
        }
        string target_dir = $"WebGL/{dir}";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.WebGL,BuildOptions.None);
    }
    
    [MenuItem ("Custom/CI/Build Android")]
    static void PerformAndroidBuild ()
    {
        GenericPreBuild();
        UnityEditor.PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        UnityEditor.PlayerSettings.SetIncrementalIl2CppBuild(BuildTargetGroup.Android, true);
        EditorUserBuildSettings.buildAppBundle = false;
        var ext = ".apk";
        var envTarget = EnvironmentVariableTarget.Process;
        PlayerSettings.Android.useCustomKeystore = false;
        if (TryGetEnv(BUILD_TYPE_VAR, out string buildType, envTarget))
        {
            if (buildType == $"RELEASE")
            {
                EditorUserBuildSettings.buildAppBundle = true;
                ext = ".aab";
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable(KEYSTORE_PATH_VAR, envTarget);
                PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable(KEYSTORE_PASS_VAR, envTarget);
                PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable(KEYSTORE_ALIAS_VAR, envTarget);
                PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable(KEYSTORE_ALIAS_PASS_VAR, envTarget);
            }
        }

        var dir = Application.productName;
        if (TryGetEnv(BUILD_NAME_VAR, out string buildPath))
        {
            dir = buildPath;
        }
        string target_dir = $"Android/{dir}{ext}";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.Android,BuildOptions.None);
    }

    private static string[] FindEnabledEditorScenes() {
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericPreBuild()
    {
        if (TryGetEnv(PRODUCT_NAME_VAR, out var productName))
        {
            PlayerSettings.productName = productName;
        }
        
        if (TryGetEnv(BUNDLE_ID_VAR, out var bundleId))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleId);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, bundleId);
        }
    }
    
    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        NamedBuildTarget buildTarget = NamedBuildTarget.Android;
        if (build_target == BuildTarget.iOS)
        {
            if (TryGetEnv(BUILD_NUMBER_IOS_VAR, out var bundleVersionNumber))
            {
                PlayerSettings.iOS.buildNumber = bundleVersionNumber;
            }
            buildTarget = NamedBuildTarget.iOS;
        }
        else if (build_target == BuildTarget.Android)
        {
            if (TryGetEnv(BUNDLE_VERSION_ANDROID_VAR, out var bundleVersionNumber) && Int32.TryParse(bundleVersionNumber, out int bundleNumber))
            {
                PlayerSettings.Android.bundleVersionCode = bundleNumber; 
            }
            buildTarget = NamedBuildTarget.Android;
        }
        else if (build_target == BuildTarget.WebGL)
        {
            if (TryGetEnv(BUNDLE_VERSION_WEBGL_VAR, out var bundleVersionNumber) && Int32.TryParse(bundleVersionNumber, out int bundleNumber))
            {
                PlayerSettings.Android.bundleVersionCode = bundleNumber; 
            }
            buildTarget = NamedBuildTarget.WebGL;
        }
        
        var envTarget = EnvironmentVariableTarget.Process;
        if (TryGetEnv(BUILD_TYPE_VAR, out string buildType, envTarget))
        {
            List<string> symbolsArr;
            var DEVELOPMENT = "DEVELOPMENT";
            if (buildType == $"RELEASE")
            {
                var symbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
                symbolsArr = symbols.Split(";").ToList();
                if (symbolsArr.Contains(DEVELOPMENT))
                {
                    symbolsArr.Remove(DEVELOPMENT);
                }
            }
            else
            {
                var symbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
                symbolsArr = symbols.Split(";").ToList();
                if (!symbolsArr.Contains(DEVELOPMENT))
                {
                    symbolsArr.Add(DEVELOPMENT);
                }
            }
            var edittedSymbols = "";
            foreach (var symbol in symbolsArr)
            {
                edittedSymbols += $"{symbol};";
            }
            Debug.Log($"scripting symbols: {edittedSymbols}");
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, edittedSymbols);
        }
        
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        QualitySettings.asyncUploadTimeSlice = 2;

        if (build_target == BuildTarget.iOS)
        {
            if (TryGetEnv(BUILD_NUMBER_IOS_VAR, out var bundleVersionNumber))
            {
                PlayerSettings.iOS.buildNumber = bundleVersionNumber;
            }
        }
        else if (build_target == BuildTarget.Android)
        {
            if (TryGetEnv(BUNDLE_VERSION_ANDROID_VAR, out var bundleVersionNumber) && Int32.TryParse(bundleVersionNumber, out int bundleNumber))
            {
                PlayerSettings.Android.bundleVersionCode = bundleNumber; 
            }
        }
        else
        {
       
        }

        if (TryGetEnv(VERSION_CODE_VAR, out var version_code))
        {
            PlayerSettings.bundleVersion = version_code;
        }

        var propsContents = $"{VERSION_CODE_VAR}={PlayerSettings.bundleVersion}";
        propsContents += $"\n{BUILD_NUMBER_IOS_VAR}={PlayerSettings.iOS.buildNumber}";
        propsContents += $"\n{BUNDLE_VERSION_ANDROID_VAR}={PlayerSettings.Android.bundleVersionCode}";
        
        var propsPath = Application.dataPath.Replace($"/Assets", "");
        propsPath += "/unity_env_vars.properties";
        System.IO.File.WriteAllText(propsPath, propsContents);
        
        Debug.Log($"created unity env vars file: {propsPath}");
        Debug.Log($"versions set from env: {PlayerSettings.bundleVersion}  android: {PlayerSettings.Android.bundleVersionCode}   ios: {PlayerSettings.iOS.buildNumber}");

        EditorUserBuildSettings.development = false;
        build_options &= ~BuildOptions.Development;
                    
        EditorUserBuildSettings.connectProfiler = false;
        EditorUserBuildSettings.allowDebugging = false;
        build_options &= ~BuildOptions.ConnectWithProfiler;
        build_options &= ~BuildOptions.AllowDebugging;
        
        var report = BuildPipeline.BuildPlayer(scenes,target_dir,build_target,build_options);
        var summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            throw new Exception($"Build failed {summary}");
        }
    }

    static bool TryGetEnv(string key, out string value, EnvironmentVariableTarget environmentVariableTarget = EnvironmentVariableTarget.Process)
    {
        value = Environment.GetEnvironmentVariable(key, environmentVariableTarget);
        return !string.IsNullOrEmpty(value);
    }

    static void SetEnv(string key, string value, EnvironmentVariableTarget environmentVariableTarget = EnvironmentVariableTarget.User)
    {
        Environment.SetEnvironmentVariable(key, value, environmentVariableTarget);
    }
}
