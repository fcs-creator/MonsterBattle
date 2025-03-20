using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterSummonWindow : EditorWindow
{
    private Texture2D brazeTexture;
    private Texture2D glaciaTexture;
    private Texture2D elderTexture;

    private GUIStyle buttonStyle;

    [MenuItem("Window/Monster Battle")]
    public static void ShowWindow()
    {
        GetWindow<MonsterSummonWindow>("ユニモンバトル");
    }

    private void OnGUI()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 20;
        }

        GUILayout.Space(30);

        GUILayout.Label("モンスター召喚", EditorStyles.boldLabel);

        defaultMonsterList();
        GUILayout.Space(10);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("モンスター一覧", EditorStyles.boldLabel);
        GUILayout.Space(10);

        monsterList();
        GUILayout.Space(10);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("対戦相手選択", EditorStyles.boldLabel);
        enemyList();

    }

    private Vector2 monsterScrollPosition;

    private void monsterList()
    {
        monsterScrollPosition = EditorGUILayout.BeginScrollView(monsterScrollPosition, GUILayout.Height(300));

        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

        foreach (var monster in monsters)
        {
            if (monster.EnemyLevel != 0)
            {
                continue;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (monster.Sprite != null)
            {
                GUILayout.Label(monster.Sprite.sprite.texture, GUILayout.Width(80), GUILayout.Height(80));
            }
            else
            {
                GUILayout.Label("No Sprite", GUILayout.Width(80), GUILayout.Height(80));
            }


            if (GUILayout.Button(monster.name, buttonStyle, GUILayout.Height(80)))
            {
                // 対象のゲームオブジェクトを選択
                Selection.activeGameObject = monster.gameObject;
                EditorGUIUtility.PingObject(monster.gameObject);
            }

            // 削除ボタン
            if (GUILayout.Button("削除", GUILayout.Width(40), GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("確認", monster.name + " を本当に削除しますか？", "OK", "キャンセル"))
                {
                    DestroyImmediate(monster.gameObject);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();

    }

    private void defaultMonsterList()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        // Assets\MosterBattle\Textures\Monster\Braze.png を表示
        if (brazeTexture == null)
        {
            brazeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MosterBattle/Textures/Monster/Braze.png");
        }

        GUILayout.Label(brazeTexture, GUILayout.Width(30), GUILayout.Height(30));
        if (GUILayout.Button("Braze を召喚", GUILayout.Height(30)))
        {
            OpenSummonWindow("Braze");
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        // Assets\MosterBattle\Textures\Monster\Glacia.png を表示
        if (glaciaTexture == null)
        {
            glaciaTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MosterBattle/Textures/Monster/Glacia.png");
        }

        GUILayout.Label(glaciaTexture, GUILayout.Width(30), GUILayout.Height(30));
        if (GUILayout.Button("Glacia を召喚", GUILayout.Height(30)))
        {
            OpenSummonWindow("Glacia");
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        // Assets\MosterBattle\Textures\Monster\Elder.png を表示
        if (elderTexture == null)
        {
            elderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MosterBattle/Textures/Monster/Elder.png");
        }

        GUILayout.Label(elderTexture, GUILayout.Width(30), GUILayout.Height(30));
        if (GUILayout.Button("Elder を召喚", GUILayout.Height(30)))
        {
            OpenSummonWindow("Elder");
        }

        GUILayout.EndHorizontal();

    }

    private Monster selectedEnemy = null;
    private Vector2 enemyScrollPosition;

    private void enemyList()
    {
        enemyScrollPosition = EditorGUILayout.BeginScrollView(enemyScrollPosition, GUILayout.Height(300));

        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

        List<Monster> enemyList = new List<Monster>();
        enemyList.AddRange(monsters);
        enemyList.Sort((a, b) => a.EnemyLevel - b.EnemyLevel);

        foreach (var monster in enemyList)
        {
            if (monster.EnemyLevel == 0 || monster.Sprite == null)
            {
                continue;
            }

            // 一回非選択にするよ
            if (selectedEnemy != monster)
            {
                monster.gameObject.SetActive(false);
            }

            GUILayout.Label("【 Level：" + monster.EnemyLevel + " 】" + monster.name, EditorStyles.boldLabel);
            GUILayout.Label(monster.Sprite.sprite.texture, GUILayout.Width(120), GUILayout.Height(120));

            if (monster.gameObject.activeSelf)
            {
                EditorGUILayout.HelpBox("選択中", MessageType.Info);
                GUILayout.Space(8);
            }
            else
            {
                if (GUILayout.Button("戦う", buttonStyle, GUILayout.Height(30)))
                {
                    monster.gameObject.SetActive(true);
                    if (selectedEnemy) selectedEnemy.gameObject.SetActive(false);
                    selectedEnemy = monster;
                }
            }

            GUILayout.Space(8);
        }

        EditorGUILayout.EndScrollView();
    }

    private void OpenSummonWindow(string monsterType)
    {
        var window = GetWindow<MonsterSummonChildWindow>("召喚ウィンドウ");
        window.monsterType = monsterType;
        window.minSize = new Vector2(300, 200);
        window.maxSize = new Vector2(300, 200);

        window.ShowUtility();
    }

}
