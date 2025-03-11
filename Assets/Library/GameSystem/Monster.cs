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
    public Monster Enemy { get; set; }                  //�ł��߂��G
    public List<Monster> Enemies { get; set; }          //�S�Ă̓G
    public MagicBook MagicBook { get; set; }            //���@�̏�
    public UIHPBar HpBar { get; private set; }          //HP�o�[
    public UIActionBar ActionBar { get; private set; }  //�A�N�V�����o�[

    public bool IsAttacking { get; private set; }                                               //�U������
    public bool IsGuarding { get; private set; }                                                //�h�䒆��
    public bool IsBackSteping { get; private set; }                                             //�o�b�N�X�e�b�v����
    public bool IsDashing { get; private set; }                                                 //�_�b�V������
    public bool IsJumping { get; private set; }                                                 //�W�����v����
    public bool IsGrounded { get; private set; }                                                //�n�ʂɂ��邩
    public bool IsAirborne { get { return !IsGrounded; } private set { IsAirborne = value; } }  //�󒆂ɂ��邩
    public bool IsDead { get; private set; }                                                    //����ł��邩
    public bool IsFacingRight { get; private set; }                                             //�E�������Ă��邩
    public bool IsStunned { get; set; }                                                         //�X�^����Ԃ�
    public bool IsFloating { get; set; }                                                        //���V��Ԃ�

    Body body;          //�{��
    Weapon weapon;      //����
    Guard guard;        //�h��
    Rigidbody2D rb;
    
    bool IsStunable = true; //�X�^���\��
    int EnemyCheckCount = 0;

    //�G�̏��
    public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.y); } }
    public Vector2 Direction { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).normalized; } }
    public float Distance { get { return new Vector2(Enemy.transform.position.x - transform.position.x, Enemy.transform.position.y - transform.position.y).magnitude; } }

    //�^�X�N���L�����Z��
    readonly Canceler canceler = new Canceler();

    public void CancelActions()
    {
        canceler.Cancel();
    }

    void Awake()
    {
        //����������ǉ�
        rb = gameObject.AddComponent<Rigidbody2D>();

        // �{�̂̐ݒ�
        body = transform.Find("Body").AddComponent<Body>();

        // ����̐ݒ�
        weapon = transform.Find("Weapon").GetComponent<Weapon>();
        weapon?.gameObject.SetActive(true);

        // �h��̐ݒ�
        guard = transform.Find("Guard").AddComponent<Guard>();
        guard?.gameObject.SetActive(false);

        //�A�N�V�����o�[�̏��L�҂�o�^
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
        //�G�̏��̍X�V
        UpdateEnemies();

        HpBar.Character = transform;

        //�ō����x���w��
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

        //�X�^����Ԃ̏���
        if (IsStunned && IsStunable) 
        {
            _ = Stun();
        }

        //���S����
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

                //���S�G�t�F�N�g���Đ�
                VFXManager.Instance.Play(VFX.Dead, transform.position, transform.rotation);

                //�A�N�V�������L�����Z��
                canceler.Cancel();
            }
        }
    }

    // �U��
    async virtual protected Task Attack() 
    {
        if(canceler.IsCancel) return;

        ActionBar.SendText("Attack");

        IsAttacking = true;

        await weapon?.ExecuteAttack();

        await Wait(Parameters.ACTION_INTERVAL_ATTACK);

        IsAttacking = false;
    }

    // �K�[�h : �A�^�b�N�ւ̑΍R
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

    //�_�b�V���F����Ɍ������Đi��
    async virtual protected Task Forward(float force) 
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Forward");

        IsDashing = true;

        //���������
        LookAtEnemy();

        //����Ɍ������Đi��
        rb.AddForce(Direction.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_DASH);

        IsDashing = false;
    }

    //�o�b�N�X�e�b�v�F���肩�痣���
    async virtual protected Task BackStep(float force)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("BackStep");

        IsBackSteping = true;

        //���������
        LookAtEnemy();

        //���肩�痣���
        rb.AddForce(-Direction.normalized * force * Parameters.ACTION_FORCE_SCALE, ForceMode2D.Impulse);
        
        await Wait(Parameters.ACTION_INTERVAL_BACKSTEP);
    }

    // �����W�����v (100�ŉ�ʏ�܂Ŕ��)
    async virtual protected Task Jump(float height) 
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("Jump");

        await JumpCommon(Vector2.up, height);
    }

    // �O�΂߃W�����v
    async virtual protected Task JumpForward(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("JumpForward");

        //�W�����v�������v�Z
        Vector2 dir = Vector2.zero;
        dir = Parameters.FORWARD_JUMP_DIRECTION;

        if (Direction.x < 0)
        {
            dir.x *= -1;
        }

        await JumpCommon(dir, height);
    }

    // ��΂߃W�����v
    async virtual protected Task JumpBackward(float height)
    {
        if (canceler.IsCancel) return;

        ActionBar.SendText("JumpBackward");

        //�W�����v�������v�Z
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

            //���������
            LookAtEnemy();

            //�K�v�ȃW�����v�͂��v�Z
            float jumpForce = Mathf.Sqrt(2 * height * Physics2D.gravity.magnitude * rb.mass * rb.gravityScale);
            jumpForce *= Parameters.JUMP_FORCE_SCALE;

            rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
        }

        await Wait(Parameters.ACTION_INTERVAL_JUMP);
    }

    async Task Stun() 
    {
        //�X�^���s�ɂ���
        IsStunable = false;

        await body.Stun();

        await Wait(Parameters.GUARD_STUN_DURATION);

        IsStunned = false;

        //�X�^���\�ɂ���
        IsStunable = true;
    }

    //���V��Ԃ̐؂�ւ�
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
                    
                    //����̃_���[�W�l�̌v�Z
                    weapon.CalcutlateDamage();

                    if (IsGuarding)
                    {
                        HpBar.TakeDamage(weapon.Damage * Parameters.WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING);
                        rb.AddForce(direction * weapon.StrikeForce * Parameters.WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING,ForceMode2D.Impulse);
                    }
                    else
                    {
                        PlayHitVFX(obj, other);

                        //�_���[�W�����Đ�����΂�
                        HpBar.TakeDamage(weapon.Damage);
                        rb.AddForce(direction * weapon.StrikeForce, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    //�U���̃q�b�g�G�t�F�N�g�̍Đ�
    void PlayHitVFX(GameObject weapon, Collider2D weaponCollider) 
    {
        // �Փ˓_���擾
        Vector3 collisionPoint = weaponCollider.ClosestPoint(transform.position);

        // �q�b�g�G�t�F�N�g���Đ�
        VFXManager.Instance.Play(VFX.HitS, collisionPoint, transform.rotation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        //�n�ʂɐڐG���̏���
        if (obj.CompareTag(Tags.Platform))
        {
            IsJumping = false;
            IsGrounded = true;

            if(rb.linearVelocity.magnitude > Parameters.LAND_VELOCITY)
            {
                //���n�����Đ�
                AudioManager.Instance.PlaySE(Parameters.SE_LAND);
            }
            
        }

        //���@�ɂ��_���[�W
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

        //�G�Ƃ̐ڐG���̃m�b�N�o�b�N����
        if (HasComponent<Monster>(obj))
        {
            IsJumping = false;
            Knockback(obj);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        //�G�Ƃ̐ڐG���̃m�b�N�o�b�N����
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

    // �L�����N�^�[�̌����𔽓]����
    public void Flip() 
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //����̕�������
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

    //�G�̏��̍X�V
    void UpdateEnemies()
    {
        if (EnemyCheckCount >= Parameters.ENEMY_CHECK_FREAKENCE)
        {
            List<Monster> enemies = new List<Monster>(Enemies);

            // �����Ă���G�𒊏o
            List<Monster> ariveEnemies = enemies.Where(e => !e.IsDead).ToList();

            // �������ɕ��ѕς��čX�V
            Enemies = ariveEnemies.OrderBy(obj => Vector3.Distance(obj.transform.position, transform.position)).ToList();

            if (Enemies.Count > 0)
            {
                //��ԋ߂��G���X�V
                Enemy = Enemies[0];
            }

            EnemyCheckCount = 0;
        }

        EnemyCheckCount++;
    }
}