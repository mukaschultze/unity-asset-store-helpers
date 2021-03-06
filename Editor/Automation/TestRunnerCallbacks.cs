#if UNITY_2019_2_OR_NEWER
using System;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public class TestCallbacks : ICallbacks {
    public Action<ITestResultAdaptor> runFinished = (result) => { };
    public Action<ITestAdaptor> runStarted = (testsToRun) => { };
    public Action<ITestResultAdaptor> testFinished = (result) => { };
    public Action<ITestAdaptor> testStarted = (test) => { };
    public void RunFinished(ITestResultAdaptor result) { runFinished(result); }
    public void RunStarted(ITestAdaptor testsToRun) { runStarted(testsToRun); }
    public void TestFinished(ITestResultAdaptor result) { testFinished(result); }
    public void TestStarted(ITestAdaptor test) { testStarted(test); }
}

public static class TestRunnerCallbacks {

    private static readonly TestCallbacks defaultCallbacks = new TestCallbacks() {
        runFinished = (result) => {
            Debug.LogFormat("Tests: {0} passed, {1} failed, {2} asserted, {3} skipped and {4} inconclusives",
                result.PassCount,
                result.FailCount,
                result.AssertCount,
                result.SkipCount,
                result.InconclusiveCount
            );
        }
    };

    [MenuItem("Common Helpers/Tests/Edit Mode", false, 300)]
    private static void RunEditModeTests() {
        RunTests(defaultCallbacks, TestMode.EditMode);
    }

    [MenuItem("Common Helpers/Tests/Play Mode", false, 300)]
    private static void RunPlayModeTests() {
        RunTests(defaultCallbacks, TestMode.PlayMode);
    }

    public static void RunTests(TestCallbacks callbacks, TestMode testModeToRun = TestMode.EditMode) {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter() { testMode = testModeToRun };
        var executionSettings = new ExecutionSettings(filter);

        testRunnerApi.RegisterCallbacks(callbacks);
        testRunnerApi.Execute(executionSettings);
    }
}
#endif
