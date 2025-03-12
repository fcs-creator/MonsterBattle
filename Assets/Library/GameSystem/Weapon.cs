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
    public Monster Owner { get; private set; }          // ����̏��L��(�����X�^�[)
    public bool IsHitableOwner { get; private set; }    // ���킪���L�҂ɓ����邩
    public float StrikeForce { get; private set; }      // ����̐�����΂���

    // ����̃_���[�W
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

    private float damage;

    Vector3 defaultLocalPosition;   // ����̏������W
    Vector3 defaultLocalScale;      // ����̏����X�P�[��
    Quaternion defaultRotation;     // ����̏�����]

    Rigidbody2D rb;
    List<GameObject> weapons;

    Vector3 initialOffset;
    float orbitRadius;

    bool isShot;

    //�^�X�N���L�����Z�����邽�߂̋��ʃg�[�N��
    readonly Canceler canceler = new Canceler();

    void Awake()
    {
        // 1��̊K�w�ɂ��郂���X�^�[�̃I�u�W�F�N�g��T���ăZ�b�g
        Owner = transform.parent.GetComponent<Monster>();

        // ����������ǉ����Ė����ɂ��Ă���
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.centerOfMass = new Vector2(0, 0);
        rb.simulated = true;
        SetGripWeapon(true);

        //����̃_���[�W
        Damage = Parameters.WEAPON_DAMAGE;

        //����̐�����΂���
        StrikeForce = Parameters.WEAPON_STRIKE_FORCE;

        // �����̈ʒu�A��]�A�X�P�[����ۑ�
        defaultLocalPosition = transform.localPosition;
        defaultLocalScale = transform.localScale;
        defaultRotation = transform.rotation;

        // �����I�t�Z�b�g���v�Z
        initialOffset = transform.position - Owner.transform.position;
        orbitRadius = initialOffset.magnitude;

        //���툵���̃I�u�W�F�N�g�����ׂĎ擾
        weapons = new List<GameObject>();
        weapons.Add(gameObject);//�e��ǉ�
        foreach (Transform child in transform)
        {
            weapons.Add(child.gameObject);// �q�I�u�W�F�N�g�����X�g�ɒǉ�
        }

        float area = 0.0f;
        float massMag = Parameters.MASS_WEAPON_MAGNIFICATION;
        float massMax = Parameters.MASS_WEAPON_MAX;
        float massMin = Parameters.MASS_WEAPON_MIN;

        foreach (GameObject obj in weapons)
        {
            // �^�O��ݒ�
            gameObject.tag = Tags.Weapon;

            if (HasComponent<SpriteRenderer>(obj))
            {
                //�X�v���C�g�̃\�[�g���C���[��ݒ�
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = SortLayer.Weapon;

                // �摜�̌`��ɍ��킹�ăR���C�_��ݒ�
                var weaponCollider = gameObject.AddComponent<PolygonCollider2D>();
                weaponCollider.autoTiling = true;
                weaponCollider.isTrigger = true;
                area += CalculateScaledArea(weaponCollider);
            }
        }

        rb.mass = Mathf.Clamp(Mathf.Sqrt(area) * massMag + massMin, massMin, massMax);  // ���ʂ�ݒ�
        Debug.Log("Weapon >> " + transform.parent.name + " : " + rb.mass + "kg");

        isShot = false;

        // �ŏ��͕�����B���Ă���
        SetActive(false);
    }

    void Start()
    {
        _ = ExcecuteActionLoop();   //�Ăяo��&Task��j��
    }

    async Task ExcecuteActionLoop()
    {
        await Wait(Parameters.START_INTERVAL);

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

        canceler.Cancel();
    }

    async public Task ExecuteAttack()
    {
        WarpDefault();

        IsHitableOwner = false;

        SetActive(true);

        await Attack();

        SetActive(false);
    }

    //��{�͂����炪�Ăяo�����
    async virtual protected Task Attack()
    {
        await Task.Yield();
    }

    //�������䂷�镐�킪����(���܂��v�f)
    async virtual protected Task ActionLoop()
    {
        await Task.Yield();
    }

    //�w��b���҂�
    async protected Task Wait(float sec)
    {
        if (canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }

    //�����ʒu�Ƀ��[�v
    void WarpDefault()
    {
        //�������������Ԃɂ���
        SetGripWeapon(true);

        transform.SetParent(Owner.transform);
        transform.localPosition = defaultLocalPosition;
        transform.rotation = defaultRotation;
        transform.localScale = defaultLocalScale;
    }

    //�����̌q�����⊮����֐�
    async Task Lerp(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Vector3 endPosition, Quaternion endRotation, Vector3 endScale, float second)
    {
        if (canceler.IsCancel) return;

        float elapsedTime = 0f;

        while (elapsedTime < second && canceler.IsNotCancel)
        {
            float t = elapsedTime / second;

            // ��Ԃ��s��
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }

    //����������ʒu�Ƀ��Z�b�g����
    async protected Task Default()
    {
        //��������郂�[�h�ɂ���
        SetGripWeapon(true);

        //���݂̏��
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        Vector3 currentScale = transform.localScale;

        //���Z�b�g��̏��(���L���郂���X�^�[�̉�]�Ɗg�k�͔��f���Ȃ�)
        Vector3 targetPosition;
        Quaternion targetRotation = defaultRotation;
        Vector3 targetScale = defaultLocalScale;

        //�����ɂ���Ė߂��ʒu��ς���
        if (Owner.IsFacingRight)
            targetPosition = Owner.transform.position + defaultLocalPosition;
        else
            targetPosition = Owner.transform.position - defaultLocalPosition;

        //�⊮����
        await Lerp(currentPosition, currentRotation, currentScale, targetPosition, targetRotation, targetScale, Parameters.DEFAULT_RETURN_TIME);

        //�Ō�ɐe�ɖ߂�
        transform.SetParent(Owner.transform);

        // �Ō�Ɋm���Ɍ��̏�Ԃɐݒ�
        transform.localPosition = defaultLocalPosition;
        transform.rotation = targetRotation;
        transform.localScale = targetScale;

        await Wait(Parameters.DEFAULT_RETURN_WAIT_TIME);
    }

    //����������
    async protected Task Drawing()
    {
        if (canceler.IsCancel) return;

        Owner.ActionBar.SendText("Weapon-Drawing");

        //SE���Đ�
        AudioManager.Instance.PlaySE(Parameters.SE_WEAPON_DRAWING);

        float elapsedTime = 0f;
        float s = Parameters.WEAPON_INTERVAL_DRAWING;

        while (elapsedTime < s && canceler.IsNotCancel)
        {
            // �o�ߎ��Ԃ̊������v�Z
            float t = elapsedTime / s;
            // �o�ߎ��Ԃ��X�V
            elapsedTime += Time.deltaTime;
            // 1�t���[���ҋ@
            await Task.Yield();
        }
    }

    //������w�肳�ꂽ(x, y)�ʒu��s�b�ňړ�������
    async protected Task Move(float x, float y, float s)
    {
        if (canceler.IsCancel) return;

        Owner.ActionBar.SendText("Weapon-Move");

        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(x, y);
        float elapsedTime = 0f;

        while (elapsedTime < s && canceler.IsNotCancel)
        {
            // �o�ߎ��Ԃ̊������v�Z
            float t = elapsedTime / s;
            // ���`��ԁiLerp�j�ňʒu���X�V
            transform.localPosition = Vector2.Lerp(start, target, t);
            // �o�ߎ��Ԃ��X�V
            elapsedTime += Time.deltaTime;
            // 1�t���[���ҋ@
            await Task.Yield();
        }

        // �ŏI�ʒu��ݒ�i�덷��␳�j
        transform.localPosition = target;
    }

    //�����angle�xs�b�ł��̏��]������
    async protected Task Spin(float angle, float s)
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
            // Z�����ŉ�]������
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
            // With the following line
            await Task.Yield();
        }

        // �Ō�Ɋm���ɖڕW�p�x�܂ŉ�]������
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    //����������X�^�[�̎��͂ŉ�]������(����������0��)
    async protected Task Rotate(float startAngle, float rotAngle, float second) 
    {
        if (canceler.IsCancel) return;

        Owner.ActionBar.SendText("Weapon-Rotate");

        if (!Owner.IsFacingRight)
            transform.localScale = new Vector2(defaultLocalScale.x * -1, defaultLocalScale.y);

        //���v��肩�ǂ�������
        bool clockwise = true;
        if ((Owner.IsFacingRight && rotAngle < 0) || (!Owner.IsFacingRight && rotAngle >= 0))
            clockwise = false;

        //�J�n�p�x�̒���
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
            //�t���[�����̉�]�ʂ��v�Z
            step = (rotAngle / second) * Time.deltaTime;

            //�����ɉ����ċt��]
            if ((clockwise && rotAngle > 0) || (!clockwise && rotAngle < 0))
                step *= -1;

            //���݂܂ł̉�]���ʂ̉��Z
            currentRotAngle += Mathf.Abs(step);

            //��]�ʂ��w��ʂɒB������I��
            if (currentRotAngle >= Mathf.Abs(rotAngle))
            {
                currentRotAngle = rotAngle;
                stop = true;
            }
            else
            {
                //�ʒu�F���a�����Ɉړ����ĕ␳����
                Vector3 desiredPosition = (transform.position - centerObject.position).normalized * orbitRadius + centerObject.position;
                transform.position = desiredPosition;
                transform.RotateAround(centerObject.position, Vector3.forward, step);
            }

            await Task.Yield();
        }

        WarpDefault();
    }

    //����������X�^�[�̎��͂�1��]����
    async protected Task Rotation360()
    {
        if (canceler.IsCancel) return;

        Transform centerObject = Owner.transform;

        float rotationSpeed = 400.0f; // ��]���x
        float angle = 0;
        float angleAmount = 0;

        while (angleAmount < 360 && canceler.IsNotCancel)
        {
            //��]�F���S���
            angle = rotationSpeed * Time.deltaTime;
            transform.RotateAround(centerObject.position, Vector3.forward, angle);

            //�ʒu�F���a�����Ɉړ����ĕ␳����
            Vector3 desiredPosition = (transform.position - centerObject.position).normalized * orbitRadius + centerObject.position;
            transform.position = desiredPosition;

            angleAmount += angle;
            await Task.Yield();
        }
    }

    //������w�肳�ꂽ�����ɔ�΂� (������-1~1�̏����Ŏw�� �����:1, ����:0, ������: -1)
    async protected Task Shot(float directionY, float power)
    {
        if (canceler.IsCancel) return;

        isShot = true;
        
        Owner.ActionBar.SendText("Weapon-Shot");

        float dirY = Mathf.Clamp(directionY, -1.0f, 1.0f);
        float dirX = 1 - Mathf.Abs(dirY);

        //�����X�^�[�̌����ɂ���ĕ����΂�������ς���
        if (!Owner.IsFacingRight)
        {
            dirX *= -1;
        }
        else 
        {
            //dirY *= -1;
        }

        //���킩���𗣂�
        SetGripWeapon(false);

        //SE�Đ�
        AudioManager.Instance.PlaySE(Parameters.SE_WEAPON_SHOT);

        //��΂�
        rb.AddForce(new Vector2(dirX,dirY) * power * Parameters.WEAPON_SHOT_FORCE_SCALE, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_SHOT);

        isShot = false;
    }

    //����������X�^�[�ɂ�鐧�䂩��؂藣��
    async protected Task Purge()
    {
        SetGripWeapon(false);

        // ���[���h���W��ۑ�
        Vector3 worldPosition = transform.position;

        // �e�I�u�W�F�N�g����؂藣��
        transform.SetParent(null);

        // ���̈ʒu�ɖ߂�
        transform.position = worldPosition;

        await Task.Yield();
    }

    private bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    //������Body�̓����蔻�肩��1�x�o������͓�����悤�ɂȂ�
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

    //����������Ă��邩�ǂ����̏�Ԃ��Z�b�g
    void SetGripWeapon(bool value) 
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

    float CalculateScaledArea(PolygonCollider2D collider)
    {
        // ���[�J�����W�ł̖ʐς��v�Z
        Vector2[] points = collider.points;
        float localArea = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Length];
            localArea += current.x * next.y - current.y * next.x;
        }
        localArea = Mathf.Abs(localArea) * 0.5f;

        // �e�̃X�P�[�����܂߂ăX�P�[����K�p
        Vector3 lossyScale = collider.transform.lossyScale;
        float totalScaledArea = localArea * Mathf.Abs(lossyScale.x) * Mathf.Abs(lossyScale.y);

        return totalScaledArea;
    }

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
}