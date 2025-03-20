using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Threading;
using System;
using UnityEditor.Experimental.GraphView;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

public class Monster : MonoBehaviour
{
    public SpriteRenderer Sprite;

    public Monster Enemy { get; set; }                  //最も近い敵
    public List<Monster> Enemies { get; set; }          //全ての敵
    public MagicBook MagicBook { get; set; }            //魔法の書
    public UIHPBar HpBar { get; private set; }          //HPバー
    public UIActionBar ActionBar { get; private set; }  //アクションバー

    public bool IsAttacking { get; private set; }                                               //攻撃中か
    public bool IsGuarding { get; private set; }                                                //防御中か
    public bool IsForward { get; private set; }                                                 //前方向に進んでいるか
    public bool IsBackward { get; private set; }                                                //後方向に進んでいるか
    public bool IsJumping { get; private set; }                                                 //ジャンプ中か
    public bool IsGrounded { get; private set; }                                                //地面にいるか
    public bool IsAirborne { get { return !IsGrounded; } private set { IsAirborne = value; } }  //空中にいるか
    public bool IsDead { get; private set; }                                                    //死んでいるか
    public bool IsFacingRight { get; private set; }                                             //右を向いているか
    public bool IsStunned { get; set; }                                                         //スタン状態か
    public bool IsFloating { get; set; }                                                        //浮遊状態か
    private bool IsStunable;                                                                    //スタン可能か

    Body body;          //本体
    Weapon weapon;      //武器
    Guard guard;        //防具
    Rigidbody2D rb;     //物理挙動

    int EnemyCheckCount = 0;

    //ゲームの情報
    public int AliveMonsterNum { get { return GameManager.AliveMonstersNum; } }

