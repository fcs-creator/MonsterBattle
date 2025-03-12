using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Threading;
using System;
using UnityEditor.Experimental.GraphView;

public class Monster : MonoBehaviour
{
    public Monster Enemy { get; set; }                  //最も近い敵
    public List<Monster> Enemies { get; set; }          //全ての敵
    public MagicBook MagicBook { get; set; }            //魔法の書
    public UIHPBar HpBar { get; private set; }          //HPバー
    public UIActionBar ActionBar { get; private set; }  //アクションバー

    public bool IsAttacking { get; private set; }                                               //攻撃中か
    public bool IsGuarding { get; private set; }                                                //防御中か
    public bool IsBackSteping { get; private set; }                                             //バックステップ中か
    public bool IsDashing { get; private set; }                                                 //ダッシュ中か
    public bool IsJumping { get; private set; }                                                 //ジャンプ中か
    public bool IsGrounded { get; private set; }                                                //地面にいるか
    public bool IsAirborne { get { return !IsGrounded; } private set { IsAirborne = value; } }  //空中にいるか
    public bool IsDead { get; private set; }                                                    //死んでいるか
    public bool IsFacingRight { get; private set; }                                             //右を向いているか
    public bool IsStunned { get; set; }                                                         //スタン状態か
    public bool IsFloating { get; set; }                                                        //浮遊状態か

    Body body;          //本体
    Weapon weapon;      //武器
    Guard guard;        //防具
    Rigidbody2D rb;
    
    bool IsStunable = true; //スタン可能か
    int EnemyCheckCount = 0;

