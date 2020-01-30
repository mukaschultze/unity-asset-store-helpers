using UnityEditor;

namespace SharedTools {
    public static class MultipleVersions {

        private const string USERNAME = "";
        private const string PASSWORD = "";

        private static readonly string[] versions = new [] {
            // @"C:\Unity\5.6.0f3\Editor\Unity.exe",
            @"C:\Unity\2017.1.0f3\Editor\Unity.exe",
            @"C:\Unity\2018.1.0f2\Editor\Unity.exe",
            @"C:\Unity\2019.1.0f2\Editor\Unity.exe"
        };

        private static readonly string[] foldersToLink = new [] {
            // Must be root folder
            SharedToolsMetadata.ST_ROOT_FOLDER.Replace("/", "\\"),
            @"Assets\Editor",
            @"Assets\Tests",
            @"Assets\SharedTools",
        };

        [MenuItem("Common Helpers/Open With/5.6.0f3", false, 1000 * 5)]
        private static void OpenWith_5_6_0f3() { OpenWith(@"C:\Unity\5.6.0f3\Editor\Unity.exe"); }

        [MenuItem("Common Helpers/Open With/2017.1.0f3", false, 1000 * 2017)]
        private static void OpenWith_2017_1_0f3() { OpenWith(@"C:\Unity\2017.1.0f3\Editor\Unity.exe"); }

        [MenuItem("Common Helpers/Open With/2018.1.0f2", false, 1000 * 2018)]
        private static void OpenWith_2018_1_0f2() { OpenWith(@"C:\Unity\2018.1.0f2\Editor\Unity.exe"); }

        [MenuItem("Common Helpers/Open With/2019.1.0f2", false, 1000 * 2019)]
        private static void OpenWith_2019_1_0f2() { OpenWith(@"C:\Unity\2019.1.0f2\Editor\Unity.exe"); }

        [MenuItem("Common Helpers/Open With/Open Multiple Versions", false, -1000)]
        private static void OpenMultiple() {
            foreach (var v in versions)
                OpenWith(v);
        }

        private static void OpenWith(string unity) {
            var projPath = Cmd.Run("cd") + string.Format(@"\Cloned\{0}", unity.GetHashCode());

            Cmd.Run("if exist \"{0}\" rmdir /S /Q \"{0}\"", projPath);
            Cmd.Run("mkdir \"{0}\"", projPath);
            Cmd.Run("mkdir \"{0}\\Assets\"", projPath);
            foreach (var folder in foldersToLink)
                Cmd.Run("mklink /J \"{0}\\{1}\" \"{1}\"", projPath, folder);
            Cmd.Run("\"{0}\" -projectPath \"{1}\" -disable-assembly-updater -username \"{2}\" -password \"{3}\"", unity, projPath, USERNAME, PASSWORD);
            Cmd.Run("if exist \"{0}\" rmdir /S /Q \"{0}\"", projPath);

        }
    }
}
