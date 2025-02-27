using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SortLayerStaticClassGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate SortLayer Class")]
    public static void GenerateSortLayerClass()
    {
        // TagManager.asset�t�@�C�����擾
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

        // �N���X���e���쐬
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

        // �X�N���v�g�t�@�C�����쐬
        string path = "Assets/Library/Helper/SortLayer.cs";
        File.WriteAllText(path, classContent.ToString(), Encoding.UTF8);

        // �A�Z�b�g�f�[�^�x�[�X�����t���b�V��
        AssetDatabase.Refresh();
        Debug.Log("SortLayer�N���X�𐶐����܂���: " + path);
    }
}
