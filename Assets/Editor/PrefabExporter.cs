using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PrefabExporter : EditorWindow
{
    private GameObject prefabToExport;

    [MenuItem("Tools/Prefab Exporter")]
    public static void ShowWindow()
    {
        GetWindow<PrefabExporter>("Prefab Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export Prefab and Dependencies", EditorStyles.boldLabel);

        prefabToExport = (GameObject)EditorGUILayout.ObjectField("Prefab to Export", prefabToExport, typeof(GameObject), true);

        if (GUILayout.Button("Export as UnityPackage"))
        {
            if (prefabToExport != null)
            {
                ExportPrefabWithDependencies();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a prefab to export.", "OK");
            }
        }
    }

    private void ExportPrefabWithDependencies()
    {
        // プレハブが選択されていない場合はスキップ
        if (prefabToExport == null)
        {
            Debug.LogError("No prefab assigned!");
            return;
        }

        // プレハブとその依存関係を収集
        string prefabPath = AssetDatabase.GetAssetPath(prefabToExport);
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("Could not find the prefab path!");
            return;
        }

        // 依存関係を取得
        string[] dependencies = AssetDatabase.GetDependencies(prefabPath);

        // 保存場所をユーザーに指定させる
        string savePath = EditorUtility.SaveFilePanel("Save UnityPackage", "", prefabToExport.name + ".unitypackage", "unitypackage");

        if (string.IsNullOrEmpty(savePath))
            return;

        // パッケージをエクスポート
        AssetDatabase.ExportPackage(dependencies, savePath, ExportPackageOptions.Interactive);

        Debug.Log($"Prefab and dependencies exported to: {savePath}");
        EditorUtility.DisplayDialog("Success", "Prefab and dependencies exported successfully!", "OK");
    }
}
