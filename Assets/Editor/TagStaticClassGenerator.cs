using System.IO;
using UnityEditor;

public class TagStaticClassGenerator : AssetPostprocessor
{
    private static readonly string filePath = "Assets/Library/Helper/Tags.cs";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        GenerateTagClass();
    }

    [MenuItem("Tools/Generate Tag Class")]
    public static void GenerateTagClass()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("public static class Tags {");
            foreach (string tag in tags)
            {
                writer.WriteLine($"    public const string {tag.Replace(" ", "_")} = \"{tag}\";");
            }
            writer.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
}