    //敵の情報
    public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.y); } }
    public Vector2 Direction { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).normalized; } }
    public float Distance { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).magnitude; } }

    //タスクをキャンセル
    readonly Canceler canceler = new Canceler();

    public void CancelActions()
    {
        canceler.Cancel();
    }

    void Awake()
    {
        //物理挙動を追加
        rb = gameObject.AddComponent<Rigidbody2D>();

        // 本体の設定
        body = transform.Find("Body").AddComponent<Body>();

        // 武器の設定
        weapon = transform.Find("Weapon").GetComponent<Weapon>();
        weapon?.gameObject.SetActive(true);

        // 防具の設定
        guard = transform.Find("Guard").AddComponent<Guard>();
        guard?.gameObject.SetActive(false);

        //アクションバーの所有者を登録
        ActionBar.Owner = this;

        IsDead = false;
        EnemyCheckCount = 0;
    }

    void Start()
    {
        _ = ExcecuteActionLoop();
    }

    public async virtual Task Action()
    {
        await Task.Yield();
    }

    async Task ExcecuteActionLoop()
    {
        IsFacingRight = true;
        if (transform.position.x >= 0) Flip();

        await Wait(Parameters.START_INTERVAL);

        while (!IsDead && canceler.IsNotCancel)
        {
            await ActionLoop();

            await Task.Yield();
        }
    }

    async virtual protected Task ActionLoop()
    {
        await Task.Yield();
    }

    async protected Task Wait(float sec)
    {
        if(canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }

    private void FixedUpdate()
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

        if(IsBackSteping)
        {
            if (Mathf.Abs(rb.linearVelocity.x) < Parameters.BACKSTEP_CANCELATION_VELOCITY)
            {
                IsBackSteping = false;
            }
        }

        //スタン状態の処理
        if (IsStunned && IsStunable) 
        {
            _ = Stun();
        }

        //死亡判定
        if (!IsDead) 
        {
            bool judge = false;

            if (HpBar.IsEmpty())                                           judge = true;
            if (transform.position.y <= Parameters.DEAD_LINE_Y_DOWN)       judge = true;
            if (transform.position.y >= Parameters.DEAD_LINE_Y_UP)         judge = true;
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

    // 攻撃
    async virtual protected Task Attack() 
    {
        if(canceler.IsCancel) return;

        ActionBar.SendText("Attack");

        IsAttacking = true;

        await weapon?.ExecuteAttack();

        await Wait(Parameters.ACTION_INTERVAL_ATTACK);

        IsAttacking = false;
    }

    // ガード : アタックへの対抗
    async virtual protected Task Guard() 
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Guard");

        IsGuarding = true;

        guard?.gameObject.SetActive(true);

        await Wait(Parameters.GUARD_DURATION);

        guard?.gameObject.SetActive(false);

        IsGuarding = false;

        await Wait(Parameters.ACTION_INTERVAL_GUARD);
    }

    //ダッシュ：相手に向かって進む
    async virtual protected Task Forward(float force) 
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Forward");

        IsDashing = true;

        //相手を見る
        LookAtEnemy();

        //相手に向かって進む
        rb.AddForce(Direction.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_DASH);

        IsDashing = false;
    }

    //バックステップ：相手から離れる
    async virtual protected Task BackStep(float force)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("BackStep");

        IsBackSteping = true;

        //相手を見る
        LookAtEnemy();

        //相手から離れる
        rb.AddForce(-Direction.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);
        
        await Wait(Parameters.ACTION_INTERVAL_BACKSTEP);
    }

    // 垂直ジャンプ (100で画面上まで飛ぶ)
    async virtual protected Task Jump(float height) 
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Jump");

        await JumpCommon(Vector2.up, height);
    }

    // 前斜めジャンプ
    async virtual protected Task JumpForward(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("JumpForward");

        //ジャンプ方向を計算
        Vector2 dir = Vector2.zero;
        dir = Parameters.FORWARD_JUMP_DIRECTION;

        if (Direction.x < 0)
        {
            dir.x *= -1;
        }

        await JumpCommon(dir, height);
    }

    // 後斜めジャンプ
    async virtual protected Task JumpBackward(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("JumpBackward");

        //ジャンプ方向を計算
        Vector2 dir = Vector2.zero;
        dir = Parameters.BACKWARD_JUMP_DIRECTION;

        if (Direction.x < 0)
        {
            dir.x *= -1;
        }

        await JumpCommon(dir, height);
    }

    async Task JumpCommon(Vector2 direction, float height)
    {
        if (IsGrounded)
        {
            AudioManager.Instance.PlaySE(Parameters.SE_JUMP);

            IsJumping = true;

            //相手を見る
            LookAtEnemy();

            //必要なジャンプ力を計算
            float jumpForce = Mathf.Sqrt(2 * height * Physics2D.gravity.magnitude * rb.mass * rb.gravityScale);
            jumpForce *= Parameters.JUMP_FORCE_SCALE;

            rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
        }

        await Wait(Parameters.ACTION_INTERVAL_JUMP);
    }

    async Task Stun() 
    {
        //スタン不可にする
        IsStunable = false;

        await body.Stun();

        await Wait(Parameters.GUARD_STUN_DURATION);

        IsStunned = false;

        //スタン可能にする
        IsStunable = true;
    }

    //浮遊状態の切り替え
    async protected Task Floating(bool value) 
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

    void OnTriggerEnter2D(Collider2D other)
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
                    
                    //武器のダメージ値の計算
                    weapon.CalcutlateDamage();

                    if (IsGuarding)
                    {
                        HpBar.TakeDamage(weapon.Damage * Parameters.WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING);
                        rb.AddForce(direction * weapon.StrikeForce * Parameters.WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING,ForceMode2D.Impulse);
                    }
                    else
                    {
                        PlayHitWeaponVFX(other);

                        //ダメージ加えて吹き飛ばす
                        HpBar.TakeDamage(weapon.Damage);
                        rb.AddForce(direction * weapon.StrikeForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    //武器のヒットエフェクトの再生
    void PlayHitWeaponVFX(Collider2D weaponCollider) 
    {
        // 衝突点を取得
        Vector3 collisionPoint = weaponCollider.ClosestPoint(transform.position);

        // ヒットエフェクトを再生
        VFXManager.Instance.Play(Parameters.VFX_HIT_S, collisionPoint, transform.rotation);
    }

    //ステージの壁のヒットエフェクトの再生
    void PlayHitWallVFX(Vector3 collisionPoint)
    {
        // ヒットエフェクトを再生
        VFXManager.Instance.Play(Parameters.VFX_HIT_WALL, collisionPoint, transform.rotation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        //ステージの壁との接触時の処理
        if (obj.CompareTag(Tags.StageWall))
        {
            if(collision.contactCount > 0) 
            {
                Debug.Log(gameObject.name + " > Hit Wall");

                var contact = collision.contacts;
                PlayHitWallVFX(contact[0].point);

                //ダメージ加えて吹き飛ばす
                HpBar.TakeDamage(Parameters.WALL_DAMAGE);
                Vector2 direction = (transform.position - obj.transform.position).normalized;
                rb.AddForce(direction * weapon.StrikeForce, ForceMode2D.Impulse);
            }
        }


        //地面に接触時の処理
        if (obj.CompareTag(Tags.Platform))
        {
            IsJumping = false;
            IsGrounded = true;

            if(rb.linearVelocity.magnitude > Parameters.LAND_VELOCITY)
            {
                //着地音を再生
                AudioManager.Instance.PlaySE(Parameters.SE_LAND);
            }
            
        }

        //魔法によるダメージ
        if (HasComponent<Magic>(obj))
        {
            Magic magic = obj.GetComponent<Magic>();

            if(magic.Owner != this)
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Platform))
        {
            IsGrounded = false;
        }
    }

    private void Knockback(GameObject enemy) 
    {
        if (HasComponent<Rigidbody2D>(enemy) && HasComponent<Rigidbody2D>(gameObject))
        { 
            Vector2 direction = (enemy.transform.position - gameObject.transform.position).normalized;
            direction.x += UnityEngine.Random.Range(-Parameters.KNOCKBACK_RANDOM_RANGE_X, Parameters.KNOCKBACK_RANDOM_RANGE_X);
            gameObject.GetComponent<Rigidbody2D>().AddForce(direction * -Parameters.KNOCKBACK_FORCE, ForceMode2D.Impulse);
            enemy.GetComponent<Rigidbody2D>().AddForce(direction * Parameters.KNOCKBACK_FORCE, ForceMode2D.Impulse);
        }
    }

    bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    // キャラクターの向きを反転する
    public void Flip() 
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //相手の方を向く
    public void LookAtEnemy() 
    {
        if (Direction.x > 0 && !IsFacingRight)
        {
            Flip();
        }
        else if (Direction.x < 0 && IsFacingRight)
        {
            Flip();
        }
    }

    public void SetHpBar(UIHPBar hpBar)
    {
        this.HpBar = hpBar;
    }

    public void SetActionBar(UIActionBar actionBar)
    {
        this.ActionBar = actionBar;
    }

    //敵の情報の更新
    void UpdateEnemies()
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
}