using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SortLayerStaticClassGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate SortLayer Class")]
    public static void GenerateSortLayerClass()
    {
        // TagManager.assetファイルを取得
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

        // クラス内容を作成
        StringBuilder classContent = new StringBuilder();
        classContent.AppendLine("public static class SortLayer");
        classContent.AppendLine("{");

        for (int i = 0; i < sortingLayers.arraySize; i++)
        {
            SerializedProperty layer = sortingLayers.GetArrayElementAtIndex(i);
            string layerName = layer.FindPropertyRelative("name").stringValue;
            classContent.AppendLine($"    public const string {layerName} = \"{layerName}\";");
        }

        classContent.AppendLine("}");

        // スクリプトファイルを作成
        string path = "Assets/Library/Helper/SortLayer.cs";
        File.WriteAllText(path, classContent.ToString(), Encoding.UTF8);

        // アセットデータベースをリフレッシュ
        AssetDatabase.Refresh();
        Debug.Log("SortLayerクラスを生成しました: " + path);
    }
}
