using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SharedTools {
    public static class Versioning {

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
                    return CurrentVersion;
                }
            }
        }

        public static Version CurrentVersion {
            get { return Version.Parse(PlayerPrefs.GetString("PLUGIN_VERSION", "0.0.0")); }
        }

        [MenuItem("Common Helpers/Update Version/Major", false, 100)]
        public static void UpdateMajor() {
            UpdateToVersion(new Version(ReleasedVersion.Major + 1, 0, 0));
        }

        [MenuItem("Common Helpers/Update Version/Minor", false, 100)]
        public static void UpdateMinor() {
            UpdateToVersion(new Version(ReleasedVersion.Major, ReleasedVersion.Minor + 1, 0));
        }

        [MenuItem("Common Helpers/Update Version/Patch", false, 100)]
        public static void UpdatePatch() {
            UpdateToVersion(new Version(ReleasedVersion.Major, ReleasedVersion.Minor, ReleasedVersion.Build + 1));
        }

        // [InitializeOnLoadMethod]
        [MenuItem("Common Helpers/Update Version/Time only", false, 100)]
        public static void UpdateTime() {
            UpdateToVersion(CurrentVersion);
        }

        public static void UpdateToVersion(Version newVersion) {

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
    }
}
