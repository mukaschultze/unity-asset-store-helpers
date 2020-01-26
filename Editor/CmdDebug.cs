using System.IO;
using FullscreenEditor;
using UnityEditor;
using UnityEngine;

namespace SharedTools {
    public static class CmdDebug {

        [MenuItem("Common Helpers/>>Run/CMD", false, -1000)]
        private static void OpenCmd() {
            Cmd.Run("start cmd");
        }

        [MenuItem("Common Helpers/>>Run/Bash", false, -1000)]
        private static void OpenBash() {
            if (FullscreenUtility.IsWindows)
                Cmd.Run("start bash");
            else
                Cmd.Run("open /bin/bash");
        }

        [MenuItem("Common Helpers/>>Run/Explorer", false, -1000)]
        private static void OpenExplorer() {
            if (FullscreenUtility.IsWindows)
                Cmd.Run("start .");
            else
                Cmd.Run("open .");
        }

        [MenuItem("Common Helpers/>>Run/VS Code", false, -1000)]
        private static void OpenVSCode() {
            Cmd.Run("code .");
        }

        [MenuItem("Common Helpers/>>Run/Link to Project")]
        private static void LinkProject() {

            if (!FullscreenUtility.IsWindows) {
                Debug.LogError("Not supported on this platform");
                return;
            }

            var link = EditorUtility.SaveFilePanel("Select folder to save link", "../Assets", "Fullscreen", "");
            var target = Path.GetFullPath("Assets/Fullscreen");

            if (string.IsNullOrEmpty(link))
                return;

            Cmd.Run("mklink /D \"{0}\" \"{1}\"", true, link, target);
        }

        [MenuItem("Common Helpers/>>Run/Tag 'n' Push Current Version")]
        private static void TagCurrentVersion() {
            var version = Automation.CurrentVersion;

            if (!EditorUtility.DisplayDialog("Tag 'n' Push", "Are you sure you want to tag and push the current commit? This will cause a release on GitHub by Travis CI", "Tag", "Cancel"))
                return;

            Cmd.Run("git push origin :refs/tags/v{0}", version);
            Cmd.Run("git tag -f -a -m \"Version {0}\" v{0}", version);
            Cmd.Run("git push --tags"); // This will cause a release in Travis CI
        }

    }
}
