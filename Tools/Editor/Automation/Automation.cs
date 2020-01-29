using System.IO;
using UnityEditor;
using UnityEngine;

namespace SharedTools {
    public static class Automation {

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
                if (Versioning.CurrentVersion == Versioning.ReleasedVersion) {
                    Debug.LogError("The current version is the same as the published version");
                    return;
                }

                Versioning.UpdateTime();
                next();
            };

            if (!Directory.Exists("Releases/"))
                return;

            AssetStoreToolsCallbacks.afterPackageExport += (sender, package) => {
                After.Condition(
                    () => File.Exists(package),
                    () => {
                        var dst = string.Format("Releases/{1}-v{0}.unitypackage", Versioning.CurrentVersion, SharedToolsMetadata.ST_PLUGIN_SLUG);

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
}
