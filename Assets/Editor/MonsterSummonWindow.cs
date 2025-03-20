using UnityEditor;
using UnityEngine;

public class MonsterSummonWindow : EditorWindow
{
    private Texture2D brazeTexture;
    private Texture2D glaciaTexture;
    private Texture2D elderTexture;

    private GUIStyle buttonStyle;

    [MenuItem("Window/Monster Summon")]
    public static void ShowWindow()
    {
        GetWindow<MonsterSummonWindow>("Monster Summon");
    }

    private void OnGUI()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 20;
        }

        GUILayout.Space(30);

        GUILayout.Label("モンスター一覧", EditorStyles.boldLabel);
        GUILayout.Space(10);

        monsterList();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("モンスター召喚", EditorStyles.boldLabel);

        GUILayout.Space(10);

        defaultMonsterList();
    }

    private void monsterList()
    {
        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

        foreach (var monster in monsters)
        {
            if (monster.IsEnemy)
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

            GUILayout.EndHorizontal();
        }

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

        GUILayout.Label(brazeTexture, GUILayout.Width(50), GUILayout.Height(50));
        if (GUILayout.Button("Braze を召喚", GUILayout.Height(50)))
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

        GUILayout.Label(glaciaTexture, GUILayout.Width(50), GUILayout.Height(50));
        if (GUILayout.Button("Glacia を召喚", GUILayout.Height(50)))
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

        GUILayout.Label(elderTexture, GUILayout.Width(50), GUILayout.Height(50));
        if (GUILayout.Button("Elder を召喚", GUILayout.Height(50)))
        {
            OpenSummonWindow("Elder");
        }

        GUILayout.EndHorizontal();

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
