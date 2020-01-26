using System;
using System.IO;
using System.Text.RegularExpressions;
using FullscreenEditor;
using UnityEditor;
using UnityEngine;

public static class Automation {

    private const string VERSION_FIELD_FILE = "Assets/Fullscreen/Editor/FullscreenPreferences.cs";
    private const string VERSION_FIELD_REGEX = @"(.*\s+Version\s+pluginVersion\s*=\s*)new\s+Version\((?:.*\d+.*){3}\);";

    private const string DATE_FIELD_FILE = "Assets/Fullscreen/Editor/FullscreenPreferences.cs";
    private const string DATE_FIELD_REGEX = @"(.*\s+DateTime\s+pluginDate\s*=\s*)new\s+DateTime\((?:.*\d+.*){3}\);";

    public static Version ReleasedVersion {
        get {
            var version = Cmd.Run("git describe --abbrev=0").Trim().Substring(1);

            try {
                return new Version(version);
            } catch {
                Debug.LogWarning("Invalid version " + version);
                return FullscreenEditor.FullscreenPreferences.pluginVersion;
            }
        }
    }

    public static Version CurrentVersion {
        get { return FullscreenEditor.FullscreenPreferences.pluginVersion; }
    }

    [MenuItem("Automation/Update Version/Major", false, 100)]
    private static void UpdateMajor() {
        UpdateToVersion(new Version(ReleasedVersion.Major + 1, 0, 0));
    }

    [MenuItem("Automation/Update Version/Minor", false, 100)]
    private static void UpdateMinor() {
        UpdateToVersion(new Version(ReleasedVersion.Major, ReleasedVersion.Minor + 1, 0));
    }

    [MenuItem("Automation/Update Version/Patch", false, 100)]
    private static void UpdatePatch() {
        UpdateToVersion(new Version(ReleasedVersion.Major, ReleasedVersion.Minor, ReleasedVersion.Build + 1));
    }

    // [InitializeOnLoadMethod]
    [MenuItem("Automation/Update Version/Time only", false, 100)]
    private static void UpdateTime() {
        UpdateToVersion(CurrentVersion);
    }

    private static void UpdateToVersion(Version newVersion) {

        var input = File.ReadAllText(VERSION_FIELD_FILE);
        var replacement = string.Format("$1new Version({0}, {1}, {2});", newVersion.Major, newVersion.Minor, newVersion.Build);
        var replaced = Regex.Replace(input, VERSION_FIELD_REGEX, replacement);

        File.WriteAllText(VERSION_FIELD_FILE, replaced);

        input = File.ReadAllText(DATE_FIELD_FILE);
        replacement = string.Format("$1new DateTime({0:0000}, {1:00}, {2:00});", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        replaced = Regex.Replace(input, DATE_FIELD_REGEX, replacement);

        File.WriteAllText(DATE_FIELD_FILE, replaced);

        if (newVersion > CurrentVersion) {
            Debug.Log("Updated version to " + newVersion);
            AssetDatabase.Refresh();
        }

    }

    [InitializeOnLoadMethod]
    private static void RunTestsBeforePackageUpload() {
        AssetStoreToolsCallbacks.beforeUpload += (sender, next) => {
            #if UNITY_2019_2_OR_NEWER
            var dialog = EditorUtility.DisplayDialogComplex(
                "Run Tests",
                "Want to run unit tests before uploading package?",
                "Run tests",
                "Don't run",
                "Cancel"
            );

            switch (dialog) {
                case 0: // Run tests

                    var callbacks = new TestCallbacks() {
                        runFinished = (result) => {
                            var failed = result.FailCount > 0 ||
                                result.AssertCount > 0;

                            if (!failed) {
                                Debug.Log("Tests OK");
                                next();
                            } else {
                                Debug.LogError("Tests failed");
                            }
                        }
                    };

                    TestRunnerCallbacks.RunTests(callbacks);
                    return;
                case 1: // Don't run tests
                    next();
                    return;
                case 2: // Cancel
                    return;
            }
            #else
            if (EditorUtility.DisplayDialog(
                    "Tests not available",
                    "Automated testing not available until Unity 2019.2, make sure to manually run unit tests",
                    "Continue",
                    "Cancel"
                ))
                next();
            #endif
        };
    }

    [InitializeOnLoadMethod]
    private static void CopyPackage() {
        AssetStoreToolsCallbacks.beforeUpload += (sender, next) => {
            if (CurrentVersion == ReleasedVersion) {
                Debug.LogError("The current version is the same as the published version");
                return;
            }

            UpdateTime();
            next();
        };

        if (!Directory.Exists("Releases/"))
            return;
            
        AssetStoreToolsCallbacks.afterPackageExport += (sender, package) => {
            After.Condition(
                () => File.Exists(package),
                () => {
                    var dst = string.Format("Releases/fullscreen-editor-v{0}.unitypackage", CurrentVersion);

                    if (File.Exists(dst)) {
                        if (new FileInfo(dst).Length == new FileInfo(package).Length)
                            return;

                        File.Delete(dst);
                    }

                    File.Copy(package, dst);
                    Cmd.Run("git add {0}", dst);
                    Debug.LogFormat("Copied {0} to {1}", package, dst);
                },
                60000
            );
        };
    }
}