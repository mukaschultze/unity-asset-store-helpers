using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SharedTools {
    public static class Versioning {

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
            get { return SharedToolsMetadata.ST_PLUGIN_VERSION; }
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

            var input = File.ReadAllText(SharedToolsMetadata.ST_VERSION_FIELD_FILE);
            var replacement = string.Format("$1new Version({0}, {1}, {2});", newVersion.Major, newVersion.Minor, newVersion.Build);
            var replaced = Regex.Replace(input, SharedToolsMetadata.ST_VERSION_FIELD_REGEX, replacement);

            File.WriteAllText(SharedToolsMetadata.ST_VERSION_FIELD_FILE, replaced);

            input = File.ReadAllText(SharedToolsMetadata.ST_DATE_FIELD_FILE);
            replacement = string.Format("$1new DateTime({0:0000}, {1:00}, {2:00});", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            replaced = Regex.Replace(input, SharedToolsMetadata.ST_DATE_FIELD_REGEX, replacement);

            File.WriteAllText(SharedToolsMetadata.ST_DATE_FIELD_FILE, replaced);

            if (newVersion > CurrentVersion) {
                Debug.Log("Updated version to " + newVersion);
                AssetDatabase.Refresh();
            }

        }
    }
}
