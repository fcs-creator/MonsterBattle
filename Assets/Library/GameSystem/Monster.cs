using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Threading;

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
    
    Body body;          //本体
    Weapon weapon;      //武器
    GameObject shield;  //シールド
    Rigidbody2D rb;
    　
    int EnemyCheckCount = 0;

    //敵の情報
    public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.y); } }
    public Vector2 Direction { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).normalized; } }
    public float Distance { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).magnitude; } }

    //タスクキャンセル用
    List<CancellationTokenSource> cancellationTokenSources = new List<CancellationTokenSource>();

    void Awake()
    {
        //物理挙動を追加
        rb = gameObject.AddComponent<Rigidbody2D>();

        // 本体の設定
        body = transform.Find("Body").AddComponent<Body>();

        // 武器の設定
        weapon = transform.Find("Weapon").GetComponent<Weapon>();
        weapon.gameObject.SetActive(true);

        // シールドの設定
        //shield = transform.Find("Shield").gameObject;
        //shield.tag = Tags.Shield;
        //shield.AddComponent<PolygonCollider2D>().autoTiling = true;
        //shield.SetActive(false);
    
        IsDead = false;
        EnemyCheckCount = 0;
    }

    CancellationToken CreateCancellationToken() 
    {
        var cts = new CancellationTokenSource();
        cancellationTokenSources.Add(cts);
        return cts.Token;
    }

    void Start()
    {
        //var token = CreateCancellationToken();
        //Task.Run(() => ExcecuteActionLoop(token), token);

        _ = ExcecuteActionLoop();
    }

    async Task ExcecuteActionLoop()
    {
        IsFacingRight = true;
        if (transform.position.x >= 0) Flip();

        while (!IsDead)
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
        await Task.Delay((int)(sec*1000));
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
            }
        }
    }

    // 攻撃
    async virtual protected Task Attack() 
    {
        ActionBar.SendText("Attack");

        IsAttacking = true;

        await weapon.ExecuteAttack();

        await Wait(Parameters.ACTION_INTERVAL_ATTACK);

        IsAttacking = false;
    }

    // ガード
    async virtual protected Task Guard() 
    {
        ActionBar.SendText("Guard");

        IsGuarding = true;

        shield.SetActive(true);

        await Wait(Parameters.ACTION_INTERVAL_GUARD);

        shield.SetActive(false);

        IsGuarding = false;
    }

    //ダッシュ：相手に向かって進む
    async virtual protected Task Dash(float force) 
    {
        ActionBar.SendText("Dash");

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
        ActionBar.SendText("BackStep");

        IsBackSteping = true;

        //相手を見る
        LookAtEnemy();

        //相手から離れる
        rb.AddForce(-Direction.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);
        
        await Wait(Parameters.ACTION_INTERVAL_BACKSTEP);
    }

    //ジャンプ (100で画面上まで飛ぶ)
    async virtual protected Task Jump(float height) 
    {
        ActionBar.SendText("Jump");

        if (IsGrounded)
        {
            IsJumping = true;

            // 必要なジャンプ力を計算
            float jumpForce = Mathf.Sqrt(2 * height * Physics2D.gravity.magnitude * rb.mass * rb.gravityScale);

            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        await Wait(Parameters.ACTION_INTERVAL_JUMP);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

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

                    if (IsGuarding)
                    {
                        HpBar.TakeDamage(weapon.Damage * Parameters.WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING);
                        rb.AddForce(direction * weapon.StrikeForce * Parameters.WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING,ForceMode2D.Impulse);
                    }
                    else
                    {
                        HpBar.TakeDamage(weapon.Damage);
                        rb.AddForce(direction * weapon.StrikeForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        //地面に接触時の処理
        if (obj.CompareTag(Tags.Platform))
        {
            IsJumping = false;
            IsGrounded = true;
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
            gameObject.GetComponent<Rigidbody2D>().AddForce(direction * -Parameters.KNOCKBACK_FORCE, ForceMode2D.Impulse);
            enemy.GetComponent<Rigidbody2D>().AddForce(direction * Parameters.KNOCKBACK_FORCE, ForceMode2D.Impulse);
        }
    }

    private bool HasComponent<T>(GameObject obj) where T : Component
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

            //距離順に並び変えて更新
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

    // 数値を異なる範囲にマッピングする関数
    float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}