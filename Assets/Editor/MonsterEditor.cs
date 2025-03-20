using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Monster), true)]
public class MonsterEditor : Editor
{

    private static bool showDetail = false;

    public override void OnInspectorGUI()
    {

        Monster monster = (Monster)target;

        if (!showDetail)
        {
            DrawCustomInspector(monster);
        }
        else
        {
            // 通常のインスペクタ表示
            DrawDefaultInspector();
        }

        // 詳細モードボタン
        if (GUILayout.Button(showDetail ? "詳細" : "もどる"))
        {
            showDetail = !showDetail;
        }

    }

    private void DrawCustomInspector(Monster monster)
    {

        serializedObject.Update();

        GUILayout.Label("【 モンスターの設定 】", EditorStyles.boldLabel);

        string inputName = EditorGUILayout.TextField("名前", monster.name);
        if (inputName != "")
        {
            monster.name = inputName;
        }

        EditorGUILayout.LabelField("見た目");

        // monsterのスプライトを表示、設定する
        // ObjectFieldの固定サイズ（幅300px, 高さ100px）
        Rect fixedRect = new Rect(GUILayoutUtility.GetLastRect().xMax - 75 - 10, GUILayoutUtility.GetLastRect().yMax + 5, 75, 75); // インスペクタ内の固定位置

        if (monster.Sprite == null)
        {
            EditorGUILayout.HelpBox("Spriteが設定されていません", MessageType.Error);
        }
        else
        {
            // monsterのスプライトを表示、設定する
            monster.Sprite.sprite = (Sprite)EditorGUILayout.ObjectField("ユニット画像", monster.Sprite.sprite, typeof(Sprite), false);
        }

        // 変更を保存
        serializedObject.ApplyModifiedProperties();

    }

}
