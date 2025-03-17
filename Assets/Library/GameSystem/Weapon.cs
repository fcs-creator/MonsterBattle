using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Drawing;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Threading;


public class Weapon : MonoBehaviour
{
    public Monster Owner { get; private set; }          // 武器の所有者(モンスター)
    public bool IsHitableOwner { get; private set; }    // 武器が所有者に当たるか
    public float StrikeForce { get; private set; }      // 武器の吹き飛ばす力

    // 武器のダメージ
    private float damage;
    public float Damage
    {
        get
        {
            damage = CalcutlateDamage();
            return damage;
        }
        private set
        {
            damage = value;
        }      
    }

    Vector3 defaultLocalPosition;   // 武器の初期座標
    Vector3 defaultLocalScale;      // 武器の初期スケール
    Quaternion defaultRotation;     // 武器の初期回転

    Rigidbody2D rb;
    List<GameObject> weapons;

    Vector3 initialOffset;
    float orbitRadius;

    bool isShot;
    bool isClone = false;

    //タスクをキャンセルするための共通トークン
    readonly Canceler canceler = new Canceler();

    //アクションのキャンセル
    public void CancelActions()
    {
        SetActive(false);
        canceler.Cancel();
    }

    public void ResetActions()
    {
        canceler.Reset();
        SetActive(true);
    }

    public void OnDestroy()
    {
        canceler.Cancel();
        canceler.Dispose();
    }

    void Awake()
    {
        // 1つ上の階層にいるモンスターのオブジェクトを探してセット
        Owner = transform.parent.GetComponent<Monster>();

        // 物理挙動を追加して無効にしておく
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.centerOfMass = new Vector2(0, 0);
            rb.simulated = true;
            SetGripWeapon(true);
        }

        //武器の吹き飛ばす力
        StrikeForce = Parameters.WEAPON_STRIKE_FORCE;

        // 初期の位置、回転、スケールを保存
        defaultLocalPosition = transform.localPosition;
        defaultLocalScale = transform.localScale;
        defaultRotation = transform.rotation;

        // 初期オフセットを計算
        initialOffset = transform.position - Owner.transform.position;
        orbitRadius = initialOffset.magnitude;

        //武器扱いのオブジェクトをすべて取得
        weapons = new List<GameObject>();
        weapons.Add(gameObject);//親を追加
        foreach (Transform child in transform)
        {
            weapons.Add(child.gameObject);// 子オブジェクトをリストに追加
        }

