using System.IO;
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
            if (CommonUtilities.IsWindows)
                Cmd.Run("start bash");
            else
                Cmd.Run("open /bin/bash");
        }

        [MenuItem("Common Helpers/>>Run/Explorer", false, -1000)]
        private static void OpenExplorer() {
            if (CommonUtilities.IsWindows)
                Cmd.Run("start .");
            else
                Cmd.Run("open .");
        }

        [MenuItem("Common Helpers/>>Run/VS Code", false, -1000)]
        private static void OpenVSCode() {
            Cmd.Run("code .");
        }

    }
}
