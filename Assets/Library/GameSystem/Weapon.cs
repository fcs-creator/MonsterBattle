using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Drawing;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Weapon : MonoBehaviour
{
    public Monster Owner { get; private set; }          // ����̏��L��(�����X�^�[)
    public bool IsHitableOwner { get; private set; }    // ���킪���L�҂ɓ����邩
    public float Damage { get; private set; }           // ����̃_���[�W
    public float StrikeForce { get; private set; }      // ����̐�����΂���

    Vector3 defaultLocalPosition;   // ����̏������W
    Vector3 defaultLocalScale;      // ����̏����X�P�[��
    Quaternion defaultRotation;     // ����̏�����]

    Rigidbody2D rb;
    List<GameObject> weapons;

    Vector3 initialOffset;
    float orbitRadius;

    void Awake()
    {
        // 1��̊K�w�ɂ��郂���X�^�[�̃I�u�W�F�N�g��T���ăZ�b�g
        Owner = transform.parent.GetComponent<Monster>();

        // ����������ǉ����Ė����ɂ��Ă���
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.simulated = false;

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
            }
        }

        // �ŏ��͕�����B���Ă���
        SetActive(false);
    }

    void Start()
    {
        _ = ExcecuteActionLoop();   //�Ăяo��&Task��j��
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
        await Task.Delay((int)(sec * 1000));
    }

    //�����ʒu�Ƀ��[�v
    public void WarpDefault()
    {
        //�������������Z�b�g
        ResetRigidbody();

        transform.SetParent(Owner.transform);
        transform.localPosition = defaultLocalPosition;
        transform.rotation = defaultRotation;
        transform.localScale = defaultLocalScale;
    }

    //�����̌q�����⊮����֐�
    async Task Lerp(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Vector3 endPosition, Quaternion endRotation, Vector3 endScale, float second)
    {
        float elapsedTime = 0f;

        while (elapsedTime < second)
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
        //�������������Z�b�g
        ResetRigidbody();

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

    //������w�肳�ꂽ(x, y)�ʒu��s�b�ňړ�������
    async protected Task Move(float x, float y, float s)
    {
        Owner.ActionBar.SendText("Weapon-Move");

        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(x, y);
        float elapsedTime = 0f;

        while (elapsedTime < s)
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
        float elapsed = 0f;
        float initialRotation = transform.rotation.eulerAngles.z;
        float targetRotation = initialRotation + angle;

        while (elapsed < s)
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

        while (!stop)
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
        Transform centerObject = Owner.transform;

        float rotationSpeed = 400.0f; // ��]���x
        float angle = 0;
        float angleAmount = 0;

        while (angleAmount < 360)
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

    //������w�肳�ꂽ�����ɔ�΂�
    async protected Task Shot(Vector2 direction, float power)
    {
        Owner.ActionBar.SendText("Weapon-Shot");

        //�����X�^�[�̌����ɂ���ĕ����΂�������ς���
        float dir = 1;   
        if (!Owner.IsFacingRight) 
        {
            dir *= -1;
        }

        // �����������g����悤�ɂ��Ĕ�΂�
        rb.simulated = true;
        rb.AddForce(direction * power * dir, ForceMode2D.Impulse);

        await Wait(Parameters.ACTION_INTERVAL_SHOT);

        await Default();
    }

    //����������X�^�[�ɂ�鐧�䂩��؂藣��
    async protected Task Purge()
    {
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

    //�������������Z�b�g
    void ResetRigidbody() 
    {
        rb.simulated = false;
        rb.linearVelocity = new Vector3(0, 0, 0);
        rb.angularVelocity = 0f;
    }
}