    //自分の情報
    public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.y); } }
    public float Hp { get { return HpBar.Hp; } }
    public Monster Target { get { return Enemy; } }

    //敵の情報
    public Vector2 EnemyDirection { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).normalized; } }
    public float EnemyDistance { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).magnitude; } }
    public float EnemyHp { get { return Enemy.HpBar.Hp; } }

    public bool IsEnemy = false;
    /*
        public void UpdateCustomize()
        {
            var bodyObj = transform.Find("Body").gameObject;
            bodyObj.transform.localPosition = Vector3.zero;
            var sr = bodyObj.GetComponent<SpriteRenderer>();
            sr.sprite = monsterSprite;
        }
    */
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
        //物理挙動を追加
        rb = gameObject.AddComponent<Rigidbody2D>();

        // 本体の設定
        body = transform.Find("Body").AddComponent<Body>(); //準備のできたタイミングで明示的にAddしてあげると良い

        // 武器の設定
        weapon = transform.Find("Weapon").GetComponent<Weapon>();
        weapon.SetOwner(this);

        // 防具の設定
        guard = transform.GetComponent<Guard>();
        guard.SetOwner(this);

        //HPバーを設定
        GameObject hpBarPrefab = Resources.Load<GameObject>(Parameters.HPBAR_RESOUCE_PATH);
        GameObject objHpBar = Instantiate(hpBarPrefab, Vector3.zero, Quaternion.identity);
        UIHPBar hpBar = objHpBar.GetComponent<UIHPBar>();
        hpBar.Character = transform;
        hpBar.Offset = Parameters.HPBAR_OFFSET;
        HpBar = hpBar;
        objHpBar.transform.SetParent(GameObject.Find("UIPlay").transform);
        objHpBar.transform.localScale = new Vector3(1, 1, 1);

        //アクションバーを設定
        GameObject actionBarPrefab = Resources.Load<GameObject>(Parameters.ACTIONBAR_RESOUCE_PATH);
        GameObject actionBarObj = Instantiate(actionBarPrefab, Vector3.zero, Quaternion.identity);
        UIActionBar actionBar = actionBarObj.GetComponent<UIActionBar>();
        actionBar.Character = transform;
        actionBar.Offset = Parameters.ACTIONBAR_OFFSET;
        actionBar.Owner = this;
        ActionBar = actionBar;
        actionBar.transform.SetParent(GameObject.Find("UIPlay").transform);
        actionBar.transform.localScale = new Vector3(1, 1, 1);

        IsStunable = true;
        IsDead = false;
        EnemyCheckCount = 0;
    }

    void Start()
    {
        _ = ExcecuteActionLoop();
    }

    void FixedUpdate()
    {
        //敵の情報の更新
        UpdateEnemies();

        HpBar.Character = transform;

        //最高速度を指定
        Vector2 maxVelocity = new Vector2(Parameters.MAX_VELOCITY_X, Parameters.MAX_VELOCITY_Y);
        Vector2 clampedVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxVelocity.x, maxVelocity.x),
            Mathf.Clamp(rb.linearVelocity.y, -maxVelocity.y, maxVelocity.y)
        );

        rb.linearVelocity = clampedVelocity;

        if (IsBackward)
        {
            if (Mathf.Abs(rb.linearVelocity.x) < Parameters.BACKSTEP_CANCELATION_VELOCITY)
            {
                IsBackward = false;
            }
        }

        //スタン状態の処理
        if (IsStunned && IsStunable)
        {
            IsStunable = false;

            _ = Stun();
        }

        //死亡判定
        if (!IsDead)
        {
            bool judge = false;

            if (HpBar.IsEmpty()) judge = true;
            if (transform.position.y <= Parameters.DEAD_LINE_Y_DOWN) judge = true;
            if (transform.position.y >= Parameters.DEAD_LINE_Y_UP) judge = true;
            if (Mathf.Abs(transform.position.x) >= Parameters.DEAD_LINE_X) judge = true;

            if (judge)
            {
                IsDead = true;
                gameObject.SetActive(false);
                HpBar.gameObject.SetActive(false);

                //死亡エフェクトを再生
                VFXManager.Instance.Play(VFX.Dead, transform.position, transform.rotation);

                //アクションをキャンセル
                canceler.Cancel();
            }
        }
    }

    public async virtual Task Action()
    {
        await Task.Yield();
    }

    private async Task ExcecuteActionLoop()
    {
        IsFacingRight = true;

        if (transform.position.x >= 0) Flip();

        await Wait(Parameters.START_INTERVAL);

        while (!IsDead && canceler.IsNotCancel)
        {
            if (!IsStunned)
            {
                await ActionLoop();
            }
            else
            {
                ActionBar.SendText("Stun");
            }

            await Task.Yield();
        }
    }

    protected async virtual Task ActionLoop()
    {
        await Task.Yield();
    }

    protected async Task Wait(float sec)
    {
        if (canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }

    // 攻撃
    protected async virtual Task Attack(int number = 1)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Attack");

        IsAttacking = true;

        await weapon.ExecuteAttack(number);

        await Wait(Parameters.ACTION_INTERVAL_ATTACK);

        IsAttacking = false;
    }

    // ガード
    protected async virtual Task Guard()
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Guard");

        IsGuarding = true;

        //ガード実行
        await guard.ExecuteGuard();

        IsGuarding = false;

        await Wait(Parameters.ACTION_INTERVAL_GUARD);
    }

    //ダッシュ：相手に向かって進む
    protected async virtual Task Forward(float force)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Forward");

        IsForward = true;

        //相手を見る
        await LookAtEnemy();

        //相手に向かって進む
        rb.AddForce(EnemyDirection.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_FORWARD);

        IsForward = false;
    }

    //バックステップ：相手から離れる
    protected async virtual Task Backward(float force)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("BackStep");

        IsBackward = true;

        //相手を見る
        await LookAtEnemy();

        //相手から離れる
        rb.AddForce(-EnemyDirection.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_BACKWARD);
    }

    // 垂直ジャンプ
    protected async virtual Task Jump(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Jump");

        await JumpCommon(Vector2.up, height);
    }

    // 前斜めジャンプ
    protected async virtual Task JumpForward(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("JumpForward");

        await LookAtEnemy();

        //ジャンプ方向を計算
        Vector2 dir = Vector2.zero;
        dir = Parameters.FORWARD_JUMP_DIRECTION;

        if (EnemyDirection.x < 0)
        {
            dir.x *= -1;
        }

        await JumpCommon(dir, height);
    }

    // 後斜めジャンプ
    protected async virtual Task JumpBackward(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("JumpBackward");

        await LookAtEnemy();

        //ジャンプ方向を計算
        Vector2 dir = Vector2.zero;
        dir = Parameters.BACKWARD_JUMP_DIRECTION;

        if (EnemyDirection.x < 0)
        {
            dir.x *= -1;
        }

        await JumpCommon(dir, height);
    }

    // 自由移動
    protected async virtual Task Move(float x, float y, float force)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Move");

        IsForward = true;

        if (x > 0 && !IsFacingRight)
        {
            Flip();
        }
        else if (x < 0 && IsFacingRight)
        {
            Flip();
        }

        rb.AddForce(new Vector2(x, y).normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_MOVE);

        IsForward = false;
    }

    // 浮遊状態の切り替え
    protected async Task Floating(bool value)
    {
        IsFloating = value;

        if (IsFloating)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = Parameters.GRAVITY_SCALE;
        }

        await Wait(Parameters.ACTION_INTERVAL_FLOATING);
    }

    // ジャンプの共通処理
    private async Task JumpCommon(Vector2 direction, float height)
    {
        AudioManager.Instance.PlaySE(Parameters.SE_JUMP);

        IsJumping = true;

        //相手を見る
        await LookAtEnemy();

        //必要なジャンプ力を計算
        float jumpForce = Mathf.Sqrt(2 * height * Physics2D.gravity.magnitude * rb.mass * rb.gravityScale);
        jumpForce *= Parameters.JUMP_FORCE_SCALE;

        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_JUMP);
    }

    // 相手の方を向く
    public async Task LookAtEnemy()
    {
        if (EnemyDirection.x > 0 && !IsFacingRight)
        {
            Flip();
        }
        else if (EnemyDirection.x < 0 && IsFacingRight)
        {
            Flip();
        }

        await Task.Yield();
    }

    // スタン状態の処理
    private async Task Stun()
    {
        weapon.CancelActions();

        await body.Flash();

        IsStunned = false;

        //スタン可能にする
        IsStunable = true;

        weapon.ResetActions();
    }

    //==============衝突判定=================//

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        //ステージの壁との接触時の処理
        if (obj.CompareTag(Tags.StageWall))
        {
            if (collision.contactCount > 0)
            {
                Debug.Log(gameObject.name + " > Hit Wall");

                var contact = collision.contacts;
                PlayHitWallVFX(contact[0].point);

                //ダメージ加えて吹き飛ばす
                HpBar.TakeDamage(Parameters.WALL_DAMAGE);
                Vector2 direction = (transform.position - obj.transform.position).normalized;
                rb?.AddForce(direction * Parameters.WALL_FORCE, ForceMode2D.Impulse);
            }
        }


        //地面に接触時の処理
        if (obj.CompareTag(Tags.Platform))
        {
            IsJumping = false;
            IsGrounded = true;

            if (rb.linearVelocity.magnitude > Parameters.LAND_VELOCITY)
            {
                //着地音を再生
                AudioManager.Instance.PlaySE(Parameters.SE_LAND);
            }

        }

        //魔法によるダメージ
        if (HasComponent<Magic>(obj))
        {
            Magic magic = obj.GetComponent<Magic>();

            if (magic.Owner != this)
            {
                HpBar.TakeDamage(magic.Damage);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        //敵との接触時のノックバック処理
        if (HasComponent<Monster>(obj))
        {
            IsJumping = false;
            Knockback(obj);
        }

        //ステージの壁との接触時の処理
        if (obj.CompareTag(Tags.StageWall))
        {
            if (collision.contactCount > 0)
            {
                Vector2 direction = (transform.position - obj.transform.position).normalized;
                rb?.AddForce(direction * Parameters.WALL_FORCE, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Platform))
        {
            IsGrounded = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        //敵との接触時のノックバック処理
        if (HasComponent<Monster>(obj))
        {
            IsJumping = false;
            Knockback(obj);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (HasComponent<Magic>(obj))
        {
            Magic magic = obj.GetComponent<Magic>();

            if (magic.Owner == this)
            {
                magic.SetCollisionEnable(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        //武器との接触時の処理
        if (obj.CompareTag(Tags.Weapon))
        {
            if (HasComponent<Weapon>(obj))
            {
                Weapon weapon = obj.GetComponent<Weapon>();

                bool hitable = false;

                if (weapon.Owner != this)
                {
                    hitable = true;
                }
                else
                {
                    if (weapon.IsHitableOwner)
                    {
                        hitable = true;
                    }
                }

                if (hitable)
                {
                    Vector2 direction = (transform.position - obj.transform.position).normalized;
                    direction = new Vector2(direction.x, direction.y + Parameters.WEAPON_ONHIT_ADD_DIRECTION_Y).normalized;

                    var enemyRb = weapon.Owner.GetComponent<Rigidbody2D>();
                    var enemyVelocity = enemyRb.linearVelocity;
                    enemyRb.linearVelocity *= Parameters.WEAPON_HIT_VELOCITY_REDUCATION_RATE;

                    if (IsGuarding)
                    {
                        HpBar.TakeDamage(weapon.Damage * Parameters.WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING);
                        rb.AddForce(direction * weapon.StrikeForce * Parameters.WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING, ForceMode2D.Impulse);
                    }
                    else
                    {
                        PlayHitWeaponVFX(other);

                        //ダメージ加えて吹き飛ばす
                        float damage = weapon.Damage;
                        HpBar.TakeDamage(damage);
                        rb.AddForce(direction * weapon.StrikeForce, ForceMode2D.Impulse);

                        Debug.Log("Damage : " + weapon.Owner.gameObject.name + " -> " + gameObject.name + " : " + damage);
                    }
                }
            }
        }
    }

    //==============補助関数=================//

    // 武器のヒットエフェクトの再生
    private void PlayHitWeaponVFX(Collider2D weaponCollider)
    {
        // 衝突点を取得
        Vector3 collisionPoint = weaponCollider.ClosestPoint(transform.position);

        // ヒットエフェクトを再生
        VFXManager.Instance.Play(Parameters.VFX_HIT_S, collisionPoint, transform.rotation);
    }

    // ステージの壁のヒットエフェクトの再生
    private void PlayHitWallVFX(Vector3 collisionPoint)
    {
        // ヒットエフェクトを再生
        VFXManager.Instance.Play(Parameters.VFX_HIT_WALL, collisionPoint, transform.rotation);
    }

    // ノックバック処理
    private void Knockback(GameObject enemy)
    {
        if (HasComponent<Rigidbody2D>(enemy) && HasComponent<Rigidbody2D>(gameObject))
        {
            Vector2 dir = (enemy.transform.position - gameObject.transform.position).normalized;

            float ownDirX;
            float enemyDirX;

            if (dir.x < 0)
            {
                ownDirX = dir.x;
                enemyDirX = -dir.x;
            }
            else
            {
                ownDirX = -dir.x;
                enemyDirX = dir.x;
            }

            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(ownDirX, dir.y) * Parameters.KNOCKBACK_FORCE, ForceMode2D.Impulse);
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(enemyDirX, dir.y) * Parameters.KNOCKBACK_FORCE, ForceMode2D.Impulse);
        }
    }

    // キャラクターの向きを反転する
    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // 敵の情報の更新
    private void UpdateEnemies()
    {
        if (EnemyCheckCount >= Parameters.ENEMY_CHECK_FREAKENCE)
        {
            List<Monster> enemies = new List<Monster>(Enemies);

            // 生きている敵を抽出
            List<Monster> ariveEnemies = enemies.Where(e => !e.IsDead).ToList();

            // 距離順に並び変えて更新
            Enemies = ariveEnemies.OrderBy(obj => Vector3.Distance(obj.transform.position, transform.position)).ToList();

            if (Enemies.Count > 0)
            {
                //一番近い敵も更新
                Enemy = Enemies[0];
            }

            EnemyCheckCount = 0;
        }

        EnemyCheckCount++;
    }

    // コンポーネントの有無を確認
    private bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

}