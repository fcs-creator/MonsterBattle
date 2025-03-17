using System.Threading.Tasks;
using UnityEngine;

public enum GuardType 
{
    None,
    Shield,
    Reflector
}

public class Guard : MonoBehaviour
{
    public Monster Owner { get; private set; }
    public GuardType type = Parameters.GUARD_DEFAULT_TYPE;                      // 現在のタイプ
    [HideInInspector] public GuardType oldType = Parameters.GUARD_DEFAULT_TYPE; // 前のタイプ
    [HideInInspector] public float offsetX;
    [HideInInspector] public float offsetY;
    [HideInInspector] public float scale;

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

        await Wait(Parameters.GUARD_DURATION);

        instance.SetActive(false);
    }

    private async Task Wait(float sec) 
    {
        await Task.Delay((int)(sec * 1000), canceler.Token);
    }
}
