using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[CustomEditor(typeof(Guard))]
public class GuardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) return;

        // 元のインスペクターを描画
        //DrawDefaultInspector();

        // ターゲットスクリプトの参照を取得
        Guard guard = (Guard)target;

        //題目
        GUILayout.Label("【 防具の設定 】", EditorStyles.boldLabel);

        // Enum選択リストを追加
        guard.type = (GuardType)EditorGUILayout.EnumPopup("タイプ", guard.type);

        GameObject guardObj = guard.transform.Find("Guard").gameObject;
        if (guardObj == null) return;

        SpriteRenderer sr = guardObj.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        switch (guard.type)
        {
            case GuardType.None:
                sr.sprite = null;
                sr.color = Color.white;
                guard.offsetX = 0;
                guard.offsetY = 0;
                guard.scale = 1;
                break;
            case GuardType.Shield:
                sr.sprite = Resources.Load<Sprite>(Parameters.SHIELD_SPRITE_RESOURCE_PATH);
                sr.color = Parameters.SHIELD_DEFALUT_COLOR;
                var maxOffset = Parameters.SHIELD_MAX_OFFSET;
                guard.offsetX = EditorGUILayout.Slider("位置X", guard.offsetX, -maxOffset, maxOffset);
                guard.offsetY = EditorGUILayout.Slider("位置Y", guard.offsetY, -maxOffset, maxOffset);
                guard.scale = EditorGUILayout.Slider("大きさ", guard.scale, Parameters.SHIELD_MIN_SCALE, Parameters.SHIELD_MAX_SCALE);
                break;
            case GuardType.Reflector:
                sr.sprite = Resources.Load<Sprite>(Parameters.REFLECTOR_SPRITE_RESOURCE_PATH);
                sr.color = Parameters.REFLECTOR_DEFALUT_COLOR;
                guard.offsetX = EditorGUILayout.Slider("位置", guard.offsetX, Parameters.REFLECTOR_MIN_OFFSET, Parameters.REFLECTOR_MAX_OFFSET);
                guard.scale = EditorGUILayout.Slider("大きさ", guard.scale, Parameters.REFLECTOR_MIN_SCALE, Parameters.REFLECTOR_MAX_SCALE);
                break;
            default:
                break;
        }
        var worldPosition = guard.transform.position;
        var lossyScale = guard.transform.lossyScale;

        //0除算回避
        var x = 0f;
        var y = 0f;
        if (lossyScale.x != 0) x = guard.scale / lossyScale.x;
        if (lossyScale.y != 0) y = guard.scale / lossyScale.y;

        guardObj.transform.localScale = new Vector3(x, y, 0);
        guardObj.transform.position = worldPosition + new Vector3(guard.offsetX, guard.offsetY, 0);

        // チェックボックスを描画
        guard.isDisplay = EditorGUILayout.Toggle("表示", guard.isDisplay);

        // ヒエラルキーのオブジェクトに反映
        if (GUI.changed)
        {
            if (guard.oldType != guard.type)
            {
                //タイプが変わったときは強制的に表示
                guard.isDisplay = true;

                switch (guard.type)
                {
                    case GuardType.None:
                        guard.offsetX = 0;
                        guard.offsetY = 0;
                        guard.scale = 1;
                        break;
                    case GuardType.Shield:
                        guard.offsetX = Parameters.SHIELD_DEFAULT_OFFSET.x;
                        guard.offsetY = Parameters.SHIELD_DEFAULT_OFFSET.y;
                        guard.scale = Parameters.SHIELD_DEFALUT_SCALE;

                        break;
                    case GuardType.Reflector:
                        guard.offsetX = Parameters.REFLECTOR_DEFAULT_OFFSET.x;
                        guard.offsetY = Parameters.REFLECTOR_DEFAULT_OFFSET.y;
                        guard.scale = Parameters.REFLECTOR_DEFAULT_SCALE;
                        break;
                    default:
                        break;
                }

                guard.oldType = guard.type;
            }

            worldPosition = guard.transform.position;
            guardObj.transform.position = worldPosition + new Vector3(guard.offsetX, guard.offsetY, 0);

            x = 0f;
            y = 0f;
            if (lossyScale.x != 0) x = guard.scale / lossyScale.x;
            if (lossyScale.y != 0) y = guard.scale / lossyScale.y;
            guardObj.transform.localScale = new Vector3(x, y, 0);

            //更新
            guard.UpdateCustomize();
            EditorUtility.SetDirty(guard);
        }
    }
}