        //面積を計算
        float area = 0.0f;

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
                if (gameObject.GetComponent<PolygonCollider2D>() == null)
                {
                    var weaponCollider = gameObject.AddComponent<PolygonCollider2D>();
                    weaponCollider.autoTiling = true;
                    weaponCollider.isTrigger = true;
                    area += CalculateScaledArea(weaponCollider);
                };
            }
        }

        //質量を設定
        float massMag = Parameters.MASS_WEAPON_MAGNIFICATION;
        float massMax = Parameters.MASS_WEAPON_MAX;
        float massMin = Parameters.MASS_WEAPON_MIN;
        rb.mass = Mathf.Clamp(Mathf.Sqrt(area) * massMag + massMin, massMin, massMax);  // 質量を設定
        Debug.Log("Weapon >> " + transform.parent.name + " : " + rb.mass + "kg");

        isShot = false;

        // 最初は武器を隠しておく
        SetActive(false);
    }

    void Start()
    {
        if (!isClone) 
        {
            _ = ExcecuteActionLoop();   //呼び出し&Taskを破棄
        }
    }

    private async Task ExcecuteActionLoop()
    {
        await Wait(Parameters.START_INTERVAL);

        if (Owner == null)
        {
            Debug.Log(gameObject.name);
            Debug.LogError("Error: Weapon Owner is null");

            return;
        }

        while (!Owner.IsDead && canceler.IsNotCancel)
        {
            await ActionLoop();
        }

        canceler.Cancel();
    }

    public async Task ExecuteAttack(int number)
    {
        WarpDefault();

        IsHitableOwner = false;

        SetActive(true);

        await Attack(number);

        SetActive(false);
    }

    //基本はこちらが呼び出される
    protected async virtual Task Attack(int number)
    {
        await Task.Yield();
    }

    //自動制御する武器にする
    protected async virtual Task ActionLoop()
    {
        await Task.Yield();
    }

    //指定秒数待つ
    protected async Task Wait(float sec)
    {
        if (canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }

    //初期位置にワープ
    private void WarpDefault()
    {
        //武器を握った状態にする
        SetGripWeapon(true);

        transform.SetParent(Owner.transform);
        transform.localPosition = defaultLocalPosition;
        transform.rotation = defaultRotation;
        transform.localScale = defaultLocalScale;
    }

    //動きの繋がりを補完する関数
    async Task Lerp(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Vector3 endPosition, Quaternion endRotation, Vector3 endScale, float second)
    {
        if (canceler.IsCancel) return;

        float elapsedTime = 0f;

        while (elapsedTime < second && canceler.IsNotCancel)
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
    public async Task Default()
    {
        //武器を握るモードにする
        SetGripWeapon(true);

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

    //居合い抜き
    public async Task Drawing()
    {
        if (canceler.IsCancel) return;

        Owner.ActionBar.SendText("Weapon-Drawing");

        //SEを再生
        AudioManager.Instance.PlaySE(Parameters.SE_WEAPON_DRAWING);

        float elapsedTime = 0f;
        float s = Parameters.WEAPON_INTERVAL_DRAWING;

        while (elapsedTime < s && canceler.IsNotCancel)
        {
            // 経過時間の割合を計算
            float t = elapsedTime / s;
            // 経過時間を更新
            elapsedTime += Time.deltaTime;
            // 1フレーム待機
            await Task.Yield();
        }
    }

    //武器を指定された(x, y)位置にs秒で移動させる
    public async Task Move(float x, float y, float s)
    {
        if (canceler.IsCancel) return;

        Owner.ActionBar.SendText("Weapon-Move");

        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(x, y);
        float elapsedTime = 0f;

        while (elapsedTime < s && canceler.IsNotCancel)
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
    public async Task Spin(float angle, float s)
    {
        if (canceler.IsCancel) return;

        float elapsed = 0f;
        float initialRotation = transform.rotation.eulerAngles.z;
        float targetRotation = initialRotation + angle;

        while (elapsed < s && canceler.IsNotCancel)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / s;
            float zRotation = Mathf.Lerp(initialRotation, targetRotation, t);
            // Z軸回りで回転させる
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
            // With the following line
            await Task.Yield();
        }

        // 最後に確実に目標角度まで回転させる
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    //武器をモンスターの周囲で回転させる(上方向が基準で0°)
    public async Task Rotate(float startAngle, float rotAngle, float second) 
    {
        if (canceler.IsCancel) return;

        Owner.ActionBar.SendText("Weapon-Rotate");

        if (!Owner.IsFacingRight)
            transform.localScale = new Vector2(defaultLocalScale.x * -1, defaultLocalScale.y);

        //時計回りかどうか判定
        bool clockwise = true;
        if ((Owner.IsFacingRight && rotAngle < 0) || (!Owner.IsFacingRight && rotAngle >= 0))
            clockwise = false;

        //開始角度の調整
        float addStartAngle = startAngle;

        if ((clockwise && startAngle < 0) || (!clockwise && startAngle > 0))
            addStartAngle *= -1;

        float startAngleToward = addStartAngle + 90.0f;// + transform.localEulerAngles.z;

        transform.position = Owner.transform.position + new Vector3(orbitRadius, 0, 0);
        transform.RotateAround(Owner.transform.position, Vector3.forward, startAngleToward);

        bool stop = false;
        float currentRotAngle = 0;
        float step;

        Transform centerObject = Owner.transform;

        while (!stop && canceler.IsNotCancel)
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
                //位置：半径を元に移動して補正する
                Vector3 desiredPosition = (transform.position - centerObject.position).normalized * orbitRadius + centerObject.position;
                transform.position = desiredPosition;
                transform.RotateAround(centerObject.position, Vector3.forward, step);
            }

            await Task.Yield();
        }

        WarpDefault();
    }

    //武器を前方に飛ばす (向きは-1~1の少数で指定 上向き:1, 正面:0, 下向き: -1)
    public async Task Shot(float directionY, float power)
    {
        if (canceler.IsCancel) return;

        isShot = true;
        
        Owner.ActionBar.SendText("Weapon-Shot");

        float dirY = Mathf.Clamp(directionY, -1.0f, 1.0f);
        float dirX = 1 - Mathf.Abs(dirY);

        //モンスターの向きによって武器飛ばす方向を変える
        if (!Owner.IsFacingRight)
        {
            dirX *= -1;
        }

        //武器から手を離す
        SetGripWeapon(false);

        //SE再生
        AudioManager.Instance.PlaySE(Parameters.SE_WEAPON_SHOT);

        //飛ばす
        rb.AddForce(new Vector2(dirX,dirY) * power * Parameters.WEAPON_SHOT_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_SHOT);

        isShot = false;
    }

    //武器を指定方向に飛ばす
    public async Task ShotDirection(Vector2 direction, float power) 
    {
        if (canceler.IsCancel) return;

        isShot = true;

        Owner.ActionBar.SendText("Weapon-ShotDirection");

        //武器から手を離す
        SetGripWeapon(false);

        //SE再生
        AudioManager.Instance.PlaySE(Parameters.SE_WEAPON_SHOT);

        //飛ばす
        rb.AddForce(direction.normalized * power * Parameters.WEAPON_SHOT_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_SHOT);

        isShot = false;
    }

    //武器を複製する
    protected async Task<Weapon[]> Clone(int num)
    {
        int n = Mathf.Clamp(num, 1, Parameters.WEAPON_CLONE_MAX);
        Weapon[] clones = new Weapon[n];

        if (canceler.IsNotCancel) 
        {
            Owner.ActionBar.SendText("Clone");

            Vector2 center = Owner.transform.position;
            float radius = orbitRadius;

            for (int i = 0; i < n; i++)
            {
                // クローンを生成
                GameObject cloneObj = Instantiate(transform.gameObject, transform.position, transform.rotation, Owner.transform);

                // クローンのスケールを設定
                var scale = cloneObj.transform.localScale;
                var rate = Parameters.WEAPON_CLONE_SCALE_RATE;
                cloneObj.transform.localScale = new Vector3(scale.x * rate, scale.y * rate, scale.z);

                // クローンの位置を円周上に配置
                float angle = 2 * Mathf.PI / n * i; // ラジアン単位で計算
                float x = center.x + radius * Mathf.Cos(angle);
                float y = center.y + radius * Mathf.Sin(angle);
                cloneObj.transform.position = new Vector3(x, y, 0);
                cloneObj.SetActive(true);

                // クローンの武器を取得
                clones[i] = cloneObj.transform.GetComponent<Weapon>();
                clones[i].canceler.Reset();
                clones[i].isClone = true;

                // クローンが行動している間に消してしまうと止まる
                //_ = DestroyCloneAfterDelay(cloneObj, Parameters.WEAPON_CLONE_DESTROY_DURATION);
            }

            SetActive(false);
        }

        await Wait(Parameters.WEAPON_INTERVAL_CLONE);

        return clones;
    }

    private async Task DestroyCloneAfterDelay(GameObject cloneObj, float delay)
    {
        await Task.Delay((int)(delay * 1000));
        if (cloneObj != null)
        {
            Destroy(cloneObj);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        //ガードと接触した時の処理
        if (obj.CompareTag(Tags.Guard))
        {
            var guard = obj.transform.parent.GetComponent<Guard>();

            if (guard.Owner != Owner)
            {
                Owner.IsStunned = true;

                //ガードエフェクトの再生
                PlayGuardVFX(obj, other);

                //武器の所有者を吹き飛ばす方向を計算
                Vector2 direction = (Owner.transform.position - guard.Instance.transform.position).normalized;
                Owner.GetComponent<Rigidbody2D>().AddForce(direction * Damage * Parameters.GUARD_FORCE_SCALE, ForceMode2D.Impulse);

                //パリィ音を再生
                AudioManager.Instance.PlaySE(Parameters.SE_PARRY);

                //スタン状態を有効にする
                Owner.IsStunned = true;

                Debug.Log("Guard Hit->Stun Flag On !!!!");
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

    //自分のBodyの当たり判定から1度出た武器は当たるようになる
    private void OnTriggerExit2D(Collider2D other)
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

    //武器を有効・無効を切り替える
    private void SetActive(bool value) 
    {
        foreach (GameObject weapon in weapons)  
        {
            weapon.SetActive(value);
        }
    }

    //武器を握っているかどうかの状態をセット
    private void SetGripWeapon(bool value) 
    {
        if (value)
        {
            rb.linearVelocity = new Vector3(0, 0, 0);
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = Parameters.WEAPON_GRAVITY_SCALE;
        }
    }

    //コライダーの面積を計算
    private float CalculateScaledArea(PolygonCollider2D collider)
    {
        // ローカル座標での面積を計算
        Vector2[] points = collider.points;
        float localArea = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Length];
            localArea += current.x * next.y - current.y * next.x;
        }
        localArea = Mathf.Abs(localArea) * 0.5f;

        // 親のスケールも含めてスケールを適用
        Vector3 lossyScale = collider.transform.lossyScale;
        float totalScaledArea = localArea * Mathf.Abs(lossyScale.x) * Mathf.Abs(lossyScale.y);

        return totalScaledArea;
    }

    //ダメージを計算
    public float CalcutlateDamage() 
    {
        if (isShot)
        {
            return rb.mass * Parameters.WEAPON_DAMAGE_SCALE;
        }
        else 
        {
            float speed = rb.linearVelocity.magnitude;
            if (speed < 1) speed = 1;
            return rb.mass * speed * Parameters.WEAPON_DAMAGE_SCALE;
        }
    }

    //コンポネントを所持しているかどうか
    private bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}