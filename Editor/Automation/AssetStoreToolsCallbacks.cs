using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace SharedTools {
    public class AssetStoreToolsCallbacks {

        public static Action<object, string> afterAssetsUploaded = (sender, err) => { };
        public static Action<object, string> afterAssetsBundleUploaded = (sender, err) => { };

        public static Action<object, string> beforePackageExport = (sender, packagePath) => { };
        public static Action<object, string> afterPackageExport = (sender, packagePath) => { };

        public static Action<object, Action> beforeUpload = (sender, next) => { next(); };
        public static Action<object> afterUpload = (sender) => { };

        public static Action<object> beforeBundleUpload = (sender) => { };
        public static Action<object> afterBundleUpload = (sender) => { };

        public static Action<object, double, double> onUploading = (sender, pctUp, pctDown) => { };

        public static Action<object> uploadSucessfull = (sender) => { };
        public static Action<object> uploadFailed = (sender) => { };

        // [InitializeOnLoadMethod]
        // private static void AddLogs() {
        //     afterAssetsUploaded += (sender, err) => UnityEngine.Debug.LogFormat("afterAssetsUploaded: {0}", err);
        //     afterAssetsBundleUploaded += (sender, err) => UnityEngine.Debug.LogFormat("afterAssetsBundleUploaded: {0}", err);

        //     beforePackageExport += (sender, packagePath) => UnityEngine.Debug.LogFormat("beforePackageExport: {0}", packagePath);
        //     afterPackageExport += (sender, packagePath) => UnityEngine.Debug.LogFormat("afterPackageExport: {0}", packagePath);

        //     beforeUpload += (sender) => UnityEngine.Debug.Log("beforeUpload");
        //     afterUpload += (sender) => UnityEngine.Debug.Log("afterUpload");

        //     beforeBundleUpload += (sender) => UnityEngine.Debug.Log("beforeBundleUpload");
        //     afterBundleUpload += (sender) => UnityEngine.Debug.Log("afterBundleUpload");

        //     onUploading += (sender, pctUp, pctDown) => UnityEngine.Debug.LogFormat("onUploading {0:00.00} {1:00.00}", pctUp, pctDown);

        //     uploadSucessfull += (sender) => UnityEngine.Debug.Log("uploadSucessfull");
        //     uploadFailed += (sender) => UnityEngine.Debug.Log("uploadFailed");
        // }

        public const BindingFlags FULL_BINDING = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static Assembly[] cachedAssemblies;
        private static readonly Dictionary<string, Patcher> patchers = new Dictionary<string, Patcher>();

        public static Type FindClass(string name) {
            var result = FindTypeInAssembly(name, typeof(Editor).Assembly);

            if (result != null)
                return result;

            if (cachedAssemblies == null)
                cachedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (var i = 0; i < cachedAssemblies.Length; i++) {
                result = FindTypeInAssembly(name, cachedAssemblies[i]);

                if (result != null)
                    return result;
            }

            return result;
        }

        private static Type FindTypeInAssembly(string name, Assembly assembly) {
            return assembly == null ?
                null :
                assembly.GetType(name, false, true);
        }

        public static MethodInfo FindMethod(Type type, string methodName) {
            return type.GetMethod(methodName, FULL_BINDING);
        }

        static AssetStoreToolsCallbacks() {
            var AssetStorePackageController = FindClass("AssetStorePackageController");

            var methods = new [] {
                "OnAssetsUploaded",
                // "OnUploadAssetBundlesFinished",
                "Export",
                "Upload",
                // "UploadAssetBundles",
                "OnAssetsUploading",
                "OnUploadSuccessfull",
                "OnSubmitionFail",
            };

            foreach (var method in methods) {
                var source = FindMethod(AssetStorePackageController, method);
                var target = FindMethod(typeof(AssetStoreToolsCallbacks), method);

                if (source == null)
                    Debug.LogErrorFormat("Source method not found in {1}: {0}", method, AssetStorePackageController.Name);
                if (target == null)
                    Debug.LogErrorFormat("Target method not found in {1}: {0}", method, typeof(AssetStoreToolsCallbacks).Name);

                try {
                    patchers[method] = new Patcher(source, target);
                    patchers[method].SwapMethods();
                } catch (Exception ex) {
                    Debug.LogException(ex);
                    Debug.LogError("Failed to patch method " + method);
                }
            }
        }

        private static Patcher GetCurrentPatcher() {
            var frame = new StackFrame(1, true);
            var methodName = frame.GetMethod().Name;
            return patchers[methodName];
        }

        private void OnAssetsUploaded(string errorMessage) {
            GetCurrentPatcher().InvokeOriginal(this, errorMessage);
            afterAssetsUploaded(this, errorMessage);
        }

        private void OnUploadAssetBundlesFinished(string errorMessage) {
            GetCurrentPatcher().InvokeOriginal(this, errorMessage);
            afterAssetsBundleUploaded(this, errorMessage);
        }

        private void Export(string packagePath) {
            beforePackageExport(this, packagePath);
            GetCurrentPatcher().InvokeOriginal(this, packagePath);
            afterPackageExport(this, packagePath);
        }

        private void Upload() {
            var patcher = GetCurrentPatcher();
            var requiredCount = beforeUpload.GetInvocationList().Length;
            var currentCount = 0;

            beforeUpload(this, () => {
                if (++currentCount != requiredCount)
                    return;

                patcher.InvokeOriginal(this, null);
                afterUpload(this);
            });
        }

        private void UploadAssetBundles() {
            beforeBundleUpload(this);
            GetCurrentPatcher().InvokeOriginal(this, null);
            afterBundleUpload(this);
        }

        private void OnAssetsUploading(double pctUp, double pctDown) {
            onUploading(this, pctUp, pctDown);
            GetCurrentPatcher().InvokeOriginal(this, pctUp, pctDown);
        }

        private void OnUploadSuccessfull() {
            GetCurrentPatcher().InvokeOriginal(this, null);
            uploadSucessfull(this);
        }

        private void OnSubmitionFail() {
            GetCurrentPatcher().InvokeOriginal(this, null);
            uploadFailed(this);
        }

    }
}
