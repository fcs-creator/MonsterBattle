using NUnit.Framework.Internal;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using static UnityEngine.Rendering.DebugUI;

public enum GuardType 
{
    None,
    Shield,
    Reflector
}

public class Guard : MonoBehaviour
{
    public Monster Owner { get; private set; }

    [SerializeField] GuardType type= Parameters.GUARD_DEFAULT_TYPE;    // 現在のタイプ
    [SerializeField] GuardType oldType = Parameters.GUARD_DEFAULT_TYPE; // 前のタイプ
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] float scale;
    [SerializeField] bool isDisplay = true;

    public GuardType Type => type;
    public GuardType OldType => oldType;
    public float OffsetX => offsetX;
    public float OffsetY => offsetY;
    public float Scale => scale;
    public bool IsDisplay => isDisplay;

    //ガードの実体
    GameObject instance;
    public GameObject Instance { get { return instance; } }

    public void SetOwner(Monster monster)
    {
        Owner = monster;
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

    public async Task ExecuteGuard() 
    {
        instance.SetActive(true);

        //モンスターの向きによって出す方向を変える
        if (!Owner.IsFacingRight)
        {
            offsetX *= -1;
        }

        await Wait(Parameters.GUARD_DURATION);

        instance.SetActive(false);
    }

    private async Task Wait(float sec) 
    {
        await Task.Delay((int)(sec * 1000), canceler.Token);
    }

    
}
