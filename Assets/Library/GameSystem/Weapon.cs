using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public Monster Owner { get; private set; }          // 武器の所有者(モンスター)
    public bool IsHitableOwner { get; private set; }    // 武器が所有者に当たるか
    public float Damage { get; private set; }           // 武器のダメージ
    public float StrikeForce { get; private set; }      // 武器の吹き飛ばす力

    Vector3 defaultLocalPosition;   // 武器の初期座標
    Vector3 defaultLocalScale;      // 武器の初期スケール
    Quaternion defaultRotation;     // 武器の初期回転

    Rigidbody2D rb;
    List<GameObject> weapons;


    void Awake()
    {
        // 1つ上の階層にいるモンスターのオブジェクトを探してセット
        Owner = transform.parent.GetComponent<Monster>();

        //武器のダメージ
        Damage = Parameters.WEAPON_DAMAGE;

        //武器の吹き飛ばす力
        StrikeForce = Parameters.WEAPON_STRIKE_FORCE;

        // 物理挙動を使えるようにしておく
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // 初期の位置、回転、スケールを保存
        defaultLocalPosition = transform.localPosition;
        defaultLocalScale = transform.localScale;
        defaultRotation = transform.rotation;

        //武器扱いのオブジェクトをすべて取得
        weapons = new List<GameObject>();
        weapons.Add(gameObject);//親を追加
        foreach (Transform child in transform)
        {
            weapons.Add(child.gameObject);// 子オブジェクトをリストに追加
        }

        foreach (GameObject obj in weapons)
        {
            // タグを設定
            gameObject.tag = Tags.Weapon;

            if (HasComponent<SpriteRenderer>(obj))
            {
                //スプライトのソートレイヤーを設定
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = SortLayer.Weapon;

                // 画像の形状に合わせてコライダを設定
                var weaponCollider = gameObject.AddComponent<PolygonCollider2D>();
                weaponCollider.autoTiling = true;
                weaponCollider.isTrigger = true;
            }
        }

        // 最初は武器を隠しておく
        SetActive(false);
    }

    void Start()
    {
        _ = ExcecuteActionLoop();   //呼び出し&Taskを破棄
    }

    async Task ExcecuteActionLoop()
    {
        if (Owner == null)
        {
            Debug.Log(gameObject.name);
            Debug.LogError("Error: Weapon Owner is null");
            
            return;
        }

        while (!Owner.IsDead)
        {
            await ActionLoop();
        }
    }

    async public Task ExecuteAttack()
    {
        WarpDefault();

        IsHitableOwner = false;

        SetActive(true);

        await Attack();

        SetActive(false);
    }

    //基本はこちらが呼び出される
    async virtual protected Task Attack()
    {
        await Task.Yield();
    }

    //自動制御する武器が作れる(おまけ要素)
    async virtual protected Task ActionLoop()
    {
        await Task.Yield();
    }

    //指定秒数待つ
    async protected Task Wait(float sec)
    {
        await Task.Delay((int)(sec * 1000));
    }

    //初期位置にワープ
    public void WarpDefault()
    {
        transform.SetParent(Owner.transform);

        transform.localPosition = defaultLocalPosition;
        transform.rotation = defaultRotation;
        transform.localScale = defaultLocalScale;
    }

    //動きの繋がりを補完する関数
    async Task Lerp(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Vector3 endPosition, Quaternion endRotation, Vector3 endScale, float second)
    {
        float elapsedTime = 0f;

        while (elapsedTime < second)
        {
            float t = elapsedTime / second;

            // 補間を行う
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }

    //武器を初期位置にリセットする
    async protected Task Default()
    {
        //現在の状態
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        Vector3 currentScale = transform.localScale;

        //リセット後の状態(所有するモンスターの回転と拡縮は反映しない)
        Vector3 targetPosition;
        Quaternion targetRotation = defaultRotation;
        Vector3 targetScale = defaultLocalScale;

        //向きによって戻す位置を変える
        if (Owner.IsFacingRight)
            targetPosition = Owner.transform.position + defaultLocalPosition;
        else
            targetPosition = Owner.transform.position - defaultLocalPosition;

        //補完する
        await Lerp(currentPosition, currentRotation, currentScale, targetPosition, targetRotation, targetScale, Parameters.DEFAULT_RETURN_TIME);

        //最後に親に戻す
        transform.SetParent(Owner.transform);

        // 最後に確実に元の状態に設定
        transform.localPosition = defaultLocalPosition;
        transform.rotation = targetRotation;
        transform.localScale = targetScale;

        await Wait(Parameters.DEFAULT_RETURN_WAIT_TIME);
    }

    //武器を指定された(x, y)位置にs秒で移動させる
    async protected Task Move(float x, float y, float s)
    {
        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(x, y);
        float elapsedTime = 0f;

        while (elapsedTime < s)
        {
            // 経過時間の割合を計算
            float t = elapsedTime / s;
            // 線形補間（Lerp）で位置を更新
            transform.localPosition = Vector2.Lerp(start, target, t);
            // 経過時間を更新
            elapsedTime += Time.deltaTime;
            // 1フレーム待機
            await Task.Yield();
        }

        // 最終位置を設定（誤差を補正）
        transform.localPosition = target;
    }

    //武器をangle度s秒でその場回転させる
    async protected Task Spin(float angle, float s)
    {
        float elapsed = 0f;
        float initialRotation = transform.rotation.eulerAngles.z;
        float targetRotation = initialRotation + angle;

        while (elapsed < s)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / s;
            float zRotation = Mathf.Lerp(initialRotation, targetRotation, t);
            // Z軸回りで回転させる
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
            await Task.Yield();
        }

        // 最後に確実に目標角度まで回転させる
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    //武器を周囲で回転させる startAngle:回転開始角度(頂点が0度), 回転量：rotAngle, 回転にかかる秒数：second   
    async protected Task Rotate(float startAngle, float rotAngle, float second)
    {
        //時計回りかどうか判定
        bool clockwise = true;
        if ((Owner.IsFacingRight && rotAngle < 0) || (!Owner.IsFacingRight && rotAngle >= 0))
            clockwise = false;

        //開始角度の調整
        float addStartAngle = startAngle;

        if ((clockwise && startAngle < 0) || (!clockwise && startAngle > 0))
            addStartAngle *= -1;

        float startAngleToward = addStartAngle + 90.0f + transform.localEulerAngles.z;

        //武器の初期位置をキャラクターの真上に設置
        float radius = Vector2.Distance(Owner.transform.position, transform.position);
        transform.position = Owner.transform.position + new Vector3(radius, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.RotateAround(Owner.transform.position, Vector3.forward, startAngleToward);

        bool stop = false;
        float currentRotAngle = 0;
        float step;

        while (!stop)
        {
            //フレーム毎の回転量を計算
            step = (rotAngle / second) * Time.deltaTime;

            //条件に応じて逆回転
            if ((clockwise && rotAngle > 0) || (!clockwise && rotAngle < 0))
                step *= -1;

            //現在までの回転総量の加算
            currentRotAngle += Mathf.Abs(step);

            //回転量が指定量に達したら終了
            if (currentRotAngle >= Mathf.Abs(rotAngle))
            {
                currentRotAngle = rotAngle;
                stop = true;
            }
            else
            {
                transform.RotateAround(Owner.transform.position, Vector3.forward, step);
            }

            await Task.Yield();
        }

        WarpDefault();
    }

    //武器を指定された方向に飛ばす
    async protected Task Shot(Vector2 direction, float power)
    {


        await Task.Yield();
    }

    //武器をモンスターによる制御から切り離す
    async protected Task Purge()
    {
        // ワールド座標を保存
        Vector3 worldPosition = transform.position;

        // 親オブジェクトから切り離す
        transform.SetParent(null);

        // 元の位置に戻す
        transform.position = worldPosition;

        await Task.Yield();
    }


    static (Vector2, Quaternion) RotateAround(Vector2 point, Vector2 pivot, float angle)
    {
        // 角度をラジアンに変換
        float radians = angle * Mathf.Deg2Rad;

        // 回転行列の計算
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // 元の点を中心点の原点に移動
        Vector2 translatedPoint = point - pivot;

        // 新しい座標を計算
        float newX = translatedPoint.x * cos - translatedPoint.y * sin;
        float newY = translatedPoint.x * sin + translatedPoint.y * cos;

        // 新しい座標を作成
        Vector2 rotatedPoint = new Vector2(newX, newY) + pivot;

        // 回転をクォータニオンで表現
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        return (rotatedPoint, rotation);
    }


    private bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    //自分のBodyの当たり判定から1度出た武器は当たるようになる
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Body))
        {
            GameObject monsterObj = other.transform.parent.gameObject;

            if (HasComponent<Monster>(monsterObj))
            {
                Monster monster = monsterObj.GetComponent<Monster>();

                if (Owner == monster)
                {
                    IsHitableOwner = true;
                }
            }
        }
    }

    void SetActive(bool value) 
    {
        foreach (GameObject weapon in weapons)  
        {
            weapon.SetActive(value);
        }
    }
}