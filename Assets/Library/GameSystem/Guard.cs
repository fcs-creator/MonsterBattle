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

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        //ガードと接触した時の処理
        if (obj.CompareTag(Tags.Weapon))
        {
            if (HasComponent<Weapon>(obj))
            {
                Weapon weapon = obj.GetComponent<Weapon>();

                if (weapon.Owner != Owner)
                {
                    weapon.Owner.IsStunned = true;

                    //ガードエフェクトの再生
                    PlayGuardVFX(obj, other);

                    //武器の所有者を吹き飛ばす方向を計算
                    Vector2 direction = (weapon.Owner.transform.position - transform.position).normalized;
                    weapon.Owner.GetComponent<Rigidbody2D>().AddForce(direction * weapon.Damage * Parameters.GUARD_FORCE_SCALE, ForceMode2D.Impulse);

                    //パリィ音を再生
                    AudioManager.Instance.PlaySE(Parameters.SE_PARRY);

                    //スタン状態を有効にする
                    weapon.Owner.IsStunned = true;

                    Debug.Log("Guard Hit->Stun Flag On !!!!");
                }
            }
        }
    }

    //ガードヒットエフェクトの再生
    void PlayGuardVFX(GameObject weapon, Collider2D weaponCollider)
    {
        // 衝突点を取得
        Vector3 collisionPoint = weaponCollider.ClosestPoint(transform.position);

        // 自分の位置を基準にして衝突の法線ベクトルを計算
        Vector3 hitNormal = (weapon.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(hitNormal);

        // ガードエフェクトの再生
        VFXManager.Instance.Play(VFX.Guard, collisionPoint, rotation);
    }

    bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}
