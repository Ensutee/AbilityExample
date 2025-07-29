using UnityEditor;


[InitializeOnLoad]
public class Booter
{
    static Booter()
    {
        SheetDemo.OpenWindow();
    }
}