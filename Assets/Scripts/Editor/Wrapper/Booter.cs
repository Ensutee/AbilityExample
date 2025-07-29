using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class Booter
{
    static Booter()
    {
        ShowWindow();
    }

    private static async void ShowWindow()
    {
        while (!EditorWindow.HasOpenInstances<SceneView>())
        {
            await Task.Yield();
            if (Application.exitCancellationToken.IsCancellationRequested) return;
        }
        
        SheetDemo.OpenWindow();
    }
}