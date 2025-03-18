using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[CustomEditor(typeof(Guard))]
public class GuardEditor : Editor
{
    //シリアライズオブジェクト
    SerializedObject serializedObjectRef;

    //シリアライズされた変数
    SerializedProperty type;
    SerializedProperty oldType;
    SerializedProperty offsetX;
    SerializedProperty offsetY;
    SerializedProperty scale;
    SerializedProperty isDisplay;

    //ガードの実体オブジェクト
    GameObject instance;

    //ガードの実体のスプライトレンダラー
    SpriteRenderer sr;

    private void OnEnable()
    {
        // 対象のオブジェクトをSerializedObjectとして取得
        serializedObjectRef = new SerializedObject(target);

        // 個々のプロパティを取得
        type = serializedObjectRef.FindProperty("type");
        oldType = serializedObjectRef.FindProperty("oldType");
        offsetX = serializedObjectRef.FindProperty("offsetX");
        offsetY = serializedObjectRef.FindProperty("offsetY");
        scale = serializedObjectRef.FindProperty("scale");
        isDisplay = serializedObjectRef.FindProperty("isDisplay");
    }

    public override void OnInspectorGUI()
    {
        // シリアライズオブジェクトの更新を開始
        serializedObject.Update();

        // 元のインスペクターを描画
        //DrawDefaultInspector();

        //題目
        GUILayout.Label("【 防具の設定 】", EditorStyles.boldLabel);

        // Enum選択リストを追加
        EditorGUILayout.PropertyField(type, new GUIContent("タイプ"));
        
        // 現在の値を判別
        GuardType guardType = (GuardType)type.enumValueIndex;
        GuardType oldGuardType = (GuardType)oldType.enumValueIndex;
        
        switch (guardType)
        {
            case GuardType.None:
                offsetX.floatValue = 0;
                offsetY.floatValue = 0;
                scale.floatValue = 1;
                break;
            case GuardType.Shield:
                var maxOffset = Parameters.SHIELD_MAX_OFFSET;
                offsetX.floatValue = EditorGUILayout.Slider("位置X", offsetX.floatValue, -maxOffset, maxOffset);
                offsetY.floatValue = EditorGUILayout.Slider("位置Y", offsetY.floatValue, -maxOffset, maxOffset);
                scale.floatValue = EditorGUILayout.Slider("大きさ", scale.floatValue, Parameters.SHIELD_MIN_SCALE, Parameters.SHIELD_MAX_SCALE);
                isDisplay.boolValue = EditorGUILayout.Toggle("表示", isDisplay.boolValue);
                break;
            case GuardType.Reflector:
                offsetX.floatValue = EditorGUILayout.Slider("位置", offsetX.floatValue, Parameters.REFLECTOR_MIN_OFFSET, Parameters.REFLECTOR_MAX_OFFSET);
                scale.floatValue = EditorGUILayout.Slider("大きさ", scale.floatValue, Parameters.REFLECTOR_MIN_SCALE, Parameters.REFLECTOR_MAX_SCALE);
                isDisplay.boolValue = EditorGUILayout.Toggle("表示", isDisplay.boolValue);
                break;
            default:
                break;
        }

        // シリアライズオブジェクトの変更を適用
        serializedObject.ApplyModifiedProperties();

        // ヒエラルキーのオブジェクトに反映
        if (GUI.changed)
        {
            // ターゲットスクリプトの参照を取得
            Guard guard = (Guard)target;

            ////ガードの実体オブジェクトを取得
            if (instance == null)
            {
                instance = guard.transform.Find("Guard").gameObject;
            }

            //ガードの実体のスプライトレンダラーを取得
            if (sr == null)
            {
                sr = instance.GetComponent<SpriteRenderer>();
            }

            switch (guardType)
            {
                case GuardType.None:
                    sr.sprite = null;
                    sr.color = Color.white;
                    break;
                case GuardType.Shield:
                    sr.sprite = Resources.Load<Sprite>(Parameters.SHIELD_SPRITE_RESOURCE_PATH);
                    sr.color = Parameters.SHIELD_DEFALUT_COLOR;
                    break;
                case GuardType.Reflector:
                    sr.sprite = Resources.Load<Sprite>(Parameters.REFLECTOR_SPRITE_RESOURCE_PATH);
                    sr.color = Parameters.REFLECTOR_DEFALUT_COLOR;
                    break;
                default:
                    break;
            }

            //タイプが変わったときは初期化する
            if (oldGuardType != guardType)
            {
                //強制的に表示
                isDisplay.boolValue = true;

                switch (guardType)
                {
                    case GuardType.None:
                        sr.sprite = null;
                        sr.color = Color.white;
                        offsetX.floatValue = 0;
                        offsetY.floatValue = 0;
                        scale.floatValue = 1;
                        break;
                    case GuardType.Shield:
                        sr.sprite = Resources.Load<Sprite>(Parameters.SHIELD_SPRITE_RESOURCE_PATH);
                        sr.color = Parameters.SHIELD_DEFALUT_COLOR;
                        offsetX.floatValue = Parameters.SHIELD_DEFAULT_OFFSET.x;
                        offsetY.floatValue = Parameters.SHIELD_DEFAULT_OFFSET.y;
                        scale.floatValue = Parameters.SHIELD_DEFALUT_SCALE;
                        break;
                    case GuardType.Reflector:
                        sr.sprite = Resources.Load<Sprite>(Parameters.REFLECTOR_SPRITE_RESOURCE_PATH);
                        sr.color = Parameters.REFLECTOR_DEFALUT_COLOR;
                        offsetX.floatValue = Parameters.REFLECTOR_DEFAULT_OFFSET.x;
                        offsetY.floatValue = Parameters.REFLECTOR_DEFAULT_OFFSET.y;
                        scale.floatValue = Parameters.REFLECTOR_DEFAULT_SCALE;
                        break;
                    default:
                        break;
                }

                oldType.enumValueIndex = type.enumValueIndex;
            }

            instance.SetActive(isDisplay.boolValue);

            var worldPosition = instance.transform.parent.position;
            var lossyScale = instance.transform.parent.lossyScale;

            //0除算回避
            float x = 0f;
            float y = 0f;
            if (lossyScale.x != 0) x = scale.floatValue / lossyScale.x;
            if (lossyScale.y != 0) y = scale.floatValue / lossyScale.y;
            instance.transform.localScale = new Vector3(x, y, 0);
            instance.transform.position = worldPosition + new Vector3(offsetX.floatValue, offsetY.floatValue, 0);

            EditorUtility.SetDirty(guard);
        }


    }
}
