using Codice.Client.BaseCommands;
using NUnit.Framework.Internal;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

public enum GuardType 
{
    None,
    Shield,
    Reflector
}

public class Guard : MonoBehaviour
{
    [HideInInspector] public Monster Owner { get; private set; }
    [HideInInspector] public GuardType type = Parameters.GUARD_DEFAULT_TYPE;    // 現在のタイプ
    [HideInInspector] public GuardType oldType = Parameters.GUARD_DEFAULT_TYPE; // 前のタイプ
    [HideInInspector] public float offsetX;
    [HideInInspector] public float offsetY;
    [HideInInspector] public float scale;
    [HideInInspector] public bool isDisplay = true;

    float defaultOffsetX;

    //ガードの実体
    GameObject instance;
    public GameObject Instance { get { return instance; } }

    //跳ね返した時の
    private float reflectionRate;
    public float ReflectionRate 
    {
        get 
        {
            var scaleRate = MapValueToRange(scale, Parameters.REFLECTOR_MIN_SCALE, Parameters.REFLECTOR_MAX_SCALE);
            Debug.Log(Parameters.REFLECT_DAMAGE_RATE + (1.0f / scaleRate));
            return Parameters.REFLECT_DAMAGE_RATE + (1.0f / scaleRate);
        }

    }

    //１～2までの値をマップして返す
    public float MapValueToRange(float value, float minValue, float maxValue)
    {
        // 最大値と最小値が同じ場合（計算できないため処理を防ぐ）
        if (Mathf.Approximately(maxValue, minValue))
        {
            Debug.LogError("maxValueとminValueが同じです。正しい範囲を指定してください。");
            return 1f; // デフォルト値を返す
        }
        
        // 値を1から2の範囲に変換
        return Mathf.Clamp(1f + (value - minValue) / (maxValue - minValue), 1.0f, 2.0f);
    }


    public void SetOwner(Monster monster)
    {
        Owner = monster;
    }

    //エディタのカスタマイズを反映
    public void UpdateCustomize()
    {
        //ガードオブジェクトを探す
        instance = transform.Find("Guard").gameObject;

        if (!instance)
        {
            Debug.LogError("Guardが見つかりません");
        }

        instance.SetActive(isDisplay);
    }

    //タスクをキャンセル
    readonly Canceler canceler = new Canceler();

    public void CancelActions()
    {
        canceler.Cancel();
    }

    public void ResetActions()
    {
        canceler.Reset();
    }

    void Awake()
    {
        //モンスターは同じ階層
        Owner = transform.GetComponent<Monster>();

        //ガードの実体は1つ下の階層
        instance = transform.Find("Guard").gameObject;

        //ガードの実体にタグを設定
        instance.tag = Tags.Guard;

        //ガードの実体にソートレイヤーを設定
        var sr = instance.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = SortLayer.Guard;

        //ガードの実体にコライダーをトリガーとして追加
        var collider = instance.AddComponent<PolygonCollider2D>();
        collider.autoTiling = true;
        collider.isTrigger = true;

        //ガードの実体を隠しておく
        instance.SetActive(false);
    }

    private void Start()
    {
        defaultOffsetX = offsetX;
    }

    private void Update()
    {
        //モンスターの向きによって出す方向を変える
        if (Owner.IsFacingRight)
        {
            offsetX = defaultOffsetX;
        }
        else
        {
            offsetX = -defaultOffsetX;
        }
    }

    public void SetType(GuardType value) 
    {
        oldType = type;
        type = value;
    }

    public async Task ExecuteGuard()
    {
        instance.SetActive(true);

        await Wait(Parameters.GUARD_DURATION);

        instance.SetActive(false);
    }

    private async Task Wait(float sec) 
    {
        try
        {
            await Task.Delay((int)(sec * 1000), canceler.Token);
        }
        catch (OperationCanceledException)
        {
            //タスクがキャンセルされた時の処理
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        //魔法によるダメージ
        if (HasComponent<Magic>(obj))
        {
            Magic magic = obj.GetComponent<Magic>();

            if (magic.Owner != this)
            {
                if (collision.contactCount > 0)
                {
                    if (Owner.IsGuarding)
                    {
                        if(type == GuardType.Reflector)
                        {
                            magic.Owner = Owner;

                            //反射音を再生
                            AudioManager.Instance.PlaySE(Parameters.SE_REFLECT);

                            var contact = collision.contacts;

                            //ガードエフェクトの再生
                            PlayGuardVFX(contact[0].point);

                            //跳ね返す方向を計算
                            Vector2 direction = (magic.transform.position - transform.position).normalized;

                            if (!magic.hasReflected)
                            {
                                magic.hasReflected = true;

                                magic.SetDamage(magic.Damage * Parameters.MAGIC_REFLECT_INCREACE_DAMAGE_RATE);

                            }

                            switch (magic.Type) 
                            {
                                case MagicType.FireBall:
                                    var fireBall = magic.GetComponent<FireBall>();
                                    var rb = fireBall.GetComponent<Rigidbody2D>();
                                    //反射させる
                                    var lv = rb.linearVelocity;
                                    rb.linearVelocity = new Vector2(lv.x * -1, lv.y);
                                    rb.AddForce(direction * Parameters.MAGIC_REFLECT_FORCE, ForceMode2D.Impulse);
                                    break;
                                case MagicType.Thunder:
                                    var localScale = magic.transform.localScale;
                                    magic.transform.localScale = new Vector2(-localScale.x, localScale.y);
                                    Owner.HpBar.TakeDamage(magic.Damage);
                                    break;
                                
                            }
                        }
                    }
                }
            }
        }
    }

    // ステージの壁のヒットエフェクトの再生
    private void PlayGuardVFX(Vector3 collisionPoint)
    {
        // ヒットエフェクトを再生
        VFXManager.Instance.Play(Parameters.VFX_GUARD, collisionPoint, transform.rotation);
    }

    // コンポーネントの有無を確認
    private bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}
