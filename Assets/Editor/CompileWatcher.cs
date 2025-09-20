using System.Diagnostics;
using UnityEditor;
using UnityEditor.Compilation;
using Debug = UnityEngine.Debug;

[InitializeOnLoad]
public static class CompileWatcher
{
    private static Stopwatch stopwatch;

    static CompileWatcher()
    {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationStarted(object obj)
    {
        stopwatch = Stopwatch.StartNew();
        Debug.Log("⏳ Compilation started...");
    }

    private static void OnCompilationFinished(object obj)
    {
        stopwatch.Stop();
        Debug.Log($"✅ Compilation finished in {stopwatch.ElapsedMilliseconds} ms");
    }
}