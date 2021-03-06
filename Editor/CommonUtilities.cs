using UnityEngine;

namespace SharedTools {
    public static class CommonUtilities {

        public static bool IsWindows { get { return Application.platform == RuntimePlatform.WindowsEditor; } }
        public static bool IsMacOS { get { return Application.platform == RuntimePlatform.OSXEditor; } }
        public static bool IsLinux { get { return Application.platform == RuntimePlatform.LinuxEditor; } }

    }
}
