using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace SharedTools {
    public static class CommonHelpers {

        [MenuItem("Common Helpers/Assembly reload _F5")]
        private static void AssemblyReload() {
            Integration.RequestScriptReload();
            Debug.Log("Reloaded");
        }

        [MenuItem("Common Helpers/Restart Editor")]
        public static void RestartEditor() {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorApplication.OpenProject(Environment.CurrentDirectory);
        }

        [MenuItem("Common Helpers/Change skin")]
        private static void ChangeSkin() {
            InternalEditorUtility.SwitchSkinAndRepaintAllViews();
        }

        [MenuItem("Common Helpers/Selected File Base64 to clipboard")]
        private static void LogFileBytes() {
            var active = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(active);
            var bytes = File.ReadAllBytes(path);
            var base64 = Convert.ToBase64String(bytes);

            Debug.LogFormat("Base64 of {0} copied to clipboard\n{1}", active.name, base64);
            EditorGUIUtility.systemCopyBuffer = base64;
        }

        [MenuItem("Common Helpers/Link to Project")]
        private static void LinkProject() {

            if (!CommonUtilities.IsWindows) {
                Debug.LogError("Not supported on this platform");
                return;
            }

            var link = EditorUtility.SaveFilePanel("Select folder to save link", "../Assets", SharedToolsMetadata.ST_PLUGIN_NAME, "");
            var target = Path.GetFullPath(SharedToolsMetadata.ST_ROOT_FOLDER);

            if (string.IsNullOrEmpty(link))
                return;

            Cmd.Run("mklink /D \"{0}\" \"{1}\"", true, link, target);
        }

        [MenuItem("Common Helpers/Tag 'n' Push Current Version")]
        private static void TagCurrentVersion() {
            var version = Versioning.CurrentVersion;

            if (!EditorUtility.DisplayDialog("Tag 'n' Push", "Are you sure you want to tag and push the current commit? This will cause a release on GitHub by Travis CI", "Tag", "Cancel"))
                return;

            Cmd.Run("git push origin :refs/tags/v{0}", version);
            Cmd.Run("git tag -f -a -m \"Version {0}\" v{0}", version);
            Cmd.Run("git push --tags"); // This will cause a release in Travis CI
        }

    }
}
