using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Monster),true)]
public class MonsterEditor : Editor
{
    //シリアライズオブジェクト
    SerializedObject serializedObjectRef;

    //シリアライズされた変数
    SerializedProperty monsterSprite;

    GameObject bodyObj;

    private void OnEnable()
    {
        // 対象のオブジェクトをSerializedObjectとして取得
        serializedObjectRef = new SerializedObject(target);

        // 個々のプロパティを取得
        monsterSprite = serializedObjectRef.FindProperty("monsterSprite");
    }

    public override void OnInspectorGUI()
    {
        // SerializedObjectを更新
        serializedObjectRef.Update();

        // 元のインスペクターを描画
        DrawDefaultInspector();

        // Monsterクラスの参照
        Monster monster = (Monster)target;

        //題目
        GUILayout.Label("【 モンスターの設定 】", EditorStyles.boldLabel);

        string inputName = EditorGUILayout.TextField("名前", monster.name);
        if (inputName != "")
        {
            monster.name = inputName;
        }
        else
        {
            monster.name = "????";
        }

        EditorGUILayout.LabelField("見た目");
        
        // ObjectFieldの固定サイズ（幅300px, 高さ100px）
        Rect fixedRect = new Rect(GUILayoutUtility.GetLastRect().xMax - 75 - 10, GUILayoutUtility.GetLastRect().yMax + 5, 75,75); // インスペクタ内の固定位置

        // 固定サイズのObjectFieldを描画
        monsterSprite.objectReferenceValue = EditorGUI.ObjectField(
            fixedRect,
            monsterSprite.objectReferenceValue,
            typeof(Sprite),
            false
        );

        // 空白を追加して次の項目にスペースを設ける
        GUILayout.Space(fixedRect.height + 10);

        // 変更を適用
        serializedObjectRef.ApplyModifiedProperties();

        if(bodyObj == null) 
        {
            bodyObj = monster.transform.Find("Body").gameObject;
        }

        if (bodyObj.transform.localPosition != Vector3.zero)
        {
            bodyObj.transform.localPosition = Vector3.zero;
        }

        // 変更があればオブジェクトを更新
        if (GUI.changed)
        {
            monster.UpdateCustomize();

            EditorUtility.SetDirty(target);
        }
        
    }
}
