using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Monster), true)]
public class MonsterEditor : Editor
{

    private static bool showDetail = false;

    [SerializeField]
    private CreateNewWeaponScript weaponScript = null;

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

        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(5);

        // 詳細モードボタン
        if (GUILayout.Button(showDetail ? "もどる" : "詳細"))
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

        // monsterのスプライトを表示、設定する
        if (monster.Sprite == null)
        {
            EditorGUILayout.HelpBox("Spriteが設定されていません", MessageType.Error);
        }
        else
        {
            // monsterのスプライトを表示、設定する
            monster.Sprite.sprite = (Sprite)EditorGUILayout.ObjectField("見た目", monster.Sprite.sprite, typeof(Sprite), false);
        }

        // プログラムするボタン
        GUILayout.Space(10);
        if (GUILayout.Button("モンスターにプログラムする"))
        {
            // Scriptファイルを開く
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(monster.ScriptFile);
            AssetDatabase.OpenAsset(script);

        }

        GUILayout.Space(5);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(5);

        GUILayout.Label("【 武器の設定 】", EditorStyles.boldLabel);

        drawWeaponList(monster);

        // 変更を保存
        serializedObject.ApplyModifiedProperties();

    }

    private void drawWeaponList(Monster monster)
    {
        foreach (var weapon in monster.Weapons)
        {
            GUILayout.BeginHorizontal();

            // 武器のスプライトを表示、設定する
            if (weapon.Sprite == null)
            {
                EditorGUILayout.HelpBox("Spriteが設定されていません", MessageType.Error);
            }
            else
            {
                weapon.Sprite.sprite = (Sprite)EditorGUILayout.ObjectField("見た目", weapon.Sprite.sprite, typeof(Sprite), false);
            }

            if (GUILayout.Button("武器にプログラムする"))
            {
                // Scriptファイルを開く
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(weapon.ScriptFile);
                AssetDatabase.OpenAsset(script);
            }

            GUILayout.EndHorizontal();
        }

        // 武器追加ボタン
        GUILayout.Space(5);
        var isLoadingWeapon = weaponScript == null || string.IsNullOrEmpty(weaponScript.WeaponScriptName);

        if (isLoadingWeapon)
        {
            if (GUILayout.Button("武器を追加する"))
            {
                weaponScript = new CreateNewWeaponScript();
                weaponScript.Create(monster.name);

                AssetDatabase.ImportAsset(weaponScript.WeaponScriptName, ImportAssetOptions.ForceUpdate);
            }
        }

        if (!isLoadingWeapon && !weaponScript.IsScriptLoaded)
        {
            EditorGUILayout.HelpBox("読み込み中", MessageType.Info);
        }

        if (!isLoadingWeapon && weaponScript.IsScriptLoaded)
        {
            if (GUILayout.Button("武器を作成！"))
            {
                CreateNewWeaponScript.CreateWeaponObject(monster, weaponScript.WeaponScriptName);
                weaponScript = null;
            }
        }


    }

}
