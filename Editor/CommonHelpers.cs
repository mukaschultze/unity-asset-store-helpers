using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace SharedTools {
    public static class CommonHelpers {

        [MenuItem("Fullscreen Debug/Assembly reload _F5")]
        private static void AssemblyReload() {
            Integration.RequestScriptReload();
            Debug.Log("Reloaded");
        }

        [MenuItem("Fullscreen Debug/Restart Editor")]
        public static void RestartEditor() {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorApplication.OpenProject(Environment.CurrentDirectory);
        }

        [MenuItem("Fullscreen Debug/Change skin")]
        private static void ChangeSkin() {
            InternalEditorUtility.SwitchSkinAndRepaintAllViews();
        }

        [MenuItem("Fullscreen Debug/Log/Selected File Base64")]
        private static void LogFileBytes() {
            var active = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(active);
            var bytes = File.ReadAllBytes(path);
            var base64 = Convert.ToBase64String(bytes);

            Debug.LogFormat("Base64 of {0} copied to clipboard\n{1}", active.name, base64);
            EditorGUIUtility.systemCopyBuffer = base64;
        }
    }
}
