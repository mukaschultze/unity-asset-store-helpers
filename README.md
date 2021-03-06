# Asset Store shared tools

## Setup

Add the following code to a new file, with the correct info for your project:

```cs
using UnityEditor;

namespace SharedTools {
    [InitializeOnLoad]
    public static class SharedToolsSetup {
        static SharedToolsSetup() {
            #if SHARED_TOOLS
            SharedToolsMetadata.ST_PLUGIN_VERSION = EnhancedHierarchy.Preferences.pluginVersion;
            SharedToolsMetadata.ST_PLUGIN_SLUG = "enhanced-hierarchy";
            SharedToolsMetadata.ST_PLUGIN_NAME = "Enhanced Hierarchy";
            SharedToolsMetadata.ST_ROOT_FOLDER = "Assets/Enhanced Hierarchy";

            SharedToolsMetadata.ST_VERSION_FIELD_FILE = "Assets/Enhanced Hierarchy/Editor/PreferencesGUI.cs";
            SharedToolsMetadata.ST_VERSION_FIELD_REGEX = @"(.*\s+Version\s+pluginVersion\s*=\s*)new\s+Version\((?:.*\d+.*){3}\);";

            SharedToolsMetadata.ST_DATE_FIELD_FILE = "Assets/Enhanced Hierarchy/Editor/PreferencesGUI.cs";
            SharedToolsMetadata.ST_DATE_FIELD_REGEX = @"(.*\s+DateTime\s+pluginDate\s*=\s*)new\s+DateTime\((?:.*\d+.*){3}\);";
            #endif
        }
    }
}
```
